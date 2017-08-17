﻿using System;
using System.IO;
using Leayal;
using SharpCompress.Readers;
using SharpCompress.Archives.SevenZip;

namespace ReShade_Installer_For_PSO2.Classes
{
    class SweetFX2Installer : Installer
    {
        internal SweetFX2Installer() : base() { }

        protected override void Install(string path, InstallationType type, bool pluginSystem)
        {
            Exception extractException = null;

            using (Stream archiveStream = AppInfo.CurrentAssembly.GetManifestResourceStream("ReShade_Installer_For_PSO2.Archives.ReShade-SweetFX2.0.8.7z"))
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
                        if (reader.Entry.Key.IsEqual("reshade-sweetfx2.dll", true))
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
                        else if (reader.Entry.Key.StartsWith("sweetfx2", StringComparison.OrdinalIgnoreCase))
                        {
                            if ((type == InstallationType.Safe) && pluginSystem)
                                fullname = Path.GetFullPath(Path.Combine(path, "..", reader.Entry.Key));
                            else
                                fullname = Path.Combine(path, reader.Entry.Key);
                            if (reader.Entry.Key.IsEqual($"sweetfx2{Path.DirectorySeparatorChar}sweetfx_settings.txt", true) || reader.Entry.Key.IsEqual($"sweetfx2{Path.AltDirectorySeparatorChar}sweetfx_settings.txt", true))
                            {
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

            Leayal.Ini.IniFile iniFile;
            if (type == InstallationType.Wrapper)
                iniFile = new Leayal.Ini.IniFile(Path.Combine(path, "d3d9.ini"));
            else
            {
                if (pluginSystem)
                    iniFile = new Leayal.Ini.IniFile(Path.Combine(path, "ReShade-SweetFX2.ini"));
                else
                    iniFile = new Leayal.Ini.IniFile(Path.Combine(path, "ddraw.ini"));
            }

            string effectroot;
            if ((type == InstallationType.Safe) && pluginSystem)
                effectroot = Path.GetFullPath(Path.Combine(path, "..", "SweetFX2"));
            else
                effectroot = Path.Combine(path, "SweetFX2");

            iniFile.SetValue("GENERAL", "EffectSearchPaths", effectroot);
            iniFile.SetValue("GENERAL", "TextureSearchPaths", Path.Combine(effectroot, "Textures"));
            iniFile.SetValue("GENERAL", "PerformanceMode", "1");
            iniFile.SetValue("GENERAL", "ScreenshotPath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SEGA", "PHANTASYSTARONLINE2", "pictures", "ReShade"));
            iniFile.SetValue("GENERAL", "TutorialProgress", "4");
            iniFile.SetValue("GENERAL", "ScreenshotFormat", "1");
            iniFile.SetValue("GENERAL", "ShowClock", "0");
            iniFile.SetValue("GENERAL", "ShowFPS", "0");
            iniFile.SetValue("INPUT", "KeyMenu", "112,0,1");
            // iniFile.SetValue("INPUT", "KeyEffects", "0,0,0");

            iniFile.Save(System.Text.Encoding.ASCII);
        }
    }
}
