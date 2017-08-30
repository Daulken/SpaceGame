using System;
using System.IO;
using System.Text;
using SharpPDF.Exceptions;
using SharpPDF.Enumerators;

namespace SharpPDF.Elements {
	
	/// <summary>
	/// A Class that implements a PDF image.
	/// </summary>
	public sealed class ImageElement : PDFElement
	{
		private PDFImageReference m_imageReference;
		
		/// <summary>
		/// Image's Reference
		/// </summary>
		public PDFImageReference ObjectXReference
		{
			get
			{
				return m_imageReference;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="imageReference">Image's Reference</param>
		/// <param name="newCoordX">X position in the PDF document</param>
		/// <param name="newCoordY">Y position in the PDF document</param>
		public ImageElement(PDFImageReference imageReference, int newCoordX, int newCoordY)
		{	
			m_imageReference = imageReference;
			_width = m_imageReference.width;	
			_height = m_imageReference.height;
			_coordX = newCoordX;
			_coordY = newCoordY;			
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="imageReference">Image's Reference</param>
		/// <param name="newCoordX">X position in the PDF document</param>
		/// <param name="newCoordY">Y position in the PDF document</param>
		/// <param name="newWidth">New width of the image</param>
		/// <param name="newHeight">New height of the image</param>
		public ImageElement(PDFImageReference imageReference, int newCoordX, int newCoordY, int newWidth, int newHeight)
		{
			m_imageReference = imageReference;
			_width = newWidth;				
			_height = newHeight;
			_coordX = newCoordX;
			_coordY = newCoordY;		
		}

		/// <summary>
		/// Method that returns the PDF codes to write the image in the document
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public override string GetText()
		{
			StringBuilder imageContent = new StringBuilder();            
			imageContent.Append("q" + Convert.ToChar(13) + Convert.ToChar(10));
			imageContent.Append(_width.ToString() + " 0 0 " + _height.ToString() + " " + _coordX.ToString() + " " + _coordY.ToString() + " cm" + Convert.ToChar(13) + Convert.ToChar(10));
			imageContent.Append("/I" + m_imageReference.objectID.ToString() + " Do" + Convert.ToChar(13) + Convert.ToChar(10));
			imageContent.Append("Q" + Convert.ToChar(13) + Convert.ToChar(10));			
			
			StringBuilder resultImage = new StringBuilder();
			resultImage.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/Length " + imageContent.Length.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("stream" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append(imageContent.ToString());
			resultImage.Append("endstream" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));

			return resultImage.ToString();
		}

		/// <summary>
		/// Method that clones the element
		/// </summary>
		/// <returns>Cloned object</returns>
		public override object Clone()
		{
			return new ImageElement(m_imageReference, this._coordX, this._coordY, this._width, this._height);
		}

	}
}
