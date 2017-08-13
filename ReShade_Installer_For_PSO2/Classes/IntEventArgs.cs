using System;

namespace ReShade_Installer_For_PSO2.Classes
{
    public class IntEventArgs : EventArgs
    {
        public int Value { get; internal set; }
        public IntEventArgs(int number) : base()
        {
            this.Value = number;
        }
    }
}
