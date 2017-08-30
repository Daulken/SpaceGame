using System;
using System.Text;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF {
	
	/// <summary>
	/// A Class that implements a PDF line style.
	/// </summary>
	internal class PDFLineStyle : IWritable
	{
		PredefinedLineStyle _lineStyle = PredefinedLineStyle.Normal;
		int _width = 1;

		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFLineStyle()
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newWidth">Line's size</param>
		public PDFLineStyle(int newWidth)
		{
			_width = newWidth;
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newStyle">Line's style</param>
		public PDFLineStyle(PredefinedLineStyle newStyle)
		{
			_lineStyle = newStyle;
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newWidth">Line's size</param>
		/// <param name="newStyle">Line's style</param>
		public PDFLineStyle(int newWidth, PredefinedLineStyle newStyle)
		{
			_width = newWidth;
			_lineStyle = newStyle;
		}

		/// <summary>
		/// Line's size
		/// </summary>
		public int width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		/// <summary>
		/// Line's style
		/// </summary>
		public PredefinedLineStyle lineStyle
		{
			get
			{
				return _lineStyle;
			}
			set
			{
				_lineStyle = value;
			}
		}

		/// <summary>
		/// Method that returns the PDF codes to write the line style in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public string GetText()
		{
			StringBuilder resultText = new StringBuilder();
			resultText.Append(_width.ToString() + " w" + Convert.ToChar(13) + Convert.ToChar(10));
			switch (_lineStyle)
			{
			case PredefinedLineStyle.Outlined:
				resultText.Append("[4 4] 0 d" + Convert.ToChar(13) + Convert.ToChar(10));
				break;
			case PredefinedLineStyle.OutlinedSmall:
				resultText.Append("[2 2] 0 d" + Convert.ToChar(13) + Convert.ToChar(10));
				break;
			case PredefinedLineStyle.OutlinedBig:
				resultText.Append("[6 6] 0 d" + Convert.ToChar(13) + Convert.ToChar(10));
				break;
			}
			return resultText.ToString();
		}
	}
}
