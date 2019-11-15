using System;
using System.Management.Automation;
using Microsoft.Graph;

namespace OneDriveModule
{
    [Cmdlet(VerbsCommon.Move, "OneDriveItem")]
    public class MoveOneDriveItem : PSCmdlet
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
            DriveItem destinationFolder = null;

            try
            {
                destinationFolder = Settings.GraphClient
                    .Users[Item.UserId]
                    .Drive
                    .Root
                    .ItemWithPath(Destination)
                    .Request()
                    .GetAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            catch (ServiceException ex)
            {
                WriteError(new ErrorRecord(ex, "GetItemFailed", ErrorCategory.NotSpecified, Destination));
            }

            if (destinationFolder is object)
            {
                WriteVerbose($"Moving item '{Item.Name}' to '{Destination}'");

                var newDriveItem = new DriveItem
                {
                    ParentReference = new ItemReference
                    {
                        Id = destinationFolder.Id
                    },
                    Name = Item.Name
                };

                try
                {
                    DriveItem driveItem = Settings.GraphClient
                        .Users[Item.UserId]
                        .Drive
                        .Items[Item.Id]
                        .Request()
                        .UpdateAsync(newDriveItem)
                        .GetAwaiter()
                        .GetResult();

                    WriteObject(new OneDriveItem()
                    {
                        Id = driveItem.Id,
                        Name = driveItem.Name,
                        UserId = Item.UserId
                    });
                }
                catch (ServiceException ex)
                {
                    WriteError(new ErrorRecord(ex, "MoveItemFailed", ErrorCategory.NotSpecified, newDriveItem));
                }
            }
        }
    }
}
