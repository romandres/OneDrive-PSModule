using System;
using System.Management.Automation;

namespace OneDriveModule
{
    public abstract class BaseGraphCmdlet : PSCmdlet
    {
        protected override void BeginProcessing()
        {
            if (Settings.GraphClientWrapper is null)
            {
                var exception = new InvalidOperationException("Connect-OneDrive needs to be executed before running other commands.");
                ThrowTerminatingError(new ErrorRecord(exception, "NotConnected", ErrorCategory.InvalidOperation, Settings.GraphClientWrapper));
            }
        }
    }
}
