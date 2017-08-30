using System;
using System.IO;
using System.Text;
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
	/// A Class that implements a PDF document.
	/// </summary>
	public class PDFDocument : IDisposable
	{
		private bool _openBookmark;
		private PDFHeader _header;
		private PDFInfo _info;
		private PDFOutlines _outlines;
		private PDFPageTree _pageTree;
		private List<PDFPage> _pages = null;
		private PDFPageMarker _pageMarker = null;
		private PDFPersistentPage _persistentPage = null;
		internal Dictionary<string, PDFAbstractFont>_fonts = new Dictionary<string, PDFAbstractFont>();
		internal List<PDFImageReferenceBase> _images = new List<PDFImageReferenceBase>();

		/// <summary>
		/// Document's page marker
		/// </summary>
		public PDFPageMarker pageMarker
		{
			set
			{
				_pageMarker = value;
			}
		}

		/// <summary>
		/// Document's information storage
		/// </summary>
		public PDFInfo info
		{
			get
			{
				return _info;
			}
		}

		/// <summary>
		/// Document's bookmark visible flag
		/// </summary>
		public bool bookmarksOpen
		{
			get
			{
				return _openBookmark;
			}
			set
			{
				_openBookmark = value;
			}
		}

		/// <summary>
		/// Document's persistent page
		/// </summary>
		public PDFPersistentPage persistentPage
		{
			get
			{
				return _persistentPage;
			}
		}

		/// <summary>
		/// Collection of pdf's page
		/// </summary>
		public PDFPage this[int index]
		{
			get
			{
				return _pages[index];
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		public PDFDocument()
		{
			_openBookmark = false;
			_outlines = new PDFOutlines();
			_pages = new List<PDFPage>();
			_persistentPage = new PDFPersistentPage(this);
			_info = new PDFInfo();
		}

		/// <summary>
		/// Dispose Method
		/// </summary>
		public void Dispose()
		{
			_header = null;
			_info = null;
			_outlines = null;
			_fonts = null;
			_pages = null;
			_pageTree = null;
			_pageMarker = null;
			_persistentPage = null;
		}

		#region Font_Mananagment

		/// <summary>
		/// Method thata adds a Font Reference of a True Type file inside the pdfDocument
		/// </summary>
		/// <param name="fontReference">Font's reference name</param>
		/// <param name="resourceName">Name of the font's TextAsset resource in Unity</param>
		public void AddTrueTypeFont(string fontReference, string resourceName)
		{
			if (!IsFontAdded(fontReference))
			{
				if (PDFFontFactory.IsPredefinedFont(fontReference))
					throw new PDFFontIsPredefinedException();
				else
					_fonts[fontReference] = PDFFontFactory.GetFontObject(resourceName, _fonts.Count + 1, DocumentFontType.TrueTypeFont);
			}
		}

		/// <summary>
		/// Methot that shows if a font is loaded inside the document
		/// </summary>
		/// <param name="fontReference">Font's reference name</param>
		/// <returns>Boolean value that tells if the font is loaded</returns>
		public bool IsFontAdded(string fontReference)
		{
			return _fonts.ContainsKey(fontReference);
		}

		/// <summary>
		/// Method that return the PDFAbstractFont Object Reference of a Standard Font
		/// </summary>
		/// <param name="fontType">Predefined Font</param>
		/// <returns>PDFAbstractFont Object</returns>
		public PDFAbstractFont GetFontReference(PredefinedFont fontType)
		{
			return GetFontReference(PDFFontFactory.GetPredefinedFontName(fontType));
		}

		/// <summary>
		/// Method that return the PDFAbstractFont Object Reference of a Font
		/// </summary>
		/// <param name="fontReference">Font's reference name</param>
		/// <returns>PDFAbstractFont Object</returns>
		public PDFAbstractFont GetFontReference(string fontReference)
		{
			if (!IsFontAdded(fontReference))
			{
				if (PDFFontFactory.IsPredefinedFont(fontReference))
					_fonts[fontReference] = PDFFontFactory.GetFontObject(fontReference, _fonts.Count + 1, DocumentFontType.Predefinedfont);
				else
					throw new PDFFontNotLoadedException();
			}
			return _fonts[fontReference];
		}

		#endregion

		/// <summary>
		/// Method that writes the PDF document on the stream
		/// </summary>
		/// <param name="outStream">Output stream</param>
		public void CreatePDF(Stream outStream)
		{
			long outputBufferLength = 0;

			PDFTrailer trailer = InitializeObjectsForPDFCreation();
			try
			{
				// PDF's definition
				outputBufferLength += WriteToBuffer(outStream, @"%PDF-1.5" + Convert.ToChar(13) + Convert.ToChar(10));

				// PDF's header object
				trailer.addObject(outputBufferLength.ToString());
				outputBufferLength += WriteToBuffer(outStream, _header.GetText());

				// PDF's info object
				trailer.addObject(outputBufferLength.ToString());
				outputBufferLength += WriteToBuffer(outStream, _info.GetText());

				// PDF's outlines object
				trailer.addObject(outputBufferLength.ToString());
				outputBufferLength += WriteToBuffer(outStream, _outlines.GetText());

				// PDF's bookmarks
				foreach (PDFBookmarkNode bookmark in _outlines.GetBookmarks())
				{
					trailer.addObject(outputBufferLength.ToString());
					outputBufferLength += WriteToBuffer(outStream, bookmark.GetText());
				}

				// Fonts's initialization
				foreach (PDFAbstractFont font in _fonts.Values)
				{
					trailer.addObject(outputBufferLength.ToString());
					outputBufferLength += WriteToBuffer(outStream, font.GetText());
					if (font is PDFTrueTypeFont)
					{
						PDFTrueTypeFont ttfFont = (PDFTrueTypeFont)font;

						trailer.addObject(outputBufferLength.ToString());
						outputBufferLength += WriteToBuffer(outStream, ttfFont.GetFontDescriptorText());
						trailer.addObject(outputBufferLength.ToString());
						outputBufferLength += WriteToBuffer(outStream, ttfFont.GetFontDescendantText());
						trailer.addObject(outputBufferLength.ToString());
						outputBufferLength += WriteToBuffer(outStream, ttfFont.GetToUnicodeText());

						// Font's stream
						trailer.addObject(outputBufferLength.ToString());
						outputBufferLength += WriteToBuffer(outStream, ttfFont.GetStreamText());
						outputBufferLength += WriteToBuffer(outStream, "stream" + Convert.ToChar(13) + Convert.ToChar(10));
						outputBufferLength += WriteToBuffer(outStream, ttfFont.subsetStream);
						outputBufferLength += WriteToBuffer(outStream, Convert.ToChar(13).ToString());
						outputBufferLength += WriteToBuffer(outStream, Convert.ToChar(10).ToString());
						outputBufferLength += WriteToBuffer(outStream, "endstream" + Convert.ToChar(13) + Convert.ToChar(10));
						outputBufferLength += WriteToBuffer(outStream, "endobj" + Convert.ToChar(13) + Convert.ToChar(10));
					}
				}

				// PDF's pagetree object
				trailer.addObject(outputBufferLength.ToString());
				outputBufferLength += WriteToBuffer(outStream, _pageTree.GetText());

				// Generation of PDF's pages
				foreach (PDFPage page in _pages)
				{
					trailer.addObject(outputBufferLength.ToString());
					outputBufferLength += WriteToBuffer(outStream, page.GetText());

					foreach (PDFElement element in page.elements)
					{
						trailer.addObject(outputBufferLength.ToString());
						outputBufferLength += WriteToBuffer(outStream, element.GetText());
					}
				}

				// PDF's Images
				foreach (PDFImageReferenceBase image in _images)
				{
					trailer.addObject(outputBufferLength.ToString());
					outputBufferLength += WriteToBuffer(outStream, image.GetText());
					outputBufferLength += WriteToBuffer(outStream, "stream" + Convert.ToChar(13) + Convert.ToChar(10));
					outputBufferLength += WriteToBuffer(outStream, image.content);
					outputBufferLength += WriteToBuffer(outStream, Convert.ToChar(13).ToString());
					outputBufferLength += WriteToBuffer(outStream, Convert.ToChar(10).ToString());
					outputBufferLength += WriteToBuffer(outStream, "endstream" + Convert.ToChar(13) + Convert.ToChar(10));
					outputBufferLength += WriteToBuffer(outStream, "endobj" + Convert.ToChar(13) + Convert.ToChar(10));
				}

				// PDF's trailer object
				trailer.xrefOffset = outputBufferLength;
				outputBufferLength += WriteToBuffer(outStream, trailer.GetText());

				// Buffer's flush
				outStream.Flush();
			}
			catch (IOException ex)
			{
				throw new PDFWritingErrorException("Error writing to PDF", ex);
			}
		}

		/// <summary>
		/// Method that writes the PDF document on a file
		/// </summary>
		/// <param name="outputFile">String that represents the name of the output file</param>
		public void CreatePDF(string outputFile)
		{
			FileStream outputStream = null;
			try
			{
				outputStream = new FileStream(outputFile, FileMode.Create);
				CreatePDF(outputStream);
			}
			catch (IOException exIO)
			{
				throw new PDFWritingErrorException("Error writing to file", exIO);
			}
			catch (PDFWritingErrorException exPDF)
			{
				throw new PDFWritingErrorException("Error writing to PDF", exPDF);
			}
			finally
			{
				if (outputStream != null)
				{
					outputStream.Close();
					outputStream = null;
				}
			}
		}

		/// <summary>
		/// Method that writes into the buffer a string
		/// </summary>
		/// <param name="myBuffer">Output Buffer</param>
		/// <param name="stringContent">String that contains the informations</param>
		/// <returns>The number of the bytes written in the Buffer</returns>
		private long WriteToBuffer(Stream myBuffer, string stringContent)
		{
			try
			{
				ASCIIEncoding asciiEncoder = new ASCIIEncoding();
				byte[] arrTemp = asciiEncoder.GetBytes(stringContent);
				myBuffer.Write(arrTemp, 0, arrTemp.Length);
				return arrTemp.Length;
			}
			catch (IOException ex)
			{
				throw new PDFBufferErrorException("Error writing to buffer", ex);
			}
		}

		/// <summary>
		/// Method that writes into the buffer a string
		/// </summary>
		/// <param name="myBuffer">Output Buffer</param>
		/// <param name="byteContent">A Byte array that contains the informations</param>
		/// <returns>The number of the bytes written in the Buffer</returns>
		private long WriteToBuffer(Stream myBuffer, byte[] byteContent)
		{
			try
			{
				myBuffer.Write(byteContent, 0, byteContent.Length);
				return byteContent.Length;
			}
			catch (IOException ex)
			{
				throw new PDFBufferErrorException("Error writing to buffer", ex);
			}
		}

		/// <summary>
		/// Private method for the initialization of all PDF objects
		/// </summary>
		private PDFTrailer InitializeObjectsForPDFCreation()
		{
			// Get image references
			_images.Clear();
			if (_persistentPage.elements.Count > 0)
			{
				foreach (PDFElement element in _persistentPage.elements)
				{
					if (element is ImageElement)
					{
						PDFImageReference reference = ((ImageElement)element).ObjectXReference;
						if (!reference.Constructed())
							throw new PDFImageNotConstructedException("PDFImageReference has not been constructed");
						
						PDFImageMaskReference maskReference = reference.imageMask;
						if (maskReference != null)
						{
							if (!maskReference.Constructed())
								throw new PDFImageNotConstructedException("PDFImageMaskReference has not been constructed");
							if (!_images.Contains(maskReference))
								_images.Add(maskReference);
						}
						
						if (!_images.Contains(reference))
							_images.Add(reference);
					}
				}
			}
			foreach (PDFPage page in _pages)
			{
				foreach (PDFElement element in page.elements)
				{
					if (element is ImageElement)
					{
						PDFImageReference reference = ((ImageElement)element).ObjectXReference;
						if (!reference.Constructed())
							throw new PDFImageNotConstructedException("PDFImageReference has not been constructed");
						
						PDFImageMaskReference maskReference = reference.imageMask;
						if (maskReference != null)
						{
							if (!maskReference.Constructed())
								throw new PDFImageNotConstructedException("PDFImageMaskReference has not been constructed");
							if (!_images.Contains(maskReference))
								_images.Add(maskReference);
						}

						if (!_images.Contains(reference))
							_images.Add(reference);
					}
				}
			}

			// Page's counters
			int pageIndex = 1;
			int pageNum = _pages.Count;
			int counterID = 0;

			// Header
			_header = new PDFHeader(_openBookmark);
			_header.objectIDHeader = 1;
			_header.objectIDInfo = 2;
			_header.objectIDOutlines = 3;

			// Info
			_info.objectIDInfo = 2;

			// Outlines
			_outlines.objectIDOutlines = 3;
			counterID = 4;

			// Bookmarks
			counterID = _outlines.initializeOutlines(counterID);

			// Fonts
			foreach (PDFAbstractFont font in _fonts.Values)
			{
				font.objectID = counterID;
				counterID++;
				if (font is PDFTrueTypeFont)
				{
					PDFTrueTypeFont ttfFont = (PDFTrueTypeFont)font;
					ttfFont.descriptorObjectID = counterID;
					counterID++;
					ttfFont.descendantObjectID = counterID;
					counterID++;
					ttfFont.toUnicodeObjectID = counterID;
					counterID++;
					ttfFont.streamObjectID = counterID;
					counterID++;
				}
			}

			// Pagetree
			_pageTree = new PDFPageTree();
			_pageTree.objectID = counterID;
			_header.pageTreeID = counterID;
			counterID++;

			// Pages
			foreach (PDFPage page in _pages)
			{
				page.objectID = counterID;
				page.pageTreeID = _pageTree.objectID;
				_pageTree.AddPage(counterID);
				counterID++;

				// Add page's Marker
				if (_pageMarker != null)
				{
					string markerText = _pageMarker.GetMarkerText(pageIndex, pageNum);
					int markerHeight = _pageMarker.fontType.fontDefinition.fontHeight * _pageMarker.fontSize;
					ParagraphElement element = new ParagraphElement(markerText, _pageMarker.coordX, _pageMarker.coordY, _pageMarker.fontType, _pageMarker.fontSize, markerHeight, page.width, _pageMarker.fontColor, _pageMarker.alignment);
					page.AddElement(element);
				}

				// Add persistent elements
				if (_persistentPage.elements.Count > 0)
					page.elements.InsertRange(0, _persistentPage.elements);

				// Add page elements
				foreach (PDFElement element in page.elements)
				{
					element.objectID = counterID;
					counterID++;
				}

				// Update page's index counter
				pageIndex += 1;
			}

			// Image references
			foreach (PDFImageReferenceBase image in _images)
			{
				image.objectID = counterID;
				counterID++;
			}

			// Trailer
			return new PDFTrailer(counterID - 1);
		}

		/// <summary>
		/// Method that creates a new page
		/// </summary>
		/// <returns>New PDF's page</returns>
		public PDFPage AddPage()
		{
			_pages.Add(new PDFPage(this));
			return _pages[_pages.Count - 1];
		}

		/// <summary>
		/// Method that creates a new page
		/// </summary>
		/// <param name="PredefinedSize">The standard page's size</param>
		/// <returns>New PDF's page</returns>
		public PDFPage AddPage(PredefinedPageSize PredefinedSize)
		{
			_pages.Add(new PDFPage(PredefinedSize, this));
			return _pages[_pages.Count - 1];
		}

		/// <summary>
		/// Method that creates a new page
		/// </summary>
		/// <returns>New PDF's page</returns>
		/// <param name="height">Height of the new page</param>
		/// <param name="width">Width of the new page</param>
		public PDFPage AddPage(int height, int width)
		{
			_pages.Add(new PDFPage(height, width, this));
			return _pages[_pages.Count - 1];
		}

		/// <summary>
		/// Method that adds a bookmark to the document
		/// </summary>
		/// <param name="Bookmark">Bookmark object</param>
		public void AddBookmark(PDFBookmarkNode Bookmark)
		{
			_outlines.AddBookmark(Bookmark);
		}

	}
}
