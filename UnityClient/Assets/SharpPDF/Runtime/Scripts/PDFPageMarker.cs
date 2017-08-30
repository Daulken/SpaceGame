using System;
using System.Text;
using SharpPDF.Fonts;
using SharpPDF.Enumerators;

namespace SharpPDF {

	/// <summary>
	/// Class that implements a pdf page marker
	/// </summary>
	public class PDFPageMarker
	{
		private PredefinedMarkerStyle _style = PredefinedMarkerStyle.Arabic;
		private int _coordX;
		private int _coordY;
		private PDFAbstractFont _fontType;
		private int _fontSize;
		private PDFColor _fontColor;
		private string _pattern;
		private PredefinedAlignment _alignment = PredefinedAlignment.Left;

		/// <summary>
		/// X position of the marker
		/// </summary>
		public int coordX
		{
			get
			{
				return _coordX;
			}
			set
			{
				_coordX = value;
			}
		}

		/// <summary>
		/// Y position of the marker
		/// </summary>
		public int coordY
		{
			get
			{
				return _coordY;
			}
			set
			{
				_coordY = value;
			}
		}

		/// <summary>
		/// Font's type
		/// </summary>
		public PDFAbstractFont fontType
		{
			get
			{
				return _fontType;
			}
			set
			{
				_fontType = value;
			}
		}

		/// <summary>
		/// Font's size
		/// </summary>
		public int fontSize
		{
			get
			{
				return _fontSize;
			}
			set
			{
				_fontSize = value;
			}
		}

		/// <summary>
		/// Marker's color
		/// </summary>
		public PDFColor fontColor
		{
			get
			{
				return _fontColor;
			}
			set
			{
				_fontColor = value;
			}
		}

		/// <summary>
		/// Markers alignment in the page
		/// </summary>
		public PredefinedAlignment alignment
		{
			get
			{
				return _alignment;
			}
			set
			{
				_alignment = value;
			}
		}

