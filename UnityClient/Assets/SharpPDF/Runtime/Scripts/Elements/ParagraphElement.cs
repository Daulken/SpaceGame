using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;
using SharpPDF.Fonts;

namespace SharpPDF.Elements {

	/// <summary>
	/// A Class that implements a PDF paragraph.
	/// </summary>
	public sealed class ParagraphElement : PDFElement
	{
		private List<ParagraphLine> _content;
		private int _fontSize;
		private int _lineHeight;
		private PDFAbstractFont _fontType;		

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newContent">Text of the paragraph</param>
		/// <param name="parWidth">Width of the paragraph</param>
		/// <param name="lineHeight">Height of a single line</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="fontReference">Font's type</param>
		/// <param name="x">X position in the PDF document</param>
		/// <param name="y">Y position in the PDF document</param>
		/// <param name="textColor">Font's color</param>
		public ParagraphElement(List<ParagraphLine> newContent, int parWidth, int lineHeight, int fontSize, PDFAbstractFont fontReference, int x, int y, PDFColor textColor)
		{
			_content = newContent;
			_fontSize = fontSize;
			_fontType = fontReference;
			_coordX = x;
			_coordY = y;
			_strokeColor = textColor;
			_width = parWidth;
			_height = lineHeight * _content.Count;
			_lineHeight = lineHeight;			
		}

