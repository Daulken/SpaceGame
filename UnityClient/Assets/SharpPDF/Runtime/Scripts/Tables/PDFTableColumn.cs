using System;
using System.Collections.Generic;
using SharpPDF.Elements;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;
using SharpPDF.Fonts;

namespace SharpPDF.Tables {
	
	/// <summary>
	/// A Class that implements an abstract table's column object
	/// </summary>
	public class PDFTableColumn : ISeparable, ICloneable
	{
		/// <summary>
		/// Event generated when the content or the height of a column is changed
		/// </summary>
		internal event ColumnTableEventHandler ColumnChanged;

		#region Protected_Properties
		
		/// <summary>
		/// Current X position inside the column
		/// </summary>
		protected int _currentX = 0;
		/// <summary>
		/// Current Y position inside the column
		/// </summary>
		protected int _currentY = 0;
		/// <summary>
		/// Column's width
		/// </summary>
		protected int _columnWidth = 0;
		/// <summary>
		/// Column's height
		/// </summary>
		protected int _columnHeight = 0;
		/// <summary>
		/// Column's Alignment
		/// </summary>
		protected PredefinedAlignment _columnAlign;
		/// <summary>
		/// Column's Vertical Alignment
		/// </summary>
		protected PredefinedVerticalAlignment _columnVerticalAlign;
		/// <summary>
		/// Column's Style
		/// </summary>
		protected PDFTableStyle _columnStyle = null;
		/// <summary>
		/// Column's elements
		/// </summary>
		protected List<PDFElement> _content = new List<PDFElement>(); 
		/// <summary>
		/// Container row
		/// </summary>
		protected PDFTableRow _containerRow;
		/// <summary>
		/// Start X position of the column
		/// </summary>
		protected int _startX = 0;
		/// <summary>
		/// Start Y position of the column
		/// </summary>
		protected int _startY = 0;
	
		#endregion

		#region Public_Properties

		/// <summary>
		/// Column's width
		/// </summary>
		public int columnWidth
		{
			get
			{
				return _columnWidth;
			}
		}

		/// <summary>
		/// Column's height
		/// </summary>
		public int columnHeight
		{
			get
			{
				return Math.Max(_columnHeight, _currentY);
			}
			set
			{
				if ((value - (_containerRow.containerTable.cellpadding * 2)) > _columnHeight)
				{
					_columnHeight = value - (_containerRow.containerTable.cellpadding * 2);
					if (ColumnChanged != null)
					{
						ColumnChanged(this, new ColumnTableEventArgs(this));
					}
				}
			}
		}

		/// <summary>
		/// Column's content align
		/// </summary>
		public PredefinedAlignment columnAlign
		{
			get
			{
				return _columnAlign;
			}
			set
			{
				_columnAlign = value;
			}
		}

		/// <summary>
		/// Column's content vertical align
		/// </summary>
		public PredefinedVerticalAlignment columnVerticalAlign
		{
			get
			{
				return _columnVerticalAlign;
			}
			set
			{
				_columnVerticalAlign = value;
			}
		}

		/// <summary>
		/// Column's style
		/// </summary>
		public PDFTableStyle columnStyle
		{
			get
			{
				return _columnStyle;
			}
			set
			{
				_columnStyle = value;
			}
		}

		#endregion

		#region Internal_Properties

		/// <summary>
		/// Container of the column
		/// </summary>
		internal PDFTableRow containerRow
		{
			get
			{
				return _containerRow;
			}
		}

		/// <summary>
		/// Start X position of the column
		/// </summary>
		internal int startX
		{
			get
			{
				return _startX;
			}
			set
			{
				_startX = value;
			}
		}

		/// <summary>
		/// Start Y position of the column
		/// </summary>
		internal int startY
		{
			get
			{
				return _startY;
			}
			set
			{
				_startY = value;
			}
		}

		#endregion

		#region ~Ctor

