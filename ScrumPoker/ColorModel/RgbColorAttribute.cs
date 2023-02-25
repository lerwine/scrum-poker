using System;

namespace ScrumPoker.ColorModel
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class RgbColorAttribute : Attribute
    {
        readonly byte _red;
        readonly byte _green;
        readonly byte _blue;
        
        // This is a positional argument
        public RgbColorAttribute(byte red, byte green, byte blue)
        {
            _red = red;
            _green = green;
            _blue = blue;
        }
        
        public byte Red { get { return _red; } }
        
        public byte Green { get { return _green; } }
        
        public byte Blue { get { return _blue; } }
    }
}
