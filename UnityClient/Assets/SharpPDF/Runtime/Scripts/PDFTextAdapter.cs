using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SharpPDF.Elements;
using SharpPDF.Enumerators;
using SharpPDF.Exceptions;
using SharpPDF.Fonts;

namespace SharpPDF {
	
	/// <summary>
	/// Abstract class that implements different functions used for text and paragraph
	/// </summary>
	public abstract class PDFTextAdapter
	{

		/// <summary>
		/// Method that format a HEX string
		/// </summary>
		/// <param name="strText">input text string</param>
		/// <returns>HEX text string</returns>
		public static string HEXFormatter(string strText)
		{
			StringBuilder returnHexText = new StringBuilder();
			foreach (char myChar in strText.ToCharArray())
				returnHexText.Append(Convert.ToByte(myChar).ToString("X2"));			
			return returnHexText.ToString();
		}

		/// <summary>
		/// Static method that checks special characters into a string
		/// </summary>
		/// <param name="strText">Input Text</param>
		/// <returns>Formatted Text</returns>
		public static string CheckText(string strText)
		{
			string checkedString = strText;
			checkedString = checkedString.Replace(@"\", @"\\");
			checkedString = checkedString.Replace(@"(", @"\(");
			checkedString = checkedString.Replace(@")", @"\)");
			return checkedString;
		}
		
		private static ParagraphLine CreateNewLine(string text, PredefinedAlignment parAlign, int parWidth, int lineLength, int lineHeight, PDFAbstractFont fontType)
		{
			ParagraphLine returnedLine;
			switch (parAlign)
			{
			case PredefinedAlignment.Left:
			default:
				returnedLine = new ParagraphLine(text, lineHeight, 0, fontType);
				break;
			case PredefinedAlignment.Right:
				returnedLine = new ParagraphLine(text, lineHeight, parWidth - lineLength, fontType);
				break;
			case PredefinedAlignment.Center:
				returnedLine = new ParagraphLine(text, lineHeight, Convert.ToInt32(Math.Round(((double)(parWidth - (double)lineLength) / 2d))), fontType);
				break;
			}

			if (fontType is PDFTrueTypeFont)
				((PDFTrueTypeFont)fontType).AddCharacters(text);

			return returnedLine;
		}

		/// <summary>
		/// Static method thats format a paragraph
		/// </summary>
		/// <param name="strText">Input Text</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="fontType">Font's type</param>
		/// <param name="parWidth">Paragrapfh's width</param>
		/// <param name="lineHeight">Line's height</param>
		/// <param name="maxLines">Maximum number of lines in a paragraph</param>
		/// <returns>IEnumerable interface that cointains ParagraphLine objects</returns>
		public static List<ParagraphLine> FormatParagraph(ref string strText, int fontSize, PDFAbstractFont fontType, int parWidth, int maxLines, int lineHeight)
		{
			return FormatParagraph(ref strText, fontSize, fontType, parWidth, maxLines, lineHeight, PredefinedAlignment.Left);	
		}

		/// <summary>
		/// Static method thats format a paragraph
		/// </summary>
		/// <param name="strText">Input Text</param>
		/// <param name="fontSize">Font's size</param>
		/// <param name="fontType">Font's type</param>
		/// <param name="parWidth">Paragrapfh's width</param>
		/// <param name="lineHeight">Line's height</param>
		/// <param name="parAlign">Paragraph's Alignment</param>
		/// <param name="maxLines">Number of maximum lines of the paragraph</param>
		/// <returns>IEnumerable interface that cointains ParagraphLine objects</returns>
		public static List<ParagraphLine> FormatParagraph(ref string strText, int fontSize, PDFAbstractFont fontType, int parWidth, int maxLines, int lineHeight, PredefinedAlignment parAlign)
		{			
			string[] lines = strText.Replace("\r", System.String.Empty).Split(new char[1]{(char)10});
			string[] words;
			string word;
			bool finished = false;
			int lineLength;
			StringBuilder lineString = new StringBuilder();	
			List<ParagraphLine> resultParagraph = new List<ParagraphLine>();
			lineLength = 0;
			int countLine = 0;
			int countWord = 0;
			while (!finished && (countLine < lines.Length))
			{
				words = lines[countLine].Split(" ".ToCharArray());
				countWord = 0;
				while (!finished && (countWord < words.Length))
				{
					word = fontType.cleanText(words[countWord]);					
					if (!string.IsNullOrEmpty(word.Trim()))
					{
						if ((fontType.getWordWidth(word + " ", fontSize) + lineLength) > parWidth)
						{	
							if (lineLength == 0)
							{
								resultParagraph.Add(PDFTextAdapter.CreateNewLine(fontType.cropWord(word, parWidth, fontSize), parAlign, parWidth, parWidth, lineHeight, fontType));
								strText.Remove(0, words[countWord].Length).Trim();
								countWord++;								
							}
							else
							{
								resultParagraph.Add(PDFTextAdapter.CreateNewLine(lineString.ToString().Trim(), parAlign, parWidth, lineLength, lineHeight, fontType));								
								lineString.Remove(0, lineString.Length);
								lineLength = 0;					                            	
							}
							if ((resultParagraph.Count) == maxLines && maxLines > 0)
							{
								finished = true;
							}
						}
						else
						{
							lineString.Append(word + " ");						
							lineLength += fontType.getWordWidth(word + " ", fontSize);
							strText = strText.Remove(0, words[countWord].Length).Trim();	
							countWord++;
						}
					}
					else
					{
						countWord++;
					}
				}
				countLine++;
				if (!finished)
				{
					if (lineLength > 0)
					{					
						resultParagraph.Add(PDFTextAdapter.CreateNewLine(lineString.ToString().Trim(), parAlign, parWidth, lineLength, lineHeight, fontType));
						lineString.Remove(0, lineString.Length);
						lineLength = 0;					
					}
					else
					{																
						resultParagraph.Add(new ParagraphLine(System.String.Empty, lineHeight, 0, fontType));
					}
					if ((resultParagraph.Count) == maxLines && maxLines > 0)
					{
						finished = true;
					}				
				}
				words = null;
			}
			return resultParagraph;	
		}

	}
}
