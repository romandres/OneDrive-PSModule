using System;
using System.Management.Automation;

namespace OneDriveModule
{
    [Cmdlet(VerbsCommon.Remove, "OneDriveItem")]
    public class RemoveOneDriveItem : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public OneDriveItem Item { get; set; }

        protected override void BeginProcessing()
        {
            if (Settings.GraphClient is null)
            {
                var exception = new InvalidOperationException("Connect-OneDrive needs to be executed before running other commands.");
                ThrowTerminatingError(new ErrorRecord(exception, "NotConnected", ErrorCategory.InvalidOperation, Settings.GraphClient));
            }
        }

        protected override void ProcessRecord()
        {
            WriteVerbose($"Removing file {Item.Name}");

            Settings.GraphClient
                .Users[Item.UserId]
                .Drive
                .Items[Item.Id]
                .Request()
                .DeleteAsync()
                .Wait();
        }
    }
}
