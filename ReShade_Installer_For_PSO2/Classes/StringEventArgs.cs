using System;

namespace ReShade_Installer_For_PSO2.Classes
{
    public class StringEventArgs : EventArgs
    {
        public string Value { get; internal set; }
        public StringEventArgs(string str) : base()
        {
            this.Value = str;
        }
    }
}
