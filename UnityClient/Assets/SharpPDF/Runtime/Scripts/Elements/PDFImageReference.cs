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
	public sealed class PDFImageReference : PDFImageReferenceBase
	{
		// The method to use for encoding the data
		public enum EncodingMode
		{
			JPEG,
			ZippedRGB,
			ZippedAlpha,
		};

		private PDFImageMaskReference m_imageMask;
		private string m_encoding = "/DCTDecode";
		private string m_colourSpace = "/DeviceRGB";
		private Byte[] m_pixelData = null;
		private int m_deflateLevel = Deflater.DEFAULT_COMPRESSION;
		private bool m_finished = false;
		private JPEGEncoder.BitmapData m_bitmapData = null;

		public PDFImageMaskReference imageMask
		{
			get
			{
				return m_imageMask;
			}
			set
			{
				m_imageMask = value;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="texture">Unity 2D texture</param>
		internal PDFImageReference(Texture2D texture, bool blocking):
			this(texture, EncodingMode.JPEG, blocking)
		{
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="texture">Unity 2D texture</param>
		/// <param name="encodingMode">Method of image encoding</param>
		/// <param name="deflateLevel">Deflate compression level (1-9) used for Zip encoding methods</param>
		internal PDFImageReference(Texture2D texture, EncodingMode encodingMode, int deflateLevel, bool blocking)
		{
			// Store the image size
			m_width = texture.width;
			m_height = texture.height;

			// Handle different encoding modes
			switch (encodingMode)
			{
			// Lossy JPEG compression
			case EncodingMode.JPEG:
				{
					// Set encoding format
					m_encoding = "/DCTDecode";
					m_colourSpace = "/DeviceRGB";

					// Set the bitmap data
					m_bitmapData = new JPEGEncoder.BitmapData(texture);
				
					// Do encoding in the background
					Thread thread = new Thread(DoThreadedJPEGEncode);
					thread.Start();
					
					// If blocking, join the thread until it is done
					if (blocking)
						thread.Join();
				}
				break;
				
			// Non-lossy, zipped RGB data
			case EncodingMode.ZippedRGB:
				{
					// Set encoding format
					m_encoding = "/FlateDecode";
					m_colourSpace = "/DeviceRGB";
				
					// Get the pixel data, flipping the image so that the bottom of the image is at the start of the array
					Color32[] pixels = texture.GetPixels32();
					m_pixelData = new Byte[m_width * m_height * 3];
					int dataIndex = 0;
					for (int y = m_height - 1; y >= 0; --y)
					{
						for (int x = 0; x < m_width; ++x)
						{
							Color32 pixel = pixels[(y * m_width) + x];
							m_pixelData[dataIndex++] = pixel.r;
							m_pixelData[dataIndex++] = pixel.g;
							m_pixelData[dataIndex++] = pixel.b;
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
				break;
				
			// Non-lossy, zipped alpha data
			case EncodingMode.ZippedAlpha:
				{
					// Set encoding format
					m_encoding = "/FlateDecode";
					m_colourSpace = "/DeviceGray";
				
					// Get the pixel data, flipping the image so that the bottom of the image is at the start of the array
					Color32[] pixels = texture.GetPixels32();
					m_pixelData = new Byte[m_width * m_height];
					int dataIndex = 0;
					for (int y = m_height - 1; y >= 0; --y)
					{
						for (int x = 0; x < m_width; ++x)
						{
							m_pixelData[dataIndex++] = pixels[(y * m_width) + x].a;
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
				break;
			}
		}

		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="texture">Unity 2D texture</param>
		/// <param name="encodingMode">Method of image encoding</param>
		internal PDFImageReference(Texture2D texture, EncodingMode encodingMode, bool blocking): this(texture, encodingMode, Deflater.DEFAULT_COMPRESSION, blocking)
		{
		}
		
		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="pixels">Array of Color32 values of width*height in size. Alpha is ignored</param>
		/// <param name="width">Width of the image</param>
		/// <param name="height">Height of the image</param>
		/// <param name="deflateLevel">Deflate compression level (1-9)</param>
		internal PDFImageReference(Color32[] pixels, int width, int height, int deflateLevel, bool blocking)
		{
			// Store the image size
			m_width = width;
			m_height = height;

			// Set encoding format
			m_encoding = "/FlateDecode";
			m_colourSpace = "/DeviceRGB";
			
			// Get the pixel data, flipping the image so that the bottom of the image is at the start of the array
			m_pixelData = new Byte[m_width * m_height * 3];
			int dataIndex = 0;
			for (int y = m_height - 1; y >= 0; --y)
			{
				for (int x = 0; x < m_width; ++x)
				{
					Color32 pixel = pixels[(y * m_width) + x];
					m_pixelData[dataIndex++] = pixel.r;
					m_pixelData[dataIndex++] = pixel.g;
					m_pixelData[dataIndex++] = pixel.b;
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
		/// <param name="pixels">Array of Color32 values of width*height in size. Alpha is ignored</param>
		/// <param name="width">Width of the image</param>
		/// <param name="height">Height of the image</param>
		internal PDFImageReference(Color32[] pixels, int width, int height, bool blocking): this(pixels, width, height, Deflater.DEFAULT_COMPRESSION, blocking)
		{
		}
		
		/// <summary>
		/// Class's constructor
		/// </summary>
		/// <param name="greyscale">Array of intensity values of width*height in size</param>
		/// <param name="width">Width of the image</param>
		/// <param name="height">Height of the image</param>
		/// <param name="deflateLevel">Deflate compression level (1-9)</param>
		internal PDFImageReference(Byte[] greyscale, int width, int height, int deflateLevel, bool blocking)
		{
			// Store the image size
			m_width = width;
			m_height = height;

			// Set encoding format
			m_encoding = "/FlateDecode";
			m_colourSpace = "/DeviceGray";
			
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
		/// <param name="greyscale">Array of intensity values of width*height in size</param>
		/// <param name="width">Width of the image</param>
		/// <param name="height">Height of the image</param>
		internal PDFImageReference(Byte[] greyscale, int width, int height, bool blocking): this(greyscale, width, height, Deflater.DEFAULT_COMPRESSION, blocking)
		{
		}

		// Handle our initialization and encoding
		private void DoThreadedJPEGEncode()
		{
			// Compress to a JPEG stream
			JPEGEncoder encoder = new JPEGEncoder(m_bitmapData, 100.0f);
			m_imageByteData = encoder.GetBytes();
			m_pixelData = null;
			m_finished = true;
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
			m_finished = true;
		}
		
		#region PDFImageReferenceBase

		/// <summary>
		/// Queries whether construction of reference has finished
		/// </summary>
		/// <returns>Whether construction has finished</returns>
		public override bool Constructed()
		{
			return m_finished;
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
			resultImage.Append("/Filter " + m_encoding + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/Width " + m_width.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/Height " + m_height.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/BitsPerComponent 8" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/ColorSpace " + m_colourSpace + Convert.ToChar(13) + Convert.ToChar(10));
			if (m_imageMask != null)
				resultImage.Append("/SMask " + m_imageMask.objectID.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append("/Length " + m_imageByteData.Length.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			resultImage.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));

			return resultImage.ToString();  
		}

		#endregion
	}
}
