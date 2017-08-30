using System;
using System.Collections;
using System.Collections.Generic;
using SharpPDF.Elements;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;
using SharpPDF.PDFControls;

namespace SharpPDF.Tables {

	/// <summary>
	/// A Class that implements an abstract table object
	/// </summary>
	public class PDFTable : PDFPositionableObject, IEnumerable, ISeparable
	{
		#region Protected_Properties

		/// <summary>
		/// Container document
		/// </summary>
		protected PDFDocument _containerDocument;
		/// <summary>
		/// Table's header
		/// </summary>
		protected PDFTableHeader _tableHeader;
		/// <summary>
		/// Table's row style
		/// </summary>
		protected PDFTableStyle _rowStyle;
		/// <summary>
		/// Table's alternate row style
		/// </summary>
		protected PDFTableStyle _alternateRowStyle;
		/// <summary>
		/// Variable that sets the current row style
		/// </summary>
		protected bool _isAlternateStyle = false;
		/// <summary>
		/// Table's rows
		/// </summary>
		protected List<PDFTableRow> _rows = new List<PDFTableRow>();
		/// <summary>
		/// Table's border size
		/// </summary>
		protected int _borderSize;
		/// <summary>
		/// Table's border color
		/// </summary>
		protected PDFColor _borderColor;
		/// <summary>
		/// Table's cellpadding
		/// </summary>
		protected int _cellpadding;

		#endregion

		#region Public_Properties

		/// <summary>
		/// Border's Size
		/// </summary>
		public int borderSize
		{
			get
			{
				return _borderSize;
			}
			set
			{
				_borderSize = value;
			}
		}

		/// <summary>
		/// Border's color
		/// </summary>
		public PDFColor borderColor
		{
			get
			{
				return _borderColor;
			}
			set
			{
				_borderColor = value;
			}
		}

		/// <summary>
		/// Table's header
		/// </summary>
		public PDFTableHeader tableHeader
		{
			get
			{
				return _tableHeader;
			}
		}

		/// <summary>
		/// The style of a table's row
		/// </summary>
		public PDFTableStyle rowStyle
		{
			get
			{
				return _rowStyle;
			}
		}

		/// <summary>
		/// The alternate style of a table's row
		/// </summary>
		public PDFTableStyle alternateRowStyle
		{
			get
			{
				return _alternateRowStyle;
			}
		}

		/// <summary>
		/// The cellpadding of the table
		/// </summary>
		public int cellpadding
		{
			get
			{
				return _cellpadding;
			}
		}

		/// <summary>
		/// Table's rows
		/// </summary>
		public PDFTableRow this[int index]
		{
			get
			{
				if (index < 0 || index >= _rows.Count)
				{
					throw new PDFBadRowIndexException();
				}
				else
				{
					return _rows[index];
				}
			}
		}
		
		/// <summary>
		/// The number of rows
		/// </summary>
		public int rowsCount
		{
			get
			{
				return _rows.Count;
			}
		}

		#endregion

		#region ~Ctor

