using System;
using System.Text.RegularExpressions;

namespace ScrumPoker.ColorModel
{
    public partial struct CssColor
    {
        public struct ByteOrFloat : IEquatable<ByteOrFloat>
        {
            private static readonly Regex RgbValueRegex = new Regex(@"0x(?<h>[a-f\d]{1,2})|(?<n>\d+)(?<p>%)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            private readonly byte _byte;
            private readonly float? _float;

            public ByteOrFloat(byte value)
            {
                _byte = value;
                _float = null;
            }

            public ByteOrFloat(float value)
            {
                _float = NormalizePercentage(value);
                _byte = 0;
            }

            public byte ByteValue { get { return _float.HasValue ? (byte)(Math.Round(_float.Value) * 255f) : _byte; } }
            
            public float FloatValue { get { return _float.HasValue ? _float.Value : (float)Math.Round(_byte / 255f); } }

            public bool IsFloatingPoint { get { return _float.HasValue; } }

            public bool Equals(ByteOrFloat other)
            {
                throw new NotImplementedException();
            }

            public override bool Equals(object obj) { return obj != null && obj is ByteOrFloat && Equals((ByteOrFloat)obj); }

            public override int GetHashCode() {return _float.HasValue ? (byte)(Math.Round(_float.Value) * 255f) : _byte;  }

            public string ToString(bool asRgbValue)
            {
                return asRgbValue ?
                    (_float.HasValue ? (_float.Value * 100f).ToString() + "%" : _byte.ToString()) :
                    _float.HasValue ? _float.Value.ToString() : _byte.ToString("x2");
            }

            public override string ToString() { return ToString(false); }
            
            public static bool TryParse(string value, out ByteOrFloat result)
            {
                Match match = RgbValueRegex.Match(value = value.Trim());
                if (!match.Success)
                {
                    result = default(ByteOrFloat);
                    return false;
                }
                Group g = match.Groups["h"];
                if (g.Success)
                    result = new ByteOrFloat(byte.Parse(g.Value, System.Globalization.NumberStyles.HexNumber));
                else if (match.Groups["p"].Success)
                    result = new ByteOrFloat(float.Parse(match.Groups["n"].Value) / 100f);
                else
                    result = new ByteOrFloat(byte.Parse(value, System.Globalization.NumberStyles.Integer));
                return true;
            }
        }
    }
}