		internal PDFTableColumn(PDFTableRow containerRow, int columnWidth, PredefinedAlignment columnAlign, PDFTableStyle columnStyle):this(containerRow, columnWidth, columnAlign, PredefinedVerticalAlignment.Middle, columnStyle)
		{		
		}

		internal PDFTableColumn(PDFTableRow containerRow, int columnWidth, PredefinedAlignment columnAlign, PredefinedVerticalAlignment columnVerticalAlign, PDFTableStyle columnStyle)
		{
			_containerRow = containerRow;
			_columnWidth = columnWidth;
			_columnAlign = columnAlign;			
			_columnVerticalAlign = columnVerticalAlign;
			_columnStyle = columnStyle;
		}

		#endregion
		
		#region Image

		/// <summary>
		/// Method that adds an image to the column
		/// </summary>
		/// <param name="element">Image element</param>
		public void AddImage(ImageElement element)
		{
			AddImage(element.ObjectXReference, element.width, element.height);
		}

		/// <summary>
		/// Method that adds an image to the column
		/// </summary>
		/// <param name="imageReference">Image reference</param>
		/// <param name="height">Display height of the image</param>
		/// <param name="width">Display width of the image</param>
		public void AddImage(PDFImageReference imageReference, int width, int height)
		{
			if (width > _columnWidth)
			{
				height = Convert.ToInt32(Math.Round((((double)_columnWidth * (double)height) / (double)width)));
				width = _columnWidth;
			}

			ImageElement imageElement = new ImageElement(imageReference, _currentX, _currentY + height, width, height);

			_content.Add(imageElement);
			_currentY += imageElement.height;

			if (ColumnChanged != null)
				ColumnChanged(this, new ColumnTableEventArgs(this));

			imageElement = null;
		}
		#endregion

		#region Text

		/// <summary>
		/// Method that adds a text to the column with the Predefined column's style settings
		/// </summary>
		/// <param name="newText">Text to add</param>
		public void AddText(string newText)
		{
			AddText(newText, _columnStyle.fontReference, _columnStyle.fontSize, _columnStyle.fontColor);
		}

		/// <summary>
		/// Method that adds a text to the column
		/// </summary>
		/// <param name="newText">Text</param>
		/// <param name="fontReference">Font of the text</param>
		/// <param name="fontSize">Font's size</param>
		public void AddText(string newText, PDFAbstractFont fontReference, int fontSize)
		{
			AddText(newText, fontReference, fontSize, _columnStyle.fontColor);
		}

		/// <summary>
		/// Method that adds a text to the column
		/// </summary>
		/// <param name="newText">Text</param>
		/// <param name="fontReference">Font of the text</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="fontColor">Font's color</param>
		public void AddText(string newText, PDFAbstractFont fontReference, int fontSize, PDFColor fontColor)
		{
			int textWidth = fontReference.getWordWidth(newText, fontSize);
			if (textWidth > _columnWidth)
			{
				_content.Add(new TextElement(fontReference.cropWord(newText, _columnWidth, fontSize), fontSize, fontReference, _currentX, _currentY + (fontReference.fontDefinition.fontHeight * fontSize), fontColor));
			}
			else
			{
				_content.Add(new TextElement(newText, fontSize, fontReference, _currentX, _currentY + (fontReference.fontDefinition.fontHeight * fontSize), fontColor));
			}
			_currentY += _content[_content.Count - 1].height;
			if (ColumnChanged != null)
			{
				ColumnChanged(this, new ColumnTableEventArgs(this));
			}
		}

		#endregion

		#region Paragraph

		/// <summary>
		/// Method that adds a paragraph to the column with the Predefined column's style settings
		/// </summary>
		/// <param name="newText">Text to add</param>
		/// <param name="lineHeight">Height of a paragraph line</param>
		/// <param name="parAlign">Alignment of the paragraph. Remember that this is the alignment of the paragraph object and it "overloads" the column's alignment.</param>
		public void AddParagraph(string newText, int lineHeight, PredefinedAlignment parAlign)
		{
			AddParagraph(newText, _columnStyle.fontReference, _columnStyle.fontSize, lineHeight, _columnWidth, _columnStyle.fontColor, parAlign);
		}

