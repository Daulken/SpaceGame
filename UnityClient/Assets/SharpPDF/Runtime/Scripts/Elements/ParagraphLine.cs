using System;
using System.Text;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;
using SharpPDF.Fonts;

namespace SharpPDF.Elements {
	
	/// <summary>
	/// A Class that implements a PDF paragraph's line.
	/// </summary>
	public sealed class ParagraphLine : IWritable, ICloneable
	{
		private string _strLine;
		private int _lineLeftMargin;
		private int _lineTopMargin;
		private PDFAbstractFont _fontType;

		/// <summary>
		/// The left margin of the line
		/// </summary>
		public int lineLeftMargin
		{
			get
			{
				return _lineLeftMargin;
			}
		}

		/// <summary>
		/// The top margin of the line
		/// </summary>
		public int lineTopMargin
		{
			get
			{
				return _lineTopMargin;
			}
		}

		/// <summary>
		/// The text of the line
		/// </summary>
		public string strLine
		{
			get
			{
				return _strLine;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="strLine">Text of the line</param>
		/// <param name="lineTopMargin">Top margin</param>
		/// <param name="lineLeftMargin">Left margin</param>
		/// <param name="fontType">Font Type</param>
		public ParagraphLine(string strLine, int lineTopMargin, int lineLeftMargin, PDFAbstractFont fontType)
		{
			_strLine = strLine;
			_lineTopMargin = lineTopMargin;
			_lineLeftMargin = lineLeftMargin;
			_fontType = fontType;
		}

		/// <summary>
		/// Method that returns the PDF codes to write the paragraph's line in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public string GetText()
		{
			StringBuilder resultString = new StringBuilder();			
			resultString.Append(_lineLeftMargin.ToString() + " -" + _lineTopMargin.ToString() + " Td" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_fontType is PDFTrueTypeFont)
			{				
				resultString.Append("(" + PDFTextAdapter.CheckText(_fontType.encodeText(_strLine)) + ") Tj" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			else
			{
				resultString.Append("(" + _fontType.encodeText(PDFTextAdapter.CheckText(_strLine)) + ") Tj" + Convert.ToChar(13) + Convert.ToChar(10));				
			}			
			resultString.Append("-" + _lineLeftMargin.ToString().Replace(",", ".") + " 0 Td" + Convert.ToChar(13) + Convert.ToChar(10));			
			return resultString.ToString();
		}

		#region ICloneable

		/// <summary>
		/// Method that clones the element
		/// </summary>
		/// <returns>Cloned object</returns>
		public object Clone()
		{
			return new ParagraphLine((string)this._strLine.Clone(), this._lineTopMargin, this._lineLeftMargin, this._fontType);
		}

		#endregion
	}
}
