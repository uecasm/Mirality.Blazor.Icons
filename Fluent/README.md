![.NET 6,8,10](https://img.shields.io/static/v1?label=.NET&message=6,8,10&color=blue) [![NuGet version (Mirality.Blazor.Icons.Fluent)](https://img.shields.io/nuget/v/Mirality.Blazor.Icons.Fluent.svg?logo=nuget)](https://www.nuget.org/packages/Mirality.Blazor.Icons.Fluent/)

This is a Blazor component wrapper library for all of the icons in the [Fluent UI System](https://github.com/microsoft/fluentui-system-icons) icon set created by Microsoft and released under the MIT license.

Inline components allow you to more easily recolor or otherwise style the icons, as they obey a directly applied `class` or inherited CSS properties, unlike external SVG image files.  (There is a way to trick SVG `use` into allowing external styling, but that has some different caveats, including current browsers lacking full support for unmodified external SVGs.)

# Installation

1. Simply import this Nuget package as usual.  (Note: it is not recommended to use this as a project reference -- due to the large number of individual files it tends to bring IDEs to an absolute crawl, and it takes a surprisingly long time to compile all the components.)

2. Optionally, add the following line to your `_Imports.razor`:

        @using Mirality.Blazor.Icons.Fluent

# Usage

To display an icon, include its component (with optional `class`), e.g.:

    <IconFluentGridDotsRegular class="text-info" />

You can also set other attributes of the resulting `<svg/>` element, such as its `style` or overriding its `height`.

All component names have been PascalCased from the snake-cased original icon names (omitting the size number), and have the `IconFluent` prefix to allow importing multiple icon sets without collisions.

For more usage information, including customisation options, see the docs for the [`Mirality.Blazor.Icons`](https://www.nuget.org/packages/Mirality.Blazor.Icons/) base package.

# Sizing

The size number from the original icon names has been removed (THIS IS A BREAKING CHANGE FROM 1.0!) and only a single icon from each size group has been kept -- usually the 24px variant where available, but otherwise the largest available size, under the assumption that SVG icons should downsize gracefully.  Including the size variants just produced way too many components such that the compiler actually broke under the memory strain.

By default all icons will render with a height of `1em` -- i.e. matching your current font height.  As such, to resize them you can just change the font size.  Alternatively, you can override the `height` to some other explicit size.

If you want to render the icon at its native size then you will need to set the font size or `height` accordingly; for example, to render a "24" icon at its native size, use `height="24px"`.  You can inspect the component source if you want to learn the native size, but for the most part this should not be important.

# Gallery

In addition to the main Microsoft Fluent UI site, you can view a gallery of the available icons at [Iconify](https://icon-sets.iconify.design/fluent/).  This library was generated from the Iconify icon data.  (Although it might become outdated at some point; submit an issue if it needs an update.)

Note that currently only the "primary" icons (non-deleted and non-aliases) have been imported.

# Offline Usage

This library embeds all the icon data, so online access is not required to load any image, nor any external files -- just the library itself.
