using System;
using System.Linq;
using System.Management.Automation;

namespace OneDriveModule
{

    [Cmdlet(VerbsCommon.Get, "OneDriveChildItem")]
    public class GetOneDriveChildItem : PSCmdlet
    {
        private const int queryTop = 1000;

        [Parameter(Mandatory = true)]
        public string Path { get; set; }

        [Parameter(Mandatory = true)]
        public string UserPrincipalName { get; set; }

        protected override void BeginProcessing()
        {
            if (Settings.GraphClient is null)
            {
                var exception = new InvalidOperationException("Connect-OneDrive needs to be executed before running other commands.");
                ThrowTerminatingError(new ErrorRecord(exception, "NotConnected", ErrorCategory.InvalidOperation, Settings.GraphClient));
            }
        }

        protected override void EndProcessing()
        {
            var nextRequest = Settings.GraphClient
                .Users[UserPrincipalName]
                .Drive
                .Root
                .ItemWithPath(Path)
                .Children
                .Request()
                .Top(queryTop);

            while (nextRequest is object)
            {
                var currentPageResult = nextRequest
                    .GetAsync()
                    .GetAwaiter()
                    .GetResult();

                nextRequest = currentPageResult.NextPageRequest;

                currentPageResult
                    .Select(item => new OneDriveItem
                    {
                        Id = item.Id,
                        Name = item.Name,
                        UserId = UserPrincipalName,
                    })
                    .ToList()
                    .ForEach(item => WriteObject(item));
            }
        }
    }
}
