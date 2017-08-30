using System;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF {

	/// <summary>
	/// A Class that implements a PDF color.
	/// </summary>
	public class PDFColor : ICloneable
	{
		private string m_red;
		private string m_green;
		private string m_blue;

		#region Standard_Colors

		/// <summary>
		/// No Color
		/// </summary>
		public readonly static PDFColor NoColor = new PDFColor("", "", "");
		/// <summary>
		/// Black Color
		/// </summary>
		public readonly static PDFColor Black = new PDFColor("0", "0", "0");
		/// <summary>
		/// White Color
		/// </summary>
		public readonly static PDFColor White = new PDFColor("1", "1", "1");
		/// <summary>
		/// Red Color
		/// </summary>
		public readonly static PDFColor Red = new PDFColor("1", "0", "0");
		/// <summary>
		/// Light Red Color
		/// </summary>
		public readonly static PDFColor LightRed = new PDFColor("1", ".75", ".75");
		/// <summary>
		/// Dark Red Color
		/// </summary>
		public readonly static PDFColor DarkRed = new PDFColor(".5", "0", "0");
		/// <summary>
		/// Orange Color
		/// </summary>
		public readonly static PDFColor Orange = new PDFColor("1", ".5", "0");
		/// <summary>
		/// Light Orange Color
		/// </summary>
		public readonly static PDFColor LightOrange = new PDFColor("1", ".75", "0");
		/// <summary>
		/// Dark Orange Color
		/// </summary>
		public readonly static PDFColor DarkOrange = new PDFColor("1", ".25", "0");
		/// <summary>
		/// Yellow Color
		/// </summary>
		public readonly static PDFColor Yellow = new PDFColor("1", "1", ".25");
		/// <summary>
		/// Light Yellow Color
		/// </summary>
		public readonly static PDFColor LightYellow = new PDFColor("1", "1", ".75");
		/// <summary>
		/// Dark Yellow Color
		/// </summary>
		public readonly static PDFColor DarkYellow = new PDFColor("1", "1", "0");
		/// <summary>
		/// Blue Color
		/// </summary>
		public readonly static PDFColor Blue = new PDFColor("0", "0", "1");
		/// <summary>
		/// Light Blue Color
		/// </summary>
		public readonly static PDFColor LightBlue = new PDFColor(".1", ".3", ".75");
		/// <summary>
		/// Dark Blue Color
		/// </summary>
		public static PDFColor DarkBlue = new PDFColor("0", "0", ".5");
		/// <summary>
		/// Green Color
		/// </summary>
		public readonly static PDFColor Green = new PDFColor("0", "1", "0");
		/// <summary>
		/// Light Green Color
		/// </summary>
		public readonly static PDFColor LightGreen = new PDFColor(".75", "1", ".75");
		/// <summary>
		/// Dark Green Color
		/// </summary>
		public readonly static PDFColor DarkGreen = new PDFColor("0", ".5", "0");
		/// <summary>
		/// Cyan Color
		/// </summary>
		public readonly static PDFColor Cyan = new PDFColor("0", ".5", "1");
		/// <summary>
		/// Light Cyan Color
		/// </summary>
		public readonly static PDFColor LightCyan = new PDFColor(".2", ".8", "1");
		/// <summary>
		/// Dark Cyan Color
		/// </summary>
		public readonly static PDFColor DarkCyan = new PDFColor("0", ".4", ".8");
		/// <summary>
		/// Purple Color
		/// </summary>
		public readonly static PDFColor Purple = new PDFColor(".5", "0", "1");
		/// <summary>
		/// Light Purple Color
		/// </summary>
		public readonly static PDFColor LightPurple = new PDFColor(".75", ".45", ".95");
		/// <summary>
		/// Dark Purple Color
		/// </summary>
		public readonly static PDFColor DarkPurple = new PDFColor(".4", ".1", ".5");
		/// <summary>
		/// Gray Color
		/// </summary>
		public readonly static PDFColor Gray = new PDFColor(".5", ".5", ".5");
		/// <summary>
		/// Light Gray Color
		/// </summary>
		public readonly static PDFColor LightGray = new PDFColor(".75", ".75", ".75");
		/// <summary>
		/// Dark Gray Color
		/// </summary>
		public readonly static PDFColor DarkGray = new PDFColor(".25", ".25", ".25");		

		#endregion

		/// <summary>
		/// R property of RGB color
		/// </summary>
		internal string r
		{
			get
			{
				return m_red;
			}
		}

		/// <summary>
		/// G property of RGB color
		/// </summary>
		internal string g
		{
			get
			{
				return m_green;
			}
		}

		/// <summary>
		/// B property of RGB color
		/// </summary>
		internal string b
		{
			get
			{
				return m_blue;
			}				
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="HEXColor">Hex Color</param>
		public PDFColor(string HEXColor)
		{
			m_red = FormatColorComponent(int.Parse(HEXColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber));
			m_green = FormatColorComponent(int.Parse(HEXColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber));
			m_blue = FormatColorComponent(int.Parse(HEXColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));			
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="r">Red component of the color</param>
		/// <param name="g">Green component of the color</param>
		/// <param name="b">Blue component of the color</param>
		public PDFColor(int r, int g, int b)
		{
			m_red = FormatColorComponent(r);
			m_green = FormatColorComponent(g);
			m_blue = FormatColorComponent(b);
		}

		/// <summary>
		/// Internal constructor for the creation of the standard colors
		/// </summary>
		/// <param name="r">Red component of the color</param>
		/// <param name="g">Green component of the color</param>
		/// <param name="b">Blue component of the color</param>
		internal PDFColor(string r, string g, string b)
		{
			m_red = r;
			m_green = g;
			m_blue = b;
		}

		/// <summary>
		/// Method that formats a int color value with the pdf color format
		/// </summary>
		/// <param name="colorValue">Component of the color</param>
		/// <returns>Formatted component of the color</returns>
		private string FormatColorComponent(int colorValue)
		{
			int colorComponent = Convert.ToInt32(Math.Round(((Convert.ToSingle(colorValue) / 255) * 100)));

			string resultValue;
			if (colorComponent == 0 || colorComponent < 0)
			{
				resultValue = "0";
			}
			else if (colorComponent < 100)
			{				
				resultValue = "." + colorComponent.ToString();
				if (resultValue[resultValue.Length - 1] == '0')
					resultValue = resultValue.Substring(0, resultValue.Length - 1);
			}
			else
			{
				resultValue = "1";
			}
			return resultValue;
		}

		/// <summary>
		/// Method that validates the color
		/// </summary>
		/// <returns>Boolean value that represents the validity of the color</returns>
		internal bool isColor()
		{
			return (!string.IsNullOrEmpty(m_red) && !string.IsNullOrEmpty(m_green) && !string.IsNullOrEmpty(m_blue));
		}

		#region ICloneable
		
		/// <summary>
		/// Method that clones the PDFColorObject
		/// </summary>
		/// <returns>Cloned Object</returns>
		public object Clone()
		{
			return new PDFColor(this.m_red, this.m_green, this.m_blue);
		}

		#endregion
	}
}
