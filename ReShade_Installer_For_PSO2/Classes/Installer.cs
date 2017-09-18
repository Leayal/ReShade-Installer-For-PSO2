using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace ReShade_Installer_For_PSO2.Classes
{
    public abstract class Installer
    {
        private BackgroundWorker worker;
        private IntEventArgs progress_current;
        private IntEventArgs progress_total;
        private StringEventArgs step;

        protected Installer()
        {
            this.progress_current = new IntEventArgs(0);
            this.progress_total = new IntEventArgs(100);
            this.step = new StringEventArgs(string.Empty);

            this.worker = new BackgroundWorker();
            this.worker.WorkerReportsProgress = false;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += this.Worker_DoWork;
            this.worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
            this.AllowCancel = true;
        }

        public bool IsBusy => this.worker.IsBusy;

        private bool _allowCancel;
        public bool AllowCancel
        {
            get => this._allowCancel;
            set
            {
                if (this._allowCancel != value)
                {
                    this._allowCancel = value;
                    this.OnAllowCancelChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler AllowCancelChanged;
        protected virtual void OnAllowCancelChanged(EventArgs e)
        {
            this.AllowCancelChanged?.Invoke(this, e);
        }

        public static Installer Create(Version version)
        {
            switch (version)
            {
                case Version.ReShade:
                    return new ReShadeInstaller();
                case Version.SweetFX2:
                    return new SweetFX2Installer();
                case Version.Both:
                    return new HybridInstaller();
                default:
                    throw new InvalidOperationException();
            }
        }

        public void InstallTo(string path)
        {
            if (!this.worker.IsBusy)
                this.worker.RunWorkerAsync(new object[] { path, false, InstallationType.Wrapper });
        }

        public void InstallTo(string path, bool pluginSystem)
        {
            if (!this.worker.IsBusy)
                this.worker.RunWorkerAsync(new object[] { path, pluginSystem, InstallationType.Safe });
        }

        public static bool IsPSO2Directory(string path)
        {
            if (File.Exists(Path.Combine(path, "gameguard.des")))
                if (File.Exists(Path.Combine(path, "pso2launcher.exe")))
                    if (File.Exists(Path.Combine(path, "PSO2JP.ini")))
                        return true;
            return false;
        }

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
            InstallationType type = (InstallationType)objs[2];
            if ((type == InstallationType.Safe) && pluginSystem)
                path = Path.Combine(path, "Plugins");

            Dictionary<string, Uri> componentlist = new Dictionary<string, Uri>();
            if (!this.Prepare(componentlist))
            { e.Cancel = true; return; }
            Dictionary<string, Leayal.IO.RecyclableMemoryStream> memorylist;
            if (!this.Download(componentlist, out memorylist))
            { e.Cancel = true; this.CleanupMemory(memorylist); return; }
            try { this.Install(path, type, pluginSystem, memorylist); }
            // Dangerous!!!!
            catch (Exception ex)
            {
                this.CleanupMemory(memorylist);
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw ex;
            }
        }
        protected virtual bool Prepare(Dictionary<string, Uri> componentlist)
        { return true; }
        protected virtual bool DownloadComponent(Leayal.Net.ExtendedWebClient webclient, string componentName, Uri componentUri, Stream writeStream, int index, int total)
        {
            this.SetProgressStep($"[{index}/{total}] Sending request to `{componentUri.OriginalString}`");
            var request = webclient.CreateRequest(componentUri);
            var response = request.GetResponse();
            if (response != null)
            {
                System.Net.HttpWebResponse httprep = response as System.Net.HttpWebResponse;
                if (httprep != null)
                {
                    if (httprep.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        this.SetProgressStep($"Downloading [{index}/{total}]: {componentName}");
                        if (httprep.ContentLength > 0)
                        {
                            this.SetProgressTotal(100);
                            writeStream.SetLength(httprep.ContentLength);
                        }
                        using (Stream remoteStream = httprep.GetResponseStream())
                        using (Leayal.ByteBuffer buffer = new Leayal.ByteBuffer(1024))
                        {
                            if (this.worker.CancellationPending)
                                return false;
                            int readbytes = remoteStream.Read(buffer, 0, buffer.Length);
                            long progressedbytes = 0;
                            while (readbytes > 0)
                            {
                                if (this.worker.CancellationPending)
                                    return false;
                                if (httprep.ContentLength > 0)
                                {
                                    progressedbytes += readbytes;
                                    this.SetProgressCurrent(Convert.ToInt32((progressedbytes * 100d) / httprep.ContentLength));
                                }
                                writeStream.Write(buffer, 0, readbytes);
                                readbytes = remoteStream.Read(buffer, 0, buffer.Length);
                            }
                        }
                        return true;
                    }
                }
                else
                {
                    this.SetProgressStep($"Downloading [{index}/{total}]: {componentName}");
                    if (response.ContentLength > 0)
                    {
                        this.SetProgressTotal(100);
                        writeStream.SetLength(response.ContentLength);
                    }
                    using (Stream remoteStream = response.GetResponseStream())
                    using (Leayal.ByteBuffer buffer = new Leayal.ByteBuffer(1024))
                    {
                        int readbytes = remoteStream.Read(buffer, 0, buffer.Length);
                        long progressedbytes = 0;
                        while (readbytes > 0)
                        {
                            if (response.ContentLength > 0)
                            {
                                progressedbytes += readbytes;
                                this.SetProgressCurrent(Convert.ToInt32((progressedbytes * 100d) / response.ContentLength));
                            }
                            writeStream.Write(buffer, 0, readbytes);
                            readbytes = remoteStream.Read(buffer, 0, buffer.Length);
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        protected virtual bool Download(Dictionary<string, Uri> componentlist, out Dictionary<string, Leayal.IO.RecyclableMemoryStream> memorylist)
        {
            if (componentlist.Count > 0)
            {
                memorylist = new Dictionary<string, Leayal.IO.RecyclableMemoryStream>();
                Leayal.IO.RecyclableMemoryStream memStream;
                int indexing = 0;

                using (Leayal.Net.ExtendedWebClient webclient = new Leayal.Net.ExtendedWebClient(new Leayal.Net.WebClient()
                {
                    AutoRedirect = true,
                    UserAgent = "Please let me use your content, thank you."
                }))
                    foreach (var component in componentlist)
                    {
                        indexing++;
                        if (this.worker.CancellationPending)
                            return false;
                        memStream = new Leayal.IO.RecyclableMemoryStream();
                        if (this.DownloadComponent(webclient, component.Key, component.Value, memStream, indexing, componentlist.Count))
                        {
                            if (memStream.Length == 0)
                            {
                                memStream.Dispose();
                                memorylist.Add(component.Key, null);
                            }
                            else
                                memorylist.Add(component.Key, memStream);
                        }
                    }
            }
            else
                memorylist = null;

            if (this.worker.CancellationPending)
                return false;
            else
                return true;
        }
        protected abstract void Install(string path, InstallationType type, bool pluginSystem, Dictionary<string, Leayal.IO.RecyclableMemoryStream> componentlist);

        /// <summary>
        /// Generate a string that contains all hook filenames. Return null if found nothing.
        /// </summary>
        public static string CheckPreviousInstallation(string path)
        {
            // Just check for hooking file is enough already (???)
            bool wrapperdx9Exist = false,
                wrapperdxdiExist = false,
                safeExist = false,
                reshademaineffect = false;
            List<string> safePluginExist = new List<string>();

            if (File.Exists(Path.Combine(path, "d3d9.dll")))
                wrapperdx9Exist = true;
            if (File.Exists(Path.Combine(path, "dxdi.dll")))
                wrapperdxdiExist = true;
            if (File.Exists(Path.Combine(path, "ddraw.dll")))
                safeExist = true;
            if (File.Exists(Path.Combine(path, "reshade.fx")))
                reshademaineffect = true;
            FileVersionInfo fvi;
            string pluginfolderpath = Path.Combine(path, "Plugins");
            if (Directory.Exists(pluginfolderpath))
                foreach (string filename in Directory.EnumerateFiles(pluginfolderpath, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    fvi = FileVersionInfo.GetVersionInfo(filename);
                    if (Leayal.StringHelper.IsEqual(fvi.OriginalFilename, "reshade32.dll", true) || Leayal.StringHelper.IsEqual(fvi.ProductName, "reshade", true))
                        safePluginExist.Add(filename);
                }

            if (wrapperdx9Exist || wrapperdxdiExist || safeExist || reshademaineffect || (safePluginExist.Count > 0))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("Found old post-processing files in the pso2_bin folder. Please disable or remove them to avoid graphics overlapping:");

                if (wrapperdx9Exist)
                    sb.AppendLine("- d3d9.dll");
                if (wrapperdxdiExist)
                    sb.AppendLine("- dxdi.dll");
                if (safeExist)
                    sb.AppendLine("- ddraw.dll");
                if (reshademaineffect)
                    sb.AppendLine("- reshade.fx (If this file exists, ReShade will execute the script in this file and may result in unexpected behaviors)");
                if (safePluginExist.Count > 0)
                    for (int i = 0; i < safePluginExist.Count; i++)
                        sb.AppendLine($"- Plugins{Path.DirectorySeparatorChar}{Path.GetFileName(safePluginExist[i])}");

                return sb.ToString();
            }
            else
                return null;
        }

        protected void SetProgressCurrent(int current)
        {
            if (this.progress_current.Value != current)
            {
                this.progress_current.Value = current;
                this.OnCurrentProgress(this.progress_current);
            }
        }
        protected void SetProgressTotal(int total)
        {
            if (this.progress_total.Value != total)
            {
                this.progress_total.Value = total;
                this.OnTotalProgress(this.progress_total);
            }
        }
        protected void SetProgressStep(string step)
        {
            if (this.step.Value != step)
            {
                this.step.Value = step;
                this.OnCurrentStep(this.step);
            }
        }

        protected virtual void CleanupMemory(Dictionary<string, Leayal.IO.RecyclableMemoryStream> memorylist)
        {
            if (memorylist != null && memorylist.Count > 0)
            {
                foreach (var item in memorylist.Values)
                    item.Dispose();
                memorylist.Clear();
                memorylist = null;
            }
        }
    }
}
