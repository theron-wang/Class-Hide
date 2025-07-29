using ClassHide.TaggerParsers;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassHide;
internal class OutlineTagger : ITagger<IOutliningRegionTag>, IDisposable
{
    private readonly ITextBuffer _buffer;
    private readonly Parser _parser;

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public OutlineTagger(ITextBuffer buffer, Parser parser)
    {
        _buffer = buffer;
        _parser = parser;
        _parser.Validated += UpdateRegions;
    }

    public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
    {
        if (!spans.Any())
        {
            // check why on file open does not collapse
            yield break;
        }

        SnapshotSpan entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(_buffer.CurrentSnapshot, SpanTrackingMode.EdgeExclusive);

        foreach (var region in GetRegions(entire))
        {
            yield return region;
        }
    }

    private void UpdateRegions(IEnumerable<Span> spans)
    {
        if (TagsChanged is not null)
        {
            if (spans is null)
            {
                var span = new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length);
                TagsChanged(this, new(span));
            }
            else
            {
                foreach (var span in spans)
                {
                    TagsChanged(this, new(new SnapshotSpan(_buffer.CurrentSnapshot, span)));
                }
            }
        }
    }

    private IEnumerable<ITagSpan<IOutliningRegionTag>> GetRegions(SnapshotSpan span)
    {
        var options = ThreadHelper.JoinableTaskFactory.Run(Options.GetLiveInstanceAsync);

        if (_parser.Regions.Any() && _parser.Regions.First().Snapshot != span.Snapshot)
        {
            foreach (var scope in _parser.GetScopes(span))
            {
                if (scope.Length < options.MinimumClassLength)
                {
                    continue;
                }

                var text = scope.GetText();

                yield return new TagSpan<IOutliningRegionTag>(scope,
                    new OutliningRegionTag(options.AutomaticallyFold, false,
                    options.PreviewOption == PreviewOption.Ellipses ? "..." : text.Length >= options.PreviewLength ? text.Substring(0, options.PreviewLength).Trim() : text,
                    text));
            }
        }
        else
        {
            var regions = _parser.Regions.Where(e => span.IntersectsWith(e.Span));
            foreach (var region in regions)
            {
                if (region.Length < options.MinimumClassLength)
                {
                    continue;
                }

                var text = region.GetText();

                yield return new TagSpan<IOutliningRegionTag>(region,
                    new OutliningRegionTag(options.AutomaticallyFold, false,
                    options.PreviewOption == PreviewOption.Ellipses ? "..." : text.Length >= options.PreviewLength ? text.Substring(0, options.PreviewLength).Trim() : text,
                    text));
            }
        }
    }

    public void Dispose()
    {
        _parser.Validated -= UpdateRegions;
    }
}
