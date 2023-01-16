![.NET 6,7](https://img.shields.io/static/v1?label=.NET&message=6,7&color=blue) [![NuGet version (Mirality.Blazor.Icons)](https://img.shields.io/nuget/v/Mirality.Blazor.Icons.svg?logo=nuget)](https://www.nuget.org/packages/Mirality.Blazor.Icons/)

This is a Blazor component library that has some simple base components (currently only one, in fact) to support the more specific icon set libraries.

# Installation

It is usually not necessary to manually install this; it should be used as a dependency automatically when using one of the icon set libraries that require it.

However if you want to "manually trim" by only including specific icons into your own projects, then you may need to add it manually:

1. Add a NuGet package reference to this as usual.

2. Optionally, add the following line to your `_Imports.razor`:

    @import Mirality.Blazor.Icons

# Usage

By itself, this package does not provide any actual icons; you need to use one of its child packages instead.  See the usage instructions in those packages for some examples.

Below are some general steps that apply to all icons using this library:

## Inline

You can optionally add `class="mbi-inline"` on the icon component; this shifts the vertical alignment slightly to better line up with adjacent inline text.  This is a scoped class so it won't do anything on your own elements, but still has a prefix to avoid picking up any extra global styles you might have with a more generic name.  You can alternatively use other alignment classes or styles provided by your framework of choice.

## Sizing

By default all icons will render with a height of `1em` regardless of their internal native size -- i.e. matching your current font height.  As such, to resize them you can just change the font size.  Alternatively, you can override the `height` to some other explicit size.

If you want to render the icon at its native size then you will need to set the font size or `height` accordingly.

By default, the `width` will be set automatically to maintain the aspect ratio (i.e. usually the same as the height), but this can also be overriden manually if you wish -- though it's usually better to use margins instead.

## Trimming

The icon set libraries have been marked as trimmable by default, which means that if you have configured your application to perform "link"-level trimming on publish, then the published library will only include the icons that you are actually using, reducing the code size significantly.

Regular builds (or publishes where you have not enabled trimming) will include the full icon sets, which are quite large.  This typically will not matter for Blazor Server apps, or during local development, but may be problematic for deploying Blazor WebAssembly apps.

If for some reason you're unable to get trimming working with your application, another option to reduce file size is to not use the icon set libraries as-is but instead copy the individual icon components to your own application or icon library project as you require them.  Take care to properly comply with the icon set's license when doing this.

Note that you will need to manually reference this base package when importing the icons locally.

# Custom Icons

This package provides the `<IconSvg>` component; while it's not really intended to be used directly, it can be used for your own custom icons if you wish them to behave similarly.

The general format of a custom icon component is as follows:

    <IconSvg viewBox="0 0 WIDTH HEIGHT" @attributes="Attributes">
        <path fill="currentColor" d="... your icon path data here ..." />
        ... more icon path data here if needed ...
    </IconSvg>

    @code {

        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? Attributes { get; set; }
    
    }

where you replace `WIDTH` and `HEIGHT` with the native width and height of your icon.

There is a vague idea (but no actual current plan) to include some additional icon sets in the future, or perhaps even an integration with the Iconify API (which I've currently avoided because I wanted easy offline and precompiled usage).
