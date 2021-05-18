using System.IO;
using System.Management.Automation;

namespace OneDriveModule
{
    [Cmdlet(VerbsLifecycle.Invoke, "OneDriveItemDownload")]
    public class DownloadOneDriveItem : BaseGraphCmdlet
    {
        private const int bufferSize = 16 * 320 * 1024;

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public OneDriveItem? Item { get; set; }

        [Parameter(Mandatory = true)]
        public string? Destination { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            string destinationPath = Path.Combine(Destination, Item!.Name);

            var fileMode = File.Exists(destinationPath) && Force.IsPresent ? FileMode.Create : FileMode.CreateNew;

            try
            {
                using (FileStream fs = new FileStream(destinationPath, fileMode, FileAccess.Write, FileShare.None, bufferSize, FileOptions.None))
                {
                    WriteVerbose($"Downloading file {Item.Name}");

                    Settings.GraphClientWrapper!.GraphServiceClient
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

                WriteObject(Item);
            }
            catch (IOException ex)
            {
                string errorId = "FileWriteError";
                var errorCategory = ErrorCategory.WriteError;

                if (ex.HResult == -2147024816)
                {
                    errorId = "FileAlreadyExists";
                    errorCategory = ErrorCategory.ResourceExists;
                }

                WriteError(new ErrorRecord(ex, errorId, errorCategory, destinationPath));
            }
        }
    }
}
