using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class MeshExportToOBJ
{
	private struct ObjMaterial
	{
		public string name;
		public string textureName;
	}

	private static int m_vertexOffset = 0;
	private static int m_normalOffset = 0;
	private static int m_uvOffset = 0;
	private static Dictionary<string, ObjMaterial> m_materialList = null;
	
	// Converts a mesh to a string
	private static string MeshToString(string name, Mesh mesh, Transform transform, Renderer renderer)
	{
		StringBuilder objBuilder = new StringBuilder();

		// Add graph
		objBuilder.Append("g ").Append(name).Append("\n");

		// Add vertices
		foreach (Vector3 v in mesh.vertices)
		{
			// This is sort of ugly - inverting x-component since we're in a different coordinate system than "everyone" is "used to".
			Vector3 wv = transform.TransformPoint(v);
			objBuilder.Append(string.Format("v {0} {1} {2}\n", -wv.x, wv.y, wv.z));
		}
		objBuilder.Append("\n");
		
		// Add normals
		foreach (Vector3 v in mesh.normals)
		{
			// This is sort of ugly - inverting x-component since we're in a different coordinate system than "everyone" is "used to".
			Vector3 wv = transform.TransformDirection(v);
			objBuilder.Append(string.Format("vn {0} {1} {2}\n", -wv.x, wv.y, wv.z));
		}
		objBuilder.Append("\n");
		
		// Add UVs
		foreach (Vector3 v in mesh.uv)
			objBuilder.Append(string.Format("vt {0} {1}\n", v.x, v.y));

		// Add sub-meshes
		Material[] materials = renderer.sharedMaterials;
		for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; ++subMeshIndex)
		{
			// Add the material for this submesh
			objBuilder.Append("\n");
			objBuilder.Append("usemtl ").Append(materials[subMeshIndex].name).Append("\n");
			objBuilder.Append("usemap ").Append(materials[subMeshIndex].name).Append("\n");

			// See if this material is already in the materiallist, and if not, add it
			if (!m_materialList.ContainsKey(materials[subMeshIndex].name))
			{
				ObjMaterial objMaterial = new ObjMaterial();
				objMaterial.name = materials[subMeshIndex].name;
				if (materials[subMeshIndex].mainTexture != null)
					objMaterial.textureName = AssetDatabase.GetAssetPath(materials[subMeshIndex].mainTexture);
				else 
					objMaterial.textureName = null;
				m_materialList.Add(objMaterial.name, objMaterial);
			}

			// Add the triangles for this submesh
			int[] triangles = mesh.GetTriangles(subMeshIndex);
			for (int triangleIndex = 0; triangleIndex < triangles.Length; triangleIndex += 3)
			{
				objBuilder.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
					triangles[triangleIndex] + 1 + m_vertexOffset,
					triangles[triangleIndex + 1] + 1 + m_normalOffset,
					triangles[triangleIndex + 2] + 1 + m_uvOffset)
					);
			}
		}

		// Increase offsets for next mesh				
		m_vertexOffset += mesh.vertices.Length;
		m_normalOffset += mesh.normals.Length;
		m_uvOffset += mesh.uv.Length;
		
		// Return this string
		return objBuilder.ToString();
	}
	
	// Writes materials out to MTL files
	private static void MaterialsToFile(string fullpath)
	{
		using (StreamWriter sw = new StreamWriter(fullpath)) 
		{
			foreach (KeyValuePair<string, ObjMaterial> kvp in m_materialList)
			{
				sw.Write("\n");
				sw.Write("newmtl {0}\n", kvp.Key);
				sw.Write("Ka 0.6 0.6 0.6\n");
				sw.Write("Kd 1.0 1.0 1.0\n");
				sw.Write("Ks 0.9 0.9 0.9\n");
				sw.Write("Ns 0.0\n");
				sw.Write("d 1.0\n");
				sw.Write("illum 2\n");

				if (kvp.Value.textureName != null)
				{
					string relativeFile = kvp.Value.textureName;
					int stripIndex = relativeFile.LastIndexOf('/');
					if (stripIndex >= 0)
						relativeFile = relativeFile.Substring(stripIndex + 1).Trim();
					string destinationFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fullpath), relativeFile);

					try
					{
						File.Copy(kvp.Value.textureName, destinationFile);
					}
					catch
					{
					}	
					
					sw.Write("map_Kd {0}", relativeFile);
				}
				
				sw.Write("\n\n\n");
			}
		}
	}
	
	// Gets lists of selected MeshFilter's and SkinnedMeshRenderer's
	private static bool GetSelectedMeshObjects(out List<MeshFilter> meshFilters, out List<SkinnedMeshRenderer> skinnedMeshes)
	{
		// Initialise the lists
		meshFilters = null;
		skinnedMeshes = null;

		// Get the selected objects
		Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
		if (selection.Length == 0)
		{
			EditorUtility.DisplayDialog("No source objects selected!", "Please select one or more MeshFilter or SkinnedMeshRenderer objects", "OK");
			return false;
		}

		// Get the mesh filters and skinned mesh renderers to export
		meshFilters = new List<MeshFilter>();
		skinnedMeshes = new List<SkinnedMeshRenderer>();
		for (int selectionIndex = 0; selectionIndex < selection.Length; ++selectionIndex)
		{
			Component[] meshFiltersInSelection = selection[selectionIndex].GetComponentsInChildren(typeof(MeshFilter));
			if (meshFiltersInSelection != null)
			{
				foreach (Component meshFilter in meshFiltersInSelection)
					meshFilters.Add(meshFilter as MeshFilter);
			}
			Component[] skinnedMeshesInSelection = selection[selectionIndex].GetComponentsInChildren(typeof(SkinnedMeshRenderer));
			if (skinnedMeshesInSelection != null)
			{
				foreach (Component skinnedMeshRenderer in skinnedMeshesInSelection)
					skinnedMeshes.Add(skinnedMeshRenderer as SkinnedMeshRenderer);
			}
		}

		// If there are no mesh filters or skinned mesh renderers in the selection, abort export
		if ((meshFilters.Count == 0) && (skinnedMeshes.Count == 0))
		{
			EditorUtility.DisplayDialog("No MeshFilter or SkinnedMeshRenderer objects selected!", "Please select one or more MeshFilter or SkinnedMeshRenderer objects", "OK");
			return false;
		}

		// Successful gather
		return true;
	}

	[MenuItem("SPACEJAM/Export Scene/Export Selected Meshes To Single OBJ", false, 102)]
	static void SaveSelectedMeshesToSingleOBJ()
	{
		List<MeshFilter> meshFilters;
		List<SkinnedMeshRenderer> skinnedMeshes;
		if (!GetSelectedMeshObjects(out meshFilters, out skinnedMeshes))
			return;
	
		// Get the location to save the file
		string objFilename = EditorUtility.SaveFilePanel("Save Combined OBJ", "", "ExportedMeshes.obj", "obj");

		// Create the target folder to write to
		try
		{
			Directory.CreateDirectory(System.IO.Path.GetDirectoryName(objFilename));
		}
		catch
		{
			EditorUtility.DisplayDialog("Save Combined OBJ", "Failed to create target folder!", "OK");
			return;
		}
		
		// Get the material filename
		string materialFilePath = System.IO.Path.ChangeExtension(objFilename, ".mtl");
		string materialFileName = System.IO.Path.GetFileName(materialFilePath);

		// Initialise the export state
		m_vertexOffset = 0;
		m_normalOffset = 0;
		m_uvOffset = 0;
		m_materialList = new Dictionary<string, ObjMaterial>();

		// Reset the number of exported meshes
		int exportedObjects = 0;

		// Export the mesh
		using (StreamWriter writer = new StreamWriter(objFilename))
		{
			// Write the material reference
			writer.Write("mtllib ./" + materialFileName + "\n");
		
			// Export each MeshFilter in turn to the same file
			for (int objectIndex = 0; objectIndex < meshFilters.Count; ++objectIndex)
			{
				MeshFilter meshFilter = meshFilters[objectIndex];
				writer.Write(MeshToString(meshFilter.name, meshFilter.mesh, meshFilter.transform, meshFilter.GetComponent<Renderer>()));
				exportedObjects++;
			}

			// Export each SkinnedMeshRenderer in turn to the same file
			for (int objectIndex = 0; objectIndex < skinnedMeshes.Count; ++objectIndex)
			{
				SkinnedMeshRenderer skinnedMesh = skinnedMeshes[objectIndex];
				writer.Write(MeshToString(skinnedMesh.name, skinnedMesh.sharedMesh, skinnedMesh.transform, skinnedMesh.GetComponent<Renderer>()));
				exportedObjects++;
			}
		}

		// Write out materials used by this export
		MaterialsToFile(materialFilePath);

		// Inform the user of the exported mesh count
		EditorUtility.DisplayDialog("Save Combined OBJ", "Exported " + exportedObjects + " objects to the file", "OK");
	}
		
	[MenuItem("SPACEJAM/Export Scene/Export Selected Meshes To Separate OBJs", false, 102)]
	static void SaveSelectedMeshesToSeparateOBJs()
	{
		List<MeshFilter> meshFilters;
		List<SkinnedMeshRenderer> skinnedMeshes;
		if (!GetSelectedMeshObjects(out meshFilters, out skinnedMeshes))
			return;
	
		// Get the location to save the file
		string objFolder = EditorUtility.SaveFolderPanel("Save Separate OBJ", "", "ExportedOBJMeshes");
		
		// Create the target folder to write to
		try
		{
			Directory.CreateDirectory(objFolder);
		}
		catch
		{
			EditorUtility.DisplayDialog("Save Separate OBJ", "Failed to create target folder!", "OK");
			return;
		}
		
		// Reset the number of exported meshes
		int exportedObjects = 0;

		// Export each MeshFilter in turn
		for (int objectIndex = 0; objectIndex < meshFilters.Count; ++objectIndex)
		{
			MeshFilter meshFilter = meshFilters[objectIndex];
		
			// Validate the filename
			string filename = meshFilter.gameObject.GetNameWithPath().Substring(1);
			foreach (char c in System.IO.Path.GetInvalidFileNameChars())
				filename = filename.Replace(c, '_');
			string fullPath = System.IO.Path.Combine(objFolder, filename + ".obj");

			// Get the material filename
			string materialFilePath = System.IO.Path.ChangeExtension(fullPath, ".mtl");
			string materialFileName = System.IO.Path.GetFileName(materialFilePath);

			// Initialise the export state
			m_vertexOffset = 0;
			m_normalOffset = 0;
			m_uvOffset = 0;
			m_materialList = new Dictionary<string, ObjMaterial>();

			// Export the mesh
			using (StreamWriter writer = new StreamWriter(fullPath))
			{
				// Write the material reference
				writer.Write("mtllib ./" + materialFileName + "\n");

				// Write the mesh
				writer.Write(MeshToString(meshFilter.name, meshFilter.mesh, meshFilter.transform, meshFilter.GetComponent<Renderer>()));

				exportedObjects++;
			}
			
			// Write out materials used by this export
			MaterialsToFile(materialFilePath);
		}

		// Export each SkinnedMeshRenderer in turn
		for (int objectIndex = 0; objectIndex < skinnedMeshes.Count; ++objectIndex)
		{
			SkinnedMeshRenderer skinnedMesh = skinnedMeshes[objectIndex];
		
			// Validate the filename
			string filename = skinnedMesh.gameObject.GetNameWithPath().Substring(1);
			foreach (char c in System.IO.Path.GetInvalidFileNameChars())
				filename = filename.Replace(c, '_');
			string fullPath = System.IO.Path.Combine(objFolder, filename + ".obj");

			// Get the material filename
			string materialFilePath = System.IO.Path.ChangeExtension(fullPath, ".mtl");
			string materialFileName = System.IO.Path.GetFileName(materialFilePath);

			// Initialise the export state
			m_vertexOffset = 0;
			m_normalOffset = 0;
			m_uvOffset = 0;
			m_materialList = new Dictionary<string, ObjMaterial>();

			// Export the mesh
			using (StreamWriter writer = new StreamWriter(fullPath))
			{
				// Write the material reference
				writer.Write("mtllib ./" + materialFileName + "\n");

				// Write the mesh
				writer.Write(MeshToString(skinnedMesh.name, skinnedMesh.sharedMesh, skinnedMesh.transform, skinnedMesh.GetComponent<Renderer>()));

				exportedObjects++;
			}
			
			// Write out materials used by this export
			MaterialsToFile(materialFilePath);
		}
		
		// Inform the user of the exported mesh count
		EditorUtility.DisplayDialog("Save Separate OBJ", "Exported " + exportedObjects + " objects", "OK");
	}
}
