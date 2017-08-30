using System;
using System.Text;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF.Elements {
	
	/// <summary>
	/// A Class that implements a PDF line.
	/// </summary>
	public sealed class LineElement : PDFElement
	{
		private int _coordX1;
		private int _coordY1;
		private PDFLineStyle _lineStyle;

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position in the PDF document</param>
		/// <param name="Y">Y position in the PDF document</param>
		/// <param name="X1">X1 position in the PDF document</param>
		/// <param name="Y1">Y1 position in the PDF document</param>
		public LineElement(int X, int Y, int X1, int Y1):
			this(X, Y, X1, Y1, 1, PredefinedLineStyle.Normal, PDFColor.Black)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position in the PDF document</param>
		/// <param name="Y">Y position in the PDF document</param>
		/// <param name="X1">X1 position in the PDF document</param>
		/// <param name="Y1">Y1 position in the PDF document</param>
		/// <param name="newWidth">Line's size</param>
		public LineElement(int X, int Y, int X1, int Y1, int newWidth):
			this(X, Y, X1, Y1, newWidth, PredefinedLineStyle.Normal, PDFColor.Black)
		{		
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position in the PDF document</param>
		/// <param name="Y">Y position in the PDF document</param>
		/// <param name="X1">X1 position in the PDF document</param>
		/// <param name="Y1">Y1 position in the PDF document</param>		
		/// <param name="newStyle">Line's style</param>
		public LineElement(int X, int Y, int X1, int Y1, PredefinedLineStyle newStyle):
			this(X, Y, X1, Y1, 1, newStyle, PDFColor.Black)
		{		
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position in the PDF document</param>
		/// <param name="Y">Y position in the PDF document</param>
		/// <param name="X1">X1 position in the PDF document</param>
		/// <param name="Y1">Y1 position in the PDF document</param>		
		/// <param name="newWidth">Line's size</param>
		/// /// <param name="newStyle">Line's style</param>
		public LineElement(int X, int Y, int X1, int Y1, int newWidth, PredefinedLineStyle newStyle):
			this(X, Y, X1, Y1, newWidth, newStyle, PDFColor.Black)
		{		
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position in the PDF document</param>
		/// <param name="Y">Y position in the PDF document</param>
		/// <param name="X1">X1 position in the PDF document</param>
		/// <param name="Y1">Y1 position in the PDF document</param>	
		/// <param name="newColor">Line's color</param>
		public LineElement(int X, int Y, int X1, int Y1, PDFColor newColor):
			this(X, Y, X1, Y1, 1, PredefinedLineStyle.Normal, newColor)
		{		
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position in the PDF document</param>
		/// <param name="Y">Y position in the PDF document</param>
		/// <param name="X1">X1 position in the PDF document</param>
		/// <param name="Y1">Y1 position in the PDF document</param>	
		/// <param name="newStyle">Line's style</param>
		/// <param name="newColor">Line's color</param>
		public LineElement(int X, int Y, int X1, int Y1, PredefinedLineStyle newStyle, PDFColor newColor):
			this(X, Y, X1, Y1, 1, newStyle, newColor)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position in the PDF document</param>
		/// <param name="Y">Y position in the PDF document</param>
		/// <param name="X1">X1 position in the PDF document</param>
		/// <param name="Y1">Y1 position in the PDF document</param>	
		/// <param name="newWidth">Line's width</param>
		/// <param name="newColor">Line's color</param>
		public LineElement(int X, int Y, int X1, int Y1, int newWidth, PDFColor newColor):
			this(X, Y, X1, Y1, newWidth, PredefinedLineStyle.Normal, newColor)
		{		
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position in the PDF document</param>
		/// <param name="Y">Y position in the PDF document</param>
		/// <param name="X1">X1 position in the PDF document</param>
		/// <param name="Y1">Y1 position in the PDF document</param>	
		/// <param name="newWidth">Line's size</param>
		/// <param name="newStyle">Line's style</param>
		/// <param name="newColor">Line's color</param>
		public LineElement(int X, int Y, int X1, int Y1, int newWidth, PredefinedLineStyle newStyle, PDFColor newColor)
		{
			_coordX = X;
			_coordY = Y;
			_coordX1 = X1;
			_coordY1 = Y1;
			_strokeColor = newColor;
			_lineStyle = new PDFLineStyle(newWidth, newStyle);
		}

		
		/// <summary>
		/// Method that returns the PDF codes to write the line in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public override string GetText()
		{
			StringBuilder lineContent = new StringBuilder();
			if (_strokeColor.isColor())
				lineContent.Append(_strokeColor.r + " " + _strokeColor.g + " " + _strokeColor.b + " RG" + Convert.ToChar(13) + Convert.ToChar(10));
			lineContent.Append("q" + Convert.ToChar(13) + Convert.ToChar(10));
			lineContent.Append(_lineStyle.GetText() + Convert.ToChar(13) + Convert.ToChar(10));
			lineContent.Append(_coordX.ToString() + " " + _coordY.ToString() + " m" + Convert.ToChar(13) + Convert.ToChar(10));
			lineContent.Append(_coordX1.ToString() + " " + _coordY1.ToString() + " l" + Convert.ToChar(13) + Convert.ToChar(10));
			lineContent.Append("S" + Convert.ToChar(13) + Convert.ToChar(10));
			lineContent.Append("Q" + Convert.ToChar(13) + Convert.ToChar(10));

			StringBuilder resultLine = new StringBuilder();
			resultLine.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			resultLine.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			resultLine.Append("/Length " + lineContent.Length.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultLine.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			resultLine.Append("stream" + Convert.ToChar(13) + Convert.ToChar(10));
			resultLine.Append(lineContent.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultLine.Append("endstream" + Convert.ToChar(13) + Convert.ToChar(10));
			resultLine.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));

			return  resultLine.ToString();			
		}

		/// <summary>
		/// Method that clones the element
		/// </summary>
		/// <returns>Cloned object</returns>
		public override object Clone()
		{
			return new LineElement(this._coordX, this._coordY, this._coordX1, this._coordY1, this._lineStyle.width, this._lineStyle.lineStyle, this._strokeColor);
		}


	}
}