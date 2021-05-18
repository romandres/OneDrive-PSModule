using System;
using System.Management.Automation;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;
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
        public string? ApplicationId { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "WithAppSecret")]
        public SecureString? ApplicationSecret { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "WithAppCertificate")]
        public string? ApplicationCertificateThumbprint { get; set; }

        [Parameter(Mandatory = true)]
        public string? TenantId { get; set; }

        protected override void EndProcessing()
        {
            var appCredential = new NetworkCredential(string.Empty, ApplicationSecret);
            Settings.GraphClientWrapper = Login(ApplicationId!, appCredential.Password, TenantId!);
        }

        private GraphClientWrapper Login(string clientId, string clientSecret, string tenantId)
        {
            var clientAppBuilder = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(AadAuthorityAudience.AzureAdMyOrg)
                .WithTenantId(tenantId);

            if (ParameterSetName == "WithAppCertificate")
            {
                // Open current users personal certificate store
                using var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.ReadOnly);

                // Find certificate by thumbprint
                var certResults = certStore.Certificates.Find(X509FindType.FindByThumbprint, ApplicationCertificateThumbprint, false);
                if (certResults.Count == 1)
                {
                    clientAppBuilder.WithCertificate(certResults[0]);
                }
                else
                {
                    throw new InvalidOperationException("Certificate could not be found using thumbprint.");
                }
            }
            else if (ParameterSetName == "WithAppSecret")
            {
                clientAppBuilder
                    .WithClientSecret(clientSecret);
            }
            else
            {
                throw new InvalidOperationException("Application secret or certificate required to connect.");
            }

            return new GraphClientWrapper(clientAppBuilder.Build());
        }
    }
}
