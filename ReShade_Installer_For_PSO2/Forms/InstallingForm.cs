using System;
using System.Windows.Forms;
using System.Threading;

namespace ReShade_Installer_For_PSO2.Forms
{
    public partial class InstallingForm : Form
    {
        public Classes.Installer Installer { get; }
        private SynchronizationContext synccontext;

        public InstallingForm(Classes.Installer theinstaller)
        {
            InitializeComponent();
            this.Installer = theinstaller;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.synccontext = SynchronizationContext.Current;
        }

        private void Installer_InstallationFinished(object sender, Classes.InstallationFinishedEventArgs e)
        {
            this.synccontext?.Send(new SendOrPostCallback(delegate { this.Close(); }), null);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Installer_AllowCancelChanged(this.Installer, EventArgs.Empty);
            this.Icon = Properties.Resources.mainico;
            this.Installer.TotalProgress += this.MyInstaller_TotalProgress;
            this.Installer.CurrentProgress += this.MyInstaller_CurrentProgress;
            this.Installer.CurrentStep += this.MyInstaller_CurrentStep;
            this.Installer.InstallationFinished += this.Installer_InstallationFinished;
            this.Installer.AllowCancelChanged += this.Installer_AllowCancelChanged;
        }

        private void Installer_AllowCancelChanged(object sender, EventArgs e)
        {
            this.synccontext?.Send(new SendOrPostCallback(delegate
            {
                if (this.Installer.AllowCancel)
                    this.button_cancel.Visible = true;
                else
                    this.button_cancel.Visible = false;
            }), null);
        }

        private void MyInstaller_CurrentStep(object sender, Classes.StringEventArgs e)
        {
            this.synccontext?.Send(new SendOrPostCallback(delegate { this.label_step.Text = e.Value; }), null);
        }

        private void MyInstaller_CurrentProgress(object sender, Classes.IntEventArgs e)
        {
            this.synccontext?.Post(new SendOrPostCallback(delegate 
            {
                if (e.Value >= this.progressBar1.Minimum && e.Value <= this.progressBar1.Maximum)
                    this.progressBar1.Value = e.Value;
            }), null);
        }

        private void MyInstaller_TotalProgress(object sender, Classes.IntEventArgs e)
        {
            this.synccontext?.Send(new SendOrPostCallback(delegate { this.progressBar1.Maximum = e.Value; }), null);            
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Installer.Cancel();
        }
    }
}
