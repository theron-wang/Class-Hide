using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;

namespace ClassHide.Parsers;
internal static class HtmlParser
{
    /// <summary>
    /// Gets the class scopes that intersect with the given span. Includes class="..."
    /// </summary>
    public static IEnumerable<SnapshotSpan> GetScopes(SnapshotSpan span)
    {
        var start = Math.Max(0, (int)span.Start - 2000);

        foreach (var scope in ClassRegexHelper.GetClassesNormalEnumerator(span.Snapshot.GetText(start, Math.Min(span.Snapshot.Length, (int)span.End + 2000) - start)))
        {
            var potentialReturn = new SnapshotSpan(span.Snapshot, start + scope.Index, scope.Length);

            if (!potentialReturn.IntersectsWith(span))
            {
                continue;
            }

            yield return potentialReturn;
        }
    }
}
