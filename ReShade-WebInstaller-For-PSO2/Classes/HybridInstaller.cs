using System;
using System.IO;
using Leayal;
using SharpCompress.Readers;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using Leayal.IO;
using System.Collections.Generic;

namespace ReShade_Installer_For_PSO2.Classes
{
    class HybridInstaller : Installer
    {
        public HybridInstaller() : base() { }

        protected override bool Prepare(Dictionary<string, Uri> componentlist)
        {
            componentlist.Add(Path.ChangeExtension(Resources.Filenames.ReShadeShaders, ".zip"), new Uri(Resources.Uri.ReShadeShaders));
            componentlist.Add(Resources.Filenames.SweetFXShaders, new Uri(Resources.Uri.SweetFXShaders));
            componentlist.Add(Resources.Filenames.ReShadeHook, new Uri(Resources.Uri.ReShadeHook));
            return true;
        }

        protected override void Install(string path, InstallationType type, bool pluginSystem, Dictionary<string, RecyclableMemoryStream> componentlist)
        {
            this.AllowCancel = false;
            Exception extractException = null;
            string sfx2destination,
                existingPresetpath;

            if ((type == InstallationType.Safe) && pluginSystem)
                existingPresetpath = Path.GetFullPath(Path.Combine(path, "..", "reshade-shaders", "pso2.ini"));
            else
                existingPresetpath = Path.Combine(path, "reshade-shaders", "pso2.ini");

            IntEventArgs current = new IntEventArgs(0);
            StringEventArgs step = new StringEventArgs(string.Empty);

            using (Stream sweetfxarchiveStream = componentlist[Resources.Filenames.SweetFXShaders])
            using (SevenZipArchive sweetfxarchive = SevenZipArchive.Open(sweetfxarchiveStream))
            {
                // Extracting files ReShade shader effect files
                using (Stream archiveStream = componentlist[Path.ChangeExtension(Resources.Filenames.ReShadeShaders, ".zip")])
                using (ZipArchive archive = ZipArchive.Open(archiveStream))
                using (IReader reader = archive.ExtractAllEntries())
                {
                    this.OnTotalProgress(new IntEventArgs(archive.Entries.Count + sweetfxarchive.Entries.Count + 1));
                    string fullname, relativeName;

                    while (reader.MoveToNextEntry())
                        if (!reader.Entry.IsDirectory)
                        {
                            current.Value++;
                            this.OnCurrentProgress(current);
                            step.Value = $"Extracting: {reader.Entry.Key}";
                            this.OnCurrentStep(step);
                            if (reader.Entry.Key.StartsWith("reshade-shaders-", StringComparison.OrdinalIgnoreCase))
                            {
                                relativeName = ReShadeInstaller.RemoveFirstDirectory(reader.Entry.Key);
                                if ((type == InstallationType.Safe) && pluginSystem)
                                    fullname = Path.GetFullPath(Path.Combine(path, "..", "reshade-shaders", relativeName));
                                else
                                    fullname = Path.Combine(path, "reshade-shaders", relativeName);
                            }
                            else
                                fullname = Path.Combine(path, reader.Entry.Key);
                            Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(Microsoft.VisualBasic.FileIO.FileSystem.GetParentPath(fullname));
                            try
                            { reader.WriteEntryToFile(fullname); }
                            catch (Exception ex)
                            { extractException = ex; break; }
                        }
                        else
                        {
                            current.Value++;
                            this.OnCurrentProgress(current);
                        }
                }

                if (extractException != null)
                    throw extractException;
                else
                {
                    if ((type == InstallationType.Safe) && pluginSystem)
                        sfx2destination = Path.GetFullPath(Path.Combine(path, "..", "reshade-shaders"));
                    else
                        sfx2destination = Path.Combine(path, "reshade-shaders");

                    // Extract SweetFX2.0.8 shader effect files

                    using (IReader reader = sweetfxarchive.ExtractAllEntries())
                    {
                        string fullname;
                        while (reader.MoveToNextEntry())
                            if (!reader.Entry.IsDirectory)
                            {
                                current.Value++;
                                this.OnCurrentProgress(current);
                                step.Value = $"Extracting: {reader.Entry.Key}";
                                this.OnCurrentStep(step);
                                if (reader.Entry.Key.StartsWith("sweetfx2", StringComparison.OrdinalIgnoreCase))
                                {
                                    fullname = Path.Combine(sfx2destination, reader.Entry.Key);
                                    if (reader.Entry.Key.IsEqual($"sweetfx2{Path.DirectorySeparatorChar}sweetfx_settings.txt", true) || reader.Entry.Key.IsEqual($"sweetfx2{Path.AltDirectorySeparatorChar}sweetfx_settings.txt", true))
                                    {
                                        if (File.Exists(fullname))
                                            continue;
                                    }
                                }
                                else
                                    continue;
                                Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(Microsoft.VisualBasic.FileIO.FileSystem.GetParentPath(fullname));
                                try
                                { reader.WriteEntryToFile(fullname); }
                                catch (Exception ex)
                                { extractException = ex; break; }
                            }
                            else
                            {
                                current.Value++;
                                this.OnCurrentProgress(current);
                            }
                    }
                }
            }

            if (extractException != null)
                throw extractException;

            if (!File.Exists(existingPresetpath))
                using (FileStream fs = File.Create(existingPresetpath))
                using (Stream contentStream = Resources.GetReShadeDefaultProfile())
                using (ByteBuffer buffer = new ByteBuffer(1024))
                {
                    int readbytes = contentStream.Read(buffer, 0, buffer.Length);
                    while (readbytes > 0)
                    {
                        fs.Write(buffer, 0, readbytes);
                        readbytes = contentStream.Read(buffer, 0, buffer.Length);
                    }
                }

            string reshadehooklocation = null;

            using (Stream archiveStream = componentlist[Resources.Filenames.ReShadeHook])
            using (SevenZipArchive archive = SevenZipArchive.Open(archiveStream))
            using (IReader reader = archive.ExtractAllEntries())
            {
                while (reader.MoveToNextEntry())
                    if (!reader.Entry.IsDirectory)
                    {
                        current.Value++;
                        this.OnCurrentProgress(current);
                        step.Value = $"Extracting: {reader.Entry.Key}";
                        this.OnCurrentStep(step);
                        if (reader.Entry.Key.IsEqual("reshade3.dll", true) || reader.Entry.Key.IsEqual("reshade32.dll", true))
                        {
                            if (type == InstallationType.Wrapper)
                                reshadehooklocation = Path.Combine(path, "d3d9.dll");
                            else
                            {
                                if (pluginSystem)
                                    reshadehooklocation = Path.Combine(path, "ReShade3.dll");
                                else
                                    reshadehooklocation = Path.Combine(path, "ddraw.dll");
                            }
                            Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(Microsoft.VisualBasic.FileIO.FileSystem.GetParentPath(reshadehooklocation));
                            try
                            { reader.WriteEntryToFile(reshadehooklocation); }
                            catch (Exception ex)
                            { extractException = ex; break; }
                        }
                    }
            }

            if (extractException != null)
                throw extractException;

            Leayal.Ini.IniFile iniFile = new Leayal.Ini.IniFile(Path.ChangeExtension(reshadehooklocation, ".ini"));

            string effectroot;
            if ((type == InstallationType.Safe) && pluginSystem)
                effectroot = Path.GetFullPath(Path.Combine(path, "..", "reshade-shaders"));
            else
                effectroot = Path.Combine(path, "reshade-shaders");

            string screenshotfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SEGA", "PHANTASYSTARONLINE2", "pictures", "ReShade");
            Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(screenshotfolder);

            iniFile.SetValue("GENERAL", "EffectSearchPaths", $"{Path.Combine(sfx2destination, "SweetFX2")},{Path.Combine(effectroot, "Shaders")}");
            iniFile.SetValue("GENERAL", "TextureSearchPaths", Path.Combine(effectroot, "Textures"));
            iniFile.SetValue("GENERAL", "PerformanceMode", "0");
            iniFile.SetValue("GENERAL", "ScreenshotPath", screenshotfolder);
            iniFile.SetValue("GENERAL", "TutorialProgress", "0");
            iniFile.SetValue("GENERAL", "PresetFiles", Path.GetFullPath(existingPresetpath));
            iniFile.SetValue("GENERAL", "CurrentPreset", "0");
            iniFile.SetValue("GENERAL", "ScreenshotFormat", "1");
            iniFile.SetValue("GENERAL", "ShowClock", "0");
            iniFile.SetValue("GENERAL", "ShowFPS", "0");
            iniFile.SetValue("INPUT", "KeyMenu", "112,0,1");
            // iniFile.SetValue("INPUT", "KeyEffects", "145,0,0");
            iniFile.Save(System.Text.Encoding.ASCII);
        }
    }
}
