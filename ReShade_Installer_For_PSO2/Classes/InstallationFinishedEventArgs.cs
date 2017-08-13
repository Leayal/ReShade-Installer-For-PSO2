using System;

namespace ReShade_Installer_For_PSO2.Classes
{
    public class InstallationFinishedEventArgs : EventArgs
    {
        public Exception Error { get; }
        public bool Cancelled { get; }
        // public Exception Error { get; }

        public InstallationFinishedEventArgs(bool cancel) : this(null, cancel) { }
        public InstallationFinishedEventArgs(Exception ex) : this(ex, false) { }
        public InstallationFinishedEventArgs(Exception ex, bool cancel)
        {
            this.Error = ex;
            this.Cancelled = cancel;
        }

        public InstallationFinishedEventArgs(System.ComponentModel.RunWorkerCompletedEventArgs e) : this(e.Error, e.Cancelled) { }
    }
}