		/// <summary>
		/// Method that adds a paragraph to the column
		/// </summary>
		/// <param name="newText">Text of the paragraph</param>
		/// <param name="fontReference">Text's font</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="lineHeight">Height of paragraph's lines</param>
		/// <param name="parWidth">Width of the paragraph</param>
		/// <param name="parAlign">Alignment of the paragraph</param>
		public void AddParagraph(string newText, PDFAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, SharpPDF.Enumerators.PredefinedAlignment parAlign)
		{
			AddParagraph(newText, fontReference, fontSize, lineHeight, parWidth, _columnStyle.fontColor, parAlign);
		}

		/// <summary>
		/// Method that adds a paragraph to the column
		/// </summary>
		/// <param name="newText">Text of the paragraph</param>
		/// <param name="fontReference">Text's font</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="lineHeight">Height of paragraph's lines</param>
		/// <param name="parWidth">Width of the paragraph</param>
		/// <param name="textColor">Font's color</param>
		/// <param name="parAlign">Alignment of the paragraph</param>
		public void AddParagraph(string newText, PDFAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, PDFColor textColor, SharpPDF.Enumerators.PredefinedAlignment parAlign)
		{
			if (parWidth > _columnWidth)
				parWidth = _columnWidth;
			List<ParagraphLine> lines = PDFTextAdapter.FormatParagraph(ref newText, fontSize, fontReference, parWidth, 0, lineHeight, parAlign);
			ParagraphElement newElement = new ParagraphElement(lines, parWidth, lineHeight, fontSize, fontReference, _currentX, _currentY, textColor);
			_content.Add(newElement);
			_currentY += (newElement.content.Count * lineHeight);
			if (ColumnChanged != null)
				ColumnChanged(this, new ColumnTableEventArgs(this));
			newElement = null;
		}

		#endregion

		#region Checked_Paragraph

		/// <summary>
		/// Method that adds a paragraph to the column with the Predefined column's style settings
		/// </summary>
		/// <param name="newText">Text to add</param>
		/// <param name="lineHeight">Height of a paragraph line</param>
		/// <param name="parAlign">Alignment of the paragraph. Remember that this is the alignment of the paragraph object and it "overloads" the column's alignment.</param>
		/// <param name="parHeight">Maximum height of the paragraph</param>
		public string AddCroppedParagraph(string newText, int lineHeight, PredefinedAlignment parAlign, int parHeight)
		{
			return AddCroppedParagraph(newText, _columnStyle.fontReference, _columnStyle.fontSize, lineHeight, _columnWidth, parHeight, _columnStyle.fontColor, parAlign);
		}

		/// <summary>
		/// Method that adds a paragraph to the column with a check on its maximum height
		/// </summary>
		/// <param name="newText">Text of the paragraph</param>
		/// <param name="fontReference">Paragraph's font</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="lineHeight">Height of paragraph's lines</param>
		/// <param name="parWidth">Width of the paragraph</param>
		/// <param name="parHeight">Maximum height of the paragraph</param>
		/// <param name="parAlign">Paragraph's Alignment</param>
		/// <returns>Text out of the paragraph's maximum height</returns>
		public string AddCroppedParagraph(string newText, PDFAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, SharpPDF.Enumerators.PredefinedAlignment parAlign)
		{
			return AddCroppedParagraph(newText, fontReference, fontSize, lineHeight, parWidth, parHeight, _columnStyle.fontColor, parAlign);
		}

