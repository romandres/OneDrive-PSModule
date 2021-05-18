using System.Management.Automation;

namespace OneDriveModule
{
    [Cmdlet(VerbsCommunications.Disconnect, "OneDrive")]
    public class DisconnectOneDrive : BaseGraphCmdlet
    {
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void EndProcessing()
        {
            Settings.GraphClientWrapper = null;
        }
    }
}
