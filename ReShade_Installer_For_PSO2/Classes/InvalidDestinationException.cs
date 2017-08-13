using System;

namespace ReShade_Installer_For_PSO2.Classes
{
    class InvalidDestinationException : Exception
    {
        public InvalidDestinationException() : base() { }
        public InvalidDestinationException(string message) : base(message) { }
    }
}
