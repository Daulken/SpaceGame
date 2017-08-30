using System;
using System.Text;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF.Elements {

	/// <summary>
	/// A Class that implements a PDF rectangle.
	/// </summary>
	public sealed class RectangleElement : PDFElement
	{
		private int _coordX1;
		private int _coordY1;
		private PDFLineStyle _lineStyle;

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position of the rectangle in the page</param>
		/// <param name="Y">Y position of the rectangle in the page</param>
		/// <param name="X1">X1 position of the rectangle in the page</param>
		/// <param name="Y1">Y1 position of the rectangle in the page</param>
		/// <param name="strokeColor">Border's color</param>
		/// <param name="fillColor">Rectancle's color</param>
		public RectangleElement(int X, int Y, int X1, int Y1, PDFColor strokeColor, PDFColor fillColor):
			this(X, Y, X1, Y1, strokeColor, fillColor, 1, PredefinedLineStyle.Normal)
		{			
		}

		/// <summary>
		/// Method that adds a rectangle to the page object
		/// </summary>
		/// <param name="X">X position of the rectangle in the page</param>
		/// <param name="Y">Y position of the rectangle in the page</param>
		/// <param name="X1">X1 position of the rectangle in the page</param>
		/// <param name="Y1">Y1 position of the rectangle in the page</param>
		/// <param name="strokeColor">Border's color</param>
		/// <param name="fillColor">Rectancle's color</param>
		/// <param name="newWidth">Border's width</param>
		public RectangleElement(int X, int Y, int X1, int Y1, PDFColor strokeColor, PDFColor fillColor, int newWidth):
			this(X, Y, X1, Y1, strokeColor, fillColor, newWidth, PredefinedLineStyle.Normal)
		{
		}

		/// <summary>
		/// Method that adds a rectangle to the page object
		/// </summary>
		/// <param name="X">X position of the rectangle in the page</param>
		/// <param name="Y">Y position of the rectangle in the page</param>
		/// <param name="X1">X1 position of the rectangle in the page</param>
		/// <param name="Y1">Y1 position of the rectangle in the page</param>
		/// <param name="strokeColor">Border's color</param>
		/// <param name="fillColor">Rectancle's color</param>
		/// <param name="newStyle">Border's style</param>
		public RectangleElement(int X, int Y, int X1, int Y1, PDFColor strokeColor, PDFColor fillColor, PredefinedLineStyle newStyle):
			this(X, Y, X1, Y1, strokeColor, fillColor, 1, newStyle)
		{
		}

		/// <summary>
		/// Method that adds a rectangle to the page object
		/// </summary>
		/// <param name="X">X position of the rectangle in the page</param>
		/// <param name="Y">Y position of the rectangle in the page</param>
		/// <param name="X1">X1 position of the rectangle in the page</param>
		/// <param name="Y1">Y1 position of the rectangle in the page</param>
		/// <param name="strokeColor">Border's color</param>
		/// <param name="fillColor">Rectancle's color</param>
		/// <param name="newWidth">Border's width</param>
		/// <param name="newStyle">Border's style</param>
		public RectangleElement(int X, int Y, int X1, int Y1, PDFColor strokeColor, PDFColor fillColor, int newWidth, PredefinedLineStyle newStyle)
		{
			_coordX = X;
			_coordY = Y;
			_coordX1 = X1;
			_coordY1 = Y1;
			_strokeColor = strokeColor;
			_fillColor = fillColor;	
			_lineStyle = new PDFLineStyle(newWidth, newStyle);
			_width = Math.Max(X, X1) - Math.Min(X, X1) + Convert.ToInt32(Math.Round((double)(newWidth / 2)));
			_height = Math.Max(Y, Y1) - Math.Min(Y, Y1) + Convert.ToInt32(Math.Round((double)(newWidth / 2)));
		}

		/// <summary>
		/// Method that returns the PDF codes to write the rectangle in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public override string GetText()
		{
			StringBuilder rectContent = new StringBuilder();
			rectContent.Append("q" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_strokeColor.isColor())
				rectContent.Append(_strokeColor.r + " " + _strokeColor.g + " " + _strokeColor.b + " RG" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_fillColor.isColor())
				rectContent.Append(_fillColor.r + " " + _fillColor.g + " " + _fillColor.b + " rg" + Convert.ToChar(13) + Convert.ToChar(10));
			rectContent.Append(_lineStyle.GetText() + Convert.ToChar(13) + Convert.ToChar(10));
			rectContent.Append(_coordX.ToString() + " " + _coordY.ToString() + " " + (_coordX1 - _coordX).ToString() + " " + (_coordY1 - _coordY).ToString() + " re" + Convert.ToChar(13) + Convert.ToChar(10));
			rectContent.Append("B" + Convert.ToChar(13) + Convert.ToChar(10));
			rectContent.Append("Q" + Convert.ToChar(13) + Convert.ToChar(10));
			
			StringBuilder resultRect = new StringBuilder();
			resultRect.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			resultRect.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			resultRect.Append("/Length " + rectContent.Length.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultRect.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			resultRect.Append("stream" + Convert.ToChar(13) + Convert.ToChar(10));
			resultRect.Append(rectContent.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultRect.Append("endstream" + Convert.ToChar(13) + Convert.ToChar(10));
			resultRect.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));

			return resultRect.ToString();
		}

		/// <summary>
		/// Method that clones the element
		/// </summary>
		/// <returns>Cloned object</returns>
		public override object Clone()
		{
			return new RectangleElement(_coordX, _coordY, _coordX1, _coordY1, (PDFColor)_strokeColor.Clone(), (PDFColor)_fillColor.Clone(), _lineStyle.width, _lineStyle.lineStyle);
		}

	}
}
