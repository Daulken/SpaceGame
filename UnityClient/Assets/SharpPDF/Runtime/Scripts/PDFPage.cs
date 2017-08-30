using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SharpPDF.Elements;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;
using SharpPDF.Fonts;

namespace SharpPDF {

	/// <summary>
	/// A Class that implements a PDF page.
	/// </summary>
	public class PDFPage : PDFBasePage, IWritable
	{
		private int _height;
		private int _width;
		private int _objectID;
		private int _pageTreeID;		

		/// <summary>
		/// Page's ID
		/// </summary>
		public int objectID
		{
			get
			{
				return _objectID;
			}
			set
			{
				_objectID = value;
			}
		}

		/// <summary>
		/// PageTree's ID
		/// </summary>
		public int pageTreeID
		{
			get
			{
				return _pageTreeID;
			}
			set
			{
				_pageTreeID = value;
			}
		}

		/// <summary>
		/// Page's height
		/// </summary>
		public int height
		{
			get
			{
				return _height;
			}
		}

		/// <summary>
		/// Page's width
		/// </summary>
		public int width
		{
			get
			{
				return _width;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		internal PDFPage(PDFDocument containerDoc):
			base(containerDoc)
		{
			_height = 792;
			_width = 612;			
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		internal PDFPage(PredefinedPageSize PredefinedSize, PDFDocument containerDoc):
			base(containerDoc)
		{
			switch (PredefinedSize)
			{
			case PredefinedPageSize.SharpPDFFormat:
				_height = 792;
				_width = 612;			
				break;
			case PredefinedPageSize.A1Page:
				_height = 2288;
				_width = 1655;			
				break;
			case PredefinedPageSize.A2Page:
				_height = 1684;
				_width = 1191;			
				break;
			case PredefinedPageSize.A3Page:
				_height = 1191;
				_width = 842;			
				break;
			case PredefinedPageSize.A4Page:
				_height = 842;
				_width = 595;			
				break;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="newHeight">Page's height</param>
		/// <param name="newWidth">Page's width</param>
		/// <param name="containerDoc">Container Document</param>
		internal PDFPage(int newHeight, int newWidth, PDFDocument containerDoc):
			base(containerDoc)
		{
			_height = newHeight;
			_width = newWidth;
		}

		/// <summary>
		/// Class's distructor
		/// </summary>
		~PDFPage()
		{
			_containerDoc = null;
			_elements = null;
		}

		/// <summary>
		/// Method that returns the PDF codes to write the page in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public string GetText()
		{
			StringBuilder pageContent = new StringBuilder();
			StringBuilder objContent = new StringBuilder();
			StringBuilder annotContent = new StringBuilder();
			pageContent.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			pageContent.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			pageContent.Append("/Type /Page" + Convert.ToChar(13) + Convert.ToChar(10));
			pageContent.Append("/Parent " + _pageTreeID.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			pageContent.Append("/Resources <<" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_containerDoc._fonts.Count > 0)
			{
				pageContent.Append("/Font <<");
				foreach (PDFAbstractFont font in _containerDoc._fonts.Values)
					pageContent.Append("/F" + font.fontNumber.ToString() + " " + font.objectID.ToString() + " 0 R "); 
				pageContent.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			if (_containerDoc._images.Count > 0)
			{
				pageContent.Append("/XObject <<");
				foreach (PDFImageReferenceBase image in _containerDoc._images)
					pageContent.Append("/I" + image.objectID.ToString() + " " + image.objectID.ToString() + " 0 R "); 
				pageContent.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			pageContent.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			pageContent.Append("/MediaBox [0 0 " + _width + " " + _height + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			pageContent.Append("/CropBox [0 0 " + _width + " " + _height + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			pageContent.Append("/Rotate 0" + Convert.ToChar(13) + Convert.ToChar(10));
			pageContent.Append("/ProcSet [/PDF /Text /ImageC]" + Convert.ToChar(13) + Convert.ToChar(10));
			foreach (PDFElement item in _elements)
			{
				// smeyer82: Add element to the correct content list
				if (item is AnnotationElement)
					annotContent.Append(item.objectID.ToString() + " 0 R ");
				else
					objContent.Append(item.objectID.ToString() + " 0 R ");				
			}
			if (objContent.Length > 0)
				pageContent.Append("/Contents [" + objContent.ToString() + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			//smeyer82: create annotation entrys
			if (annotContent.Length > 0)
				pageContent.Append("/Annots [" + annotContent.ToString() + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			pageContent.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			pageContent.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			objContent = null;
			return pageContent.ToString();
		}
	}
}