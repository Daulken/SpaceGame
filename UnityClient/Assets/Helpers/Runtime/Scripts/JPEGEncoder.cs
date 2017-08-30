using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Threading;
     
// ---
// Translation to C# for Unity by GammaPsh
//
// prior Version is from Matthew (Unity Forum), found here:
// http://blur.st/jpeg
//
// Original copyright notice is below:
// ---
//
// Ported to UnityScript by Matthew Wegner, Flashbang Studios
//
// Original code is from as3corelib, found here:
// http://code.google.com/p/as3corelib/source/browse/trunk/src/com/adobe/images/JPGEncoder.as
//
// Original copyright notice is below:
//       
//	  Copyright (c) 2008, Adobe Systems Incorporated
//	  All rights reserved.
// 
//	  Redistribution and use in source and binary forms, with or without
//	  modification, are permitted provided that the following conditions are
//	  met:
// 
//	  * Redistributions of source code must retain the above copyright notice,
//	    this list of conditions and the following disclaimer.
// 
//	  * Redistributions in binary form must reproduce the above copyright
//	    notice, this list of conditions and the following disclaimer in the
//	    documentation and/or other materials provided with the distribution.
// 
//	  * Neither the name of Adobe Systems Incorporated nor the names of its
//	    contributors may be used to endorse or promote products derived from
//	    this software without specific prior written permission.
// 
//	  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
//	  IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
//	  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
//	  PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//	  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//	  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//	  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//	  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//	  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//	  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//	  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

// Class that converts BitmapData into a valid JPEG
public class JPEGEncoder
{
	// An expandable stream of bytes
	class ByteArray
	{
		private MemoryStream m_stream;
		private BinaryWriter m_writer;
       
		public ByteArray()
		{
			m_stream = new MemoryStream();
			m_writer = new BinaryWriter(m_stream);
		}
       
		// Function from AS3 -- add a byte to our stream
		public void writeByte(Byte value)
		{
			m_writer.Write(value);
		}

		// Add a word to our stream
		public void writeWord(int value)
		{
			writeByte((Byte)((value >> 8) & 0xFF));
			writeByte((Byte)((value) & 0xFF));
		}
		
		// Spit back all bytes
		public Byte[] GetAllBytes()
		{
			Byte[] buffer = new Byte[m_stream.Length];
			m_stream.Position = 0;
			m_stream.Read(buffer, 0, buffer.Length);
			return buffer;
		}
	}
     
	// Holds a length/value pair for Huffman encoding
	struct HuffmanTableEntry
	{
		private int m_length;
		public int length
		{
			get
			{
				return m_length;
			}
			set
			{
				m_length = value;
			}
		}

