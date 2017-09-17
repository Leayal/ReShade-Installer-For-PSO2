using System;
using System.IO;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using SharpCompress.Readers;
using Leayal;
using Leayal.Net;
using System.Net;
using System.Collections.Generic;
using Leayal.IO;

namespace ReShade_Installer_For_PSO2.Classes
{
    class ReShadeInstaller : Installer
    {
        internal ReShadeInstaller() : base() { }

        protected override bool Prepare(Dictionary<string, Uri> componentlist)
        {
            componentlist.Add(Resources.Filenames.ReShadeShaders, new Uri(Resources.Uri.ReShadeShaders));
            componentlist.Add(Resources.Filenames.ReShadeHook, new Uri(Resources.Uri.ReShadeHook));
            return true;
        }

        protected override void Install(string path, InstallationType type, bool pluginSystem, Dictionary<string, RecyclableMemoryStream> componentlist)
        {
            this.AllowCancel = false;
            Exception extractException = null;

            int current = 0;
            string step = string.Empty;

            string existingPresetpath = Path.Combine(path, "reshade-shaders", "pso2.ini");
            string reshadehooklocation = null;

            using (Leayal.IO.RecyclableMemoryStream archiveStream = componentlist[Resources.Filenames.ReShadeShaders])
            {
                archiveStream.Position = 0;

                using (ZipArchive archive = ZipArchive.Open(archiveStream))
                using (IReader reader = archive.ExtractAllEntries())
                {
                    this.SetProgressTotal(archive.Entries.Count + 1);
                    string fullname, relativeName;

                    while (reader.MoveToNextEntry())
                        if (!reader.Entry.IsDirectory)
                        {
                            current++;
                            this.SetProgressCurrent(current);
                            step = $"Extracting: {reader.Entry.Key}";
                            this.SetProgressStep(step);
                            if (reader.Entry.Key.StartsWith("reshade-shaders-", StringComparison.OrdinalIgnoreCase))
                            {
                                relativeName = RemoveFirstDirectory(reader.Entry.Key);
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
                            current++;
                            this.SetProgressCurrent(current);
                        }
                }
            }
            if (extractException != null)
                throw extractException;

            using (Leayal.IO.RecyclableMemoryStream memStream = componentlist[Resources.Filenames.ReShadeHook])
            {
                memStream.Position = 0;

                using (SevenZipArchive archive = SevenZipArchive.Open(memStream))
                using (IReader reader = archive.ExtractAllEntries())
                {
                    while (reader.MoveToNextEntry())
                        if (!reader.Entry.IsDirectory)
                        {
                            current++;
                            this.SetProgressCurrent(current);
                            step = $"Extracting: {reader.Entry.Key}";
                            this.SetProgressStep(step);
                            if (reader.Entry.Key.IsEqual("reshade3.dll", true))
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

            Leayal.Ini.IniFile iniFile = new Leayal.Ini.IniFile(Path.ChangeExtension(reshadehooklocation, ".ini"));

            string effectroot;
            if ((type == InstallationType.Safe) && pluginSystem)
                effectroot = Path.GetFullPath(Path.Combine(path, "..", "reshade-shaders"));
            else
                effectroot = Path.Combine(path, "reshade-shaders");
            iniFile.SetValue("GENERAL", "EffectSearchPaths", Path.Combine(effectroot, "Shaders"));
            iniFile.SetValue("GENERAL", "TextureSearchPaths", Path.Combine(effectroot, "Textures"));
            iniFile.SetValue("GENERAL", "PerformanceMode", "0");
            iniFile.SetValue("GENERAL", "ScreenshotPath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SEGA", "PHANTASYSTARONLINE2", "pictures", "ReShade"));
            iniFile.SetValue("GENERAL", "TutorialProgress", "4");
            iniFile.SetValue("GENERAL", "PresetFiles", Path.GetFullPath(existingPresetpath));
            iniFile.SetValue("GENERAL", "CurrentPreset", "0");
            iniFile.SetValue("GENERAL", "ScreenshotFormat", "1");
            iniFile.SetValue("GENERAL", "ShowClock", "0");
            iniFile.SetValue("GENERAL", "ShowFPS", "0");
            iniFile.SetValue("INPUT", "KeyMenu", "112,0,1");
            // iniFile.SetValue("INPUT", "KeyEffects", "145,0,0");
            iniFile.Save(System.Text.Encoding.ASCII);
        }

        private string RemoveFirstDirectory(string path)
        {
            int thesearch = path.IndexOf('\\');
            if (thesearch > -1)
                return path.Substring(thesearch + 1);
            else
            {
                thesearch = path.IndexOf('/');
                if (thesearch > -1)
                    return path.Substring(thesearch + 1);
                else
                    return path;
            }
        }
    }
}
