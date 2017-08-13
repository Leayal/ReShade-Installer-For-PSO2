using System;
using System.Windows.Forms;
using System.Threading;

namespace ReShade_Installer_For_PSO2.Forms
{
    public partial class InstallingForm : Form
    {
        private Classes.Installer myInstaller;
        private SynchronizationContext synccontext;

        public InstallingForm(Classes.Installer installer)
        {
            InitializeComponent();
            this.myInstaller = installer;
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
            this.Icon = Properties.Resources.mainico;
            this.myInstaller.TotalProgress += this.MyInstaller_TotalProgress;
            this.myInstaller.CurrentProgress += this.MyInstaller_CurrentProgress;
            this.myInstaller.CurrentStep += this.MyInstaller_CurrentStep;
            this.myInstaller.InstallationFinished += this.Installer_InstallationFinished;
        }

        private void MyInstaller_CurrentStep(object sender, Classes.StringEventArgs e)
        {
            this.synccontext?.Send(new SendOrPostCallback(delegate { this.label_step.Text = e.Value; }), null);
        }

        private void MyInstaller_CurrentProgress(object sender, Classes.IntEventArgs e)
        {
            this.synccontext?.Post(new SendOrPostCallback(delegate { this.progressBar1.Value = e.Value; }), null);
        }

        private void MyInstaller_TotalProgress(object sender, Classes.IntEventArgs e)
        {
            this.synccontext?.Send(new SendOrPostCallback(delegate { this.progressBar1.Maximum = e.Value; }), null);            
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.myInstaller.Cancel();
        }
    }
}
