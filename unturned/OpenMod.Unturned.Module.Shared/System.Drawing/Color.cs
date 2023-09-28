//
// System.Drawing.Color.cs
//
// Authors:
// 	Dennis Hayes (dennish@raytek.com)
// 	Ben Houston  (ben@exocortex.org)
// 	Gonzalo Paniagua (gonzalo@ximian.com)
// 	Juraj Skripsky (juraj@hotfeet.ch)
//	Sebastien Pouliot  <sebastien@ximian.com>
//      Jiri Volejnik <aconcagua21@volny.cz>
//      Filip Navara <filip.navara@gmail.com>
//
// (C) 2002 Dennis Hayes
// (c) 2002 Ximian, Inc. (http://www.ximiam.com)
// (C) 2005 HotFeet GmbH (http://www.hotfeet.ch)
// Copyright (C) 2004,2006-2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

using System.ComponentModel;

namespace System.Drawing
{
	[Serializable]
	[TypeConverter(typeof(ColorConverter))]
	public struct Color
	{
		int value;

		internal Color(int value)
		{
			this.value = value;
		}

		#region Unimplemented bloated properties
		//
		// These properties were implemented very poorly on Mono, this
		// version will only store the int32 value and any helper properties
		// like Name, IsKnownColor, IsSystemColor, IsNamedColor are not
		// currently implemented, and would be implemented in the future
		// using external tables/hastables/dictionaries, without bloating
		// the Color structure
		//
		public string Name
		{
			get
			{
				return KnownColors.NameByArgb.TryGetValue((uint)value, out string name) ? name : String.Empty;
			}
		}

		public bool IsKnownColor
		{
			get
			{
				return KnownColors.NameByArgb.ContainsKey((uint)value);
			}
		}

		public bool IsSystemColor
		{
			get
			{
				return false;
			}
		}

		public bool IsNamedColor
		{
			get
            {
                return !string.IsNullOrEmpty(Name);
            }
		}
		#endregion

		public static Color FromArgb(int red, int green, int blue)
		{
			return FromArgb(255, red, green, blue);
		}

		public static Color FromArgb(int alpha, int red, int green, int blue)
		{
			if ((red > 255) || (red < 0))
				throw CreateColorArgumentException(red, "red");
			if ((green > 255) || (green < 0))
				throw CreateColorArgumentException(green, "green");
			if ((blue > 255) || (blue < 0))
				throw CreateColorArgumentException(blue, "blue");
			if ((alpha > 255) || (alpha < 0))
				throw CreateColorArgumentException(alpha, "alpha");

			Color color = new Color();
			color.value = (int)((uint)alpha << 24) + (red << 16) + (green << 8) + blue;
			return color;
		}

		public int ToArgb()
		{
			return (int)value;
		}

		public static Color FromArgb(int alpha, Color baseColor)
		{
			return FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
		}

		public static Color FromArgb(int argb)
		{
			return new Color(argb);
		}

		public static Color FromKnownColor(KnownColor color)
		{
			return KnownColors.FromKnownColor(color);
		}

		public static Color FromName(string name)
		{
			if (KnownColors.ArgbByName.TryGetValue(name, out uint argb))
				return new Color((int)argb);
			return new Color();
		}


		public static readonly Color Empty;

		public static bool operator ==(Color left, Color right)
		{
			return left.value == right.value;
		}

		public static bool operator !=(Color left, Color right)
		{
			return left.value != right.value;
		}

		public float GetBrightness()
		{
			byte minval = Math.Min(R, Math.Min(G, B));
			byte maxval = Math.Max(R, Math.Max(G, B));

			return (float)(maxval + minval) / 510;
		}

		public float GetSaturation()
		{
			byte minval = (byte)Math.Min(R, Math.Min(G, B));
			byte maxval = (byte)Math.Max(R, Math.Max(G, B));

			if (maxval == minval)
				return 0.0f;

			int sum = maxval + minval;
			if (sum > 255)
				sum = 510 - sum;

			return (float)(maxval - minval) / sum;
		}

		public float GetHue()
		{
			int r = R;
			int g = G;
			int b = B;
			byte minval = (byte)Math.Min(r, Math.Min(g, b));
			byte maxval = (byte)Math.Max(r, Math.Max(g, b));

			if (maxval == minval)
				return 0.0f;

			float diff = (float)(maxval - minval);
			float rnorm = (maxval - r) / diff;
			float gnorm = (maxval - g) / diff;
			float bnorm = (maxval - b) / diff;

			float hue = 0.0f;
			if (r == maxval)
				hue = 60.0f * (6.0f + bnorm - gnorm);
			if (g == maxval)
				hue = 60.0f * (2.0f + rnorm - bnorm);
			if (b == maxval)
				hue = 60.0f * (4.0f + gnorm - rnorm);
			if (hue > 360.0f)
				hue = hue - 360.0f;

			return hue;
		}