		/// <summary>
		/// Method that adds a paragraph to the column with a check on its maximum height
		/// </summary>
		/// <param name="newText">Text of the paragraph</param>
		/// <param name="fontReference">Paragraph's font</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="lineHeight">Height of paragraph's lines</param>
		/// <param name="parWidth">Width of the paragraph</param>
		/// <param name="parHeight">Maximum height of the paragraph</param>
		/// /// <param name="textColor">Text's color</param>
		/// <param name="parAlign">Paragraph's Alignment</param>
		/// <returns>Text out of the paragraph's maximum height</returns>
		public string AddCroppedParagraph(string newText, PDFAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, PDFColor textColor, SharpPDF.Enumerators.PredefinedAlignment parAlign)
		{
			if (parWidth > _columnWidth)
				parWidth = _columnWidth;
			int maxLines = Convert.ToInt32(Math.Floor(((double)parHeight / (double)lineHeight)));
			List<ParagraphLine> lines = PDFTextAdapter.FormatParagraph(ref newText, fontSize, fontReference, parWidth, maxLines, lineHeight, parAlign);
			ParagraphElement newElement = new ParagraphElement(lines, parWidth, lineHeight, fontSize, fontReference, _currentX, _currentY, textColor);
			_content.Add(newElement);
			_currentY += (newElement.height);
			if (ColumnChanged != null)
				ColumnChanged(this, new ColumnTableEventArgs(this));
			newElement = null;			
			return newText;
		}

		#endregion

		#region Public_Methods

		/// <summary>
		/// Method that adds a break inside the column
		/// </summary>
		/// <param name="brHeight">Height of the break</param>
		public void InsertBreak(int brHeight)
		{
			_currentY += brHeight;
		}

		#endregion

		#region ISeparable

		/// <summary>
		/// Method that returns the basic components of the PDFTableColumn
		/// </summary>
		/// <returns>Collection of PDFElements</returns>
		public List<PDFElement> GetBasicElements()
		{
			List<PDFElement> ResultElements = new List<PDFElement>(_content);				
			foreach (PDFElement Elem in ResultElements)
			{
				switch (_columnAlign)
				{
				case PredefinedAlignment.Left:
				default:
					Elem.coordX += _startX;
					break;
				case PredefinedAlignment.Center:
					Elem.coordX += _startX + Convert.ToInt32(Math.Round((double)(_columnWidth - Elem.width) / 2));
					break;
				case PredefinedAlignment.Right:
					Elem.coordX += _startX + (_columnWidth - Elem.width);
					break;
				}
				switch (_columnVerticalAlign)
				{
				case PredefinedVerticalAlignment.Top:
				default:
					Elem.coordY = _startY - Elem.coordY;
					break;
				case PredefinedVerticalAlignment.Bottom:
					Elem.coordY = _startY - Elem.coordY - (_containerRow.rowHeight - _currentY - (_containerRow.containerTable.cellpadding * 2));
					break;
				case PredefinedVerticalAlignment.Middle:
					Elem.coordY = _startY - Elem.coordY - Convert.ToInt32(Math.Round((((double)_containerRow.rowHeight - (double)_currentY - ((double)_containerRow.containerTable.cellpadding * 2d)) / 2d)));
					break;
				}
			}
			if (!_containerRow.rowStyle.Equals(_columnStyle))
			{
				ResultElements.Insert(0, new RectangleElement(_startX - _containerRow.containerTable.cellpadding, _startY + _containerRow.containerTable.cellpadding, _startX + _columnWidth + _containerRow.containerTable.cellpadding, _startY + _containerRow.containerTable.cellpadding - _containerRow.rowHeight, _containerRow.containerTable.borderColor, _columnStyle.bgColor, _containerRow.containerTable.borderSize));
			}	
			return ResultElements;
		}

		#endregion

		#region ICloneable

		/// <summary>
		/// Method that clones the PDFTableColumn object
		/// </summary>
		/// <returns>Cloned Column</returns>
		public object Clone()
		{
			PDFTableColumn ReturnedColumn = new PDFTableColumn(_containerRow, _columnWidth, _columnAlign, _columnVerticalAlign, _columnStyle);
			ReturnedColumn._columnHeight = columnHeight;			
			ReturnedColumn._currentX = _currentX;
			ReturnedColumn._currentY = _currentY;
			foreach (PDFElement Elem in _content)
			{
				ReturnedColumn._content.Add((PDFElement)Elem.Clone());
			}
			return ReturnedColumn;
		}

		#endregion
	}
}
