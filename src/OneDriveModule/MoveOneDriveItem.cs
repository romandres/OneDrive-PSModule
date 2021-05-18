using System;
using System.Management.Automation;
using Microsoft.Graph;

namespace OneDriveModule
{
    [Cmdlet(VerbsCommon.Move, "OneDriveItem")]
    public class MoveOneDriveItem : BaseGraphCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public OneDriveItem? Item { get; set; }

        [Parameter(Mandatory = true)]
        public string? Destination { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            DriveItem? destinationFolder = null;

            try
            {
                destinationFolder = Settings.GraphClientWrapper!.GraphServiceClient
                    .Users[Item!.UserId]
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
                var newDriveItem = new DriveItem
                {
                    ParentReference = new ItemReference
                    {
                        Id = destinationFolder.Id
                    },
                    Name = Item!.Name
                };

                try
                {
                    WriteVerbose($"Moving item '{Item.Name}' with ID '{Item.Id}' to '{Destination}'.");

                    DriveItem driveItem = Settings.GraphClientWrapper!.GraphServiceClient
                        .Users[Item.UserId]
                        .Drive
                        .Items[Item.Id]
                        .Request()
                        .UpdateAsync(newDriveItem)
                        .GetAwaiter()
                        .GetResult();

                    if (driveItem is object)
                    {
                        WriteObject(new OneDriveItem()
                        {
                            Id = driveItem.Id,
                            Name = driveItem.Name,
                            UserId = Item.UserId
                        });
                    }
                    else
                    {
                        var exception = new InvalidOperationException("DriveItem returned by move operation is null which means the move operation was not successful.");
                        WriteError(new ErrorRecord(exception, "MoveItemFailed", ErrorCategory.NotSpecified, newDriveItem));
                    }
                }
                catch (ServiceException ex)
                {
                    WriteError(new ErrorRecord(ex, "MoveItemFailed", ErrorCategory.NotSpecified, newDriveItem));
                }
            }
        }
    }
}
