using System;
using System.Management.Automation;

namespace OneDriveModule
{
    [Cmdlet(VerbsCommunications.Disconnect, "OneDrive")]
    public class DisconnectOneDrive : PSCmdlet
    {
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
            Settings.GraphClient = null;
        }
    }
}
