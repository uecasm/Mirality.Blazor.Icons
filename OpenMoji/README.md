![.NET 6,7](https://img.shields.io/static/v1?label=.NET&message=6,7&color=blue) [![NuGet version (Mirality.Blazor.Icons.OpenMoji)](https://img.shields.io/nuget/v/Mirality.Blazor.Icons.OpenMoji.svg?logo=nuget)](https://www.nuget.org/packages/Mirality.Blazor.Icons.Fluent/)

This is a Blazor component wrapper library for all of the icons in the [OpenMoji](https://openmoji.org/) icon set, released under the CC-BY-SA 4.0 license.

# Installation

1. Simply import this Nuget package as usual.  (Note: it is not recommended to use this as a project reference -- due to the large number of individual files it tends to bring IDEs to an absolute crawl, and it takes a surprisingly long time to compile all the components.)

2. Optionally, add the following line to your `_Imports.razor`:

    @import Mirality.Blazor.Icons.OpenMoji

# Usage

To display an icon, include its component, e.g.:

    <IconOpenMojiClosedMailboxWithRaisedFlag />

You can also set other attributes of the resulting `<svg/>` element, such as its `class`, `style`, or overriding its `height`.

(The OpenMoji icons are fixed-palette, so you can't use classes to recolour them, but there may still be other interesting things you can do this way.)

All component names have been PascalCased from the snake-cased original icon names, and have the `IconOpenMoji` prefix to allow importing multiple icon sets without collisions.

For more usage information, including customisation options, see the docs for the [`Mirality.Blazor.Icons`](https://www.nuget.org/packages/Mirality.Blazor.Icons/) base package.

# Sizing

By default, all icons will render with a height of `1em` -- i.e. matching your current font height.  As such, to resize them you can just change the font size.  Alternatively, you can override the `height` to some other explicit size.

# Gallery

In addition to the main OpenMoji site, you can view a gallery of the available icons at [Iconify](https://icon-sets.iconify.design/openmoji/).  This library was generated from the Iconify icon data.  (Although it might become outdated at some point; submit an issue if it needs an update.)

Note that currently only the "primary" icons (non-deleted and non-aliases) have been imported.

# Offline Usage

This library embeds all the icon data, so online access is not required to load any image, nor any external files -- just the library itself.
