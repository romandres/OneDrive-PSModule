using System.Management.Automation;
using System.Net;
using System.Net.Http.Headers;
using System.Security;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace OneDriveModule
{

    [Cmdlet(VerbsCommunications.Connect, "OneDrive")]
    public class ConnectOneDrive : PSCmdlet
    {
        private readonly string[] scopes = new string[]
        {
            "https://graph.microsoft.com/.default"
        };

        [Parameter(Mandatory = true)]
        public string ApplicationId { get; set; }

        [Parameter(Mandatory = true)]
        public SecureString ApplicationSecret { get; set; }

        [Parameter(Mandatory = true)]
        public string TenantId { get; set; }

        protected override void EndProcessing()
        {
            var appCredential = new NetworkCredential(string.Empty, ApplicationSecret);
            Settings.GraphClient = Login(ApplicationId, appCredential.Password, TenantId);
        }

        private GraphServiceClient Login(string clientId, string clientSecret, string tenantId)
        {
            var clientApp = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(AadAuthorityAudience.AzureAdMyOrg)
                .WithTenantId(tenantId)
                .Build();

            var authResult = clientApp
                .AcquireTokenForClient(scopes)
                .ExecuteAsync()
                .GetAwaiter()
                .GetResult();

            var graphClient = new GraphServiceClient(
                        "https://graph.microsoft.com/v1.0",
                        new DelegateAuthenticationProvider(async (requestMessage) =>
                        {
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", authResult.AccessToken);
                        }));

            return graphClient;
        }
    }
}
