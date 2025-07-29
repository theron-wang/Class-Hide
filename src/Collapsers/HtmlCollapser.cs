using ClassHide.TaggerParsers;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace ClassHide;

[Export(typeof(ITextViewCreationListener))]
[ContentType("html")]
[ContentType("WebForms")]
[TextViewRole(PredefinedTextViewRoles.Document)]
[TextViewRole(PredefinedTextViewRoles.Analyzable)]
internal class HtmlCollapser : Collapser
{
    public override void TextViewCreated(ITextView textView)
    {
        // Handle legacy Razor editor; this completion controller is prioritized but
        // we should only use the Razor completion controller in that case
        if (textView.TextBuffer.IsLegacyRazorEditor())
        {
            return;
        }

        base.TextViewCreated(textView);
    }

    protected override Parser GetParser(ITextBuffer textBuffer)
    {
        return new HtmlTaggerParser(textBuffer);
    }
}
