using System;
using Microsoft.VisualBasic.ApplicationServices;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using ReShade_Installer_For_PSO2.Classes;
using System.Runtime.InteropServices;
using System.Drawing;

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
            }
            else
            {
                InstallerAppController controller = new InstallerAppController(installationpath, sweetver, hasplugin, safemode);
                controller.Run(cmds);
            }
            AppDomain.CurrentDomain.AssemblyResolve -= ev;
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

        private class InstallerAppController : WindowsFormsApplicationBase
        {
            private string _installationpath = string.Empty,
                _sweetver = string.Empty;
            private bool _hasplugin = false, _safemode = false;

            public InstallerAppController(string installationpath, string sweetver, bool hasplugin, bool safemode) : base(AuthenticationMode.Windows)
            {
                this.ShutdownStyle = Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses;
                this.IsSingleInstance = false;
                this.SaveMySettingsOnExit = false;
                this._installationpath = installationpath;
                this._sweetver = sweetver;
                this._hasplugin = hasplugin;
                this._safemode = safemode;
            }

            protected override bool OnInitialize(ReadOnlyCollection<string> commandLineArgs)
            {
                this.EnableVisualStyles = true;
                return base.OnInitialize(commandLineArgs);
            }

            private void Installer_InstallationFinished(object sender, InstallationFinishedEventArgs e)
            {
                Classes.Installer installer = sender as Classes.Installer;
                installer.InstallationFinished += Installer_InstallationFinished;
                if (e.Error != null)
                {
                    if (Console.Error != null)
                        Console.Error.Write(e.Error.ToString());
                    System.Environment.ExitCode = 2;
                }
                else if (e.Cancelled)
                    System.Environment.ExitCode = 1;
                else
                    System.Environment.ExitCode = 0;
                Application.Exit();
            }

            protected override bool OnStartup(StartupEventArgs eventArgs)
            {
                Classes.Installer installer;
                switch (_sweetver.ToLower())
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
                        System.Environment.ExitCode = 3;
                        Application.Exit();
                        return false;
                }
                Forms.InstallingForm progressForm = new Forms.InstallingForm(installer);
                installer.InstallationFinished += this.Installer_InstallationFinished;

                int parentProcessID = Leayal.WMI.ProcessParent.GetCurrentProcessParentID();
                if (parentProcessID > -1)
                    try
                    {
                        using (System.Diagnostics.Process parentProc = System.Diagnostics.Process.GetProcessById(parentProcessID))
                        {
                            Rectangle rectangle = this.GetWindowRectangle(parentProc.MainWindowHandle);
                            if (!rectangle.IsEmpty)
                                progressForm.Location = new Point(
                                            (rectangle.Left + rectangle.Width / 2) - (progressForm.Size.Width / 2),
                                            (rectangle.Top + rectangle.Height / 2) - (progressForm.Size.Height / 2)
                                            );
                            
                        }
                    }
                    catch { }

                this.MainForm = progressForm;
                if (progressForm != null)
                {
                    if (!_safemode)
                        installer.InstallTo(_installationpath);
                    else
                        installer.InstallTo(_installationpath, _hasplugin);
                }
                return base.OnStartup(eventArgs);
            }

            protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
            {
                if (this.MainForm != null)
                    this.MainForm.Activate();
                base.OnStartupNextInstance(eventArgs);
            }

            private Rectangle GetWindowRectangle(IntPtr handle)
            {
                Rect dundun = new Rect();
                if (handle != IntPtr.Zero)
                    if (GetWindowRect(handle, ref dundun))
                        return dundun;
                return new Rectangle();
            }

            [DllImport("user32.dll")]
            internal static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

            internal struct Rect
            {
                public int Left { get; set; }
                public int Top { get; set; }
                public int Right { get; set; }
                public int Bottom { get; set; }

                public static implicit operator Rectangle(Rect rect)
                {
                    return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                }
            }
        }
    }
}
