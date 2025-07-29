using ClassHide.TaggerParsers;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using System;
using System.Collections.Generic;

namespace ClassHide;

internal abstract class Collapser : ITextViewCreationListener, IDisposable
{
    private readonly Dictionary<Parser, ITextView> _queuedParsers = [];

    protected abstract Parser GetParser(ITextBuffer textBuffer);

    public virtual void TextViewCreated(ITextView textView)
    {
        var options = ThreadHelper.JoinableTaskFactory.Run(Options.GetLiveInstanceAsync);

        if (options.AutomaticallyFold)
        {
            var parser = textView.TextBuffer.Properties.GetOrCreateSingletonProperty(() => GetParser(textView.TextBuffer));

            parser.ValidatedParser += ParserValidated;
            _queuedParsers.Add(parser, textView);
        }
    }

    private void ParserValidated(Parser parser)
    {
        parser.ValidatedParser -= ParserValidated;

        var textView = _queuedParsers[parser];

        var componentModel = ThreadHelper.JoinableTaskFactory.Run(VS.Services.GetComponentModelAsync);
        var outliningManagerService = componentModel.GetService<IOutliningManagerService>();

        var manager = outliningManagerService.GetOutliningManager(textView);

        if (manager is null)
        {
            return;
        }

        var snapshot = new SnapshotSpan(textView.TextBuffer.CurrentSnapshot, 0, textView.TextBuffer.CurrentSnapshot.Length);
        manager.CollapseAll(snapshot, collapsible =>
        {
            return parser.Regions.Contains(collapsible.Extent.GetSpan(textView.TextBuffer.CurrentSnapshot));
        });

        _queuedParsers.Remove(parser);
    }

    public void Dispose()
    {
        foreach (var parser in _queuedParsers)
        {
            parser.Key.ValidatedParser -= ParserValidated;
        }
    }
}
