//*****************************************************************
//******************Bezier Fix by scarlip (2004)*******************
//*****************************************************************
using System;
using System.Text;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF.Elements {
	
	/// <summary>
	/// A Class that implements a PDF circle.
	/// </summary>
	public sealed class CircleElement : PDFElement
	{
		private const float kappa = 0.5522847498f;
		private int _ray;
		private PDFLineStyle _lineStyle;

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position in the PDF document</param>
		/// <param name="Y">Y position in the PDF document</param>
		/// <param name="Ray">Ray of the circle</param>
		/// <param name="strokeColor">Color of circle's border</param>
		/// <param name="fillColor">Color of the circle</param>
		public CircleElement(int X, int Y, int Ray, PDFColor strokeColor, PDFColor fillColor):
			this(X, Y, Ray, strokeColor, fillColor, 1, PredefinedLineStyle.Normal)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position in the PDF document</param>
		/// <param name="Y">Y position in the PDF document</param>
		/// <param name="Ray">Ray of the circle</param>
		/// <param name="strokeColor">Color of circle's border</param>
		/// <param name="fillColor">Color of the circle</param>
		/// <param name="newWidth">Border's size</param>
		public CircleElement(int X, int Y, int Ray, PDFColor strokeColor, PDFColor fillColor, int newWidth):
			this(X, Y, Ray, strokeColor, fillColor, newWidth, PredefinedLineStyle.Normal)
		{		
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position in the PDF document</param>
		/// <param name="Y">Y position in the PDF document</param>
		/// <param name="Ray">Ray of the circle</param>
		/// <param name="strokeColor">Color of circle's border</param>
		/// <param name="fillColor">Color of the circle</param>
		/// <param name="newStyle">Border's style</param>
		public CircleElement(int X, int Y, int Ray, PDFColor strokeColor, PDFColor fillColor, PredefinedLineStyle newStyle):
			this(X, Y, Ray, strokeColor, fillColor, 1, newStyle)
		{		
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="X">X position in the PDF document</param>
		/// <param name="Y">Y position in the PDF document</param>
		/// <param name="Ray">Ray of the circle</param>
		/// <param name="strokeColor">Color of circle's border</param>
		/// <param name="fillColor">Color of the circle</param>
		/// <param name="newWidth">Border's size</param>
		/// <param name="newStyle">Border's style</param>
		public CircleElement(int X, int Y, int Ray, PDFColor strokeColor, PDFColor fillColor, int newWidth, PredefinedLineStyle newStyle)
		{
			_coordX = X;
			_coordY = Y;
			_ray = Ray;
			_strokeColor = strokeColor;
			_fillColor = fillColor;
			_lineStyle = new PDFLineStyle(newWidth, newStyle);
			_height = (Ray * 2) + Convert.ToInt32(Math.Round((double)(newWidth / 2)));
			_width = _height;
		}

		/// <summary>
		/// Method that returns the PDF codes to write the circle in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public override string GetText()
		{
			// http://www.whizkidtech.redprince.net/bezier/circle/            
			StringBuilder circleContent = new StringBuilder();
			circleContent.Append("q" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_strokeColor.isColor())
				circleContent.Append(_strokeColor.r + " " + _strokeColor.g + " " + _strokeColor.b + " RG" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_fillColor.isColor())
				circleContent.Append(_fillColor.r + " " + _fillColor.g + " " + _fillColor.b + " rg" + Convert.ToChar(13) + Convert.ToChar(10));
			circleContent.Append(_lineStyle.GetText() + Convert.ToChar(13) + Convert.ToChar(10));
			float kappa_ray = _ray * kappa;
			// move to 12 (x,y+r) m
			circleContent.Append(_coordX.ToString() + " " + Math.Round((float)(_coordY + _ray), 0).ToString() + " m" + Convert.ToChar(13) + Convert.ToChar(10));
			// arc to 3 (x+k,y+r) (x+r,y+k) (x+r,y) c
			circleContent.Append(Math.Round((_coordX + kappa_ray), 0).ToString() + " " + Math.Round((float)(_coordY + _ray), 0).ToString() + " " +
                Math.Round((float)(_coordX + _ray), 0).ToString() + " " + Math.Round((float)(_coordY + kappa_ray), 0).ToString() + " " +
                Math.Round((float)(_coordX + _ray), 0).ToString() + " " + _coordY.ToString() + " c" + Convert.ToChar(13) + Convert.ToChar(10));
			// arc to 6 (x+r,y-k) (x+k,y-r) (x,y-r) c
			circleContent.Append(Math.Round((float)(_coordX + _ray), 0).ToString() + " " + Math.Round((float)(_coordY - kappa_ray), 0).ToString() + " " +
                Math.Round((float)(_coordX + kappa_ray), 0).ToString() + " " + Math.Round((float)(_coordY - _ray), 0).ToString() + " " +
                _coordX.ToString() + " " + Math.Round((float)(_coordY - _ray), 0).ToString() + " c" + Convert.ToChar(13) + Convert.ToChar(10));
			// arc to 9 (x-k,y-r) (x-r,y-k) (x-r,y) c
			circleContent.Append(Math.Round((float)(_coordX - kappa_ray), 0).ToString() + " " + Math.Round((float)(_coordY - _ray), 0).ToString() + " " +
                Math.Round((float)(_coordX - _ray), 0).ToString() + " " + Math.Round((float)(_coordY - kappa_ray), 0).ToString() + " " +
                Math.Round((float)(_coordX - _ray), 0).ToString() + " " + _coordY.ToString() + " c" + Convert.ToChar(13) + Convert.ToChar(10));
			// arc to 12 (x-r,y+k) (x-k,y+r) (x,y+r) c
			circleContent.Append(Math.Round((float)(_coordX - _ray), 0).ToString() + " " + Math.Round((float)(_coordY + kappa_ray), 0).ToString() + " " +
                Math.Round((float)(_coordX - kappa_ray), 0).ToString() + " " + Math.Round((float)(_coordY + _ray), 0).ToString() + " " +
                _coordX.ToString() + " " + Math.Round((float)(_coordY + _ray), 0).ToString() + " c" + Convert.ToChar(13) + Convert.ToChar(10));
			circleContent.Append("B" + Convert.ToChar(13) + Convert.ToChar(10));
			circleContent.Append("Q" + Convert.ToChar(13) + Convert.ToChar(10));

			StringBuilder resultCircle = new StringBuilder();
			resultCircle.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			resultCircle.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			resultCircle.Append("/Length " + circleContent.Length.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultCircle.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			resultCircle.Append("stream" + Convert.ToChar(13) + Convert.ToChar(10));
			resultCircle.Append(circleContent.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultCircle.Append("endstream" + Convert.ToChar(13) + Convert.ToChar(10));
			resultCircle.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));

			return resultCircle.ToString();            
		}

		/// <summary>
		/// Method that clones the element
		/// </summary>
		/// <returns>Cloned object</returns>
		public override object Clone()
		{
			return new CircleElement(this._coordX, this._coordY, this._ray, (PDFColor)this._strokeColor.Clone(), (PDFColor)this._fillColor.Clone(), this._lineStyle.width, this._lineStyle.lineStyle);
		}
	}
}

