//*****************************************************************
//******************Inserted by smeyer82 (2004)********************
//*****************************************************************
using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using SharpPDF;
using SharpPDF.Enumerators;

namespace SharpPDF.Elements {

	/// <summary>
	/// Creates an annotation element.
	/// </summary>
	public sealed class AnnotationElement : PDFElement
	{
		private string _content;
		private Enumerators.PredefinedAnnotationStyle _style;
		private List<string> _styleNames = new List<string>();
		private bool _open = false;

		/// <summary>
		/// Initializes the array of style names
		/// </summary>
		internal void InitializeStyleNames()
		{
			_styleNames.Insert((int)PredefinedAnnotationStyle.None, "Note");
			_styleNames.Insert((int)PredefinedAnnotationStyle.Comment, "Comment");
			_styleNames.Insert((int)PredefinedAnnotationStyle.Key, "Key");
			_styleNames.Insert((int)PredefinedAnnotationStyle.Note, "Note");
			_styleNames.Insert((int)PredefinedAnnotationStyle.Help, "Help");
			_styleNames.Insert((int)PredefinedAnnotationStyle.NewParagraph, "NewParagraph");
			_styleNames.Insert((int)PredefinedAnnotationStyle.Paragraph, "Paragraph");
			_styleNames.Insert((int)PredefinedAnnotationStyle.Insert, "Insert");
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newContent">Content of the annotation element</param>
		/// <param name="newCoordX">X position on page</param>
		/// <param name="newCoordY">Y position on page</param>
		/// <param name="newColor">The color of the annotation element</param>
		/// <param name="newStyle">The style of the annotation element</param>
		public AnnotationElement(string newContent, int newCoordX, int newCoordY, PDFColor newColor, Enumerators.PredefinedAnnotationStyle newStyle):
			this(newContent, newCoordX, newCoordY, newColor, newStyle, false)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newContent">Content of the annotation element</param>
		/// <param name="newCoordX">X position on page</param>
		/// <param name="newCoordY">Y position on page</param>
		public AnnotationElement(string newContent, int newCoordX, int newCoordY):
			this(newContent, newCoordX, newCoordY, PDFColor.LightGray, PredefinedAnnotationStyle.Comment, false)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newContent">Content of the annotation element</param>
		/// <param name="newCoordX">X position on page</param>
		/// <param name="newCoordY">Y position on page</param>
		/// <param name="open">Makes the annotation element open or closes at starttime</param>
		public AnnotationElement(string newContent, int newCoordX, int newCoordY, bool open):
			this(newContent, newCoordX, newCoordY, PDFColor.LightGray, PredefinedAnnotationStyle.Comment, open)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newContent">Content of the annotation element</param>
		/// <param name="newCoordX">X position on page</param>
		/// <param name="newCoordY">Y position on page</param>
		/// <param name="newColor">The color of the annotation element</param>
		public AnnotationElement(string newContent, int newCoordX, int newCoordY, PDFColor newColor):
			this(newContent, newCoordX, newCoordY, newColor, PredefinedAnnotationStyle.Comment, false)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newContent">Content of the annotation element</param>
		/// <param name="newCoordX">X position on page</param>
		/// <param name="newCoordY">Y position on page</param>
		/// <param name="newColor">The color of the annotation element</param>
		/// <param name="open">Makes the annotation element open or closes at starttime</param>
		public AnnotationElement(string newContent, int newCoordX, int newCoordY, PDFColor newColor, bool open):
			this(newContent, newCoordX, newCoordY, newColor, PredefinedAnnotationStyle.Comment, open)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newContent">Content of the annotation element</param>
		/// <param name="newCoordX">X position on page</param>
		/// <param name="newCoordY">Y position on page</param>
		/// <param name="newStyle">The style of the annotation element</param>
		public AnnotationElement(string newContent, int newCoordX, int newCoordY, Enumerators.PredefinedAnnotationStyle newStyle):
			this(newContent, newCoordX, newCoordY, PDFColor.LightGray, newStyle, false)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newContent">Content of the annotation element</param>
		/// <param name="newCoordX">X position on page</param>
		/// <param name="newCoordY">Y position on page</param>
		/// <param name="newStyle">The style of the annotation element</param>
		/// <param name="open">Makes the annotation element open or closes at starttime</param>
		public AnnotationElement(string newContent, int newCoordX, int newCoordY, Enumerators.PredefinedAnnotationStyle newStyle, bool open):
			this(newContent, newCoordX, newCoordY, PDFColor.LightGray, newStyle, open)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newContent">Content of the annotation element</param>
		/// <param name="newCoordX">X position on page</param>
		/// <param name="newCoordY">Y position on page</param>
		/// <param name="newColor">The color of the annotation element</param>
		/// <param name="newStyle">The style of the annotation element</param>
		/// <param name="open">Makes the annotation element open or closes at starttime</param>
		public AnnotationElement(string newContent, int newCoordX, int newCoordY, PDFColor newColor, Enumerators.PredefinedAnnotationStyle newStyle, bool open)
		{
			_content = newContent;
			_coordX = newCoordX;
			_coordY = newCoordY;
			_strokeColor = newColor;
			_style = newStyle;
			_open = open;
			_height = 0;
			_width = 0;

			InitializeStyleNames();
		}
		
		/// <summary>
		/// Sets the content of the annotation
		/// </summary>
		public string content
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
		/// Sets the color of the annotation element
		/// </summary>
		public PDFColor color
		{
			get
			{
				return _strokeColor;
			}

			set
			{
				_strokeColor = value;
			}
		}

		/// <summary>
		/// The style of the Annotation
		/// </summary>
		public PredefinedAnnotationStyle style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
			}
		}

		/// <summary>
		/// Method that returns the PDF codes to write the annotation in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public override string GetText()
		{
			StringBuilder result = new StringBuilder();

			result.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			result.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			result.Append("/Type /Annot" + Convert.ToChar(13) + Convert.ToChar(10));
			result.Append("/Subtype /Text" + Convert.ToChar(13) + Convert.ToChar(10));
			result.Append("/Rect [" + _coordX.ToString() + " 0 0 " + _coordY.ToString() + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			result.Append("/Contents (" + _content + ")" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_open)
				result.Append("/Open true" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_strokeColor.isColor())
				result.Append("/C [" + _strokeColor.r + " " + _strokeColor.g + " " + _strokeColor.b + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_style != Enumerators.PredefinedAnnotationStyle.None)
				result.Append("/Name /" + _styleNames[(int)_style] + Convert.ToChar(13) + Convert.ToChar(10));
			result.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			result.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));

			return result.ToString();
		}

		/// <summary>
		/// Method that clones the element
		/// </summary>
		/// <returns>Cloned object</returns>
		public override object Clone()
		{
			return new AnnotationElement((string)this._content.Clone(), this._coordX, this._coordY, (PDFColor)this._strokeColor.Clone(), this._style);
		}

	}
}
