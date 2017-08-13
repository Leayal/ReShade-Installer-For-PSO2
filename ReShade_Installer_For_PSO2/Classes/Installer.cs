using System;
using System.IO;
using System.ComponentModel;

namespace ReShade_Installer_For_PSO2.Classes
{
    public abstract class Installer
    {
        private BackgroundWorker worker;
        protected Installer()
        {
            this.worker = new BackgroundWorker();
            this.worker.WorkerReportsProgress = false;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += this.Worker_DoWork;
            this.worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
        }

        public bool IsBusy => this.worker.IsBusy;

        public static Installer Create(Version version)
        {
            switch (version)
            {
                case Version.ReShade:
                    return new ReShadeInstaller();
                case Version.SweetFX2:
                    return new SweetFX2Installer();
                default:
                    throw new InvalidOperationException();
            }
        }

        public void InstallTo(string path, bool pluginSystem)
        {
            this.worker.RunWorkerAsync(new object[] { path, pluginSystem });
        }

        public static bool IsPSO2Directory(string path)
        {
            if (File.Exists(Path.Combine(path, "gameguard.des")))
                if (File.Exists(Path.Combine(path, "pso2launcher.exe")))
                    if (File.Exists(Path.Combine(path, "PSO2JP.ini")))
                        return true;
            return false;
        }

        protected abstract void Install(string path, bool pluginSystem);

        public event EventHandler<InstallationFinishedEventArgs> InstallationFinished;
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.InstallationFinished?.Invoke(this, new InstallationFinishedEventArgs(e));
        }

        public event EventHandler<IntEventArgs> TotalProgress;
        protected virtual void OnTotalProgress(IntEventArgs e)
        { this.TotalProgress?.Invoke(this, e); }
        public event EventHandler<IntEventArgs> CurrentProgress;
        protected virtual void OnCurrentProgress(IntEventArgs e)
        { this.CurrentProgress?.Invoke(this, e); }
        public event EventHandler<StringEventArgs> CurrentStep;
        protected virtual void OnCurrentStep(StringEventArgs e)
        { this.CurrentStep?.Invoke(this, e); }

        public void Cancel()
        {
            if (this.IsBusy)
                this.worker.CancelAsync();
        }

        protected bool CancellationPending => this.worker.CancellationPending;

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] objs = e.Argument as object[];
            string path = (string)objs[0];
            bool pluginSystem = (bool)objs[1];
            if (pluginSystem)
                path = Path.Combine(path, "Plugins");
            this.Install(path, pluginSystem);
        }
    }
}
