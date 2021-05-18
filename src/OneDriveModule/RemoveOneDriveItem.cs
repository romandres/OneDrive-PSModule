using System.Management.Automation;
using Microsoft.Graph;

namespace OneDriveModule
{
    [Cmdlet(VerbsCommon.Remove, "OneDriveItem")]
    public class RemoveOneDriveItem : BaseGraphCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public OneDriveItem? Item { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            WriteVerbose($"Removing item {Item!.Name}");

            try
            {
                Settings.GraphClientWrapper!.GraphServiceClient
                    .Users[Item.UserId]
                    .Drive
                    .Items[Item.Id]
                    .Request()
                    .DeleteAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            catch (ServiceException ex)
            {
                WriteError(new ErrorRecord(ex, "RemoveItemFailed", ErrorCategory.NotSpecified, Item));
            }
        }
    }
}
