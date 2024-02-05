using Community.VisualStudio.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClassHide;

internal partial class OptionsProvider
{
    [ComVisible(true)]
    public class GeneralOptions : BaseOptionPage<Options> { }
}

public class Options : BaseOptionModel<Options>
{
    [Category("General")]
    [DisplayName("Enable class hiding")]
    [Description("Enables or disables class hiding")]
    [DefaultValue(true)]
    public bool EnableOutlines { get; set; } = true;

    [Category("General")]
    [DisplayName("Automatic collapse")]
    [Description("Automatically collapse classes on file open")]
    [DefaultValue(true)]
    public bool AutomaticallyFold { get; set; } = true;

    [Category("General")]
    [DisplayName("Minimum class length")]
    [Description("The minimum text length within class=\"\" to enable collapsing. Defaults to 0")]
    [DefaultValue(0)]
    public int MinimumClassLength { get; set; } = 0;

    [Category("General")]
    [DisplayName("Preview length")]
    [Description("The maximum number of characters shown in preview when classes are hidden; only takes effect when Preview is set to Truncate")]
    [DefaultValue(20)]
    public int PreviewLength { get; set; } = 20;

    [Category("General")]
    [DisplayName("Preview")]
    [Description("The text to display over the truncated section")]
    [TypeConverter(typeof(EnumConverter))]
    [DefaultValue(PreviewOption.Ellipses)]
    public PreviewOption PreviewOption { get; set; } = PreviewOption.Ellipses;
}

public enum PreviewOption
{
    Ellipses,
    Truncate
}