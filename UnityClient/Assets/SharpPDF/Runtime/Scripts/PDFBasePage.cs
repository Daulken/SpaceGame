using System;
using System.Collections;
using System.Collections.Generic;
using SharpPDF.Bookmarks;
using SharpPDF.Elements;
using SharpPDF.Enumerators;
using SharpPDF.Exceptions;
using SharpPDF.Fonts;
using SharpPDF.PDFControls;
using SharpPDF.Tables;

namespace SharpPDF {

	/// <summary>
	/// Class that represents a base page of the PDF document
	/// </summary>
	public abstract class PDFBasePage
	{
		/// <summary>
		/// Elements of the Page
		/// </summary>
		protected List<PDFElement> _elements;
		/// <summary>
		/// Container Document
		/// </summary>
		protected PDFDocument _containerDoc;

		internal List<PDFElement> elements
		{
			get
			{
				return _elements;
			}
		}

		internal PDFBasePage(PDFDocument Container)
		{
			_containerDoc = Container;
			_elements = new List<PDFElement>();
		}
	
		#region IAbsoluteContainer

		/// <summary>
		/// Method that adds a generic element into the page
		/// </summary>
		/// <param name="element">PDFElement object</param>
		public void AddElement(PDFElement element)
		{
			if (element != null)
				_elements.Add(element);
		}

		/// <summary>
		/// Method that adds a PDFControl into the page
		/// </summary>
		/// <param name="control">PDFControl object</param>
		public void AddControl(PDFControl control)
		{
			if (control != null)
				_elements.AddRange(control.GetBasicElements());
		}

		/// <summary>
		/// Method that adds a table to the page
		/// </summary>
		/// <param name="table">PDFTable object</param>
		public void AddTable(PDFTable table)
		{
			if (table != null)
				_elements.AddRange(table.GetBasicElements());
		}

		/// <summary>
		/// Method that adds a table to the page, with a check on the maximum height
		/// </summary>
		/// <param name="myTable">Table's object</param>
		/// <param name="tabHeight">Maximum height of the table</param>
		public PDFTable AddTable(PDFTable table, int tableHeight)
		{
			if (table != null)
			{
				PDFTable newTable = table.CropTable(tableHeight);
				_elements.AddRange(table.GetBasicElements());
				return newTable;
			}
			return null;
		}

		#endregion

		#region Checked_Paragraph

		/// <summary>
		/// Method that adds a paragraph to the base page with a check on its maximum height
		/// </summary>
		/// <param name="newText">Text of the paragraph</param>
		/// <param name="x">X position of the paragraph inside the page</param>
		/// <param name="y">Y position of the paragraph inside the page</param>
		/// <param name="fontReference">Font of the paragraph</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="lineHeight">Height of paragraph's lines</param>
		/// <param name="parWidth">Width of the paragraph</param>
		/// <param name="parHeight">Maximum height of the paragraph</param>
		/// <returns>Text out of the paragraph's maximum height</returns>
		public string AddCroppedParagraph(string newText, int x, int y, PDFAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight)
		{
			return AddCroppedParagraph(newText, x, y, fontReference, fontSize, lineHeight, parWidth, parHeight, PDFColor.Black, PredefinedAlignment.Left);
		}

		/// <summary>
		/// Method that adds a paragraph to the base page with a check on its maximum height
		/// </summary>
		/// <param name="newText">Text of the paragraph</param>
		/// <param name="x">X position of the paragraph inside the page</param>
		/// <param name="y">Y position of the paragraph inside the page</param>
		/// <param name="fontReference">Font of the paragraph</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="lineHeight">Height of paragraph's lines</param>
		/// <param name="parWidth">Width of the paragraph</param>
		/// <param name="parHeight">Maximum height of the paragraph</param>
		/// <param name="textColor">Text's color</param>
		/// <returns>Text out of the paragraph's maximum height</returns>
		public string AddCroppedParagraph(string newText, int x, int y, PDFAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, PDFColor textColor)
		{
			return AddCroppedParagraph(newText, x, y, fontReference, fontSize, lineHeight, parWidth, parHeight, textColor, PredefinedAlignment.Left);
		}

		/// <summary>
		/// Method that adds a paragraph to the base page with a check on its maximum height
		/// </summary>
		/// <param name="newText">Text of the paragraph</param>
		/// <param name="x">X position of the paragraph inside the page</param>
		/// <param name="y">Y position of the paragraph inside the page</param>
		/// <param name="fontReference">Font of the paragraph</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="lineHeight">Height of paragraph's lines</param>
		/// <param name="parWidth">Width of the paragraph</param>
		/// <param name="parHeight">Maximum height of the paragraph</param>
		/// <param name="parAlign">Align of the paragraph</param>
		/// <returns>Text out of the paragraph's maximum height</returns>
		public string AddCroppedParagraph(string newText, int x, int y, PDFAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, PredefinedAlignment parAlign)
		{
			return AddCroppedParagraph(newText, x, y, fontReference, fontSize, lineHeight, parWidth, parHeight, PDFColor.Black, parAlign);
		}

		/// <summary>
		/// Method that adds a paragraph to the base page with a check on its maximum height
		/// </summary>
		/// <param name="newText">Text of the paragraph</param>
		/// <param name="x">X position of the paragraph inside the page</param>
		/// <param name="y">Y position of the paragraph inside the page</param>
		/// <param name="fontReference">Font of the paragraph</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="lineHeight">Height of paragraph's lines</param>
		/// <param name="parWidth">Width of the paragraph</param>
		/// <param name="parHeight">Maximum height of the paragraph</param>
		/// <param name="textColor">Text's color</param>
		/// <param name="parAlign">Align of the paragraph</param>
		/// <returns>Text out of the paragraph's maximum height</returns>
		public string AddCroppedParagraph(string newText, int x, int y, PDFAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, PDFColor textColor, PredefinedAlignment parAlign)
		{
			_elements.Add(new ParagraphElement(new List<ParagraphLine>(PDFTextAdapter.FormatParagraph(ref newText, fontSize, fontReference, parWidth, Convert.ToInt32(Math.Floor(((double)parHeight / (double)lineHeight))), lineHeight, parAlign)), parWidth, lineHeight, fontSize, fontReference, x, y, textColor));
			return newText;
		}
		
		#endregion		
		
	}
}
