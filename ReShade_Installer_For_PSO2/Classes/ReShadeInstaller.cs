using System;
using System.IO;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Readers;
using Leayal;

namespace ReShade_Installer_For_PSO2.Classes
{
    class ReShadeInstaller : Installer
    {
        internal ReShadeInstaller() : base() { }

        protected override void Install(string path, bool pluginSystem)
        {
            Exception extractException = null;
            
            string existingPresetpath = Path.Combine(path, "reshade-shaders", "pso2.ini");

            using (Stream archiveStream = Leayal.AppInfo.CurrentAssembly.GetManifestResourceStream("ReShade_Installer_For_PSO2.Archives.ReShade3.7z"))
            using (SevenZipArchive archive = SevenZipArchive.Open(archiveStream))
            using (IReader reader = archive.ExtractAllEntries())
            {
                this.OnTotalProgress(new IntEventArgs(archive.Entries.Count));
                string fullname;
                IntEventArgs current = new IntEventArgs(0);
                StringEventArgs step = new StringEventArgs(string.Empty);

                while (reader.MoveToNextEntry())
                    if (!reader.Entry.IsDirectory)
                    {
                        current.Value++;
                        this.OnCurrentProgress(current);
                        step.Value = $"Extracting: {reader.Entry.Key}";
                        this.OnCurrentStep(step);
                        if (reader.Entry.Key.IsEqual("reshade3.dll", true))
                        {
                            if (pluginSystem)
                                fullname = Path.Combine(path, reader.Entry.Key);
                            else
                                fullname = Path.Combine(path, "ddraw.dll");
                        }
                        else if (reader.Entry.Key.IsEqual($"reshade-shaders{Path.DirectorySeparatorChar}pso2.ini", true) || reader.Entry.Key.IsEqual($"reshade-shaders{Path.AltDirectorySeparatorChar}pso2.ini", true))
                        {
                            fullname = Path.Combine(path, reader.Entry.Key);
                            existingPresetpath = fullname;
                            if (File.Exists(fullname))
                                continue;
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

            Leayal.Ini.IniFile iniFile;
            if (pluginSystem)
                iniFile = new Leayal.Ini.IniFile(Path.Combine(path, "ReShade3.ini"));
            else
                iniFile = new Leayal.Ini.IniFile(Path.Combine(path, "ddraw.ini"));
            iniFile.SetValue("GENERAL", "EffectSearchPaths", Path.Combine(path, "reshade-shaders", "Shaders"));
            iniFile.SetValue("GENERAL", "TextureSearchPaths", Path.Combine(path, "reshade-shaders", "Textures"));
            iniFile.SetValue("GENERAL", "PerformanceMode", "1");
            iniFile.SetValue("GENERAL", "ScreenshotPath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SEGA", "PHANTASYSTARONLINE2", "pictures", "ReShade"));
            iniFile.SetValue("GENERAL", "TutorialProgress", "4");
            iniFile.SetValue("GENERAL", "PresetFiles", existingPresetpath);
            iniFile.SetValue("GENERAL", "CurrentPreset", "0");
            iniFile.SetValue("GENERAL", "ScreenshotFormat", "1");
            iniFile.SetValue("GENERAL", "ShowClock", "0");
            iniFile.SetValue("GENERAL", "ShowFPS", "0");
            // iniFile.SetValue("INPUT", "KeyMenu", "0,0,0");
            // iniFile.SetValue("INPUT", "KeyEffects", "145,0,0");
            iniFile.Save(System.Text.Encoding.ASCII);
        }
    }
}
