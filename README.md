# Class Hide

![Visual Studio Marketplace Installs](https://img.shields.io/visual-studio-marketplace/i/TheronWang.ClassHide)
![Visual Studio Marketplace Version (including pre-releases)](https://img.shields.io/visual-studio-marketplace/v/TheronWang.ClassHide)

This extension collapses lengthy HTML class attributes to increase code readability, especially when working with utility frameworks like Tailwind CSS. Say goodbye to file clutter and welcome a more streamlined coding experience.

Download from the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=TheronWang.ClassHide).

## Collapse Classes

![Class hide](https://raw.githubusercontent.com/theron-wang/Class-Hide/main/art/class-hide.gif)

Instantly hide all class attributes by using Ctrl+R, Ctrl+H or by clicking **Collapse All Classes** under the Tools menu.

## Settings

![Settings](https://raw.githubusercontent.com/theron-wang/Class-Hide/main/art/settings.png)

Extension settings can be located under Tools > Options > Class Hide.

- **`Automatic collapse`**: Automatically collapse classes on file open
- **`Enable class hiding`**: Enables or disables class hiding
- **`Minimum class length`**: The minimum text length within class=\"\" to enable collapsing
- **`Preview`**: The text to display over the truncated section. Can be set to `Ellipses`, showing `...`, or `Truncate`, which will display a shortened version of the class attribute
- **`Preview length`**: The maximum number of characters shown in preview when classes are hidden; only takes effect when `Preview` is set to `Truncate`
- **`Delimiter`**: Text after the first instance of this delimiter will be hidden. If the delimiter is not found, the entire class will be collapsed.

## Bugs / Suggestions

If you run into any issues or come up with any feature suggestions while using this extension, please create an issue [on the GitHub repo](https://github.com/theron-wang/Class-Hide/issues/new).