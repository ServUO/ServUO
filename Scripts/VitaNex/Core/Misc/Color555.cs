#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Drawing;

using Server;
#endregion

namespace VitaNex
{
	[Parsable]
	public struct Color555 : IEquatable<Color555>, IEquatable<Color>, IEquatable<short>, IEquatable<ushort>
    {
		#region Colors
		public static readonly Color555 MinValue = UInt16.MinValue;
		public static readonly Color555 MaxValue = UInt16.MaxValue;

		public static readonly Color555 Empty = Color.Empty;

		public static readonly Color555 AliceBlue = Color.AliceBlue;
		public static readonly Color555 AntiqueWhite = Color.AntiqueWhite;
		public static readonly Color555 Aqua = Color.Aqua;
		public static readonly Color555 Aquamarine = Color.Aquamarine;
		public static readonly Color555 Azure = Color.Azure;
		public static readonly Color555 Beige = Color.Beige;
		public static readonly Color555 Bisque = Color.Bisque;
		public static readonly Color555 Black = Color.Black;
		public static readonly Color555 BlanchedAlmond = Color.BlanchedAlmond;
		public static readonly Color555 Blue = Color.Blue;
		public static readonly Color555 BlueViolet = Color.BlueViolet;
		public static readonly Color555 Brown = Color.Brown;
		public static readonly Color555 BurlyWood = Color.BurlyWood;
		public static readonly Color555 CadetBlue = Color.CadetBlue;
		public static readonly Color555 Chartreuse = Color.Chartreuse;
		public static readonly Color555 Chocolate = Color.Chocolate;
		public static readonly Color555 Coral = Color.Coral;
		public static readonly Color555 CornflowerBlue = Color.CornflowerBlue;
		public static readonly Color555 Cornsilk = Color.Cornsilk;
		public static readonly Color555 Crimson = Color.Crimson;
		public static readonly Color555 Cyan = Color.Cyan;
		public static readonly Color555 DarkBlue = Color.DarkBlue;
		public static readonly Color555 DarkCyan = Color.DarkCyan;
		public static readonly Color555 DarkGoldenrod = Color.DarkGoldenrod;
		public static readonly Color555 DarkGray = Color.DarkGray;
		public static readonly Color555 DarkGreen = Color.DarkGreen;
		public static readonly Color555 DarkKhaki = Color.DarkKhaki;
		public static readonly Color555 DarkMagenta = Color.DarkMagenta;
		public static readonly Color555 DarkOliveGreen = Color.DarkOliveGreen;
		public static readonly Color555 DarkOrange = Color.DarkOrange;
		public static readonly Color555 DarkOrchid = Color.DarkOrchid;
		public static readonly Color555 DarkRed = Color.DarkRed;
		public static readonly Color555 DarkSalmon = Color.DarkSalmon;
		public static readonly Color555 DarkSeaGreen = Color.DarkSeaGreen;
		public static readonly Color555 DarkSlateBlue = Color.DarkSlateBlue;
		public static readonly Color555 DarkSlateGray = Color.DarkSlateGray;
		public static readonly Color555 DarkTurquoise = Color.DarkTurquoise;
		public static readonly Color555 DarkViolet = Color.DarkViolet;
		public static readonly Color555 DeepPink = Color.DeepPink;
		public static readonly Color555 DeepSkyBlue = Color.DeepSkyBlue;
		public static readonly Color555 DimGray = Color.DimGray;
		public static readonly Color555 DodgerBlue = Color.DodgerBlue;
		public static readonly Color555 Firebrick = Color.Firebrick;
		public static readonly Color555 FloralWhite = Color.FloralWhite;
		public static readonly Color555 ForestGreen = Color.ForestGreen;
		public static readonly Color555 Fuchsia = Color.Fuchsia;
		public static readonly Color555 Gainsboro = Color.Gainsboro;
		public static readonly Color555 GhostWhite = Color.GhostWhite;
		public static readonly Color555 Gold = Color.Gold;
		public static readonly Color555 Goldenrod = Color.Goldenrod;
		public static readonly Color555 Gray = Color.Gray;
		public static readonly Color555 Green = Color.Green;
		public static readonly Color555 GreenYellow = Color.GreenYellow;
		public static readonly Color555 Honeydew = Color.Honeydew;
		public static readonly Color555 HotPink = Color.HotPink;
		public static readonly Color555 IndianRed = Color.IndianRed;
		public static readonly Color555 Indigo = Color.Indigo;
		public static readonly Color555 Ivory = Color.Ivory;
		public static readonly Color555 Khaki = Color.Khaki;
		public static readonly Color555 Lavender = Color.Lavender;
		public static readonly Color555 LavenderBlush = Color.LavenderBlush;
		public static readonly Color555 LawnGreen = Color.LawnGreen;
		public static readonly Color555 LemonChiffon = Color.LemonChiffon;
		public static readonly Color555 LightBlue = Color.LightBlue;
		public static readonly Color555 LightCoral = Color.LightCoral;
		public static readonly Color555 LightCyan = Color.LightCyan;
		public static readonly Color555 LightGoldenrodYellow = Color.LightGoldenrodYellow;
		public static readonly Color555 LightGray = Color.LightGray;
		public static readonly Color555 LightGreen = Color.LightGreen;
		public static readonly Color555 LightPink = Color.LightPink;
		public static readonly Color555 LightSalmon = Color.LightSalmon;
		public static readonly Color555 LightSeaGreen = Color.LightSeaGreen;
		public static readonly Color555 LightSkyBlue = Color.LightSkyBlue;
		public static readonly Color555 LightSlateGray = Color.LightSlateGray;
		public static readonly Color555 LightSteelBlue = Color.LightSteelBlue;
		public static readonly Color555 LightYellow = Color.LightYellow;
		public static readonly Color555 Lime = Color.Lime;
		public static readonly Color555 LimeGreen = Color.LimeGreen;
		public static readonly Color555 Linen = Color.Linen;
		public static readonly Color555 Magenta = Color.Magenta;
		public static readonly Color555 Maroon = Color.Maroon;
		public static readonly Color555 MediumAquamarine = Color.MediumAquamarine;
		public static readonly Color555 MediumBlue = Color.MediumBlue;
		public static readonly Color555 MediumOrchid = Color.MediumOrchid;
		public static readonly Color555 MediumPurple = Color.MediumPurple;
		public static readonly Color555 MediumSeaGreen = Color.MediumSeaGreen;
		public static readonly Color555 MediumSlateBlue = Color.MediumSlateBlue;
		public static readonly Color555 MediumSpringGreen = Color.MediumSpringGreen;
		public static readonly Color555 MediumTurquoise = Color.MediumTurquoise;
		public static readonly Color555 MediumVioletRed = Color.MediumVioletRed;
		public static readonly Color555 MidnightBlue = Color.MidnightBlue;
		public static readonly Color555 MintCream = Color.MintCream;
		public static readonly Color555 MistyRose = Color.MistyRose;
		public static readonly Color555 Moccasin = Color.Moccasin;
		public static readonly Color555 NavajoWhite = Color.NavajoWhite;
		public static readonly Color555 Navy = Color.Navy;
		public static readonly Color555 OldLace = Color.OldLace;
		public static readonly Color555 Olive = Color.Olive;
		public static readonly Color555 OliveDrab = Color.OliveDrab;
		public static readonly Color555 Orange = Color.Orange;
		public static readonly Color555 OrangeRed = Color.OrangeRed;
		public static readonly Color555 Orchid = Color.Orchid;
		public static readonly Color555 PaleGoldenrod = Color.PaleGoldenrod;
		public static readonly Color555 PaleGreen = Color.PaleGreen;
		public static readonly Color555 PaleTurquoise = Color.PaleTurquoise;
		public static readonly Color555 PaleVioletRed = Color.PaleVioletRed;
		public static readonly Color555 PapayaWhip = Color.PapayaWhip;
		public static readonly Color555 PeachPuff = Color.PeachPuff;
		public static readonly Color555 Peru = Color.Peru;
		public static readonly Color555 Pink = Color.Pink;
		public static readonly Color555 Plum = Color.Plum;
		public static readonly Color555 PowderBlue = Color.PowderBlue;
		public static readonly Color555 Purple = Color.Purple;
		public static readonly Color555 Red = Color.Red;
		public static readonly Color555 RosyBrown = Color.RosyBrown;
		public static readonly Color555 RoyalBlue = Color.RoyalBlue;
		public static readonly Color555 SaddleBrown = Color.SaddleBrown;
		public static readonly Color555 Salmon = Color.Salmon;
		public static readonly Color555 SandyBrown = Color.SandyBrown;
		public static readonly Color555 SeaGreen = Color.SeaGreen;
		public static readonly Color555 SeaShell = Color.SeaShell;
		public static readonly Color555 Sienna = Color.Sienna;
		public static readonly Color555 Silver = Color.Silver;
		public static readonly Color555 SkyBlue = Color.SkyBlue;
		public static readonly Color555 SlateBlue = Color.SlateBlue;
		public static readonly Color555 SlateGray = Color.SlateGray;
		public static readonly Color555 Snow = Color.Snow;
		public static readonly Color555 SpringGreen = Color.SpringGreen;
		public static readonly Color555 SteelBlue = Color.SteelBlue;
		public static readonly Color555 Tan = Color.Tan;
		public static readonly Color555 Teal = Color.Teal;
		public static readonly Color555 Thistle = Color.Thistle;
		public static readonly Color555 Tomato = Color.Tomato;
		public static readonly Color555 Transparent = Color.Transparent;
		public static readonly Color555 Turquoise = Color.Turquoise;
		public static readonly Color555 Violet = Color.Violet;
		public static readonly Color555 Wheat = Color.Wheat;
		public static readonly Color555 White = Color.White;
		public static readonly Color555 WhiteSmoke = Color.WhiteSmoke;
		public static readonly Color555 Yellow = Color.Yellow;
		public static readonly Color555 YellowGreen = Color.YellowGreen;
		#endregion Colors