		/// <summary>
		/// Marker's pattern. In the pattern there are two simbols: #n# (that represents the
		/// actual page) and #N# (that represents the number of pages).
		/// The Default pattern is : "Page #n# of #N#"
		/// </summary>
		/// <example>
		/// This example shows how to use the pattern property:
		/// <code>
		/// pdfPageMarker marker = new pdfPageMarker(400,30);
		/// marker.pattern = "Page #n#/#N#";
		/// myDoc.pageMarker = marker
		/// ......
		/// ......
		/// </code>
		/// The result on the document is : "Page 1/2"
		/// </example>
		public string pattern
		{
			get
			{
				return _pattern;
			}
			set
			{
				_pattern = value;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="coordX">X position in the PDF document</param>
		/// <param name="coordY">Y position in the PDF document</param>
		/// <param name="style">Marker's style</param>
		/// <param name="fontReference">Font's reference object</param>
		/// <param name="fontSize">Font's Size</param>
		public PDFPageMarker(int coordX, int coordY, PredefinedMarkerStyle style, PDFAbstractFont fontReference, int fontSize):
			this(coordX, coordY, style, fontReference, fontSize, PDFColor.Black, PredefinedAlignment.Left, "Page #n# of #N#")
		{		
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="coordX">X position in the PDF document</param>
		/// <param name="coordY">Y position in the PDF document</param>
		/// <param name="style">Marker's style</param>
		/// <param name="fontReference">Font's reference object</param>
		/// <param name="fontSize">Font's Size</param>
		/// <param name="fontColor">Font's Color</param>		
		public PDFPageMarker(int coordX, int coordY, PredefinedMarkerStyle style, PDFAbstractFont fontReference, int fontSize, PDFColor fontColor):
			this(coordX, coordY, style, fontReference, fontSize, fontColor, PredefinedAlignment.Left, "Page #n# of #N#")
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="coordX">X position in the PDF document</param>
		/// <param name="coordY">Y position in the PDF document</param>
		/// <param name="style">Marker's style</param>
		/// <param name="fontReference">Font's reference object</param>
		/// <param name="fontSize">Font's Size</param>
		/// <param name="pattern">Page Marker's Pattern</param>
		public PDFPageMarker(int coordX, int coordY, PredefinedMarkerStyle style, PDFAbstractFont fontReference, int fontSize, string pattern):
			this(coordX, coordY, style, fontReference, fontSize, PDFColor.Black, PredefinedAlignment.Left, pattern)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="coordX">X position in the PDF document</param>
		/// <param name="coordY">Y position in the PDF document</param>
		/// <param name="style">Marker's style</param>
		/// <param name="fontReference">Font's reference object</param>
		/// <param name="fontSize">Font's Size</param>
		/// <param name="alignment">Markers alignment horizontally on the page</param>
		public PDFPageMarker(int coordX, int coordY, PredefinedMarkerStyle style, PDFAbstractFont fontReference, int fontSize, PredefinedAlignment alignment):
			this(coordX, coordY, style, fontReference, fontSize, PDFColor.Black, alignment, "Page #n# of #N#")
		{		
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="coordX">X position in the PDF document</param>
		/// <param name="coordY">Y position in the PDF document</param>
		/// <param name="style">Marker's style</param>
		/// <param name="fontReference">Font's reference object</param>
		/// <param name="fontSize">Font's Size</param>
		/// <param name="alignment">Markers alignment horizontally on the page</param>
		/// <param name="fontColor">Font's Color</param>		
		public PDFPageMarker(int coordX, int coordY, PredefinedMarkerStyle style, PDFAbstractFont fontReference, int fontSize, PredefinedAlignment alignment, PDFColor fontColor):
			this(coordX, coordY, style, fontReference, fontSize, fontColor, alignment, "Page #n# of #N#")
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="coordX">X position in the PDF document</param>
		/// <param name="coordY">Y position in the PDF document</param>
		/// <param name="style">Marker's style</param>
		/// <param name="fontReference">Font's reference object</param>
		/// <param name="fontSize">Font's Size</param>
		/// <param name="alignment">Markers alignment horizontally on the page</param>
		/// <param name="pattern">Page Marker's Pattern</param>
		public PDFPageMarker(int coordX, int coordY, PredefinedMarkerStyle style, PDFAbstractFont fontReference, int fontSize, PredefinedAlignment alignment, string pattern):
			this(coordX, coordY, style, fontReference, fontSize, PDFColor.Black, alignment, pattern)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="coordX">X position in the PDF document</param>
		/// <param name="coordY">Y position in the PDF document</param>
		/// <param name="style">Marker's style</param>
		/// <param name="fontReference">Font's reference object</param>
		/// <param name="fontSize">Font's Size</param>
		/// <param name="fontColor">Font's Color</param>
		/// <param name="alignment">Markers alignment horizontally on the page</param>
		/// <param name="pattern">Page Marker's Pattern</param>
		public PDFPageMarker(int coordX, int coordY, PredefinedMarkerStyle style, PDFAbstractFont fontReference, int fontSize, PDFColor fontColor, PredefinedAlignment alignment, string pattern)
		{
			_style = style;
			_coordX = coordX;
			_coordY = coordY;
			_fontType = fontReference;
			_fontSize = fontSize;
			_fontColor = fontColor;
			_pattern = pattern;
			_alignment = alignment;
		}

		/// <summary>
		/// Method that creates a string for the page's marker
		/// </summary>
		/// <param name="pgIndex">Actual page</param>
		/// <param name="pgNum">Number of pages</param>
		/// <returns>Text that represents page's marker</returns>
		public string GetMarkerText(int pgIndex, int pgNum)
		{
			string strPgIndex;
			string strPgNum;
			switch (_style)
			{
			case PredefinedMarkerStyle.Arabic:
			default:
				strPgIndex = pgIndex.ToString();
				strPgNum = pgNum.ToString();
				break;
			case PredefinedMarkerStyle.Roman:
				strPgIndex = ArabicToRoman(pgIndex);
				strPgNum = ArabicToRoman(pgNum);
				break;
			}
			return _pattern.Replace("#n#", strPgIndex).Replace("#N#", strPgNum);
		}

		/// <summary>
		/// Private method that converts arabic numbers into roman numbers
		/// </summary>
		/// <param name="arabic">Arabic number</param>
		/// <returns>Equivalent roman number</returns>
		private string ArabicToRoman(int arabic)
		{			
			StringBuilder roman = new StringBuilder();

			while (arabic - 1000000 >= 0)
			{
				roman.Append("m");
				arabic -= 1000000;
			}
			while (arabic - 900000 >= 0)
			{
				roman.Append("cm");
				arabic -= 900000;
			}
			while (arabic - 500000 >= 0)
			{
				roman.Append("d");
				arabic -= 500000;
			}
			while (arabic - 400000 >= 0)
			{
				roman.Append("cd");
				arabic -= 400000;
			}
			while (arabic - 100000 >= 0)
			{
				roman.Append("c");
				arabic -= 100000;
			}
			while (arabic - 90000 >= 0)
			{
				roman.Append("xc");
				arabic -= 90000;
			}
			while (arabic - 50000 >= 0)
			{
				roman.Append("l");
				arabic -= 50000;
			}
			while (arabic - 40000 >= 0)
			{
				roman.Append("xl");
				arabic -= 40000;
			}
			while (arabic - 10000 >= 0)
			{
				roman.Append("x");
				arabic -= 10000;
			}
			while (arabic - 9000 >= 0)
			{
				roman.Append("Mx");
				arabic -= 9000;
			}
			while (arabic - 5000 >= 0)
			{
				roman.Append("v");
				arabic -= 5000;
			}
			while (arabic - 4000 >= 0)
			{
				roman.Append("Mv");
				arabic -= 4000;
			}
			while (arabic - 1000 >= 0)
			{
				roman.Append("M");
				arabic -= 1000;
			}
			while (arabic - 900 >= 0)
			{
				roman.Append("CM");
				arabic -= 900;
			}
			while (arabic - 500 >= 0)
			{
				roman.Append("D");
				arabic -= 500;
			}
			while (arabic - 400 >= 0)
			{
				roman.Append("CD");
				arabic -= 400;
			}
			while (arabic - 100 >= 0)
			{
				roman.Append("C");
				arabic -= 100;
			}
			while (arabic - 90 >= 0)
			{
				roman.Append("XC");
				arabic -= 90;
			}
			while (arabic - 50 >= 0)
			{
				roman.Append("L");
				arabic -= 50;
			}
			while (arabic - 40 >= 0)
			{
				roman.Append("XL");
				arabic -= 40;
			}
			while (arabic - 10 >= 0)
			{
				roman.Append("X");
				arabic -= 10;
			}
			while (arabic - 9 >= 0)
			{
				roman.Append("IX");
				arabic -= 9;
			}
			while (arabic - 5 >= 0)
			{
				roman.Append("V");
				arabic -= 5;
			}
			while (arabic - 4 >= 0)
			{
				roman.Append("IV");
				arabic -= 4;
			}
			while (arabic - 1 >= 0)
			{
				roman.Append("I");
				arabic -= 1;
			}

			return roman.ToString();			
		}
		
	}
}
