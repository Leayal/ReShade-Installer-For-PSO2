using System;
using System.IO;
using Leayal;

namespace ReShade_Installer_For_PSO2.Classes
{
    public static class Resources
    {
        public static class Filenames
        {
            public const string SweetFXShaders = "SweetFX2.0.8.7z";
            public const string ReShadeShaders = "reshade-shaders.7z";
            public const string ReShadeHook = "ReShade3.7z";
        }
        public static Stream GetSweetFXShaders()
        {
            return AppInfo.EntryAssembly.GetManifestResourceStream($"ReShade_Installer_For_PSO2.Archives.{Filenames.SweetFXShaders}");
        }

        public static Stream GetReShadeShaders()
        {
            return AppInfo.EntryAssembly.GetManifestResourceStream($"ReShade_Installer_For_PSO2.Archives.{Filenames.ReShadeShaders}");
        }

        public static Stream GetReShadeHook()
        {
            return AppInfo.EntryAssembly.GetManifestResourceStream($"ReShade_Installer_For_PSO2.Archives.{Filenames.ReShadeHook}");
        }
    }
}
