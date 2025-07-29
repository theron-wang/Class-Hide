using ClassHide.Parsers;
using Microsoft.VisualStudio.Text;
using System.Collections.Generic;

namespace ClassHide.TaggerParsers;
internal class RazorTaggerParser(ITextBuffer buffer) : Parser(buffer)
{
    protected override IEnumerable<SnapshotSpan> GetLanguageSpecificScopes(SnapshotSpan span)
    {
        return RazorParser.GetScopes(span);
    }
}
