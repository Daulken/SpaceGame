using System;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;
using SharpPDF.Fonts;

namespace SharpPDF.Tables {
	
	/// <summary>
	/// Class that represents the style of a table's row
	/// </summary>
	public class PDFTableStyle
	{
		#region Protected_Properties

		/// <summary>
		/// Style's font
		/// </summary>
		protected PDFAbstractFont _font;
		/// <summary>
		/// Font's size
		/// </summary>
		protected int _fontSize;
		/// <summary>
		/// Font's color
		/// </summary>
		protected PDFColor _fontColor;
		/// <summary>
		/// Background color
		/// </summary>
		protected PDFColor _bgColor;

		#endregion

		/// <summary>
		/// Type of the Font
		/// </summary>
		public PDFAbstractFont fontReference
		{
			get
			{
				return _font;
			}			
		}

		/// <summary>
		/// Size of the Font
		/// </summary>
		public int fontSize
		{
			get
			{
				return _fontSize;
			}
		}

		/// <summary>
		/// Color of the Font
		/// </summary>
		public PDFColor fontColor
		{
			get
			{
				return _fontColor;
			}
		}

		/// <summary>
		/// Color of the BackGround
		/// </summary>
		public PDFColor bgColor
		{
			get
			{
				return _bgColor;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFTableStyle(PDFAbstractFont fontReference, int fontSize, PDFColor fontColor, PDFColor bgColor)
		{		
			_font = fontReference;
			_fontSize = fontSize;
			_fontColor = fontColor;
			_bgColor = bgColor;
		}
	}
}
