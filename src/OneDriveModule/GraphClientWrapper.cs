using System;
using System.Net.Http.Headers;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace OneDriveModule
{
    internal class GraphClientWrapper
    {
        private readonly string[] scopes = new string[]
        {
            "https://graph.microsoft.com/.default"
        };

        private readonly IConfidentialClientApplication clientApp;

        private AuthenticationResult? authenticationResult;
        private IGraphServiceClient? graphServiceClient;

        public GraphClientWrapper(IConfidentialClientApplication confidentialClientApplication)
        {
            clientApp = confidentialClientApplication;
        }

        public IGraphServiceClient GraphServiceClient
        {
            get
            {
                // Renew token 5min before it expires
                bool renewToken = authenticationResult is null || authenticationResult.ExpiresOn.AddMinutes(-5) <= DateTime.Now;

                if (renewToken)
                {
                    authenticationResult = clientApp
                        .AcquireTokenForClient(scopes)
                        .ExecuteAsync()
                        .GetAwaiter()
                        .GetResult();

                    graphServiceClient = new GraphServiceClient(
                        "https://graph.microsoft.com/v1.0",
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                        new DelegateAuthenticationProvider(async (requestMessage) =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                        {
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", authenticationResult.AccessToken);
                        })
                    );
                }

                return graphServiceClient!;
            }
        }
    }
}