		/// <summary>
		/// Method that adds a paragraph to the base page
		/// </summary>
		/// <param name="newText">Text of the paragraph</param>
		/// <param name="x">X position of the paragraph in the page</param>
		/// <param name="y">Y position of the paragraph in the page</param>
		/// <param name="fontReference">Font of the paragraph</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="lineHeight">Height of paragrah's lines</param>
		/// <param name="parWidth">Width of the paragraph</param>
		/// <param name="textColor">Text's color</param>
		/// <param name="parAlign">Align of the paragraph</param>
		public ParagraphElement(string newText, int x, int y, PDFAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, PDFColor textColor, PredefinedAlignment parAlign)
		{
			_content = PDFTextAdapter.FormatParagraph(ref newText, fontSize, fontReference, parWidth, 0, lineHeight, parAlign);
			_fontSize = fontSize;
			_fontType = fontReference;
			_coordX = x;
			_coordY = y;
			_strokeColor = textColor;
			_width = parWidth;
			_height = lineHeight * _content.Count;
			_lineHeight = lineHeight;			
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newContent">Text of the paragraph</param>
		/// <param name="paragraphWidth">Width of the paragraph</param>
		/// <param name="lineHeight">Height of a single line</param>
		/// <param name="newFontSize">Font's size</param>
		/// <param name="newFontType">Font's type</param>
		/// <param name="newCoordX">X position in the PDF document</param>
		/// <param name="newCoordY">Y position in the PDF document</param>
		public ParagraphElement(List<ParagraphLine> newContent, int paragraphWidth, int lineHeight, int newFontSize, PDFAbstractFont newFontType, int newCoordX, int newCoordY):
			this(newContent, paragraphWidth, lineHeight, newFontSize, newFontType, newCoordX, newCoordY, PDFColor.Black)
		{
		}

		/// <summary>
		/// Method that adds a paragraph to the base page
		/// </summary>
		/// <param name="newText">Text of the paragraph</param>
		/// <param name="x">X position of the paragraph in the page</param>
		/// <param name="y">Y position of the paragraph in the page</param>
		/// <param name="fontReference">Font of the paragraph</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="lineHeight">Height of paragrah's lines</param>
		/// <param name="parWidth">Width of the paragraph</param>
		public ParagraphElement(string newText, int x, int y, PDFAbstractFont fontReference, int fontSize, int lineHeight, int parWidth):
			this(newText, x, y, fontReference, fontSize, lineHeight, parWidth, PDFColor.Black, PredefinedAlignment.Left)
		{
		}

		/// <summary>
		/// Method that adds a paragraph to the base page
		/// </summary>
		/// <param name="newText">Text of the paragraph</param>
		/// <param name="x">X position of the paragraph in the page</param>
		/// <param name="y">Y position of the paragraph in the page</param>
		/// <param name="fontReference">Font of the paragraph</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="lineHeight">Height of paragrah's lines</param>
		/// <param name="parWidth">Width of the paragraph</param>
		/// <param name="parAlign">Align of the paragraph</param>
		public ParagraphElement(string newText, int x, int y, PDFAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, PredefinedAlignment parAlign):
			this(newText, x, y, fontReference, fontSize, lineHeight, parWidth, PDFColor.Black, parAlign)
		{
		}

		/// <summary>
		/// Method that adds a paragraph to the base page
		/// </summary>
		/// <param name="newText">Text of the paragraph</param>
		/// <param name="x">X position of the paragraph in the page</param>
		/// <param name="y">Y position of the paragraph in the page</param>
		/// <param name="fontReference">Font of the paragraph</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="lineHeight">Height of paragrah's lines</param>
		/// <param name="parWidth">Width of the paragraph</param>
		/// <param name="textColor">Text's color</param>
		public ParagraphElement(string newText, int x, int y, PDFAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, PDFColor textColor):
			this(newText, x, y, fontReference, fontSize, lineHeight, parWidth, textColor, PredefinedAlignment.Left)
		{
		}
		
		/// <summary>
		/// IEnumerable interface that contains paragraph's lines
		/// </summary>
		public List<ParagraphLine> content
		{
			get
			{
				return _content;
			}

			set
			{
				_content = value;
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
		/// Font's type
		/// </summary>
		public PDFAbstractFont fontType
		{
			get
			{
				return fontType;
			}

			set
			{
				_fontType = value;
			}
		}

		/// <summary>
		/// Method that returns the PDF codes to write the paragraph in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public override string GetText()
		{
			StringBuilder hexContent = new StringBuilder();
			hexContent.Append("q" + Convert.ToChar(13) + Convert.ToChar(10));
			hexContent.Append("BT" + Convert.ToChar(13) + Convert.ToChar(10));
			hexContent.Append("/F" + _fontType.fontNumber.ToString() + " " + _fontSize.ToString() + " Tf" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_strokeColor.isColor())
				hexContent.Append(_strokeColor.r + " " + _strokeColor.g + " " + _strokeColor.b + " rg" + Convert.ToChar(13) + Convert.ToChar(10));            
			hexContent.Append(_coordX.ToString() + " " + _coordY.ToString() + " Td" + Convert.ToChar(13) + Convert.ToChar(10));
			hexContent.Append("14 TL" + Convert.ToChar(13) + Convert.ToChar(10));
			foreach (ParagraphLine line in _content)
				hexContent.Append(line.GetText());
			hexContent.Append("ET" + Convert.ToChar(13) + Convert.ToChar(10));
			hexContent.Append("Q" + Convert.ToChar(13) + Convert.ToChar(10));
			
			StringBuilder resultText = new StringBuilder();
			resultText.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			resultText.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			resultText.Append("/Filter [/ASCIIHexDecode]" + Convert.ToChar(13) + Convert.ToChar(10));
			resultText.Append("/Length " + ((hexContent.Length * 2) + 1).ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultText.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			resultText.Append("stream" + Convert.ToChar(13) + Convert.ToChar(10));
			resultText.Append(PDFTextAdapter.HEXFormatter(hexContent.ToString()) + ">" + Convert.ToChar(13) + Convert.ToChar(10));			
			resultText.Append("endstream" + Convert.ToChar(13) + Convert.ToChar(10));            
			resultText.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));

			return resultText.ToString();
		}

		/// <summary>
		/// Method that clones the element
		/// </summary>
		/// <returns>Cloned object</returns>
		public override object Clone()
		{
			List<ParagraphLine> newContent = new List<ParagraphLine>();
			foreach (ParagraphLine myLine in this._content)
				newContent.Add((ParagraphLine)myLine.Clone());
			return new ParagraphElement(newContent, this._width, this._lineHeight, this._fontSize, this._fontType, this._coordX, this._coordY, (PDFColor)this._strokeColor.Clone());
		}
	}
}