		public static Color555 Parse(string input)
		{
			if (input == null)
			{
				throw new ArgumentNullException(nameof(input));
			}

            if (Insensitive.Equals(input, "None") || Insensitive.Equals(input, "Empty"))
            {
                return Empty;
            }

            if (Insensitive.StartsWith(input, "0x"))
            {
                return FromArgb(Convert.ToInt32(input.Substring(2), 16));
            }

            if (Insensitive.StartsWith(input, "#"))
            {
                return FromArgb(Convert.ToInt32(input.Substring(1), 16));
            }

            if (Int32.TryParse(input, out var val))
            {
                return FromArgb(val);
            }

            var rgb = input.Split(',');

            if (rgb.Length >= 3)
            {
                if (Byte.TryParse(rgb[0], out var r) && Byte.TryParse(rgb[1], out var g) && Byte.TryParse(rgb[2], out var b))
                {
                    return FromArgb(r, g, b);
                }
            }

            return FromName(input);
        }

		public static bool TryParse(string input, out Color555 value)
		{
			try
			{
				value = Parse(input);

				return true;
			}
			catch
			{
				value = Color.Empty;

				return false;
			}
		}

		public static Color555 FromKnownColor(KnownColor color)
		{
			return new Color555(Color.FromKnownColor(color));
		}

