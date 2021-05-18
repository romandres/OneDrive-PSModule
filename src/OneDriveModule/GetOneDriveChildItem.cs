using System.Linq;
using System.Management.Automation;

namespace OneDriveModule
{

    [Cmdlet(VerbsCommon.Get, "OneDriveChildItem")]
    public class GetOneDriveChildItem : BaseGraphCmdlet
    {
        private const int queryTop = 1000;

        [Parameter(Mandatory = true)]
        public string? Path { get; set; }

        [Parameter(Mandatory = true)]
        public string? UserPrincipalName { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void EndProcessing()
        {
            var nextRequest = Settings.GraphClientWrapper!.GraphServiceClient
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
                        UserId = UserPrincipalName!,
                    })
                    .ToList()
                    .ForEach(item => WriteObject(item));
            }
        }
    }
}
