using ClassHide.TaggerParsers;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace ClassHide;

[Export(typeof(ITextViewCreationListener))]
[ContentType("razor")]
[ContentType("LegacyRazorCSharp")]
[ContentType("LegacyRazor")]
[ContentType("LegacyRazorCoreCSharp")]
[TextViewRole(PredefinedTextViewRoles.Document)]
[TextViewRole(PredefinedTextViewRoles.Analyzable)]
internal class RazorCollapser : Collapser
{
    protected override Parser GetParser(ITextBuffer textBuffer)
    {
        return new RazorTaggerParser(textBuffer);
    }
}