		public static Color555 FromName(string name)
		{
			return new Color555(Color.FromName(name));
		}

		public static Color555 FromArgb(int argb)
		{
			return new Color555(Color.FromArgb(argb));
        }

        public static Color555 FromArgb(int r, int g, int b)
        {
            return new Color555(Color.FromArgb(r, g, b));
        }

        public static Color555 FromArgb(int a, int r, int g, int b)
        {
            return new Color555(Color.FromArgb(a, r, g, b));
        }

        public static Color555 FromArgb(int a, Color baseColor)
        {
            return new Color555(Color.FromArgb(a, baseColor));
        }

        public static Color555 FromArgb(int a, Color555 baseColor)
        {
            return new Color555(Color.FromArgb(a, baseColor._Color));
        }

        public static Color555 FromRgb(short rgb)
		{
			return new Color555(rgb);
		}

		public static Color555 FromRgb(ushort rgb)
		{
			return new Color555(rgb);
		}

		public static Color555 FromColor(Color color)
		{
			return new Color555(color);
		}

		private string _String;

		private readonly ushort _RGB;
		private readonly int _ARGB;
		private readonly Color _Color;

		public byte R => _Color.R;
		public byte G => _Color.G;
		public byte B => _Color.B;

		public bool IsEmpty => _Color.IsEmpty;
		public bool IsKnownColor => _Color.IsKnownColor;
		public bool IsNamedColor => _Color.IsNamedColor;
		public bool IsSystemColor => _Color.IsSystemColor;