		private int m_value;
		public int value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}
	}
     
	// Another flash class--emulating the stuff the encoder uses
	public class BitmapData
	{
		private Color32[] m_pixels;
		private int m_width;
		private int m_height;

		// Pull all of our pixels off the texture (Unity stuff isn't thread safe, and this is faster)
		public BitmapData(Texture2D texture)
		{
			m_height = texture.height;
			m_width = texture.width;
			m_pixels = texture.GetPixels32();
		}

		// Get the width of the texture
		public int width
		{
			get
			{
				return m_width;
			}
		}

		// Get the height of the texture
		public int height
		{
			get
			{
				return m_height;
			}
		}

		// Get the colour of a given pixel
		public Color32 GetColor(int x, int y)
		{
			x = Mathf.Clamp(x, 0, m_width - 1);
			y = Mathf.Clamp(y, 0, m_height - 1);
			return m_pixels[y * width + x];
		}
	}
     

	// Static table initialization
	private static int[] ms_zigZag = {
             0, 1, 5, 6,14,15,27,28,
             2, 4, 7,13,16,26,29,42,
             3, 8,12,17,25,30,41,43,
             9,11,18,24,31,40,44,53,
            10,19,23,32,39,45,52,54,
            20,22,33,38,46,51,55,60,
            21,34,37,47,50,56,59,61,
            35,36,48,49,57,58,62,63
        };

	// JPEG tables generated from input data
	private int[] m_yTable = new int[64];
	private int[] m_uvTable = new int[64];
	private float[] m_fdYTable = new float[64];
	private float[] m_fdUVTable = new float[64];

	// JPEG huffman encoding tables
	private HuffmanTableEntry[] m_huffmanTable_YDC;
	private HuffmanTableEntry[] m_huffmanTable_UVDC;
	private HuffmanTableEntry[] m_huffmanTable_YAC;
	private HuffmanTableEntry[] m_huffmanTable_UVAC;
	private int[] m_standardDCLuminanceNRCodes = {0,0,1,5,1,1,1,1,1,1,0,0,0,0,0,0,0};
	private int[] m_standardDCLuminanceValues = {0,1,2,3,4,5,6,7,8,9,10,11};
	private int[] m_standardACLuminanceNRCodes = {0,0,2,1,3,3,2,4,3,5,5,4,4,0,0,1,0x7d};
	private int[] m_standardACLuminanceValues = {
        0x01,0x02,0x03,0x00,0x04,0x11,0x05,0x12,
        0x21,0x31,0x41,0x06,0x13,0x51,0x61,0x07,
        0x22,0x71,0x14,0x32,0x81,0x91,0xa1,0x08,
        0x23,0x42,0xb1,0xc1,0x15,0x52,0xd1,0xf0,
        0x24,0x33,0x62,0x72,0x82,0x09,0x0a,0x16,
        0x17,0x18,0x19,0x1a,0x25,0x26,0x27,0x28,
        0x29,0x2a,0x34,0x35,0x36,0x37,0x38,0x39,
        0x3a,0x43,0x44,0x45,0x46,0x47,0x48,0x49,
        0x4a,0x53,0x54,0x55,0x56,0x57,0x58,0x59,
        0x5a,0x63,0x64,0x65,0x66,0x67,0x68,0x69,
        0x6a,0x73,0x74,0x75,0x76,0x77,0x78,0x79,
        0x7a,0x83,0x84,0x85,0x86,0x87,0x88,0x89,
        0x8a,0x92,0x93,0x94,0x95,0x96,0x97,0x98,
        0x99,0x9a,0xa2,0xa3,0xa4,0xa5,0xa6,0xa7,
        0xa8,0xa9,0xaa,0xb2,0xb3,0xb4,0xb5,0xb6,
        0xb7,0xb8,0xb9,0xba,0xc2,0xc3,0xc4,0xc5,
        0xc6,0xc7,0xc8,0xc9,0xca,0xd2,0xd3,0xd4,
        0xd5,0xd6,0xd7,0xd8,0xd9,0xda,0xe1,0xe2,
        0xe3,0xe4,0xe5,0xe6,0xe7,0xe8,0xe9,0xea,
        0xf1,0xf2,0xf3,0xf4,0xf5,0xf6,0xf7,0xf8,
        0xf9,0xfa
    };
	private int[] m_standardDCChrominanceNRCodes = {0,0,3,1,1,1,1,1,1,1,1,1,0,0,0,0,0};
	private int[] m_standardDCChrominanceValues = {0,1,2,3,4,5,6,7,8,9,10,11};
	private int[] m_standardACChrominanceNRCodes = {0,0,2,1,2,4,4,3,4,7,5,4,4,0,1,2,0x77};
	private int[] m_standardACChrominanceValues = {
        0x00,0x01,0x02,0x03,0x11,0x04,0x05,0x21,
        0x31,0x06,0x12,0x41,0x51,0x07,0x61,0x71,
        0x13,0x22,0x32,0x81,0x08,0x14,0x42,0x91,
        0xa1,0xb1,0xc1,0x09,0x23,0x33,0x52,0xf0,
        0x15,0x62,0x72,0xd1,0x0a,0x16,0x24,0x34,
        0xe1,0x25,0xf1,0x17,0x18,0x19,0x1a,0x26,
        0x27,0x28,0x29,0x2a,0x35,0x36,0x37,0x38,
        0x39,0x3a,0x43,0x44,0x45,0x46,0x47,0x48,
        0x49,0x4a,0x53,0x54,0x55,0x56,0x57,0x58,
        0x59,0x5a,0x63,0x64,0x65,0x66,0x67,0x68,
        0x69,0x6a,0x73,0x74,0x75,0x76,0x77,0x78,
        0x79,0x7a,0x82,0x83,0x84,0x85,0x86,0x87,
        0x88,0x89,0x8a,0x92,0x93,0x94,0x95,0x96,
        0x97,0x98,0x99,0x9a,0xa2,0xa3,0xa4,0xa5,
        0xa6,0xa7,0xa8,0xa9,0xaa,0xb2,0xb3,0xb4,
        0xb5,0xb6,0xb7,0xb8,0xb9,0xba,0xc2,0xc3,
        0xc4,0xc5,0xc6,0xc7,0xc8,0xc9,0xca,0xd2,
        0xd3,0xd4,0xd5,0xd6,0xd7,0xd8,0xd9,0xda,
        0xe2,0xe3,0xe4,0xe5,0xe6,0xe7,0xe8,0xe9,
        0xea,0xf2,0xf3,0xf4,0xf5,0xf6,0xf7,0xf8,
        0xf9,0xfa
    };
	private HuffmanTableEntry[] m_categoryFloatBitCode = new HuffmanTableEntry[65535];
	private int[] m_category = new int[65535];

	// Current working state of encoder
	private int m_currentByteState = 0;
	private int m_currentBitIndex = 7;
	private ByteArray m_encodedBytes = new ByteArray();

	// Core processing
	private int[] DU = new int[64];
	private float[] YDU = new float[64];
	private float[] UDU = new float[64];
	private float[] VDU = new float[64];

	// Encoding data
	private BitmapData m_bitmapData;
	private int m_qualitySF = 0;
	
	private void InitializeQuantisationTables(int sf)
	{
		int[] YQT = {
            16, 11, 10, 16, 24, 40, 51, 61,
            12, 12, 14, 19, 26, 58, 60, 55,
            14, 13, 16, 24, 40, 57, 69, 56,
            14, 17, 22, 29, 51, 87, 80, 62,
            18, 22, 37, 56, 68,109,103, 77,
            24, 35, 55, 64, 81,104,113, 92,
            49, 64, 78, 87,103,121,120,101,
            72, 92, 95, 98,112,100,103, 99
        };

		for (int i = 0; i < 64; i++)
		{
			float t = Mathf.Clamp(Mathf.Floor((YQT[i] * sf + 50) / 100), 1, 255);
			m_yTable[ms_zigZag[i]] = Mathf.FloorToInt(t);
		}

		int[] UVQT = {
            17, 18, 24, 47, 99, 99, 99, 99,
            18, 21, 26, 66, 99, 99, 99, 99,
            24, 26, 56, 99, 99, 99, 99, 99,
            47, 66, 99, 99, 99, 99, 99, 99,
            99, 99, 99, 99, 99, 99, 99, 99,
            99, 99, 99, 99, 99, 99, 99, 99,
            99, 99, 99, 99, 99, 99, 99, 99,
            99, 99, 99, 99, 99, 99, 99, 99
        };

		for (int i = 0; i < 64; i++)
		{
			float t = Mathf.Clamp(Mathf.Floor((UVQT[i] * sf + 50) / 100), 1, 255);
			m_uvTable[ms_zigZag[i]] = Mathf.FloorToInt(t);
		}

		float[] aasf = {
            1.0f, 1.387039845f, 1.306562965f, 1.175875602f,
            1.0f, 0.785694958f, 0.541196100f, 0.275899379f
        };

		int tableIndex = 0;
		for (int row = 0; row < 8; row++)
		{
			for (int col = 0; col < 8; col++)
			{
				m_fdYTable[tableIndex] = (1.0f / (m_yTable[ms_zigZag[tableIndex]] * aasf[row] * aasf[col] * 8.0f));
				m_fdUVTable[tableIndex] = (1.0f / (m_uvTable[ms_zigZag[tableIndex]] * aasf[row] * aasf[col] * 8.0f));
				tableIndex++;
			}
		}
	}

	private HuffmanTableEntry[] ComputeHuffmanTable(int[] nrcodes, int[] std_table)
	{
		int codevalue = 0;
		int pos_in_table = 0;
		HuffmanTableEntry[] HT = new HuffmanTableEntry[16 * 16];
		for (int k = 1; k<=16; k++)
		{
			for (int j=1; j<=nrcodes[k]; j++)
			{
				HT[std_table[pos_in_table]] = new HuffmanTableEntry();
				HT[std_table[pos_in_table]].value = codevalue;
				HT[std_table[pos_in_table]].length = k;
				pos_in_table++;
				codevalue++;
			}
			codevalue *= 2;
		}
		return HT;
	}
 
	private void InitializeHuffmanTables()
	{
		m_huffmanTable_YDC = ComputeHuffmanTable(m_standardDCLuminanceNRCodes, m_standardDCLuminanceValues);
		m_huffmanTable_UVDC = ComputeHuffmanTable(m_standardDCChrominanceNRCodes, m_standardDCChrominanceValues);
		m_huffmanTable_YAC = ComputeHuffmanTable(m_standardACLuminanceNRCodes, m_standardACLuminanceValues);
		m_huffmanTable_UVAC = ComputeHuffmanTable(m_standardACChrominanceNRCodes, m_standardACChrominanceValues);
	}
 
	private void InitializeCategoryBitCodes()
	{
		int nrlower = 1;
		int nrupper = 2;
		int nr;
		HuffmanTableEntry bs;
		for (int cat = 1; cat <= 15; ++cat)
		{
			// Positive numbers
			for (nr = nrlower; nr < nrupper; ++nr)
			{
				m_category[32767 + nr] = cat;
 
				bs = new HuffmanTableEntry();
				bs.length = cat;
				bs.value = nr;
				m_categoryFloatBitCode[32767 + nr] = bs;
			}
			// Negative numbers
			for (nr = -(nrupper-1); nr <= -nrlower; ++nr)
			{
				m_category[32767 + nr] = cat;
 
				bs = new HuffmanTableEntry();
				bs.length = cat;
				bs.value = nrupper - 1 + nr;
				m_categoryFloatBitCode[32767 + nr] = bs;
			}

			nrlower <<= 1;
			nrupper <<= 1;
		}
	}
 
	// IO functions
   
	private void writeBits(HuffmanTableEntry bs)
	{
		int value = bs.value;
		int posval = bs.length - 1;
		while (posval >= 0)
		{
			if ((value & System.Convert.ToUInt32(1 << posval)) != 0)
			{
				m_currentByteState |= (int)System.Convert.ToUInt32(1 << m_currentBitIndex);
			}
			posval--;
			m_currentBitIndex--;
			if (m_currentBitIndex < 0)
			{
				if (m_currentByteState == 0xFF)
				{
					m_encodedBytes.writeByte(0xFF);
					m_encodedBytes.writeByte(0);
				}
				else
				{
					m_encodedBytes.writeByte((Byte)m_currentByteState);
				}
				m_currentBitIndex = 7;
				m_currentByteState = 0;
			}
		}
	}
 
	// DCT & quantization core
 
	private float[] fDCTQuant(float[] data, float[] fdtbl)
	{
		float tmp0;
		float tmp1;
		float tmp2;
		float tmp3;
		float tmp4;
		float tmp5;
		float tmp6;
		float tmp7;
		float tmp10;
		float tmp11;
		float tmp12;
		float tmp13;
		float z1;
		float z2;
		float z3;
		float z4;
		float z5;
		float z11;
		float z13;
		int i;
		/* Pass 1: process rows. */
		int dataOff = 0;
		for (i=0; i<8; i++)
		{
			tmp0 = data[dataOff + 0] + data[dataOff + 7];
			tmp7 = data[dataOff + 0] - data[dataOff + 7];
			tmp1 = data[dataOff + 1] + data[dataOff + 6];
			tmp6 = data[dataOff + 1] - data[dataOff + 6];
			tmp2 = data[dataOff + 2] + data[dataOff + 5];
			tmp5 = data[dataOff + 2] - data[dataOff + 5];
			tmp3 = data[dataOff + 3] + data[dataOff + 4];
			tmp4 = data[dataOff + 3] - data[dataOff + 4];
 
			/* Even part */
			tmp10 = tmp0 + tmp3;    /* phase 2 */
			tmp13 = tmp0 - tmp3;
			tmp11 = tmp1 + tmp2;
			tmp12 = tmp1 - tmp2;
 
			data[dataOff + 0] = tmp10 + tmp11; /* phase 3 */
			data[dataOff + 4] = tmp10 - tmp11;
 
			z1 = (tmp12 + tmp13) * 0.707106781f; /* c4 */
			data[dataOff + 2] = tmp13 + z1; /* phase 5 */
			data[dataOff + 6] = tmp13 - z1;
 
			/* Odd part */
			tmp10 = tmp4 + tmp5; /* phase 2 */
			tmp11 = tmp5 + tmp6;
			tmp12 = tmp6 + tmp7;
 
			/* The rotator is modified from fig 4-8 to avoid extra negations. */
			z5 = (tmp10 - tmp12) * 0.382683433f; /* c6 */
			z2 = 0.541196100f * tmp10 + z5; /* c2-c6 */
			z4 = 1.306562965f * tmp12 + z5; /* c2+c6 */
			z3 = tmp11 * 0.707106781f; /* c4 */
 
			z11 = tmp7 + z3;    /* phase 5 */
			z13 = tmp7 - z3;
 
			data[dataOff + 5] = z13 + z2; /* phase 6 */
			data[dataOff + 3] = z13 - z2;
			data[dataOff + 1] = z11 + z4;
			data[dataOff + 7] = z11 - z4;
 
			dataOff += 8; /* advance pointer to next row */
		}
 
		/* Pass 2: process columns. */
		dataOff = 0;
		for (i=0; i<8; i++)
		{
			tmp0 = data[dataOff + 0] + data[dataOff + 56];
			tmp7 = data[dataOff + 0] - data[dataOff + 56];
			tmp1 = data[dataOff + 8] + data[dataOff + 48];
			tmp6 = data[dataOff + 8] - data[dataOff + 48];
			tmp2 = data[dataOff + 16] + data[dataOff + 40];
			tmp5 = data[dataOff + 16] - data[dataOff + 40];
			tmp3 = data[dataOff + 24] + data[dataOff + 32];
			tmp4 = data[dataOff + 24] - data[dataOff + 32];
 
			/* Even part */
			tmp10 = tmp0 + tmp3;    /* phase 2 */
			tmp13 = tmp0 - tmp3;
			tmp11 = tmp1 + tmp2;
			tmp12 = tmp1 - tmp2;
 
			data[dataOff + 0] = tmp10 + tmp11; /* phase 3 */
			data[dataOff + 32] = tmp10 - tmp11;
 
			z1 = (tmp12 + tmp13) * 0.707106781f; /* c4 */
			data[dataOff + 16] = tmp13 + z1; /* phase 5 */
			data[dataOff + 48] = tmp13 - z1;
 
			/* Odd part */
			tmp10 = tmp4 + tmp5; /* phase 2 */
			tmp11 = tmp5 + tmp6;
			tmp12 = tmp6 + tmp7;
 
			/* The rotator is modified from fig 4-8 to avoid extra negations. */
			z5 = (tmp10 - tmp12) * 0.382683433f; /* c6 */
			z2 = 0.541196100f * tmp10 + z5; /* c2-c6 */
			z4 = 1.306562965f * tmp12 + z5; /* c2+c6 */
			z3 = tmp11 * 0.707106781f; /* c4 */
 
			z11 = tmp7 + z3;    /* phase 5 */
			z13 = tmp7 - z3;
 
			data[dataOff + 40] = z13 + z2; /* phase 6 */
			data[dataOff + 24] = z13 - z2;
			data[dataOff + 8] = z11 + z4;
			data[dataOff + 56] = z11 - z4;
 
			dataOff++; /* advance pointer to next column */
		}
 
		// Quantize/descale the coefficients
		for (i=0; i<64; i++)
		{
			// Apply the quantization and scaling factor & Round to nearest integer
			data[i] = Mathf.Round((data[i] * fdtbl[i]));
		}
		return data;
	}
 
	// Chunk writing
 
	private void writeAPP0()
	{
		m_encodedBytes.writeWord(0xFFE0); // marker
		m_encodedBytes.writeWord(16); // length
		m_encodedBytes.writeByte(0x4A); // J
		m_encodedBytes.writeByte(0x46); // F
		m_encodedBytes.writeByte(0x49); // I
		m_encodedBytes.writeByte(0x46); // F
		m_encodedBytes.writeByte(0); // = "JFIF",'\0'
		m_encodedBytes.writeByte(1); // versionhi
		m_encodedBytes.writeByte(1); // versionlo
		m_encodedBytes.writeByte(0); // xyunits
		m_encodedBytes.writeWord(1); // xdensity
		m_encodedBytes.writeWord(1); // ydensity
		m_encodedBytes.writeByte(0); // thumbnwidth
		m_encodedBytes.writeByte(0); // thumbnheight
	}
 
	private void writeSOF0(int width, int height)
	{
		m_encodedBytes.writeWord(0xFFC0); // marker
		m_encodedBytes.writeWord(17);   // length, truecolor YUV JPG
		m_encodedBytes.writeByte(8);    // precision
		m_encodedBytes.writeWord(height);
		m_encodedBytes.writeWord(width);
		m_encodedBytes.writeByte(3);    // nrofcomponents
		m_encodedBytes.writeByte(1);    // IdY
		m_encodedBytes.writeByte(0x11); // HVY
		m_encodedBytes.writeByte(0);    // QTY
		m_encodedBytes.writeByte(2);    // IdU
		m_encodedBytes.writeByte(0x11); // HVU
		m_encodedBytes.writeByte(1);    // QTU
		m_encodedBytes.writeByte(3);    // IdV
		m_encodedBytes.writeByte(0x11); // HVV
		m_encodedBytes.writeByte(1);    // QTV
	}
 
	private void writeDQT()
	{
		m_encodedBytes.writeWord(0xFFDB); // marker
		m_encodedBytes.writeWord(132);    // length
		m_encodedBytes.writeByte(0);
		for (int i = 0; i < 64; i++)
			m_encodedBytes.writeByte((Byte)m_yTable[i]);
		m_encodedBytes.writeByte(1);
		for (int i = 0; i < 64; i++)
			m_encodedBytes.writeByte((Byte)m_uvTable[i]);
	}
 
	private void writeDHT()
	{
		m_encodedBytes.writeWord(0xFFC4); // marker
		m_encodedBytes.writeWord(0x01A2); // length

		m_encodedBytes.writeByte(0); // HTYDCinfo
		for (int i = 0; i < 16; i++)
			m_encodedBytes.writeByte((Byte)m_standardDCLuminanceNRCodes[i + 1]);
		for (int i = 0; i <= 11; i++)
			m_encodedBytes.writeByte((Byte)m_standardDCLuminanceValues[i]);
 
		m_encodedBytes.writeByte(0x10); // HTYACinfo
		for (int i = 0; i < 16; i++)
			m_encodedBytes.writeByte((Byte)m_standardACLuminanceNRCodes[i + 1]);
		for (int i = 0; i <= 161; i++)
			m_encodedBytes.writeByte((Byte)m_standardACLuminanceValues[i]);
 
		m_encodedBytes.writeByte(1); // HTUDCinfo
		for (int i = 0; i < 16; i++)
			m_encodedBytes.writeByte((Byte)m_standardDCChrominanceNRCodes[i + 1]);
		for (int i = 0; i <= 11; i++)
			m_encodedBytes.writeByte((Byte)m_standardDCChrominanceValues[i]);
 
		m_encodedBytes.writeByte(0x11); // HTUACinfo
		for (int i = 0; i < 16; i++)
			m_encodedBytes.writeByte((Byte)m_standardACChrominanceNRCodes[i + 1]);
		for (int i = 0; i <= 161; i++)
			m_encodedBytes.writeByte((Byte)m_standardACChrominanceValues[i]);
	}
 
	private void writeSOS()
	{
		m_encodedBytes.writeWord(0xFFDA); // marker
		m_encodedBytes.writeWord(12); // length
		m_encodedBytes.writeByte(3); // nrofcomponents
		m_encodedBytes.writeByte(1); // IdY
		m_encodedBytes.writeByte(0); // HTY
		m_encodedBytes.writeByte(2); // IdU
		m_encodedBytes.writeByte(0x11); // HTU
		m_encodedBytes.writeByte(3); // IdV
		m_encodedBytes.writeByte(0x11); // HTV
		m_encodedBytes.writeByte(0); // Ss
		m_encodedBytes.writeByte(0x3f); // Se
		m_encodedBytes.writeByte(0); // Bf
	}
 
	// Core processing
 
	private float processDU(float[] CDU, float[] fdtbl, float DC, HuffmanTableEntry[] HTDC, HuffmanTableEntry[] HTAC)
	{
		HuffmanTableEntry EOB = HTAC[0x00];
		HuffmanTableEntry M16zeroes = HTAC[0xF0];
		int i;
 
		float[] DU_DCT = fDCTQuant(CDU, fdtbl);
		//ZigZag reorder
		for (i=0; i<64; i++)
		{
			DU[ms_zigZag[i]] = Mathf.RoundToInt(DU_DCT[i]);
		}
		int Diff = Mathf.RoundToInt(DU[0] - DC);
		DC = DU[0];
		//Encode DC
		if (Diff == 0)
		{
			writeBits(HTDC[0]); // Diff might be 0
		}
		else
		{
			writeBits(HTDC[m_category[32767 + Diff]]);
			writeBits(m_categoryFloatBitCode[32767 + Diff]);
		}
		//Encode ACs
		int end0pos = 63;
		for (; (end0pos>0)&&(DU[end0pos]==0); end0pos--)
		{
		}
		;
		//end0pos = first element in reverse order !=0
		if (end0pos == 0)
		{
			writeBits(EOB);
			return DC;
		}
		i = 1;
		while (i <= end0pos)
		{
			int startpos = i;
			for (; (DU[i]==0) && (i<=end0pos); i++)
			{
			}
			int nrzeroes = i - startpos;
			if (nrzeroes >= 16)
			{
				for (int nrmarker=1; nrmarker <= nrzeroes/16; nrmarker++)
				{
					writeBits(M16zeroes);
				}
				nrzeroes = (nrzeroes & 0xF);
			}
			writeBits(HTAC[nrzeroes * 16 + m_category[32767 + DU[i]]]);
			writeBits(m_categoryFloatBitCode[32767 + DU[i]]);
			i++;
		}
		if (end0pos != 63)
		{
			writeBits(EOB);
		}
		return DC;
	}
 
	private void ConvertRGBToYUV(BitmapData img, int xpos, int ypos)
	{      
		int pos = 0;
		for (int y = 0; y < 8; y++)
		{
			for (int x = 0; x < 8; x++)
			{
				Color32 pixel = img.GetColor(xpos + x, img.height - (ypos + y));
				YDU[pos] = ((0.29900f * pixel.r) + (0.58700f * pixel.g) + (0.11400f * pixel.b)) - 128.0f;
				UDU[pos] = (-0.16874f * pixel.r) + (-0.33126f * pixel.g) + (0.50000f * pixel.b);
				VDU[pos] = (0.50000f * pixel.r) + (-0.41869f * pixel.g) + (-0.08131f * pixel.b);
				pos++;
			}
		}
	}

	
	// ---------------------------------------------------------------------

	// Constructor for JPEGEncoder class
    // The quality level is between 1 and 100 that determines the level of compression used in the generated JPEG
	public JPEGEncoder(BitmapData bitmapData, float quality)
	{
		// Save out texture data to our own data structure
		m_bitmapData = bitmapData;

		// Clamp the quality to valid range
		quality = Mathf.Clamp(quality, 1.0f, 100.0f);
		
		// Calculate quality table to use
		m_qualitySF = (quality < 50.0f) ? Mathf.RoundToInt(5000.0f / quality) : Mathf.RoundToInt(200.0f - quality * 2.0f);

		// Create tables -- technically we could only do this once for multiple encodes
		InitializeHuffmanTables();
		InitializeCategoryBitCodes();
		InitializeQuantisationTables(m_qualitySF);
       
		// Do actual encoding

		// Initialize bit writer
		m_encodedBytes = new ByteArray();
		m_currentByteState = 0;
		m_currentBitIndex = 7;
 
		// Add JPEG headers
		m_encodedBytes.writeWord(0xFFD8); // SOI
		writeAPP0();
		writeDQT();
		writeSOF0(m_bitmapData.width, m_bitmapData.height);
		writeDHT();
		writeSOS();
 
		// Encode 8x8 macroblocks
		float DCY = 0;
		float DCU = 0;
		float DCV = 0;
		m_currentByteState = 0;
		m_currentBitIndex = 7;
		for (int ypos = 0; ypos < m_bitmapData.height; ypos += 8)
		{
			for (int xpos = 0; xpos < m_bitmapData.width; xpos += 8)
			{
				ConvertRGBToYUV(m_bitmapData, xpos, ypos);
				DCY = processDU(YDU, m_fdYTable, DCY, m_huffmanTable_YDC, m_huffmanTable_YAC);
				DCU = processDU(UDU, m_fdUVTable, DCU, m_huffmanTable_UVDC, m_huffmanTable_UVAC);
				DCV = processDU(VDU, m_fdUVTable, DCV, m_huffmanTable_UVDC, m_huffmanTable_UVAC);
               
				// Let other threads do stuff too
				Thread.Sleep(0);
			}
		}
 
		// Do the bit alignment of the EOI marker
		if (m_currentBitIndex >= 0)
		{
			HuffmanTableEntry fillbits = new HuffmanTableEntry();
			fillbits.length = m_currentBitIndex + 1;
			fillbits.value = (1 << (m_currentBitIndex + 1)) - 1;
			writeBits(fillbits);
		}
 
		m_encodedBytes.writeWord(0xFFD9); //EOI
	}

    // Get the result
	public Byte[] GetBytes()
	{
		return m_encodedBytes.GetAllBytes();
	}
   
}
