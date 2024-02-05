using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Outlining;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassHide;

[Command(PackageGuids.guidVSPackageCmdSetString, PackageIds.CollapseAllCmdId)]
internal class CollapseAllCommand : BaseCommand<CollapseAllCommand>
{
    private static readonly string[] _contentTypes = ["html", "WebForms", "razor", "LegacyRazorCSharp", "LegacyRazor", "LegacyRazorCoreCSharp"];

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

        if (_contentTypes.Any(c => c.Equals(docView.TextView.TextSnapshot.ContentType.TypeName, StringComparison.InvariantCultureIgnoreCase)))
        {
            parser = docView.TextBuffer.Properties.GetOrCreateSingletonProperty(() => new Parser(docView.TextBuffer));
        }
        else
        {
            return;
        }

        var snapshot = new SnapshotSpan(docView.TextBuffer.CurrentSnapshot, 0, docView.TextBuffer.CurrentSnapshot.Length);
        manager.CollapseAll(snapshot, collapsible =>
        {
            return parser.Regions.Contains(collapsible.Extent.GetSpan(docView.TextBuffer.CurrentSnapshot));
        });
    }
}