		/// <summary>
		/// Class's Constructor
		/// </summary>
		/// <param name="containerDocument">Container of the table</param>
		public PDFTable(PDFDocument containerDocument):
			this(containerDocument, 1, PDFColor.Black, 5, new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White), new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White), new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White))
		{

		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="containerDocument">Container of the table</param>
		/// <param name="border">Width of the border</param>
		/// <param name="borderColor">Color of the border</param>
		public PDFTable(PDFDocument containerDocument, int border, PDFColor borderColor):
			this(containerDocument, border, borderColor, 5, new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White), new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White), new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White))
		{

		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="containerDocument">Container of the table</param>
		/// <param name="cellpadding">Cellpadding of the table</param>
		public PDFTable(PDFDocument containerDocument, int cellpadding):
			this(containerDocument, 1, PDFColor.Black, cellpadding, new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White), new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White), new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White))
		{

		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="containerDocument">Container of the table</param>
		/// <param name="border">Width of the border</param>
		/// <param name="borderColor">Color of the border</param>
		/// <param name="cellpadding">Cellpadding of the table</param>
		public PDFTable(PDFDocument containerDocument, int border, PDFColor borderColor, int cellpadding):
			this(containerDocument, border, borderColor, cellpadding, new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White), new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White), new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White))
		{

		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="containerDocument">Container of the table</param>
		/// <param name="border">Width of the border</param>
		/// <param name="borderColor">Color of the border</param>
		/// <param name="cellpadding">Cellpadding of the table</param>
		/// <param name="headerStyle">Style of the header</param>
		public PDFTable(PDFDocument containerDocument, int border, PDFColor borderColor, int cellpadding, PDFTableStyle headerStyle):
			this(containerDocument, border, borderColor, cellpadding, headerStyle, new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White), new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White))
		{

		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="containerDocument">Container of the table</param>
		/// <param name="border">Width of the border</param>
		/// <param name="borderColor">Color of the border</param>
		/// <param name="cellpadding">Cellpadding of the table</param>
		/// <param name="headerStyle">Style of the header</param>
		/// <param name="rowStyle">Style of the rows</param>
		public PDFTable(PDFDocument containerDocument, int border, PDFColor borderColor, int cellpadding, PDFTableStyle headerStyle, PDFTableStyle rowStyle):
			this(containerDocument, border, borderColor, cellpadding, headerStyle, rowStyle, new PDFTableStyle(containerDocument.GetFontReference(PredefinedFont.Helvetica), 10, PDFColor.Black, PDFColor.White))
		{

		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="containerDocument">Container of the table</param>
		/// <param name="border">Width of the border</param>
		/// <param name="borderColor">Color of the border</param>
		/// <param name="cellpadding">Cellpadding of the table</param>
		/// <param name="headerStyle">Style of the header</param>
		/// <param name="rowStyle">Style of the rows</param>
		/// <param name="alternateRowStyle">Alternate style of the rows</param>
		public PDFTable(PDFDocument containerDocument, int border, PDFColor borderColor, int cellpadding, PDFTableStyle headerStyle, PDFTableStyle rowStyle, PDFTableStyle alternateRowStyle)
		{
			_containerDocument = containerDocument;
			_tableHeader = new PDFTableHeader(this, headerStyle);
			_tableHeader.ColumnAdded += new ColumnTableEventHandler(ColumnAdded);
			_rowStyle = rowStyle;
			_alternateRowStyle = alternateRowStyle;
			_borderSize = border;
			_borderColor = borderColor;
			_cellpadding = cellpadding;
		}

		#endregion

		#region Public_Methods

		/// <summary>
		/// Method that creates a new row and adds it to the table
		/// </summary>
		/// <returns>A new PDFTableRow object</returns>
		public PDFTableRow AddRow()
		{
			PDFTableRow newRow = new PDFTableRow(_tableHeader);
			_rows.Add(newRow);			
			return newRow;
		}

		/// <summary>
		/// Enumerator for table's rows
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return _rows.GetEnumerator();
		}		
		
		/// <summary>
		/// Method that returns the basic elements of the table
		/// </summary>
		/// <returns>Collection of basic elements</returns>
		public List<PDFElement> GetBasicElements()
		{
			int startX = _coordX;
			int startY = _coordY;
			List<PDFElement> elements = new List<PDFElement>();
			if (_tableHeader.visible)
			{
				this._tableHeader.startX = startX;
				this._tableHeader.startY = startY;
				elements.AddRange(this._tableHeader.GetBasicElements());
				this._tableHeader.startX = 0;
				this._tableHeader.startY = 0;
			}
			startY -= _tableHeader.rowHeight;
			foreach (PDFTableRow Row in _rows)
			{
				Row.startX = startX;
				Row.startY = startY;
				elements.AddRange(Row.GetBasicElements());
				Row.startX = 0;
				Row.startY = 0;
				startY -= Row.rowHeight;
			}			
			return elements;
		}

		/// <summary>
		/// Method that crop the table at the defined table's height
		/// </summary>
		/// <param name="tabHeight">Maximum height of the table</param>
		/// <returns>Table outside the maximum height</returns>
		public PDFTable CropTable(int tabHeight)
		{
			int rowIndex = 0;
			int currentHeight = 0;
			bool finished = false;
			PDFTable ResultTable = null;
			if (_tableHeader.visible)
			{
				tabHeight -= tableHeader.rowHeight;
			}
			while (rowIndex < _rows.Count && currentHeight <= tabHeight && !finished)
			{				
				if (_rows[rowIndex].rowHeight > tabHeight)
				{
					throw new PDFBadRowHeightException();
				}
				else if (_rows[rowIndex].rowHeight <= tabHeight - currentHeight)
				{
					currentHeight += _rows[rowIndex].rowHeight;
					rowIndex++;
				}
				else
				{
					finished = true;
				}
			}
			if (rowIndex < _rows.Count)
			{
				ResultTable = new PDFTable(_containerDocument);
				ResultTable._borderColor = _borderColor;
				ResultTable._borderSize = _borderSize;
				ResultTable._cellpadding = _cellpadding;
				ResultTable._rowStyle = _rowStyle;
				ResultTable._tableHeader = (PDFTableHeader)this._tableHeader.Clone();				
				ResultTable._rowStyle = this._rowStyle;
				ResultTable._alternateRowStyle = this._alternateRowStyle;
				while (rowIndex < _rows.Count)
				{
					ResultTable._rows.Add(_rows[rowIndex]);
					_rows.RemoveAt(rowIndex);
				}
			}
			return ResultTable;
		}

		#endregion

		#region Internal_Methods

		internal PDFTableStyle GetAndAlternateCurrentStyle()
		{			
			if (this._isAlternateStyle && this._alternateRowStyle != null)
			{
				this._isAlternateStyle = false;
				return this._alternateRowStyle;				
			}
			else
			{
				this._isAlternateStyle = true;
				return this._rowStyle;				
			}
		}					

		#endregion

		#region Protected_Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ColumnAdded(object sender, ColumnTableEventArgs e)
		{
			foreach (PDFTableRow myRow in _rows)
			{
				myRow.AddColumn(new PDFTableColumn(myRow, e.Column.columnWidth, e.Column.columnAlign, e.Column.columnVerticalAlign, myRow.rowStyle));				
			}
		}

		#endregion

	}
}
