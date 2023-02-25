using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ScrumPoker.ColorModel
{
    public partial struct CssColor : IEquatable<CssColor>
    {
        private readonly ColorStringFormat _defaultStringFormat;
        private readonly NamedColors? _name;
        private readonly ByteOrFloat _red;
        private readonly ByteOrFloat _green;
        private readonly ByteOrFloat _blue;
        private readonly ushort _hue;
        private readonly float _saturation;
        private readonly float _lightness;
        private readonly float? _alpha;
        private static readonly Regex ParseRegex = new Regex(@"^(?:rgb(?<rgba>a)?\((?<rgb>[^\)]+)\)|hsl(?<hsla>a)?\((?<hsl>[^\)]+)\)|#(?<hex>[a-f\d]{3}(?:[a-f\d]|[a-f\d]{3}[a-f\d]{2}?)?)|[a-z]{3,29})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        public NamedColors? Name { get { return _name; } }
        
        public ByteOrFloat Red { get { return _red; } }
        
        public ByteOrFloat Green { get { return _green; } }
        
        public ByteOrFloat Blue { get { return _blue; } }
        
        public ushort Hue { get { return _hue; } }

        public float Saturation { get { return _saturation; } }
        
        public float Lightness { get { return _lightness; } }
        
        public float? Alpha { get { return _alpha; } }
        
        public CssColor(NamedColors name)
        {
            _name = name;
            byte r, g, b;
            GetRgb(name, out r, out g, out b);
            _red = new ByteOrFloat(r);
            _green = new ByteOrFloat(g);
            _blue = new ByteOrFloat(b);
            _alpha = null;
            _defaultStringFormat = ColorStringFormat.Named;
            RgbToHsl(_red.FloatValue, _green.FloatValue, _blue.FloatValue, out _hue, out _saturation, out _lightness);
        }

        public CssColor(byte r, byte g, byte b)
        {
            _red = new ByteOrFloat(r);
            _green = new ByteOrFloat(g);
            _blue = new ByteOrFloat(b);
            _alpha = null;
            _defaultStringFormat = ColorStringFormat.Hex;
            _name = GetName(r, g, b);
            RgbToHsl(_red.FloatValue, _green.FloatValue, _blue.FloatValue, out _hue, out _saturation, out _lightness);
        }

        public CssColor(byte r, byte g, byte b, float a)
        {
            _red = new ByteOrFloat(r);
            _green = new ByteOrFloat(g);
            _blue = new ByteOrFloat(b);
            _alpha = NormalizePercentage(a);
            _defaultStringFormat = ColorStringFormat.Hex;
            _name = GetName(r, g, b);
            RgbToHsl(_red.FloatValue, _green.FloatValue, _blue.FloatValue, out _hue, out _saturation, out _lightness);
        }

        private CssColor(ByteOrFloat r, ByteOrFloat g, ByteOrFloat b, float? a, ColorStringFormat defaultStringFormat)
        {
            _red = r;
            _blue = b;
            _green = g;
            _alpha = null;
            _defaultStringFormat = defaultStringFormat;
            _name = ((_alpha = a).HasValue && a.Value != 1f) ? null : GetName(r.ByteValue, g.ByteValue, b.ByteValue);
            RgbToHsl(r.FloatValue, g.FloatValue, b.FloatValue, out _hue, out _saturation, out _lightness);
        }

        private CssColor(ushort h, float s, float l, float? a)
        {
            _hue = h;
            _saturation = s;
            _lightness = l;
            float r, g, b;
            HslToRgb(h, s, l, out r, out g, out b);
            _red = new ByteOrFloat(r);
            _green = new ByteOrFloat(g);
            _blue = new ByteOrFloat(b);
            _name = ((_alpha = a).HasValue && a.Value != 1f) ? null : GetName(FromPercentage(r), FromPercentage(g), FromPercentage(b));
            _defaultStringFormat = ColorStringFormat.HSL;
        }

        private static float ToPercentage(byte value) { return Convert.ToSingle(value) / 255f; }

        private static byte FromPercentage(float value)
        {
            if (value < 0f || value > 1f)
                throw new ArgumentOutOfRangeException("value");
            return Convert.ToByte(value * 255f);
        }
        public static CssColor Rgb8(byte red, byte green, byte blue)
        {
            return  new CssColor(new ByteOrFloat(red), new ByteOrFloat(green), new ByteOrFloat(blue), null, ColorStringFormat.RGB);
        }

        public static CssColor Rgb8(byte red, byte green, byte blue, float alpha)
        {
            return  new CssColor(new ByteOrFloat(red), new ByteOrFloat(green), new ByteOrFloat(blue), NormalizePercentage(alpha), ColorStringFormat.RGB);
        }

        public static CssColor RgbF(float red, float green, float blue)
        {
            return  new CssColor(new ByteOrFloat(red), new ByteOrFloat(green), new ByteOrFloat(blue), null, ColorStringFormat.RGB);
        }

        private static float NormalizePercentage(float value)
        {
            return (value < 0f) ? 0f : (value > 1f) ? 1f : (float)Math.Round(value, 2);
        }
        
        public static CssColor RgbF(float red, float green, float blue, float alpha)
        {
            return  new CssColor(new ByteOrFloat(red), new ByteOrFloat(green), new ByteOrFloat(blue), NormalizePercentage(alpha), ColorStringFormat.RGB);
        }

        public static CssColor Hsl(ushort hue, float saturation, float lightness)
        {
            if ((lightness = NormalizePercentage(lightness)) == 0f  || lightness == 1f)
                return new CssColor(0, 0f, lightness, null);
            if ((saturation = NormalizePercentage(saturation)) == 0f)
                return new CssColor(0, 0f, lightness, null);
            while (hue > 359)
                hue -= 360;
            return new CssColor(hue, saturation, lightness, null);
        }
        
        public static CssColor Hsl(ushort hue, float saturation, float lightness, float alpha)
        {
            if ((lightness = NormalizePercentage(lightness)) == 0f  || lightness == 1f)
                return new CssColor(0, 0f, lightness, NormalizePercentage(alpha));
            if ((saturation = NormalizePercentage(saturation)) == 0f)
                return new CssColor(0, 0f, lightness, NormalizePercentage(alpha));
            while (hue > 359)
                hue -= 360;
            return new CssColor(hue, saturation, lightness, NormalizePercentage(alpha));
        }

        public static CssColor Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            Match match = ParseRegex.Match(value = value.Trim());
            if (!match.Success)
                throw new ArgumentOutOfRangeException("Invalid color string format", "value");
            Group mg = match.Groups["rgb"];
            float? alpha;
            ByteOrFloat _red, _green, _blue;
            if (mg.Success)
            {
                string[] rgb = mg.Value.Trim().Split(',');
                if (match.Groups["rgba"].Success)
                {
                    if (rgb.Length != 4)
                        throw new ArgumentOutOfRangeException("Invalid number of RGBA parameters", "value");
                    float a;
                    if (!float.TryParse(rgb[3].Trim(), out a) || a < 0f || a > 1f)
                        throw new ArgumentOutOfRangeException("Invalid alpha parameter", "value");
                    alpha = a;
                }
                else
                {
                    if (rgb.Length != 3)
                        throw new ArgumentOutOfRangeException("Invalid number of RGB parameters", "value");
                    alpha = null;
                }
                if (!ByteOrFloat.TryParse(rgb[0], out _red))
                    throw new ArgumentOutOfRangeException("Invalid red parameter", "value");
                if (!ByteOrFloat.TryParse(rgb[1], out _green))
                    throw new ArgumentOutOfRangeException("Invalid green parameter", "value");
                if (!ByteOrFloat.TryParse(rgb[2], out _blue))
                    throw new ArgumentOutOfRangeException("Invalid blue parameter", "value");
                return new CssColor(_red, _green, _blue, alpha,  ColorStringFormat.RGB);
            }
            if ((mg = match.Groups["hsl"]).Success)
            {
                string[] hsl = mg.Value.Trim().Split(',');
                if (match.Groups["hsla"].Success)
                {
                    if (hsl.Length != 4)
                        throw new ArgumentOutOfRangeException("Invalid number of HSLA parameters", "value");
                    float a;
                    if (!float.TryParse(hsl[3].Trim(), out a) || a < 0f || a > 1f)
                        throw new ArgumentOutOfRangeException("Invalid alpha parameter", "value");
                    alpha = a;
                }
                else
                {
                    if (hsl.Length != 3)
                        throw new ArgumentOutOfRangeException("Invalid number of HSL parameters", "value");
                    alpha = null;
                }
                ushort h;
                if (!ushort.TryParse(hsl[0].Trim(), out h) || h > 360)
                    throw new ArgumentOutOfRangeException("Invalid hue parameter", "value");
                float s;
                if (!TryParsePercentValue(hsl[1], out s))
                    throw new ArgumentOutOfRangeException("Invalid saturation parameter", "value");
                float l;
                if (!TryParsePercentValue(hsl[2], out l))
                    throw new ArgumentOutOfRangeException("Invalid lightness parameter", "value");
                if (l > 0f && l < 1f)
                    return new CssColor((s == 0f || h == 360) ? (byte)0 : h, s, l, alpha);
                return new CssColor(0, 0f, l, alpha);
            }
            if (match.Groups["hex"].Success)
                switch (value.Length)
                {
                    case 4:
                        return new CssColor(new ByteOrFloat(FromHexChar(value[1])), new ByteOrFloat(FromHexChar(value[2])), new ByteOrFloat(FromHexChar(value[3])), null, ColorStringFormat.Hex);
                    case 5:
                        return new CssColor(new ByteOrFloat(FromHexChar(value[1])), new ByteOrFloat(FromHexChar(value[2])), new ByteOrFloat(FromHexChar(value[3])),
                            ToPercentage(FromHexChar(value[4])), ColorStringFormat.Hex);
                    case 7:
                        return new CssColor(new ByteOrFloat(byte.Parse(value.Substring(1, 2), System.Globalization.NumberStyles.HexNumber)),
                            new ByteOrFloat(byte.Parse(value.Substring(3, 2), System.Globalization.NumberStyles.HexNumber)),
                            new ByteOrFloat(byte.Parse(value.Substring(5), System.Globalization.NumberStyles.HexNumber)),
                            null, ColorStringFormat.Hex);
                    default:
                        return new CssColor(new ByteOrFloat(byte.Parse(value.Substring(1, 2), System.Globalization.NumberStyles.HexNumber)),
                            new ByteOrFloat(byte.Parse(value.Substring(3, 2), System.Globalization.NumberStyles.HexNumber)),
                            new ByteOrFloat(byte.Parse(value.Substring(5), System.Globalization.NumberStyles.HexNumber)),
                            ToPercentage(byte.Parse(value.Substring(7), System.Globalization.NumberStyles.HexNumber)), ColorStringFormat.Hex);
                }
            NamedColors name;
            if (Enum.TryParse(value, true, out name))
                return new CssColor(name);
            throw new ArgumentOutOfRangeException("unknown color name", "value");
        }

        public static bool TryParse(string value, out CssColor result)
        {
            if (value == null)
            {
                result = default(CssColor);
                return false;
            }
            Match match = ParseRegex.Match(value = value.Trim());
            if (!match.Success)
            {
                result = default(CssColor);
                return false;
            }
            Group mg = match.Groups["rgb"];
            float? alpha;
            ByteOrFloat _red, _green, _blue;
            if (mg.Success)
            {
                string[] rgb = mg.Value.Trim().Split(',');
                if (match.Groups["rgba"].Success)
                {
                    if (rgb.Length != 4)
                    {
                        result = default(CssColor);
                        return false;
                    }
                    float a;
                    if (!float.TryParse(rgb[3].Trim(), out a) || a < 0f || a > 1f)
                    {
                        result = default(CssColor);
                        return false;
                    }
                    alpha = a;
                }
                else
                {
                    if (rgb.Length != 3)
                    {
                        result = default(CssColor);
                        return false;
                    }
                    alpha = null;
                }
                if (ByteOrFloat.TryParse(rgb[0], out _red) && ByteOrFloat.TryParse(rgb[1], out _green) && ByteOrFloat.TryParse(rgb[2], out _blue))
                {
                    result = new CssColor(_red, _green, _blue, alpha,  ColorStringFormat.RGB);
                    return true;
                }
            }
            else if ((mg = match.Groups["hsl"]).Success)
            {
                string[] hsl = mg.Value.Trim().Split(',');
                if (match.Groups["hsla"].Success)
                {
                    if (hsl.Length != 4)
                    {
                        result = default(CssColor);
                        return false;
                    }
                    float a;
                    if (!float.TryParse(hsl[3].Trim(), out a) || a < 0f || a > 1f)
                    {
                        result = default(CssColor);
                        return false;
                    }
                    alpha = a;
                }
                else
                {
                    if (hsl.Length != 3)
                    {
                        result = default(CssColor);
                        return false;
                    }
                    alpha = null;
                }
                ushort h;
                float s, l;
                if (ushort.TryParse(hsl[0].Trim(), out h) && h < 361 && TryParsePercentValue(hsl[1], out s) && TryParsePercentValue(hsl[2], out l))
                {
                    if (l > 0f && l < 1f)
                        result = new CssColor((s == 0f) ? (byte)0 : h, s, l, alpha);
                    else
                        result = new CssColor(0, 0f, l, alpha);
                    return true;
                }
            }
            else if (match.Groups["hex"].Success)
            {
                switch (value.Length)
                {
                    case 4:
                        result = new CssColor(new ByteOrFloat(FromHexChar(value[1])), new ByteOrFloat(FromHexChar(value[2])), new ByteOrFloat(FromHexChar(value[3])), null, ColorStringFormat.Hex);
                        break;
                    case 5:
                        result = new CssColor(new ByteOrFloat(FromHexChar(value[1])), new ByteOrFloat(FromHexChar(value[2])), new ByteOrFloat(FromHexChar(value[3])),
                            ToPercentage(FromHexChar(value[4])), ColorStringFormat.Hex);
                        break;
                    case 7:
                        result = new CssColor(new ByteOrFloat(byte.Parse(value.Substring(1, 2), System.Globalization.NumberStyles.HexNumber)),
                            new ByteOrFloat(byte.Parse(value.Substring(3, 2), System.Globalization.NumberStyles.HexNumber)),
                            new ByteOrFloat(byte.Parse(value.Substring(5), System.Globalization.NumberStyles.HexNumber)),
                            null, ColorStringFormat.Hex);
                        break;
                    default:
                        result = new CssColor(new ByteOrFloat(byte.Parse(value.Substring(1, 2), System.Globalization.NumberStyles.HexNumber)),
                            new ByteOrFloat(byte.Parse(value.Substring(3, 2), System.Globalization.NumberStyles.HexNumber)),
                            new ByteOrFloat(byte.Parse(value.Substring(5), System.Globalization.NumberStyles.HexNumber)),
                            ToPercentage(byte.Parse(value.Substring(7), System.Globalization.NumberStyles.HexNumber)), ColorStringFormat.Hex);
                        break;
                }
                return true;
            }
            else
            {
                NamedColors name;
                if (Enum.TryParse(value, true, out name))
                {
                    result = new CssColor(name);
                    return true;
                }
            }
            result = default(CssColor);
            return false;
        }

        public bool Equals(CssColor other)
        {
            switch (_defaultStringFormat)
            {
                 case ColorStringFormat.Named:
                    return other._name.HasValue && _name.Value == other._name.Value;
                case ColorStringFormat.HSL:
                    return _hue == other._hue && _saturation == other._saturation && _lightness == other._lightness && _alpha == other._alpha;
                default:
                    return _red.Equals(other._red) && _green.Equals(other._green) && _blue.Equals(other._blue) && _alpha == other._alpha;
            }
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is CssColor && Equals((CssColor)obj);
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            switch (_defaultStringFormat)
            {
                 case ColorStringFormat.Named:
                    return _name.Value.GetHashCode();
                case ColorStringFormat.HSL:
                    unchecked
                    {
                        int hash = 20 + _hue.GetHashCode();
                        hash = hash * 13 + _saturation.GetHashCode();
                        hash = hash * 13 + _lightness.GetHashCode();
                        hash = hash * 13 + (_alpha.HasValue ? _alpha.Value.GetHashCode() : 0);
                        return hash;
                    }
                default:
                    unchecked
                    {
                        int hash = 20 + _red.GetHashCode();
                        hash = hash * 13 + _green.GetHashCode();
                        hash = hash * 13 + _blue.GetHashCode();
                        hash = hash * 13 + (_alpha.HasValue ? _alpha.Value.GetHashCode() : 0);
                        return hash;
                    }
            }
        }

        public string ToString(ColorStringFormat format)
        {
            switch (format)
            {
                case ColorStringFormat.HSL:
                    return _alpha.HasValue ? "hsla(" + _hue.ToString() + ", " + ((int)Math.Round(_saturation * 100f)).ToString() + "%, " + ((int)Math.Round(_lightness * 100f)).ToString() + "%, " + _alpha.Value.ToString() + ")" :
                        "hsl(" + _hue.ToString() + ", " + ((int)Math.Round(_saturation * 100f)).ToString() + "%, " + ((int)Math.Round(_lightness * 100f)).ToString() + "%)";
                case ColorStringFormat.Named:
                    if (_name.HasValue)
                        return _name.Value.ToString("F");
                    break;
                case ColorStringFormat.RGB:
                    return _alpha.HasValue ? "rgba(" + _red.ToString(true) + ", " + _green.ToString(true) + ", " + _blue.ToString(true) + ", " + _alpha.Value.ToString() + ")" :
                        "rgb(" + _red.ToString(true) + ", " + _green.ToString(true) + ", " + _blue.ToString(true) + ")";
            }
            return _alpha.HasValue ? "#" + _red.ByteValue.ToString("x2") + _green.ByteValue.ToString("x2") + _blue.ByteValue.ToString("x2") + FromPercentage(_alpha.Value).ToString("x2") :
                "#" + _red.ByteValue.ToString("x2") + _green.ByteValue.ToString("x2") + _blue.ByteValue.ToString("x2");
        }

        public override string ToString() { return ToString(_defaultStringFormat); }

        private static readonly Dictionary<NamedColors, RgbColorAttribute> _namedRgbCache = new Dictionary<NamedColors, RgbColorAttribute>();
        private static NamedColors? GetName(byte red, byte green, byte blue)
        {
            lock (_namedRgbCache)
                foreach (NamedColors name in Enum.GetValues(typeof(NamedColors)))
                {
                     RgbColorAttribute attr;
                    if (!_namedRgbCache.TryGetValue(name, out attr))
                    {
                        RgbColorAttribute[] arr = typeof(NamedColors).GetField(name.ToString("F")).GetCustomAttributes(typeof(RgbColorAttribute), false) as RgbColorAttribute[];
                        if (arr.Length > 0)
                            attr = arr[0];
                        else
                            attr = new RgbColorAttribute(0, 0, 0);
                        _namedRgbCache.Add(name, attr);
                    }
                    if (attr.Red == red && attr.Green == green && attr.Blue == blue)
                        return name;
                }
            return null;
        }
        private static void GetRgb(NamedColors name, out byte r, out byte g, out byte b)
        {
            RgbColorAttribute attr;
            lock (_namedRgbCache)
                if (!_namedRgbCache.TryGetValue(name, out attr))
                {
                    RgbColorAttribute[] arr = typeof(NamedColors).GetField(name.ToString("F")).GetCustomAttributes(typeof(RgbColorAttribute), false) as RgbColorAttribute[];
                    if (arr.Length > 0)
                        attr = arr[0];
                    else
                        attr = new RgbColorAttribute(0, 0, 0);
                    _namedRgbCache.Add(name, attr);
                }
            r = attr.Red;
            g = attr.Green;
            b = attr.Blue;
        }
        
        private static bool TryParsePercentValue(string value, out float result)
        {
            if ((value = value.Trim()).Length > 1 && value[value.Length - 1] == '%')
                return float.TryParse(value.Substring(0, value.Length - 1), out result);
            result = 0;
            return false;
        }

        private static void RgbToHsl(float r, float g, float b, out ushort h, out float s, out float l)
        {
            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);
            float d = max - min;
            l = (float)((max + min) / 2f);
            if (d == 0)
            {
                h = 0;
                s = 0f;
            }
            else
            {
                if (l < 0.5f)
                    s = (float)(d / (max + min));
                else
                    s = (float)(d / (2f - max - min));
                if (r == max)
                    h = Convert.ToUInt16((g - b) / d);
                else if (g == max)
                    h = Convert.ToUInt16(2f + (b - r) / d);
                else if (b == max)
                    h = Convert.ToUInt16(4f + (r - g) / d);
                else
                    h = 0;
            }
        }

        private static float ColorCalc(float c, float t1, float t2)
        {

            if (c < 0f) c += 1f;
            if (c > 1f) c -= 1f;
            if (6.0d * c < 1f) return t1 + (t2 - t1) * 6f * c;
            if (2.0d * c < 1f) return t2;
            if (3.0d * c < 2f) return t1 + (t2 - t1) * (2f / 3f - c) * 6f;
            return t1;
        }

        private static void HslToRgb(ushort h, float s, float l, out float r, out float g, out float b)
        {
            if (s == 0)
                r = g = b = l;
            else
            {
                float t1, t2;
                float th = h / 6f;

                if (l < 0.5f)
                    t2 = l * (1f + s);
                else
                    t2 = l + s - (l * s);
                t1 = 2f * l - t2;

                r = ColorCalc(th + (1f / 3f), t1, t2);
                g = ColorCalc(th, t1, t2);
                b = ColorCalc(th - (1f / 3f), t1, t2);
            }
        }

        private static byte FromHexChar(char c)
        {
            switch (c)
            {
                case '0':
                    return 0;
                case '1':
                    return 0x11;
                case '2':
                    return 0x22;
                case '3':
                    return 0x33;
                case '4':
                    return 0x44;
                case '5':
                    return 0x55;
                case '6':
                    return 0x66;
                case '7':
                    return 0x77;
                case '8':
                    return 0x88;
                case '9':
                    return 0x99;
                case 'a':
                case 'A':
                    return 0xaa;
                case 'b':
                case 'B':
                    return 0xbb;
                case 'c':
                case 'C':
                    return 0xcc;
                case 'd':
                case 'D':
                    return 0xdd;
                case 'e':
                case 'E':
                    return 0xee;
                default:
                    return 0xff;
            }
        }
    }
}
