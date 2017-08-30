using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
 
public class LocalizationImporter : AssetPostprocessor 
{
	private static string GetStringtableFromODS(string odsFilename)
	{
		if (!odsFilename.ToLower().EndsWith(".ods"))
			return null;
		string stringtableFilename = odsFilename.Substring(0, odsFilename.Length - 4) + "_Stringtable.bytes";
		string filePart = System.IO.Path.GetFileName(stringtableFilename);
		string pathPart = System.IO.Path.GetDirectoryName(stringtableFilename);
		stringtableFilename = System.IO.Path.Combine(System.IO.Path.Combine(pathPart, "Resources"), filePart);
		return stringtableFilename;
	}

	private static void CreateStringtable(string odsFilename, string stringtableFilename)
	{
		// Load the ODS file data
		string absoluteODSFilename = Application.dataPath + odsFilename.Substring("Assets".Length);
		FileStream fileIn = new FileStream(absoluteODSFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		BinaryReader reader = new BinaryReader(fileIn);
		byte[] odsFileData = reader.ReadAllBytes();

		// Decompress the ODS 'content.xml' file
		OpenOffice.ODSDocument odsFile = OpenOffice.LoadODSFile(odsFileData);

		// Get the translations - this is on the first sheet
		OpenOffice.ODSSheet odsSheet = odsFile.m_sheets["Sheet1"];
			
		// Split up the ODS row data into headers and translations
		List<string> languageHeaders = odsSheet.m_rows[0].m_cells.GetRange(1, odsSheet.m_rows[0].m_cells.Count - 1);
		List<OpenOffice.ODSRow> translationRows = odsSheet.m_rows.GetRange(1, odsSheet.m_rows.Count - 1);

		// Go through each language in the table
		Dictionary<SystemLanguage, Dictionary<string, string>> allTranslations = new Dictionary<SystemLanguage, Dictionary<string, string>>();
		for (int columnIndex = 0; columnIndex < languageHeaders.Count; ++columnIndex)
		{
			// If this is an empty header, stop reading the stringtable
			string languageString = languageHeaders[columnIndex];
			if (string.IsNullOrEmpty(languageString))
				break;

			// If this isn't a recognised language, ignore it
			if (!Enum.IsDefined(typeof(SystemLanguage), languageString))
				continue;

			// Now generate the translation table for this language
			Dictionary<string, string> languageDictionary = new Dictionary<string, string>();
			foreach (OpenOffice.ODSRow row in translationRows)
			{
				string key = row.m_cells[0];
				if (string.IsNullOrEmpty(key))
					continue;

				string value = row.m_cells[1 + columnIndex];
				if (string.IsNullOrEmpty(value))
					languageDictionary[key] = "";
				else
					languageDictionary[key] = value.Replace("\\n", "\n");
			}

			// Add the table to the translation dictionary for this language
			SystemLanguage language = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), languageString);
			allTranslations.Add(language, languageDictionary);
		}

		// Create the stringtable contents
		string content = Storage.SerializeToString(allTranslations);

		// Write the binary stringtable contents
		string absoluteStringtableFilename = Application.dataPath + stringtableFilename.Substring("Assets".Length);
		string absoluteStringtablePath = System.IO.Path.GetDirectoryName(absoluteStringtableFilename);
		Directory.CreateDirectory(absoluteStringtablePath);
		StreamWriter writer = File.CreateText(absoluteStringtableFilename);
		writer.Write(content);
		writer.Flush();
		writer.Close();
			
		// Refresh the database, so the new file is loaded
		AssetDatabase.Refresh();
	}

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedToAssets, string[] movedFromAssets)
	{
		// Go through all removed ODS assets
		foreach (string asset in deletedAssets)
		{
			// Ensure we also remove related stringtable assets
			string stringtable = GetStringtableFromODS(asset);
			if (!string.IsNullOrEmpty(stringtable))
				AssetDatabase.DeleteAsset(stringtable);
		}

		// Go through all renamed ODS assets
		for (int moveIndex = 0; moveIndex < movedFromAssets.Length; ++moveIndex)
		{
			// Ensure we also rename related stringtable assets
			string oldStringtable = GetStringtableFromODS(movedFromAssets[moveIndex]);
			string newStringtable = GetStringtableFromODS(movedToAssets[moveIndex]);
			if (!string.IsNullOrEmpty(oldStringtable) || !string.IsNullOrEmpty(newStringtable))
			{
				if (string.IsNullOrEmpty(oldStringtable))
					CreateStringtable(movedToAssets[moveIndex], newStringtable);
				else if (string.IsNullOrEmpty(newStringtable))
					AssetDatabase.DeleteAsset(oldStringtable);
				else
					AssetDatabase.RenameAsset(oldStringtable, newStringtable);
			}
		}

		// Go through all new/updated ODS assets
		foreach (string asset in importedAssets)
		{
			// Ensure we also create/update related stringtable assets
			string stringtable = GetStringtableFromODS(asset);
			if (!string.IsNullOrEmpty(stringtable))
				CreateStringtable(asset, stringtable);
		}
	}

}
