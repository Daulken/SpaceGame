using System;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using SharpPDF.Exceptions;

namespace SharpPDF.Elements {

	/// <summary>
	/// Class that represents an image reference inside the document.
	/// </summary>
	public sealed class PDFImageMaskReference : PDFImageReferenceBase
	{
		private Byte[] m_pixelData = null;
		private int m_deflateLevel = Deflater.DEFAULT_COMPRESSION;
		
		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="texture">Unity 2D texture</param>
		/// <param name="deflateLevel">Deflate compression level (1-9)</param>
		internal PDFImageMaskReference(Texture2D texture, int deflateLevel, bool blocking)
		{
			// Store the image size
			m_width = texture.width;
			m_height = texture.height;

			// Get the pixel data, flipping the image so that the bottom of the image is at the start of the array
			Color32[] pixels = texture.GetPixels32();
			m_pixelData = new Byte[m_width * m_height];
			int dataIndex = 0;
			for (int y = m_height - 1; y >= 0; --y)
			{
				for (int x = 0; x < m_width; ++x)
				{
					Color32 pixel = pixels[(y * m_width) + x];
					m_pixelData[dataIndex++] = pixel.a;
				}
			}

			// Set the deflate level
			m_deflateLevel = deflateLevel;

			// Do encoding in the background
			Thread thread = new Thread(DoThreadedZipEncode);
			thread.Start();
			
			// If blocking, join the thread until it is done
			if (blocking)
				thread.Join();
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="texture">Unity 2D texture</param>
		internal PDFImageMaskReference(Texture2D texture, bool blocking): this(texture, Deflater.DEFAULT_COMPRESSION, blocking)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="greyscale">Array of alpha values of width*height in size</param>
		/// <param name="width">Width of the image</param>
		/// <param name="height">Height of the image</param>
		/// <param name="deflateLevel">Deflate compression level (1-9)</param>
		internal PDFImageMaskReference(Byte[] greyscale, int width, int height, int deflateLevel, bool blocking)
		{
			// Store the image size
			m_width = width;
			m_height = height;

			// Get the greyscale data, flipping the image so that the bottom of the image is at the start of the array
			m_pixelData = new Byte[m_width * m_height * 3];
			int dataIndex = 0;
			for (int y = m_height - 1; y >= 0; --y)
			{
				for (int x = 0; x < m_width; ++x)
				{
					m_pixelData[dataIndex++] = greyscale[(y * m_width) + x];
				}
			}

			// Set the deflate level
			m_deflateLevel = deflateLevel;

			// Do encoding in the background
			Thread thread = new Thread(DoThreadedZipEncode);
			thread.Start();
			
			// If blocking, join the thread until it is done
			if (blocking)
				thread.Join();
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="greyscale">Array of alpha values of width*height in size</param>
		/// <param name="width">Width of the image</param>
		/// <param name="height">Height of the image</param>
		internal PDFImageMaskReference(Byte[] greyscale, int width, int height, bool blocking): this(greyscale, width, height, Deflater.DEFAULT_COMPRESSION, blocking)
		{
		}

		// Handle our initialization and encoding
		private void DoThreadedZipEncode()
		{
			// Compress to a ZLib stream
			MemoryStream stream = new MemoryStream();
			Deflater deflater = new Deflater(m_deflateLevel);
			DeflaterOutputStream deflaterStream = new DeflaterOutputStream(stream, deflater);
			deflaterStream.Write(m_pixelData, 0, m_pixelData.Length);
			deflaterStream.Flush();
			deflaterStream.Close();
			m_imageByteData = stream.GetBuffer();
			m_pixelData = null;
		}
		
		#region PDFImageReferenceBase

		/// <summary>
		/// Queries whether construction of reference has finished
		/// </summary>
		/// <returns>Whether construction has finished</returns>
		public override bool Constructed()
		{
			// Pixel data is only null'd after deflate finishes
			return (m_pixelData == null);
		}
		
		/// <summary>
		/// Method that returns the PDF codes to write the image reference in the document.
		/// </summary>
		/// <returns>String that contains PDF codes</returns>
		public override string GetText()
		{
			StringBuilder resultImage = new StringBuilder();
			resultImage.Append(m_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/Type /XObject" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/Subtype /Image" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/Name /I" + m_objectID.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/Filter /FlateDecode" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/Width " + m_width.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/Height " + m_height.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/BitsPerComponent 8" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/ColorSpace /DeviceGray" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/Matte [1.0 1.0 1.0]" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/Length " + m_imageByteData.Length.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));

			return resultImage.ToString();  
		}

		#endregion
	}
}
