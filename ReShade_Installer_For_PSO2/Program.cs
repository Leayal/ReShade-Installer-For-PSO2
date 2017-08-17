using System;
using Microsoft.VisualBasic.ApplicationServices;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using ReShade_Installer_For_PSO2.Classes;

namespace ReShade_Installer_For_PSO2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ResolveEventHandler ev = new ResolveEventHandler(AssemblyLoader.AssemblyResolve);
            AppDomain.CurrentDomain.AssemblyResolve += ev;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string[] cmds = System.Environment.GetCommandLineArgs();

            string installationpath = string.Empty,
                sweetver = string.Empty;
            bool hasplugin = false, safemode = false;

            for (int i = 0; i < cmds.Length; i++)
            {
                if (cmds[i].StartsWith("-path:", StringComparison.OrdinalIgnoreCase))
                {
                    if (cmds[i].Length > 6)
                        installationpath = cmds[i].Remove(0, 6);
                }
                else if (cmds[i].StartsWith("-hasplugin", StringComparison.OrdinalIgnoreCase))
                    hasplugin = true;
                else if (cmds[i].StartsWith("-ver:", StringComparison.OrdinalIgnoreCase))
                {
                    if (cmds[i].Length > 6)
                        sweetver = cmds[i].Remove(0, 5);
                }
                else if (cmds[i].StartsWith("-safe", StringComparison.OrdinalIgnoreCase))
                    safemode = true;
            }

            if (string.IsNullOrWhiteSpace(installationpath) || string.IsNullOrWhiteSpace(sweetver))
            {
                AppController myController = new AppController();
                myController.Run(cmds);
                AppDomain.CurrentDomain.AssemblyResolve -= ev;
            }
            else
            {
                Classes.Installer installer;
                switch(sweetver.ToLower())
                {
                    case "sweetfx2":
                        installer = Classes.Installer.Create(Classes.Version.SweetFX2);
                        break;
                    case "reshade":
                        installer = Classes.Installer.Create(Classes.Version.ReShade);
                        break;
                    case "both":
                        installer = Classes.Installer.Create(Classes.Version.Both);
                        break;
                    default:
                        Environment.Exit(3);
                        return;
                }                
                Forms.InstallingForm progressForm = new Forms.InstallingForm(installer);
                progressForm.Show();
                installer.InstallationFinished += Installer_InstallationFinished;

                if (!safemode)
                    installer.InstallTo(installationpath);
                else
                    installer.InstallTo(installationpath, hasplugin);
            }
        }

        private static void Installer_InstallationFinished(object sender, InstallationFinishedEventArgs e)
        {
            Classes.Installer installer = sender as Classes.Installer;
            installer.InstallationFinished += Installer_InstallationFinished;
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.ToString(), "Error while installing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(2);
            }
            else if (e.Cancelled)
            {
                MessageBox.Show($"The installation has been cancelled.", "Installation Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(1);
            }
            else
            {
                // MessageBox.Show($"Installation completed successfully.", "Installation Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
            }
        }

        private class AppController : WindowsFormsApplicationBase
        {
            public AppController() : base(AuthenticationMode.Windows)
            {
                this.ShutdownStyle = Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses;
                this.IsSingleInstance = true;
                this.SaveMySettingsOnExit = false;
            }

            protected override bool OnInitialize(ReadOnlyCollection<string> commandLineArgs)
            {
                this.EnableVisualStyles = true;
                return base.OnInitialize(commandLineArgs);
            }

            protected override void OnCreateMainForm()
            {
                this.MainForm = new Forms.MyMainMenu();
            }

            protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
            {
                if (this.MainForm != null)
                    this.MainForm.Activate();
                base.OnStartupNextInstance(eventArgs);
            }
        }
    }
}
