using ClassHide.TaggerParsers;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace ClassHide;

[Export(typeof(ITaggerProvider))]
[TagType(typeof(IOutliningRegionTag))]
[ContentType("JavaScript")]
[ContentType("TypeScript")]
[ContentType("jsx")]
[TextViewRole(PredefinedTextViewRoles.Document)]
[TextViewRole(PredefinedTextViewRoles.Analyzable)]
public class JSOutlineTaggerProvider : ITaggerProvider
{
    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
    {
        var parser = buffer.Properties.GetOrCreateSingletonProperty<Parser>(() => new JSTaggerParser(buffer));

        return buffer.Properties.GetOrCreateSingletonProperty(() => new OutlineTagger(buffer, parser)) as ITagger<T>;
    }
}
