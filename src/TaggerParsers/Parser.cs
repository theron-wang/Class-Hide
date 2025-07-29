using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassHide.TaggerParsers;
internal abstract class Parser
{
    private ITextSnapshot _snapshot;
    private Options _options;

    private readonly object _updateLock = new();
    private readonly ITextBuffer _buffer;

    public HashSet<SnapshotSpan> Regions { get; protected set; } = [];

    /// <summary>
    /// Sends an <see cref="IEnumerable{T}"/> of type <see cref="Span"/> of changed spans or null if the entire document was revalidated
    /// </summary>
    public Action<IEnumerable<Span>> Validated;
    public Action<Parser> ValidatedParser;

    public Parser(ITextBuffer buffer)
    {
        _buffer = buffer;
        _buffer.ChangedHighPriority += OnBufferChange;
        Options.Saved += OptionsChanged;

        _options = ThreadHelper.JoinableTaskFactory.Run(Options.GetLiveInstanceAsync);

        StartUpdate();
    }

    protected abstract IEnumerable<SnapshotSpan> GetLanguageSpecificScopes(SnapshotSpan span);

    public IEnumerable<SnapshotSpan> GetScopes(SnapshotSpan span)
    {
        foreach (var scope in GetLanguageSpecificScopes(span))
        {
            var t = scope.GetText();

            if (!string.IsNullOrWhiteSpace(_options.Delimiter) && t.Contains(_options.Delimiter))
            {
                var startIndex = t.IndexOf(_options.Delimiter);

                if (startIndex + 1 < t.Length)
                {
                    startIndex++;
                }
                if (startIndex + 1 < t.Length && char.IsWhiteSpace(t[startIndex + 1]))
                {
                    startIndex++;
                }

                yield return new SnapshotSpan(scope.Start + startIndex, scope.End);
            }
            else
            {
                yield return scope;
            }
        }
    }

    private void OnBufferChange(object sender, TextContentChangedEventArgs e)
    {
        if (_options.EnableOutlines)
        {
            _snapshot = e.After;
            ThreadHelper.JoinableTaskFactory.StartOnIdle(() =>
            {
                NormalUpdate(e);
            }).FireAndForget();
        }
    }

    private void StartUpdate()
    {
        if (_options.EnableOutlines)
        {
            ThreadHelper.JoinableTaskFactory.StartOnIdle(ForceUpdate).FireAndForget();
        }
    }

    private void ForceUpdate()
    {
        Regions = [.. GetScopes(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length))];

        Validated?.Invoke(null);
        ValidatedParser?.Invoke(this);
    }

    private void NormalUpdate(TextContentChangedEventArgs e)
    {
        lock (_updateLock)
        {
            foreach (var change in e.Changes)
            {
                Regions.RemoveWhere(e =>
                    e.Span.IntersectsWith(change.OldSpan) ||
                    (change.OldSpan.IsEmpty && e.Span.Contains(change.OldSpan)));
            }

            foreach (var region in Regions.Select(r => r).ToList())
            {
                Regions.Remove(region);
                Regions.Add(region.TranslateTo(e.After, SpanTrackingMode.EdgeInclusive));
            }

            if (_snapshot != null && _snapshot != e.After)
            {
                return;
            }

            List<Span> update = [];
            foreach (var change in e.Changes)
            {
                foreach (var scope in GetScopes(new SnapshotSpan(e.After, change.NewSpan)))
                {
                    Regions.RemoveWhere(r =>
                        r.Span.IntersectsWith(scope) ||
                        (scope.IsEmpty && r.Span.Contains(scope)));

                    update.Add(scope.Span);

                    Regions.Add(scope);
                }
            }

            Validated?.Invoke(update);
            ValidatedParser?.Invoke(this);
        }
    }

    public void Dispose()
    {
        _buffer.ChangedHighPriority -= OnBufferChange;
        Options.Saved -= OptionsChanged;
    }

    private void OptionsChanged(Options options)
    {
        _options = options;
        StartUpdate();
    }
}
