using System;

namespace SharpPDF.Tables {
	
	/// <summary>
	/// Arguments of a ColumnTableEventHandler
	/// </summary>
	public class ColumnTableEventArgs : EventArgs
	{
		/// <summary>
		/// Column that generates the event
		/// </summary>
		protected PDFTableColumn _column;

		/// <summary>
		/// Column that generates the event
		/// </summary>
		public PDFTableColumn Column
		{
			get
			{
				return this._column;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="column">Column that generates the event</param>
		public ColumnTableEventArgs(PDFTableColumn column)
		{
			this._column = column;
		}
	}
}
