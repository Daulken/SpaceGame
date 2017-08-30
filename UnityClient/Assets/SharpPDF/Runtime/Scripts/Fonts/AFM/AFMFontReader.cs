using System;
using System.IO;
using UnityEngine;
using SharpPDF;
using SharpPDF.Enumerators;
using SharpPDF.Exceptions;
using SharpPDF.Fonts;

namespace SharpPDF.Fonts.AFM {
	
	/// <summary>
	/// Class that reads an afm file to import the font metric into the PDF font object.
	/// </summary>
	internal class AFMFontReader : FontReader
	{
		private char[] m_delimiterTokens = {' ','\t'};
		private char[] m_semicolonToken = {';'};
		TextAsset m_fontResource = null;

		/// <summary>
		/// Class's Constructor
		/// </summary>
		/// <param name="fontReference">Font Reference</param>
		public AFMFontReader(string fontReference): base()
		{
			m_fontResource = Resources.Load("SharpPDF/AFM/" + fontReference + ".afm", typeof(TextAsset)) as TextAsset;
			if (m_fontResource == null)
				throw new PDFMissingFontFileException(fontReference);
		}

		/// <summary>
		/// Method that returs the definition of a font
		/// </summary>
		/// <returns>Font definition object</returns>
		public override PDFFontDefinition GetFontDefinition()
		{
			MemoryStream memoryStream = new MemoryStream(m_fontResource.bytes);
			StreamReader streamReader = new StreamReader(memoryStream);

			PDFFontDefinition myDefinition = new PDFFontDefinition();
			bool startCharMetric = false;
			
			try
			{				
				string strLine = streamReader.ReadLine();
				while (strLine != null)
				{					
					string[] afmValues = strLine.Split(m_delimiterTokens);
					switch (afmValues[0])
					{
					case "FontName":
						myDefinition.fontName = afmValues[1].Trim();
						break;
					case "FullName":
						myDefinition.fullFontName = afmValues[1].Trim();
						break;
					case "FamilyName":
						myDefinition.familyName = afmValues[1].Trim();
						break;
					case "Weight":
						myDefinition.fontWeight = afmValues[1].Trim();
						break;
					case "ItalicAngle":
						myDefinition.italicAngle = int.Parse(afmValues[1]);
						break;
					case "IsFixedPitch":
						myDefinition.isFixedPitch = bool.Parse(afmValues[1]);
						break;
					case "CharacterSet":
						myDefinition.characterSet = afmValues[1].Trim();
						break;
					case "FontBBox":
						myDefinition.fontBBox[0] = int.Parse(afmValues[1]);
						myDefinition.fontBBox[1] = int.Parse(afmValues[2]);
						myDefinition.fontBBox[2] = int.Parse(afmValues[3]);
						myDefinition.fontBBox[3] = int.Parse(afmValues[4]);
						myDefinition.fontHeight = Convert.ToInt32(Math.Round((((double)myDefinition.fontBBox[3] - (double)myDefinition.fontBBox[1]) / 1000)));
						if (myDefinition.fontHeight == 0)
						{
							myDefinition.fontHeight = 1;
						}
						break;
					case "UnderlinePosition":
						myDefinition.underlinePosition = int.Parse(afmValues[1]);
						break;
					case "UnderlineThickness":
						myDefinition.underlineThickness = int.Parse(afmValues[1]);
						break;
					case "EncodingScheme":
						myDefinition.encodingScheme = afmValues[1].Trim();
						break;
					case "CapHeight":
						myDefinition.capHeight = int.Parse(afmValues[1]);
						break;
					//Font Height
					/*
					case "XHeight":
						myDefinition.fontHeight = int.Parse(afmValues[1]);
						break;
					*/
					case "Ascender":
						myDefinition.ascender = int.Parse(afmValues[1]);
						break;
					case "Descender":
						myDefinition.descender = int.Parse(afmValues[1]);
						break;
					case "StdHW":
						myDefinition.StdHW = int.Parse(afmValues[1]);
						break;
					case "StdVW":
						myDefinition.StdVW = int.Parse(afmValues[1]);
						break;

					case "StartCharMetrics":
						startCharMetric = true;
						break;
					case "EndCharMetrics":
						startCharMetric = false;
						break;
					case "C":
						// Font's Character Metric
						if (startCharMetric)
						{
							PDFCharacterMetric myChar = GetCharacterMetric(strLine);
							myDefinition.fontMetrics[myChar.charIndex] = myChar.charWidth;
						}
						break;
					}

					strLine = streamReader.ReadLine();
				}
			}
			catch (Exception/* ex*/)
			{
				throw new PDFBadFontFileException();
			}
			return myDefinition;
		}
		
		/// <summary>
		/// Method that returns the metric of a single character
		/// </summary>
		/// <param name="characterMetric">String that contains character info</param>
		/// <returns>Character Metric Object</returns>
		private PDFCharacterMetric GetCharacterMetric(string characterMetric)
		{
			int charWidth = 0;
			//int charIndex = 0;
			string charName = null;

			string[] charTokens = characterMetric.Split(m_semicolonToken);
			foreach (string charToken in charTokens)
			{
				string[] tokenValues = charToken.Trim().Split(m_delimiterTokens);
				switch (tokenValues[0])
				{
				case "C":
					/*charIndex = */
					int.Parse(tokenValues[1]);
					break;
				case "WX":
					charWidth = int.Parse(tokenValues[1]);
					break;
				case "N":
					charName = tokenValues[1];
					break;
				}
			}

			charTokens = null;

			PDFCharacterMetric myChar = new PDFCharacterMetric(charName, GlyphConverter.UnicodeFromGlyph(charName), charWidth);
			return myChar;
		}

		/// <summary>
		/// Class's destructor
		/// </summary>
		public override void Dispose()
		{
		}
	}
}
