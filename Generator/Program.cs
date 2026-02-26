using System.Net.Http.Json;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        _ = services.AddLogging(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
            });
        });
        _ = services.AddHttpClient("Iconify", client =>
            {
                client.BaseAddress = new Uri("https://raw.githubusercontent.com/iconify/icon-sets/master/json/");
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mirality");
            });
        _ = services.AddOptions<IconGeneratorService.Options>()
            .Configure(options =>
            {
                var baseOutput = @"..\..\..\..\";
                options.Mappings.Add(new IconGeneratorService.IconMapping("fluent", "IconFluent", $@"{baseOutput}Fluent\Icons\Fluent"));
                options.Mappings.Add(new IconGeneratorService.IconMapping("openmoji", "IconOpenMoji", $@"{baseOutput}OpenMoji\Icons\OpenMoji"));
            });
        _ = services.AddHostedService<IconGeneratorService>();
    })
    .Build();

host.Run();


internal abstract class ForegroundService : BackgroundService
{
    private readonly IHostApplicationLifetime _Lifetime;

    protected ForegroundService(IHostApplicationLifetime lifetime)
    {
        _Lifetime = lifetime;
    }

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await ExecuteForegroundAsync(stoppingToken);
        }
        finally
        {
            _Lifetime.StopApplication();
        }
    }

    protected abstract Task ExecuteForegroundAsync(CancellationToken stoppingToken);
}

internal class IconGeneratorService : ForegroundService
{
    private readonly HttpClient _Http;
    private readonly IOptions<Options> _Options;
    private readonly ILogger<IconGeneratorService> _Logger;

    public IconGeneratorService(IHostApplicationLifetime lifetime, IHttpClientFactory httpFactory, IOptions<Options> options, ILogger<IconGeneratorService> logger)
        : base(lifetime)
    {
        _Http = httpFactory.CreateClient("Iconify");
        _Options = options;
        _Logger = logger;
    }

    public class Options
    {
        public List<IconMapping> Mappings { get; } = new();
    }
    public record IconMapping(string IconPrefix, string FilenamePrefix, string OutputPath);

    protected override async Task ExecuteForegroundAsync(CancellationToken cancel)
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        XNamespace svgNs = "http://www.w3.org/2000/svg";

        foreach (var mapping in _Options.Value.Mappings)
        {
            using var prefixScope = _Logger.BeginScope(mapping.IconPrefix);

            var icons = (await _Http.GetFromJsonAsync<JsonObject>($"{mapping.IconPrefix}.json", options, cancel))!;
            _ = Directory.CreateDirectory(mapping.OutputPath);

            foreach (var file in Directory.GetFiles(mapping.OutputPath, "*.razor"))
            {
                File.Delete(file);
            }

            icons["info"]!["lastModified"] = icons["lastModified"]!.Deserialize<JsonNode>();
            await using (var stream = File.Create(Path.Combine(mapping.OutputPath, "info.json")))
            {
                await JsonSerializer.SerializeAsync(stream, icons["info"], new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true }, cancel);
            }

            var defaultLeft = icons["left"]?.GetValue<int>() ?? 0;
            var defaultTop = icons["top"]?.GetValue<int>() ?? 0;
            var defaultWidth = icons["width"]?.GetValue<int>() ?? 16;
            var defaultHeight = icons["height"]?.GetValue<int>() ?? 16;
            var iconsJson = icons["icons"]!.AsObject();

            if (mapping.IconPrefix == "fluent")
            {
                iconsJson = FilterFluentIcons(iconsJson);
            }

            foreach (var icon in iconsJson)
            {
                cancel.ThrowIfCancellationRequested();
                if (icon.Value!["hidden"]?.GetValue<bool>() == true) continue;

                _Logger.LogInformation(icon.Key);

                var path = Path.Combine(mapping.OutputPath, $"{mapping.FilenamePrefix}{MakeFilename(icon.Key)}.razor");
                var body = icon.Value!["body"]!.ToString();
                var left = icon.Value!["left"]?.GetValue<int>() ?? defaultLeft;
                var top = icon.Value!["top"]?.GetValue<int>() ?? defaultTop;
                var width = icon.Value!["width"]?.GetValue<int>() ?? defaultWidth;
                var height = icon.Value!["height"]?.GetValue<int>() ?? defaultHeight;

                var razor = $@"<IconSvg viewBox=""{left} {top} {width} {height}"" @attributes=""Attributes"">{body}</IconSvg>

@code {{

[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? Attributes {{ get; set; }}

}}";

                await File.WriteAllTextAsync(path, razor, Encoding.UTF8, cancel);
            }
        }
    }

    private static readonly Regex FluentIconNameRegex = new(
        @"^(?<base>.+)-(?<size>\d+)-(?<style>regular|filled|light)$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

    /// <summary>The Fluent icon set is just way too damn big (~19k icons!) and the compiler runs
    /// out of memory.  Pick a single representative size for each icon and only offer that.</summary>
    private static JsonObject FilterFluentIcons(JsonObject allIcons)
    {
        var preferred = new Dictionary<string, (int size, KeyValuePair<string, JsonNode?> content)>();

        foreach (var icon in allIcons)
        {
            var m = FluentIconNameRegex.Match(icon.Key);
            if (!m.Success)
            {
                throw new FormatException($"Unexpected format of icon name {icon.Key}");
            }

            var name = m.Groups["base"].Value;
            var size = int.Parse(m.Groups["size"].Value);
            var style = m.Groups["style"].Value;
            var key = $"{name}-{style}";

            if (preferred.TryGetValue(key, out var pref))
            {
                if (size == 24 || (pref.size != 24 && size > pref.size))
                {
                    preferred[key] = (size, icon);
                }
            }
            else
            {
                preferred[key] = (size, icon);
            }
        }

        var result = new JsonObject();
        foreach (var icon in preferred)
        {
            result.Add(icon.Key, icon.Value.content.Value!.DeepClone());
        }
        return result;
    }

    // iconName is something like "grid-dots-regular" and we want "GridDotsRegular"
    private static string MakeFilename(string iconName) => string.Join("", iconName.Split('-').Select(WordCase));
    private static string WordCase(string word) => char.ToUpperInvariant(word[0]) + (word.Length > 1 ? word[1..] : "");
}
