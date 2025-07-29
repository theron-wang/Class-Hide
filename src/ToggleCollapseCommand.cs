using ClassHide.TaggerParsers;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Outlining;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassHide;

[Command(PackageGuids.guidVSPackageCmdSetString, PackageIds.CollapseAllCmdId)]
internal class ToggleCollapseCommand : BaseCommand<ToggleCollapseCommand>
{
    private static readonly string[] _htmlContentTypes = ["html", "WebForms"];
    private static readonly string[] _razorContentTypes = ["razor", "LegacyRazorCSharp", "LegacyRazor", "LegacyRazorCoreCSharp"];
    private static readonly string[] _jsContentTypes = ["JavaScript", "TypeScript", "tsx"];

    protected override void BeforeQueryStatus(EventArgs e)
    {
        var options = ThreadHelper.JoinableTaskFactory.Run(Options.GetLiveInstanceAsync);
        Command.Visible = options.EnableOutlines;
    }

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        var docView = await VS.Documents.GetActiveDocumentViewAsync();
        var buffer = docView?.TextBuffer;

        if (docView is null || buffer is null)
        {
            return;
        }

        var componentModel = await VS.Services.GetComponentModelAsync();
        var outliningManagerService = componentModel.GetService<IOutliningManagerService>();

        var manager = outliningManagerService.GetOutliningManager(docView?.TextView);

        if (manager is null)
        {
            return;
        }

        Parser parser = null;

        if (_razorContentTypes.Any(c => c.Equals(docView.TextView.TextSnapshot.ContentType.TypeName, StringComparison.InvariantCultureIgnoreCase)))
        {
            parser = docView.TextBuffer.Properties.GetOrCreateSingletonProperty(() => new RazorTaggerParser(docView.TextBuffer));
        }
        else if (_htmlContentTypes.Any(c => c.Equals(docView.TextView.TextSnapshot.ContentType.TypeName, StringComparison.InvariantCultureIgnoreCase)))
        {
            parser = docView.TextBuffer.Properties.GetOrCreateSingletonProperty(() => new HtmlTaggerParser(docView.TextBuffer));
        }
        else if (_jsContentTypes.Any(c => c.Equals(docView.TextView.TextSnapshot.ContentType.TypeName, StringComparison.InvariantCultureIgnoreCase)))
        {
            parser = docView.TextBuffer.Properties.GetOrCreateSingletonProperty(() => new JSTaggerParser(docView.TextBuffer));
        }
        else
        {
            return;
        }

        var snapshot = new SnapshotSpan(docView.TextBuffer.CurrentSnapshot, 0, docView.TextBuffer.CurrentSnapshot.Length);

        if (manager.GetAllRegions(snapshot)
            .All(r =>
                !parser.Regions.Contains(r.Extent.GetSpan(docView.TextBuffer.CurrentSnapshot)) ||
                (r.IsCollapsible && r.IsCollapsed)))
        {
            manager.ExpandAll(snapshot, c => parser.Regions.Contains(c.Extent.GetSpan(docView.TextBuffer.CurrentSnapshot)));
        }
        else
        {
            manager.CollapseAll(snapshot, c => parser.Regions.Contains(c.Extent.GetSpan(docView.TextBuffer.CurrentSnapshot)));
        }
    }
}
