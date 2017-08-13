using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Leayal.Forms;
using System.Security.Principal;

namespace ReShade_Installer_For_PSO2.Forms
{
    public partial class MyMainMenu : Form
    {
        private ExtendedToolTip etooltip;
        private SynchronizationContext syncContext;
        private bool IsInstalling = false;

        public MyMainMenu()
        {
            InitializeComponent();
            this.etooltip = new ExtendedToolTip();
            this.etooltip.UseFading = true;
            this.etooltip.Opacity = 0.9d;
            this.etooltip.PreferedSize = new Size(400, 400);
            this.etooltip.Font = new Font(this.Font.FontFamily, 9f);
            this.etooltip.ForeColor = Color.Black;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Icon = Properties.Resources.mainico;
            // Name them right ???
            this.reshade.Tag = Classes.Version.ReShade;
            this.swfxv2.Tag = Classes.Version.SweetFX2;

            this.syncContext = SynchronizationContext.Current;
            this.q_usingArksLayerPlugin_yes.Checked = false;
            this.q_usingArksLayerPlugin_no.Checked = false;
            this.etooltip.SetToolTip(this.q_usingArksLayerPlugin, "Obiviously, PSO2 Plugin System is a system that make pso2 game support plugins. The system is made by Arks-Layer's staff.\nPSO2 Tweaker use this plugin system. Therefore, if you're using Tweaker, please select 'Yes'.");

            /*string pso2dir = Microsoft.Win32.Registry.GetValue(System.IO.Path.Combine(Microsoft.Win32.Registry.CurrentUser.Name, "AIDA"), "PSO2Dir", string.Empty) as string;
            if (string.IsNullOrWhiteSpace(pso2dir))
                pso2dir = Microsoft.Win32.Registry.GetValue(System.IO.Path.Combine(Microsoft.Win32.Registry.CurrentUser.Name, "AIDA"), "PSO2Dir", string.Empty) as string;
            //*/
            this.textBox1.Text = Microsoft.Win32.Registry.GetValue(System.IO.Path.Combine(Microsoft.Win32.Registry.CurrentUser.Name, "Software", "AIDA"), "PSO2Dir", string.Empty) as string;

            if (WindowsIdentity.GetCurrent().Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid))
            {
                Bitmap icon = new Bitmap(this.button_Install.Height - 6, this.button_Install.Height - 6);
                using (var shield = SystemIcons.Shield.ToBitmap())
                {
                    shield.MakeTransparent();
                    using (var grIcon = Graphics.FromImage(icon))
                    {
                        grIcon.Clear(Color.Transparent);
                        grIcon.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        grIcon.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        grIcon.DrawImage(shield, new Rectangle(Point.Empty, icon.Size), new Rectangle(Point.Empty, shield.Size), GraphicsUnit.Pixel);
                        grIcon.Flush();
                    }
                }
                
                this.button_Install.ImageAlign = ContentAlignment.MiddleRight;
                this.button_Install.TextImageRelation = TextImageRelation.ImageBeforeText;
                this.button_Install.Image = icon;
            }
        }

        private void SetEnabled(bool enabled)
        {
            this.panel1.Enabled = enabled;
            this.groupBox1.Enabled = enabled;
            this.textBox1.Enabled = enabled;
            this.button_browse.Enabled = enabled;
            this.button_Install.Enabled = enabled;
        }

