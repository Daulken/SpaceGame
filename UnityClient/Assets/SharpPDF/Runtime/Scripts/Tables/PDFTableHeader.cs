using System;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF.Tables {

	/// <summary>
	/// A Class that implements an abstract table's header object
	/// </summary>
	public class PDFTableHeader : PDFTableRow
	{

		/// <summary>
		/// Event generated when a column is added to the table's header
		/// </summary>
		internal event ColumnTableEventHandler ColumnAdded;
		
		/// <summary>
		/// Property that tells if the PDFTableHeader has to be showed
		/// </summary>
		protected bool _visible;

		/// <summary>
		/// Property that tells if the PDFTableHeader has to be showed
		/// </summary>
		public bool visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		internal PDFTableHeader(PDFTable Table, PDFTableStyle headerStyle):base(Table)
		{
			_visible = true;
			_rowHeight = Table.cellpadding * 2;
			_rowWidth = Table.cellpadding * 2;
			_rowStyle = headerStyle;
		}

		/// <summary>
		/// Method that adds a new column
		/// </summary>
		/// <param name="columnWidth">Width of the column</param>
		public void AddColumn(int columnWidth)
		{
			this.AddColumn(columnWidth, PredefinedAlignment.Left, PredefinedVerticalAlignment.Middle);
		}

		/// <summary>
		/// Method that adds a new column
		/// </summary>
		/// <param name="columnWidth">Width of the column</param>
		/// <param name="columnAlign">Alignment of the column</param>
		public void AddColumn(int columnWidth, PredefinedAlignment columnAlign)
		{
			this.AddColumn(columnWidth, columnAlign, PredefinedVerticalAlignment.Middle);
		}

		/// <summary>
		/// Method that adds a new column
		/// </summary>
		/// <param name="columnWidth">Width of the column</param>
		/// <param name="columnAlign">Alignment of the column</param>
		/// <param name="columnVerticalAlign">Vertical alignment of the column</param>
		public void AddColumn(int columnWidth, PredefinedAlignment columnAlign, PredefinedVerticalAlignment columnVerticalAlign)
		{
			_cols.Add(new PDFTableColumn(this, columnWidth - (_containerTable.cellpadding * 2), columnAlign, columnVerticalAlign, _rowStyle));			
			_cols[_cols.Count - 1].ColumnChanged += new ColumnTableEventHandler(ColumnChanged);
			this._rowWidth = this.calculateWidth();
			if (this.ColumnAdded != null)
			{
				this.ColumnAdded(this, new ColumnTableEventArgs(_cols[_cols.Count - 1]));
			}
		}
		
		/// <summary>
		/// Method that clones the PDFTableHeader Object
		/// </summary>
		/// <returns>Cloned Object</returns>
		public override object Clone()
		{
			PDFTableHeader ReturnedRow = new PDFTableHeader(_containerTable, new PDFTableStyle(this._rowStyle.fontReference, this._rowStyle.fontSize, this._rowStyle.fontColor, this._rowStyle.bgColor));
			ReturnedRow.rowVerticalAlign = this._rowVerticalAlignment;			
			ReturnedRow.visible = this._visible;
			foreach (PDFTableColumn Col in _cols)
			{
				ReturnedRow._cols.Add((PDFTableColumn)Col.Clone());				
			}
			ReturnedRow.rowHeight = this._rowHeight;
			ReturnedRow._rowWidth = this._rowWidth;
			return ReturnedRow;
		}

	}
}
