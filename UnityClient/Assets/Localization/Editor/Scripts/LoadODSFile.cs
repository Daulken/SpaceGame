using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using ICSharpCode.SharpZipLib.Zip;

public static class OpenOffice
{
	public class ODSRow
	{
		public List<string> m_cells = new List<string>();
	};

	public class ODSSheet
	{
		public List<ODSRow> m_rows = new List<ODSRow>();
	};

	public class ODSDocument
	{
		public Dictionary<string, ODSSheet> m_sheets = new Dictionary<string, ODSSheet>();
	};

	// Namespaces. We need this to initialize XmlNamespaceManager so that we can search XmlDocument.
	private static string[,] namespaces = new string[,] 
	{
		{"table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0"},
		{"office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0"},
		{"style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0"},
		{"text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0"},            
		{"draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0"},
		{"fo", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0"},
		{"dc", "http://purl.org/dc/elements/1.1/"},
		{"meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0"},
		{"number", "urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0"},
		{"presentation", "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0"},
		{"svg", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0"},
		{"chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0"},
		{"dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0"},
		{"math", "http://www.w3.org/1998/Math/MathML"},
		{"form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0"},
		{"script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0"},
		{"ooo", "http://openoffice.org/2004/office"},
		{"ooow", "http://openoffice.org/2004/writer"},
		{"oooc", "http://openoffice.org/2004/calc"},
		{"dom", "http://www.w3.org/2001/xml-events"},
		{"xforms", "http://www.w3.org/2002/xforms"},
		{"xsd", "http://www.w3.org/2001/XMLSchema"},
		{"xsi", "http://www.w3.org/2001/XMLSchema-instance"},
		{"rpt", "http://openoffice.org/2005/report"},
		{"of", "urn:oasis:names:tc:opendocument:xmlns:of:1.2"},
		{"rdfa", "http://docs.oasis-open.org/opendocument/meta/rdfa#"},
		{"config", "urn:oasis:names:tc:opendocument:xmlns:config:1.0"}
	};

	public static ODSDocument LoadODSFile(byte[] fileData)
	{
		// Decompress the ODS 'content.xml' file
		MemoryStream stream = new MemoryStream(fileData);
		ZipFile zipFile = new ZipFile(stream);
		ZipEntry zipEntry = zipFile.GetEntry("content.xml");
		if (zipEntry == null)
			return null;

		// Extract that file to MemoryStream.
		Stream contentStream = zipFile.GetInputStream(zipEntry);

		// Create XmlDocument from MemoryStream (MemoryStream contains content.xml).
		XmlDocument contentXml = new XmlDocument();
		contentXml.Load(contentStream);

		// Set up the namespace manager
		XmlNamespaceManager nmsManager = new XmlNamespaceManager(contentXml.NameTable);
		for (int i = 0; i < namespaces.GetLength(0); i++)
			nmsManager.AddNamespace(namespaces[i, 0], namespaces[i, 1]);

		// Initialise the ODS file
		ODSDocument odsFileData = new ODSDocument();

		// Enumerate the tables (sheets) in the document
		XmlNodeList tableNodes = contentXml.SelectNodes("/office:document-content/office:body/office:spreadsheet/table:table", nmsManager);
		foreach (XmlNode tableNode in tableNodes)
		{
			// Get the name of the sheet
			string sheetName = tableNode.Attributes["table:name"].Value;

			// Initialise the data for this sheet
			ODSSheet odsSheetData = new ODSSheet();
			int maxColumnCount = 0;

			// Enumerate the rows in this sheet
			XmlNodeList rowNodes = tableNode.SelectNodes("table:table-row", nmsManager);
			foreach (XmlNode rowNode in rowNodes)
			{
				// Initialise the data for this row
				ODSRow odsRowData = new ODSRow();
				int columnCount = 0;

				// Enumerate the cells in this row
				XmlNodeList cellNodes = rowNode.SelectNodes("table:table-cell", nmsManager);
				XmlNode lastCell = cellNodes[cellNodes.Count - 1];
				foreach (XmlNode cellNode in cellNodes)
				{
					// Read the current cell
					XmlAttribute cellAttr = cellNode.Attributes["office:value"];
					string cellValue = (cellAttr == null) ? (String.IsNullOrEmpty(cellNode.InnerText) ? null : cellNode.InnerText) : cellAttr.Value;

					// Get the number of cell copies
					XmlAttribute cellRepeated = cellNode.Attributes["table:number-columns-repeated"];
					int cellCount = (cellRepeated == null) ? 1 : Convert.ToInt32(cellRepeated.Value, CultureInfo.InvariantCulture);
						
					// Add cellCount number of cellValue values to this row, unless it's the null last column
					if ((cellValue != null) || !object.ReferenceEquals(cellNode, lastCell))
					{
						columnCount += cellCount;
						for (int cellIndex = 0; cellIndex < cellCount; ++cellIndex)
							odsRowData.m_cells.Add(cellValue);
					}
				}

				// Update the maximum number of cells in any row
				maxColumnCount = Mathf.Max(maxColumnCount, columnCount);

				// Get the number of row copies
				XmlAttribute rowsRepeated = rowNode.Attributes["table:number-rows-repeated"];
				int rowCount = (rowsRepeated == null) ? 1 : Convert.ToInt32(rowsRepeated.Value, CultureInfo.InvariantCulture);

				// Add rowCount number of rows to this sheet, unless it's an empty row
				if (odsRowData.m_cells.Count > 0)
				{
					for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
						odsSheetData.m_rows.Add(odsRowData);
				}
			}

			// Now pad each column to the maximum column width
			foreach (ODSRow odsRowData in odsSheetData.m_rows)
			{
				for (int cellIndex = odsRowData.m_cells.Count; cellIndex < maxColumnCount; ++cellIndex)
					odsRowData.m_cells.Add(System.String.Empty);
			}

			// Store the data for this sheet
			odsFileData.m_sheets[sheetName] = odsSheetData;
		}

		// Return the ODS file data
		return odsFileData;
	}

	public static ODSDocument LoadODSFile(TextAsset fileData)
	{
		// Load the raw bytes from the given asset
		return LoadODSFile(fileData.bytes);
	}

}	// class OpenOffice
