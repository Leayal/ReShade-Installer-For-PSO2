using System;
using System.IO;
using Leayal;
using SharpCompress.Readers;
using SharpCompress.Archives.SevenZip;

namespace ReShade_Installer_For_PSO2.Classes
{
    class HybridInstaller : Installer
    {
        public HybridInstaller() : base() { }

        protected override void Install(string path, InstallationType type, bool pluginSystem)
        {
            Exception extractException = null;
            string sfx2destination,
                existingPresetpath = Path.Combine(path, "reshade-shaders", "pso2.ini");

            IntEventArgs current = new IntEventArgs(0);
            StringEventArgs step = new StringEventArgs(string.Empty);

            Stream reshadearchiveStream = AppInfo.CurrentAssembly.GetManifestResourceStream("ReShade_Installer_For_PSO2.Archives.ReShade-SweetFX2.0.8.7z");
            SevenZipArchive reshadearchive = SevenZipArchive.Open(reshadearchiveStream);

            using (Stream archiveStream = Leayal.AppInfo.CurrentAssembly.GetManifestResourceStream("ReShade_Installer_For_PSO2.Archives.ReShade3.7z"))
            using (SevenZipArchive archive = SevenZipArchive.Open(archiveStream))
            using (IReader reader = archive.ExtractAllEntries())
            {
                this.OnTotalProgress(new IntEventArgs(archive.Entries.Count + reshadearchive.Entries.Count));
                string fullname;
                

                while (reader.MoveToNextEntry())
                    if (!reader.Entry.IsDirectory)
                    {
                        current.Value++;
                        this.OnCurrentProgress(current);
                        step.Value = $"Extracting: {reader.Entry.Key}";
                        this.OnCurrentStep(step);
                        if (reader.Entry.Key.IsEqual("reshade3.dll", true))
                        {
                            if (type == InstallationType.Wrapper)
                                fullname = Path.Combine(path, "d3d9.dll");
                            else
                            {
                                if (pluginSystem)
                                    fullname = Path.Combine(path, reader.Entry.Key);
                                else
                                    fullname = Path.Combine(path, "ddraw.dll");
                            }
                        }
                        else if (reader.Entry.Key.StartsWith("reshade-shaders", StringComparison.OrdinalIgnoreCase))
                        {
                            if ((type == InstallationType.Safe) && pluginSystem)
                                fullname = Path.GetFullPath(Path.Combine(path, "..", reader.Entry.Key));
                            else
                                fullname = Path.Combine(path, reader.Entry.Key);
                            if (reader.Entry.Key.IsEqual($"reshade-shaders{Path.DirectorySeparatorChar}pso2.ini", true) || reader.Entry.Key.IsEqual($"reshade-shaders{Path.AltDirectorySeparatorChar}pso2.ini", true))
                            {
                                existingPresetpath = fullname;
                                if (File.Exists(fullname))
                                    continue;
                            }
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
                using (reshadearchiveStream)
                using (reshadearchive)
                using (IReader reader = reshadearchive.ExtractAllEntries())
                {
                    string fullname;
                    while (reader.MoveToNextEntry())
                        if (!reader.Entry.IsDirectory)
                        {
                            current.Value++;
                            this.OnCurrentProgress(current);
                            step.Value = $"Extracting: {reader.Entry.Key}";
                            this.OnCurrentStep(step);
                            if (reader.Entry.Key.IsEqual("reshade-sweetfx2.dll", true))
                                continue;
                            else if (reader.Entry.Key.StartsWith("sweetfx2", StringComparison.OrdinalIgnoreCase))
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

            if (extractException != null)
                throw extractException;

            Leayal.Ini.IniFile iniFile;
            if (type == InstallationType.Wrapper)
                iniFile = new Leayal.Ini.IniFile(Path.Combine(path, "d3d9.ini"));
            else
            {
                if (pluginSystem)
                    iniFile = new Leayal.Ini.IniFile(Path.Combine(path, "ReShade3.ini"));
                else
                    iniFile = new Leayal.Ini.IniFile(Path.Combine(path, "ddraw.ini"));
            }

            string effectroot;
            if ((type == InstallationType.Safe) && pluginSystem)
                effectroot = Path.GetFullPath(Path.Combine(path, "..", "reshade-shaders"));
            else
                effectroot = Path.Combine(path, "reshade-shaders");
            iniFile.SetValue("GENERAL", "EffectSearchPaths", $"{Path.Combine(sfx2destination, "SweetFX2")},{Path.Combine(effectroot, "Shaders")}");
            iniFile.SetValue("GENERAL", "TextureSearchPaths", Path.Combine(effectroot, "Textures"));
            iniFile.SetValue("GENERAL", "PerformanceMode", "1");
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
    }
}