		public string Name => _Color.Name;

		public Color555(Color color)
		{
			_Color = color;

			_ARGB = color.ToArgb();

			_RGB = (ushort)((_ARGB >> 16) & 0x8000 | (_ARGB >> 9) & 0x7C00 | (_ARGB >> 6) & 0x03E0 | (_ARGB >> 3) & 0x1F);

            _String = null;
        }

		public Color555(short rgb)
			: this((ushort)rgb)
		{ }

		public Color555(ushort rgb)
		{
			_RGB = rgb;

			_ARGB = ((rgb & 0x7C00) << 9) | ((rgb & 0x03E0) << 6) | ((rgb & 0x1F) << 3);
			_ARGB = ((rgb & 0x8000) * 0x1FE00) | _ARGB | ((_ARGB >> 5) & 0x070707);

			_Color = Color.FromArgb(_ARGB);

            _String = null;
        }

		public override int GetHashCode()
		{
			return _ARGB;
		}

		public override bool Equals(object obj)
		{
			return (obj is Color555 c16 && Equals(c16)) 
				|| (obj is Color c32 && Equals(c32)) 
				|| (obj is short srgb && Equals(srgb)) 
				|| (obj is ushort urgb && Equals(urgb));
		}

		public bool Equals(Color555 other)
		{
			return _RGB == other._RGB;
		}

		public bool Equals(Color other)
		{
			return _Color == other;
		}

		public bool Equals(short other)
		{
			return _RGB == other;
		}

		public bool Equals(ushort other)
		{
			return _RGB == other;
        }

        public override string ToString()
		{
			return _String ?? (_String = _Color.ToString());
		}

		public ushort ToRgb()
		{
			return _RGB;
		}

		public int ToArgb()
		{
			return _ARGB;
		}

		public Color ToColor()
		{
			return _Color;
		}

		public KnownColor ToKnownColor()
		{
			return _Color.ToKnownColor();
		}

		public static bool operator ==(Color555 l, Color555 r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Color555 l, Color555 r)
		{
			return !l.Equals(r);
		}

		public static bool operator ==(Color555 l, Color r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Color555 l, Color r)
		{
			return !l.Equals(r);
		}

		public static bool operator ==(Color l, Color555 r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(Color l, Color555 r)
		{
			return !r.Equals(l);
		}

		public static bool operator ==(Color555 l, ushort r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Color555 l, ushort r)
		{
			return !l.Equals(r);
		}

		public static bool operator ==(ushort l, Color555 r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(ushort l, Color555 r)
		{
			return !r.Equals(l);
		}

		public static implicit operator Color555(Color value)
		{
			return new Color555(value);
		}

		public static implicit operator Color555(ushort value)
		{
			return new Color555(value);
		}

		public static implicit operator Color(Color555 value)
		{
			return value._Color;
		}

		public static implicit operator ushort(Color555 value)
		{
			return value._RGB;
		}
	}
}