using System;
using System.Text;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF {

	/// <summary>
	/// A Class that implements a PDF font.
	/// </summary>
	public class PDFFont : IWritable
	{		
		private PredefinedFont _fontStyle;
		private int _objectID;
		private int _fontNumber;

		/// <summary>
		/// Font's style
		/// </summary>
		public PredefinedFont fontStyle
		{
			get
			{
				return _fontStyle;
			}
		}

		/// <summary>
		/// Font's ID
		/// </summary>
		public int objectID
		{
			get
			{
				return _objectID;
			}
			set
			{
				_objectID = value;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newFontStyle">Font's style</param>
		/// <param name="newFontNumber">Font's number in the PDF </param>
		internal PDFFont(PredefinedFont newFontStyle, int newFontNumber)
		{
			_fontStyle = newFontStyle;
			_fontNumber = newFontNumber;
		}

		/// <summary>
		/// Method that returns the PDF codes to write the Font in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public string GetText()
		{
			StringBuilder content = new StringBuilder();
			content.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("/Type /Font" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("/Subtype /Type1" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("/Name /F" + _fontNumber.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("/BaseFont /" + PDFFont.GetFontName(_fontStyle) + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("/Encoding /WinAnsiEncoding" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			return content.ToString();
		}

		/// <summary>
		/// Static Mehtod that returns the name of the font
		/// </summary>
		/// <param name="fontType">Font's Type</param>
		/// <returns>String that contains the name of the font</returns>
		public static string GetFontName(PredefinedFont fontType)
		{
			switch (fontType)
			{
			case PredefinedFont.Helvetica:
				return "Helvetica";
			case PredefinedFont.HelveticaBold:
				return "Helvetica-Bold";
			case PredefinedFont.HelveticaOblique:
				return "Helvetica-Oblique";
			case PredefinedFont.HelvetivaBoldOblique:
				return "Helvetica-BoldOblique";
			case PredefinedFont.Courier:
				return "Courier";
			case PredefinedFont.CourierBold:
				return "Courier-Bold";
			case PredefinedFont.CourierOblique:
				return "Courier-Oblique";
			case PredefinedFont.CourierBoldOblique:
				return "Courier-BoldOblique";
			case PredefinedFont.Times:
				return "Times-Roman";
			case PredefinedFont.TimesBold:
				return "Times-Bold";
			case PredefinedFont.TimesOblique:
				return "Times-Italic";
			case PredefinedFont.TimesBoldOblique:
				return "Times-BoldItalic";
			default:
				return System.String.Empty;
			}
		}

	}
}