		public KnownColor ToKnownColor()
		{
			throw new NotImplementedException();
		}

		public bool IsEmpty
		{
			get
			{
				return value == 0;
			}
		}

		public byte A
		{
			get { return (byte)(value >> 24); }
		}

		public byte R
		{
			get { return (byte)(value >> 16); }
		}

		public byte G
		{
			get { return (byte)(value >> 8); }
		}

		public byte B
		{
			get { return (byte)value; }
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Color))
				return false;
			Color c = (Color)obj;
			return this == c;
		}

		public override int GetHashCode()
		{
			return value;
		}

		public override string ToString()
		{
			if (IsEmpty)
				return "Color [Empty]";

			return String.Format("Color [A={0}, R={1}, G={2}, B={3}]", A, R, G, B);
		}

		static ArgumentException CreateColorArgumentException(int value, string color)
		{
			return new ArgumentException(string.Format("'{0}' is not a valid"
				+ " value for '{1}'. '{1}' should be greater or equal to 0 and"
				+ " less than or equal to 255.", value, color));
		}

		static public Color Transparent
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Transparent]); }
		}

		static public Color AliceBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.AliceBlue]); }
		}

		static public Color AntiqueWhite
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.AntiqueWhite]); }
		}

		static public Color Aqua
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Aqua]); }
		}

		static public Color Aquamarine
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Aquamarine]); }
		}

		static public Color Azure
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Azure]); }
		}

		static public Color Beige
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Beige]); }
		}

		static public Color Bisque
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Bisque]); }
		}

		static public Color Black
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Black]); }
		}

		static public Color BlanchedAlmond
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.BlanchedAlmond]); }
		}

		static public Color Blue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Blue]); }
		}

		static public Color BlueViolet
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.BlueViolet]); }
		}

		static public Color Brown
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Brown]); }
		}

		static public Color BurlyWood
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.BurlyWood]); }
		}

		static public Color CadetBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.CadetBlue]); }
		}

		static public Color Chartreuse
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Chartreuse]); }
		}

		static public Color Chocolate
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Chocolate]); }
		}

		static public Color Coral
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Coral]); }
		}

		static public Color CornflowerBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.CornflowerBlue]); }
		}

		static public Color Cornsilk
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Cornsilk]); }
		}

		static public Color Crimson
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Crimson]); }
		}

		static public Color Cyan
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Cyan]); }
		}

		static public Color DarkBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkBlue]); }
		}

		static public Color DarkCyan
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkCyan]); }
		}

		static public Color DarkGoldenrod
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkGoldenrod]); }
		}

		static public Color DarkGray
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkGray]); }
		}

		static public Color DarkGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkGreen]); }
		}

		static public Color DarkKhaki
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkKhaki]); }
		}

		static public Color DarkMagenta
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkMagenta]); }
		}

		static public Color DarkOliveGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkOliveGreen]); }
		}

		static public Color DarkOrange
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkOrange]); }
		}

		static public Color DarkOrchid
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkOrchid]); }
		}

		static public Color DarkRed
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkRed]); }
		}

		static public Color DarkSalmon
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkSalmon]); }
		}

		static public Color DarkSeaGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkSeaGreen]); }
		}

		static public Color DarkSlateBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkSlateBlue]); }
		}

		static public Color DarkSlateGray
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkSlateGray]); }
		}

		static public Color DarkTurquoise
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkTurquoise]); }
		}

		static public Color DarkViolet
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DarkViolet]); }
		}

		static public Color DeepPink
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DeepPink]); }
		}

		static public Color DeepSkyBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DeepSkyBlue]); }
		}

		static public Color DimGray
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DimGray]); }
		}

		static public Color DodgerBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.DodgerBlue]); }
		}

		static public Color Firebrick
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Firebrick]); }
		}

		static public Color FloralWhite
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.FloralWhite]); }
		}

		static public Color ForestGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.ForestGreen]); }
		}

		static public Color Fuchsia
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Fuchsia]); }
		}

		static public Color Gainsboro
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Gainsboro]); }
		}

		static public Color GhostWhite
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.GhostWhite]); }
		}

		static public Color Gold
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Gold]); }
		}

		static public Color Goldenrod
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Goldenrod]); }
		}

		static public Color Gray
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Gray]); }
		}

		static public Color Green
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Green]); }
		}

		static public Color GreenYellow
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.GreenYellow]); }
		}

		static public Color Honeydew
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Honeydew]); }
		}

		static public Color HotPink
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.HotPink]); }
		}

		static public Color IndianRed
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.IndianRed]); }
		}

		static public Color Indigo
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Indigo]); }
		}

		static public Color Ivory
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Ivory]); }
		}

		static public Color Khaki
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Khaki]); }
		}

		static public Color Lavender
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Lavender]); }
		}

		static public Color LavenderBlush
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LavenderBlush]); }
		}

		static public Color LawnGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LawnGreen]); }
		}

		static public Color LemonChiffon
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LemonChiffon]); }
		}

		static public Color LightBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightBlue]); }
		}

		static public Color LightCoral
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightCoral]); }
		}

		static public Color LightCyan
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightCyan]); }
		}

		static public Color LightGoldenrodYellow
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightGoldenrodYellow]); }
		}

		static public Color LightGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightGreen]); }
		}

		static public Color LightGray
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightGray]); }
		}

		static public Color LightPink
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightPink]); }
		}

		static public Color LightSalmon
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightSalmon]); }
		}

		static public Color LightSeaGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightSeaGreen]); }
		}

		static public Color LightSkyBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightSkyBlue]); }
		}

		static public Color LightSlateGray
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightSlateGray]); }
		}

		static public Color LightSteelBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightSteelBlue]); }
		}

		static public Color LightYellow
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LightYellow]); }
		}

		static public Color Lime
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Lime]); }
		}

		static public Color LimeGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.LimeGreen]); }
		}

		static public Color Linen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Linen]); }
		}

		static public Color Magenta
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Magenta]); }
		}

		static public Color Maroon
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Maroon]); }
		}

		static public Color MediumAquamarine
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.MediumAquamarine]); }
		}

		static public Color MediumBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.MediumBlue]); }
		}

		static public Color MediumOrchid
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.MediumOrchid]); }
		}

		static public Color MediumPurple
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.MediumPurple]); }
		}

		static public Color MediumSeaGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.MediumSeaGreen]); }
		}

		static public Color MediumSlateBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.MediumSlateBlue]); }
		}

		static public Color MediumSpringGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.MediumSpringGreen]); }
		}

		static public Color MediumTurquoise
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.MediumTurquoise]); }
		}

		static public Color MediumVioletRed
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.MediumVioletRed]); }
		}

		static public Color MidnightBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.MidnightBlue]); }
		}

		static public Color MintCream
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.MintCream]); }
		}

		static public Color MistyRose
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.MistyRose]); }
		}

		static public Color Moccasin
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Moccasin]); }
		}

		static public Color NavajoWhite
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.NavajoWhite]); }
		}

		static public Color Navy
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Navy]); }
		}

		static public Color OldLace
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.OldLace]); }
		}

		static public Color Olive
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Olive]); }
		}

		static public Color OliveDrab
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.OliveDrab]); }
		}

		static public Color Orange
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Orange]); }
		}

		static public Color OrangeRed
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.OrangeRed]); }
		}

		static public Color Orchid
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Orchid]); }
		}

		static public Color PaleGoldenrod
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.PaleGoldenrod]); }
		}

		static public Color PaleGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.PaleGreen]); }
		}

		static public Color PaleTurquoise
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.PaleTurquoise]); }
		}

		static public Color PaleVioletRed
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.PaleVioletRed]); }
		}

		static public Color PapayaWhip
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.PapayaWhip]); }
		}

		static public Color PeachPuff
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.PeachPuff]); }
		}

		static public Color Peru
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Peru]); }
		}

		static public Color Pink
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Pink]); }
		}

		static public Color Plum
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Plum]); }
		}

		static public Color PowderBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.PowderBlue]); }
		}

		static public Color Purple
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Purple]); }
		}

		static public Color Red
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Red]); }
		}

		static public Color RosyBrown
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.RosyBrown]); }
		}

		static public Color RoyalBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.RoyalBlue]); }
		}

		static public Color SaddleBrown
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.SaddleBrown]); }
		}

		static public Color Salmon
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Salmon]); }
		}

		static public Color SandyBrown
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.SandyBrown]); }
		}

		static public Color SeaGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.SeaGreen]); }
		}

		static public Color SeaShell
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.SeaShell]); }
		}

		static public Color Sienna
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Sienna]); }
		}

		static public Color Silver
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Silver]); }
		}

		static public Color SkyBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.SkyBlue]); }
		}

		static public Color SlateBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.SlateBlue]); }
		}

		static public Color SlateGray
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.SlateGray]); }
		}

		static public Color Snow
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Snow]); }
		}

		static public Color SpringGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.SpringGreen]); }
		}

		static public Color SteelBlue
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.SteelBlue]); }
		}

		static public Color Tan
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Tan]); }
		}

		static public Color Teal
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Teal]); }
		}

		static public Color Thistle
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Thistle]); }
		}

		static public Color Tomato
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Tomato]); }
		}

		static public Color Turquoise
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Turquoise]); }
		}

		static public Color Violet
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Violet]); }
		}

		static public Color Wheat
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Wheat]); }
		}

		static public Color White
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.White]); }
		}

		static public Color WhiteSmoke
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.WhiteSmoke]); }
		}

		static public Color Yellow
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.Yellow]); }
		}

		static public Color YellowGreen
		{
			get { return new Color((int)KnownColors.ArgbValues[(int)KnownColor.YellowGreen]); }
		}
	}
}