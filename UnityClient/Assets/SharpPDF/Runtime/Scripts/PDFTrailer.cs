using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF {
	
	/// <summary>
	/// A Class that implements a PDF trailer.
	/// </summary>
	internal class PDFTrailer : IWritable
	{
		
		private int _lastObjectID;
		private List<string> _objectOffsets;
		private long _xrefOffset;

		/// <summary>
		/// The offset of the XREF table
		/// </summary>
		public long xrefOffset
		{
			get
			{
				return _xrefOffset;
			}

			set
			{
				_xrefOffset = value;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="lastObjectID">The ID of the last object in the document</param>
		public PDFTrailer(int lastObjectID)
		{
			_lastObjectID = lastObjectID;
			_objectOffsets = new List<string>();
		}

		/// <summary>
		/// Class's destructor
		/// </summary>
		~PDFTrailer()
		{
			_objectOffsets = null;
		}

		/// <summary>
		/// Method that adds an object to the trailer object
		/// </summary>
		/// <param name="offset"></param>
		public void addObject(string offset)
		{
			_objectOffsets.Add(new string('0', 10 - offset.Length) + offset);			
		}

		/// <summary>
		/// Method that returns the PDF codes to write the trailer in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public string GetText()
		{
			StringBuilder content = new StringBuilder();
			content.Append("xref" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("0 " + (_lastObjectID + 1).ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("0000000000 65535 f" + Convert.ToChar(13) + Convert.ToChar(10));
			foreach (string offset in _objectOffsets)
				content.Append(offset + " 00000 n" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("trailer" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("/Size " + (_lastObjectID + 1).ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("/Root 1 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("/Info 2 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("startxref" + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append(_xrefOffset.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			content.Append("%%EOF");
			return content.ToString();
		}

	}
}
