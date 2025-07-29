using ClassHide.TaggerParsers;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace ClassHide;

[Export(typeof(ITextViewCreationListener))]
[ContentType("JavaScript")]
[ContentType("TypeScript")]
[ContentType("jsx")]
[TextViewRole(PredefinedTextViewRoles.Document)]
[TextViewRole(PredefinedTextViewRoles.Analyzable)]
internal class JSCollapser : Collapser
{
    protected override Parser GetParser(ITextBuffer textBuffer)
    {
        return new JSTaggerParser(textBuffer);
    }
}
