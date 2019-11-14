using System;
using System.IO;
using System.Management.Automation;

namespace OneDriveModule
{
    [Cmdlet(VerbsLifecycle.Invoke, "OneDriveItemDownload")]
    public class DownloadOneDriveItem : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public OneDriveItem Item { get; set; }

        [Parameter(Mandatory = true)]
        public string Destination { get; set; }

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
            string destinationPath = Path.Combine(Destination, Item.Name);

            using (FileStream fs = new FileStream(destinationPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 16 * 320 * 1024, FileOptions.None))
            {
                WriteVerbose($"Downloading file {Item.Name}");

                Settings.GraphClient
                    .Users[Item.UserId]
                    .Drive
                    .Items[Item.Id]
                    .Content
                    .Request()
                    .GetAsync()
                    .GetAwaiter()
                    .GetResult()
                    .CopyTo(fs);
            }
        }
    }
}
