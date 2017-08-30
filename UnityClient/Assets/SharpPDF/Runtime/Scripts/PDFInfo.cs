using System;
using System.Text;
using System.Collections.Generic;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF {

	/// <summary>
	/// A Class that implements a PDF info.
	/// </summary>
	public class PDFInfo : IWritable
	{
		private int _objectIDInfo;
		private string _title = null;
		private string _author = null;
		private string _creatorApplication = null;
		private string _subject = null;
		private List<string> _keywords = null;

		/// <summary>
		/// Info's ID
		/// </summary>
		public int objectIDInfo
		{
			get
			{
				return _objectIDInfo;
			}
			set
			{
				_objectIDInfo = value;
			}
		}

		/// <summary>
		/// Document title
		/// </summary>
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}

		/// <summary>
		/// Document author
		/// </summary>
		public string Author
		{
			get
			{
				return _author;
			}
			set
			{
				_author = value;
			}
		}

		/// <summary>
		/// Document creator
		/// </summary>
		public string CreatorApplication
		{
			get
			{
				return _creatorApplication;
			}
			set
			{
				_creatorApplication = value;
			}
		}

		/// <summary>
		/// Document subject
		/// </summary>
		public string Subject
		{
			get
			{
				return _subject;
			}
			set
			{
				_subject = value;
			}
		}

		/// <summary>
		/// Document keywords
		/// </summary>
		public List<string> Keywords
		{
			get
			{
				return _keywords;
			}
		}
		
		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFInfo()
		{
			_keywords = new List<string>();
		}

		// Ensure that the string is valid for writing to a PDF
		private string ValidateString(string text)
		{
			return text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
		}

		/// <summary>
		/// Method that returns the PDF codes to write the Info in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public string GetText()
		{
			// Format the creation date and time
			DateTime now = DateTime.Now;
			string utcRelation = string.Format("{0:zzz}", now).Replace(":", "'") + "'";
			string creationDate = string.Format("D:{0:yyyyMMddHHmmss}{1}", now, utcRelation);

			StringBuilder strInfo = new StringBuilder();						
			strInfo.Append(_objectIDInfo.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			strInfo.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			if (!string.IsNullOrEmpty(_title))
				strInfo.Append("/Title (" + ValidateString(_title) + ")" + Convert.ToChar(13) + Convert.ToChar(10));
			if (!string.IsNullOrEmpty(_author))
				strInfo.Append("/Author (" + ValidateString(_author) + ")" + Convert.ToChar(13) + Convert.ToChar(10));
			if (!string.IsNullOrEmpty(_creatorApplication))
			{
				strInfo.Append("/Creator (" + ValidateString(_creatorApplication) + ")" + Convert.ToChar(13) + Convert.ToChar(10));
				strInfo.Append("/Producer (" + ValidateString(_creatorApplication) + ")" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			if (!string.IsNullOrEmpty(_subject))
				strInfo.Append("/Subject (" + ValidateString(_subject) + ")" + Convert.ToChar(13) + Convert.ToChar(10));
			if ((_keywords != null) && (_keywords.Count > 0))
			{
				string keywords = string.Join(";", _keywords.ToArray());
				strInfo.Append("/Keywords (" + ValidateString(keywords) + ")" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			strInfo.Append("/CreationDate (" + creationDate + ")" + Convert.ToChar(13) + Convert.ToChar(10));			
			strInfo.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			strInfo.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			return strInfo.ToString();
		}
		
	}
}