        private bool NeedAdministration(string directory)
        {
            try
            {
                System.IO.File.Create(System.IO.Path.Combine(directory, DateTime.Now.ToBinary().ToString()), 1, System.IO.FileOptions.DeleteOnClose).Close();
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (this.etooltip != null)
                this.etooltip.Dispose();
            base.OnFormClosed(e);
        }

        private void button_Install_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBox1.Text) || !System.IO.Directory.Exists(this.textBox1.Text))
            {
                MessageBox.Show(this, "Please select PSO2 game directory.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (!this.q_usingArksLayerPlugin_yes.Checked && !this.q_usingArksLayerPlugin_no.Checked)
            {
                MessageBox.Show(this, $"Please answer the question '{this.q_usingArksLayerPlugin.Text}' above.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Control selectedver = null;
            if (this.groupBox1.HasChildren)
                foreach (Control c in this.groupBox1.Controls)
                {
                    RadioButton rb = c as RadioButton;
                    if (rb != null)
                    {
                        if (rb.Checked)
                            selectedver = c;
                    }
                }
            if (selectedver == null)
            {
                MessageBox.Show(this, $"Please select the post-processing version in '{this.groupBox1.Text}' above.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (!Classes.Installer.IsPSO2Directory(this.textBox1.Text))
            {
                if (MessageBox.Show(this, "The selected folder doesn't seem to be PSO2 game directory.\nAre you sure you still want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
            }

            if (MessageBox.Show(this, $"Installing to:\n{this.textBox1.Text}\n\nAlthough this is an installer. There will be no uninstaller. Continue???", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                try
                {

                    this.SetEnabled(false);
                    this.IsInstalling = true;


                    if (this.NeedAdministration(this.textBox1.Text))
                    {
                        if (Classes.FlexibleMessageBox.Show(this, "Installer can not write files to the destination.\nMaybe it requires elevated access, do you want to try to install as Admin now?", "Administrator elevation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
                            {
                                proc.StartInfo.FileName = Leayal.AppInfo.ApplicationFilename;
                                string ver = string.Empty;
                                switch ((Classes.Version)selectedver.Tag)
                                {
                                    case Classes.Version.SweetFX2:
                                        ver = "sweetfx2";
                                        break;
                                    case Classes.Version.ReShade:
                                        ver = "reshade";
                                        break;
                                    default:
                                        return;
                                }
                                if (this.q_usingArksLayerPlugin_yes.Checked)
                                    proc.StartInfo.Arguments = Leayal.ProcessHelper.TableStringToArgs(new string[] { "-hasplugin", $"-path:{this.textBox1.Text}", $"-ver:{ver}" });
                                else
                                    proc.StartInfo.Arguments = Leayal.ProcessHelper.TableStringToArgs(new string[] { $"-path:{this.textBox1.Text}", $"-ver:{ver}" });
                                proc.StartInfo.Verb = "runas";
                                proc.Start();
                                proc.WaitForExit();
                                if (proc.ExitCode == 0)
                                {
                                    MessageBox.Show(this, $"{selectedver.Text} has been installed successfully.", "Installation Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    Application.Exit();
                                }
                            }
                        }
                        this.SetEnabled(true);
                        this.IsInstalling = false;
                    }
                    else
                    {
                        Classes.Installer installer = Classes.Installer.Create((Classes.Version)selectedver.Tag);
                        InstallingForm progressForm = new InstallingForm(installer);
                        progressForm.Show();
                        installer.InstallationFinished += this.Installer_InstallationFinished;

                        installer.InstallTo(this.textBox1.Text, this.q_usingArksLayerPlugin_yes.Checked);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.SetEnabled(true);
                    this.IsInstalling = false;
                }
        }

        private void Installer_InstallationFinished(object sender, Classes.InstallationFinishedEventArgs e)
        {
            this.IsInstalling = false;
            if (e.Error != null)
                MessageBox.Show(this, e.Error.ToString(), "Error while installing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (e.Cancelled)
            {
                MessageBox.Show(this, $"The installation has been cancelled.", "Installation Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Control selectedver = null;
                if (this.groupBox1.HasChildren)
                    foreach (Control c in this.groupBox1.Controls)
                    {
                        RadioButton rb = c as RadioButton;
                        if (rb != null)
                        {
                            if (rb.Checked)
                                selectedver = c;
                        }
                    }
                MessageBox.Show(this, $"{selectedver.Text} has been installed successfully.", "Installation Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
            this.SetEnabled(true);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
                if (this.IsInstalling)
                    e.Cancel = true;
        }

        private void button_browse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.FileName = "pso2.exe";
                ofd.Filter = "PSO2 Executable|pso2.exe";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.DefaultExt = "exe";
                ofd.Multiselect = false;
                ofd.RestoreDirectory = true;
                ofd.Title = "Select pso2's executable file";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    this.textBox1.Text = Microsoft.VisualBasic.FileIO.FileSystem.GetParentPath(ofd.FileName);
                }
            }
        }

        private void swfx2_source_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form myself = this;
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate {
                try { System.Diagnostics.Process.Start("https://reshade.me/sweetfx#download"); }
                catch (Exception ex)
                { this.syncContext?.Post(new SendOrPostCallback(delegate { MessageBox.Show(myself, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }), null); }
            }));
        }

        private void reshade_source_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form myself = this;
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate {
                try { System.Diagnostics.Process.Start("https://reshade.me/forum/releases/2341-3-0"); }
                catch (Exception ex)
                { this.syncContext?.Post(new SendOrPostCallback(delegate { MessageBox.Show(myself, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }), null); }
            }));
        }
    }
}
