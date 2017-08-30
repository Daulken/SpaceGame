using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

 
public class FindReferences : EditorWindow
{
	private class Reference
	{
		public Component m_referringComponent = null;
		public string m_referringMember = String.Empty;
		public bool m_referringFieldPublic = false;
		public Type m_referredType = null;
		public GameObject m_referredRootGameObject = null;
		public GameObject m_referredGameObject = null;
		
		public static string HelpFormat()
		{
			return
				"Reference format is \"GameObject [Component.Member]\". If an associated object is referenced (eg, a child, or a Component on a selected GameObject), this is shown with \" -> GameObject [Component]\".\n" +
				"\n" +
				"References displayed in bold are public attributes, otherwise they are private.\n" +
				"\n" +
				"Click on a reference to select the object in the project hierarchy views.\n";
		}

		public override string ToString()
		{
			if (m_referringComponent == null)
				return "";
			StringBuilder sb = new StringBuilder();
			sb.Append(m_referringComponent.gameObject.GetNameWithPath());
			sb.Append(" [");
			sb.Append(m_referringComponent.GetType().Name);
			if (!string.IsNullOrEmpty(m_referringMember))
			{
				sb.Append(".");
				sb.Append(m_referringMember);
			}
			sb.Append("]");
			if ((m_referredType != null) || (m_referredGameObject != null))
			{
				sb.Append(" -> ");
				if (m_referredGameObject != null)
				{
					sb.Append(m_referredGameObject.GetNameWithPath().Replace(m_referredRootGameObject.GetNameWithPath(), ""));
					if (m_referredType != null)
						sb.Append(" [");
				}
				if (m_referredType != null)
					sb.Append(m_referredType.Name);
				if ((m_referredGameObject != null) && (m_referredType != null))
					sb.Append("]");
			}
			return sb.ToString();
		}

		public GUIStyle DisplayStyle()
		{
			GUIStyle displayStyle = new GUIStyle(EditorStyles.textField);
			displayStyle.alignment = TextAnchor.MiddleLeft;
			displayStyle.wordWrap = false;
			if (m_referringFieldPublic)
				displayStyle.font = EditorStyles.boldFont;
			return displayStyle;
		}
	}

	private struct IgnoreProperties
	{
		public Type m_type;
		public string[] m_properties;

		public IgnoreProperties(Type type, string[] properties)
		{
			m_type = type;
			m_properties = properties;
		}
	};

	private UnityEngine.Object m_selectedObject = null;
	private UnityEngine.Object m_lastSelectedObject = null;
	private GameObject m_selectedGameObject = null;
	private Component[] m_selectedComponents = new Component[0];
	private bool m_testChildren = false;
	private bool m_ignoreInternalReferences = false;
	private bool m_ignoreDisabledComponents = false;
	private Type m_selectedObjectType = null;
	private bool m_selectedObjectIsScript = false;
	private GameObject m_gameObjectForSelection = null;

	private Dictionary<Component, List<object>> m_visitedObjects = new Dictionary<Component, List<object>>();

	private Vector2 m_scrollPositionExact = Vector2.zero;
	private Vector2 m_scrollPositionGameObject = Vector2.zero;
	private Vector2 m_scrollPositionComponent = Vector2.zero;
	private Vector2 m_scrollPositionChildGameObject = Vector2.zero;

	private List<Reference> m_matchListExact = new List<Reference>();
	private List<Reference> m_matchListGameObject = new List<Reference>();
	private List<Reference> m_matchListComponent = new List<Reference>();
	private List<Reference> m_matchListChildGameObject = new List<Reference>();

	private static IgnoreProperties[] ms_ignoredProperties = new IgnoreProperties[] {
			new IgnoreProperties(typeof(GameObject), new string[] {
					"transform",
					"rigidbody",
					"rigidbody2D",
					"camera",
					"light",
					"animation",
					"constantForce",
					"constantForce2D",
					"renderer",
					"audio",
					"guiText",
					"networkView",
					"guiTexture",
					"guiElement",
					"collider",
					"collider2D",
					"hingeJoint",
					"hingeJoint2D",
					"particleEmitter",
					"particleSystem",
					"gameObject",
				}
			),
			new IgnoreProperties(typeof(Component), new string[] {
					"transform",
					"rigidbody",
					"rigidbody2D",
					"camera",
					"light",
					"animation",
					"constantForce",
					"constantForce2D",
					"renderer",
					"audio",
					"guiText",
					"networkView",
					"guiTexture",
					"guiElement",
					"collider",
					"collider2D",
					"hingeJoint",
					"hingeJoint2D",
					"particleEmitter",
					"particleSystem",
					"gameObject",
				}
			),
			new IgnoreProperties(typeof(Collider), new string[] {
					"material",
				}
			),
			new IgnoreProperties(typeof(Renderer), new string[] {
					"material",
					"materials"
				}
			),
			new IgnoreProperties(typeof(MeshFilter), new string[] {
					"mesh",
				}
			),
			new IgnoreProperties(typeof(SkinnedMeshRenderer), new string[] {
					"mesh",
				}
			),
			new IgnoreProperties(typeof(Transform), new string[] {
					"parent",
					"root",
				}
			),
			new IgnoreProperties(typeof(Camera), new string[] {
					"allCameras",
				}
			),
			new IgnoreProperties(typeof(Vector2), new string[] {
					"zero",
					"one",
					"up",
					"right",
				}
			),
			new IgnoreProperties(typeof(Vector3), new string[] {
					"zero",
					"one",
					"up",
					"down",
					"left",
					"right",
					"fwd",
					"forward",
					"back",
				}
			),
			new IgnoreProperties(typeof(Vector4), new string[] {
					"zero",
					"one",
				}
			),
			new IgnoreProperties(typeof(Color), new string[] {
					"red",
					"green",
					"blue",
					"white",
					"black",
					"cyan",
					"yellow",
					"magenta",
					"grey",
					"gray",
					"clear",
				}
			),
		};

	[MenuItem("SPACEJAM/Assets/Find References", false, 100)]
	private static void OnSearchForReferences()
	{
		// Show the window with the selected object
		FindReferences window = GetWindow<FindReferences>(false, "FindReferences", true);
		window.m_selectedObject = Selection.activeObject;
	}

	public void OnGUI()
	{
		EditorGUIUtility.labelWidth = 180.0f;

		// Allow the selected object to be changed
		m_selectedObject = EditorGUILayout.ObjectField("Object Referenced: ", m_selectedObject, typeof(UnityEngine.Object), true) as UnityEngine.Object;
		
		// If no object is selected, do nothing
		if (m_selectedObject == null)
		{
			EditorGUIUtility.labelWidth = 0.0f;
			return;
		}

		// If the selected object has changed, or the type is invalid (which happens when scripts recompile :/)
		if ((m_selectedObject != m_lastSelectedObject) || (m_selectedObjectType == null))
		{
			m_lastSelectedObject = m_selectedObject;

			// Get the components and game object associated with the selection
			m_selectedGameObject = null;
			m_selectedComponents = null;
			GameObject selectedGameObject = m_selectedObject as GameObject;
			Component selectedComponent = m_selectedObject as Component;
			if (selectedGameObject != null)
				m_selectedComponents = selectedGameObject.GetComponents<Component>();
			else if (selectedComponent != null)
				m_selectedGameObject = selectedComponent.gameObject;
			m_gameObjectForSelection = m_selectedGameObject ?? (m_selectedObject as GameObject);
			
			// Get the selected object type
			m_selectedObjectType = (m_selectedObject != null) ? m_selectedObject.GetType() : null;
			m_selectedObjectIsScript = false;
			if (m_selectedObjectType == typeof(MonoScript))
			{
				MonoScript script = m_selectedObject as MonoScript;
				Type scriptType = script.GetClass();
				if (scriptType != null)
				{
					m_selectedObjectIsScript = true;
					m_selectedObjectType = scriptType;
				}
			}
		}
		
		// Display information about selected object
		EditorHelpers.StartBox();
		string assetPath = AssetDatabase.GetAssetPath(m_selectedObject);
		if (!string.IsNullOrEmpty(assetPath))
		{
			GUILayout.Label("Project Path:");
			GUILayout.Label(assetPath, EditorStyles.wordWrappedLabel);
		}
		else
		{
			if (m_gameObjectForSelection != null)
			{
				GUILayout.Label("Scene Path:");
				GUILayout.Label(m_gameObjectForSelection.GetNameWithPath(), EditorStyles.wordWrappedLabel);
			}
		}
		GUILayout.Label("Type: " + (m_selectedObjectIsScript ? string.Format("MonoScript ({0})", m_selectedObjectType.Name) : m_selectedObjectType.Name));
		EditorHelpers.EndBox();

		// Flag to see if we also check child GameObjects
		m_ignoreDisabledComponents = EditorGUILayout.Toggle("Ignore Disabled Components: ", m_ignoreDisabledComponents);
		if (!m_selectedObjectIsScript)
		{
			m_testChildren = EditorGUILayout.Toggle("Include Child GameObjects: ", m_testChildren);
			m_ignoreInternalReferences = EditorGUILayout.Toggle("Ignore Internal References: ", m_ignoreInternalReferences);
		}
		
		// Find references for selected object
		if (GUILayout.Button("Find References to this Object"))
			FindObjectsReferencingSelection();

		EditorGUILayout.Space();

		// Display help text to make it easier to use
		EditorGUILayout.HelpBox(Reference.HelpFormat(), MessageType.Info);
		
		// Show references to the exact object
		if (m_matchListExact.Count > 0)
		{
			GUILayout.Label("References to Exact Selected Object: (" + m_matchListExact.Count + ")");
			m_scrollPositionExact = GUILayout.BeginScrollView(m_scrollPositionExact);
			EditorHelpers.StartBox();
			foreach (Reference reference in m_matchListExact)
			{
				if (GUILayout.Button(reference.ToString(), reference.DisplayStyle(), GUILayout.ExpandWidth(true)))
				{
					Selection.activeObject = reference.m_referringComponent;
					EditorGUIUtility.PingObject(reference.m_referringComponent);
				}
			}
			EditorHelpers.EndBox();
			GUILayout.EndScrollView();
		}

		// Show references to any GameObject the reference belongs to
		if (m_matchListGameObject.Count > 0)
		{
			GUILayout.Label("References to Selected GameObject: (" + m_matchListGameObject.Count + ")");
			m_scrollPositionGameObject = GUILayout.BeginScrollView(m_scrollPositionGameObject);
			EditorHelpers.StartBox();
			foreach (Reference reference in m_matchListGameObject)
			{
				if (GUILayout.Button(reference.ToString(), reference.DisplayStyle(), GUILayout.ExpandWidth(true)))
				{
					Selection.activeObject = reference.m_referringComponent;
					EditorGUIUtility.PingObject(reference.m_referringComponent);
				}
			}
			EditorHelpers.EndBox();
			GUILayout.EndScrollView();
		}

		// Show references to any specific Components on a GameObject
		if (m_matchListComponent.Count > 0)
		{
			GUILayout.Label("References to Components on Selected GameObject: (" + m_matchListComponent.Count + ")");
			m_scrollPositionComponent = GUILayout.BeginScrollView(m_scrollPositionComponent);
			EditorHelpers.StartBox();
			foreach (Reference reference in m_matchListComponent)
			{
				if (GUILayout.Button(reference.ToString(), reference.DisplayStyle(), GUILayout.ExpandWidth(true)))
				{
					Selection.activeObject = reference.m_referringComponent;
					EditorGUIUtility.PingObject(reference.m_referringComponent);
				}
			}
			EditorHelpers.EndBox();
			GUILayout.EndScrollView();
		}

		// Show references to any child GameObject the reference belongs to
		if (m_matchListChildGameObject.Count > 0)
		{
			GUILayout.Label("References to Selected GameObject Children: (" + m_matchListChildGameObject.Count + ")");
			m_scrollPositionChildGameObject = GUILayout.BeginScrollView(m_scrollPositionChildGameObject);
			EditorHelpers.StartBox();
			foreach (Reference reference in m_matchListChildGameObject)
			{
				if (GUILayout.Button(reference.ToString(), reference.DisplayStyle(), GUILayout.ExpandWidth(true)))
				{
					Selection.activeObject = reference.m_referringComponent;
					EditorGUIUtility.PingObject(reference.m_referringComponent);
				}
			}
			EditorHelpers.EndBox();
			GUILayout.EndScrollView();
		}

		// Add this, so the controls don't space out
		GUILayout.FlexibleSpace();

		EditorGUIUtility.labelWidth = 0.0f;
	}

	private int[] GetArrayRankIndicesAtIndex(Array array, int index)
	{
		int noofRanks = array.Rank;
		int[] indices = new int[noofRanks];
		for (int rankIndex = 0; rankIndex < noofRanks; ++rankIndex)
		{
			var divider = 1;
			for (int innerRankIndex = rankIndex + 1; innerRankIndex < noofRanks; ++innerRankIndex)
				divider *= array.GetLength(innerRankIndex);
			indices[rankIndex] = array.GetLowerBound(rankIndex) + index / divider % array.GetLength(rankIndex);
		}
		return indices;
	}

	/// <summary>
	/// Class that can be used to compare matches for sorting.
	/// </summary>
	private class MatchComparer : Comparer<Reference>
	{
		public override int Compare(Reference a, Reference b)
		{
			// Handle one or more null references
			if (a.m_referringComponent == null)
			{
				if (b.m_referringComponent == null)
					return 0;
				else
					return 1;
			}
			else if (b.m_referringComponent == null)
			{
				return -1;
			}

			return string.Compare(a.m_referringComponent.gameObject.GetNameWithPath(), b.m_referringComponent.gameObject.GetNameWithPath());
		}
	}
	
	private bool IsValidComponentForScan(Component component)
	{
		// Ignore things in the database, we just want to test scenes
		if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(component)))
			return false;
		
		// Ignore those that are not visible in the scene - hidden or in project prefabs
		if ((component.gameObject.hideFlags != HideFlags.None) && (component.gameObject.hideFlags != HideFlags.NotEditable))
			return false;

		// If this component is disabled, ignore it if required
		if (m_ignoreDisabledComponents)
		{
			MonoBehaviour behaviour = component as MonoBehaviour;
			if ((behaviour != null) && !behaviour.enabled)
				return false;
		}
	
		// Can scan this component
		return true;
	}

	// Get all fields from a type, including private ones from base classes
	private IEnumerable<FieldInfo> GetAllFieldsFromType(Type type, BindingFlags bindingFlags)
	{
		if ((type == null) || (type == typeof(UnityEngine.Object)))
			return Enumerable.Empty<FieldInfo>();
		return type.GetFields(bindingFlags | BindingFlags.DeclaredOnly).Concat(GetAllFieldsFromType(type.BaseType, bindingFlags));
	}

	private void FindObjectsReferencingSelection()
	{
		// Ensure we clear the last results incase no matches are found
		m_matchListExact.Clear();
		m_matchListGameObject.Clear();
		m_matchListComponent.Clear();
		m_matchListChildGameObject.Clear();

		// Clear the current visit list
		m_visitedObjects.Clear();
		
		// Gather a list of child GameObjects
		List<GameObject> childGameObjectList = new List<GameObject>();
		if (m_testChildren && (m_gameObjectForSelection != null))
		{
			foreach (Transform transform in m_gameObjectForSelection.GetComponentsInChildren<Transform>(true))
			{
				if (transform == m_gameObjectForSelection.transform)
					continue;
				childGameObjectList.Add(transform.gameObject);
			}
		}
		GameObject[] childGameObjects = childGameObjectList.ToArray();

		// Find all components in the scene
		Component[] components = Resources.FindObjectsOfTypeAll(typeof(Component)) as Component[];

		// Pre-count the number of scene components for the progress bar
		int noofValidComponents = 0;
		for (int componentIndex = 0, noofComponents = components.Length; componentIndex < noofComponents; ++componentIndex)
		{
			Component component = components[componentIndex];
			if (!IsValidComponentForScan(component))
				continue;
			noofValidComponents++;
		}
		
		// Loop through all components
		int validComponentIndex = 0;
		for (int componentIndex = 0, noofComponents = components.Length; componentIndex < noofComponents; ++componentIndex)
		{
			Component component = components[componentIndex];
			if (!IsValidComponentForScan(component))
				continue;

			// Display the progress bar, checking for user cancel
			if (EditorUtility.DisplayCancelableProgressBar("Scanning...", "Scanning for references in the current scene", (validComponentIndex + 1) / (float)noofValidComponents))
				break;

			// Script assets need to behave differently, since there aren't exact references
			if (m_selectedObjectIsScript)
			{
				if (component.GetType() == m_selectedObjectType)
				{
					Reference reference = new Reference();
					reference.m_referringComponent = component;
					m_matchListExact.Add(reference);
				}
			}
			// A normal reference
			else
			{
				// Scan this component for references
				ScanObjectForReferences(component, component, childGameObjects, "");
			}

			validComponentIndex++;
		}

		// Clear the progress bar
		EditorUtility.ClearProgressBar();

		// Sort matches to make them easier to read
		m_matchListExact.Sort(new MatchComparer());
		m_matchListGameObject.Sort(new MatchComparer());
		m_matchListComponent.Sort(new MatchComparer());
		m_matchListChildGameObject.Sort(new MatchComparer());
	}

	// Scan this object for references
	private void ScanObjectForReferences(Component component, object scanObject, GameObject[] childGameObjects, string nestedMemberName)
	{
		// If we've already visited this object for this component, ignore it, so we don't get circular references and infinite loops
		try
		{
			if (m_visitedObjects.ContainsKey(component) && m_visitedObjects[component].Contains(scanObject))
				return;
		}
		catch
		{
			return;
		}

		// Add the scan object
		if (!m_visitedObjects.ContainsKey(component))
			m_visitedObjects[component] = new List<object>() { scanObject };
		else
			m_visitedObjects[component].Add(scanObject);

		// Cache the types of various objects
		Type gameObjectType = typeof(GameObject);
		Type componentType = typeof(Component);
		Type currentType = scanObject.GetType();

		// Loop through all fields of this object
		IEnumerable<FieldInfo> fields = GetAllFieldsFromType(currentType, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
		foreach (FieldInfo fieldInfo in fields)
		{
			// Specifically ignore certain elements, as Unity complains
			if (CullPropertyType(currentType, fieldInfo.Name))
				continue;

			// Handle arrays
			if (fieldInfo.FieldType.IsArray)
			{
				Array arrayValue = fieldInfo.GetValue(scanObject) as Array;
				if (arrayValue != null)
				{
					for (int arrayIndex = 0; arrayIndex < arrayValue.Length; ++arrayIndex)
					{
						int[] rankIndices = GetArrayRankIndicesAtIndex(arrayValue, arrayIndex);
						object arrayEntry = arrayValue.GetValue(rankIndices);
						Type entryType = (fieldInfo.FieldType.HasElementType) ? fieldInfo.FieldType.GetElementType() : arrayEntry.GetType();
						string entryName = fieldInfo.Name + "[" + arrayIndex.ToString() + "]";
						if (entryType.IsAssignableFrom(m_selectedObjectType) || gameObjectType.IsAssignableFrom(entryType) || componentType.IsAssignableFrom(entryType))
						{
							UnityEngine.Object testObject = arrayEntry as UnityEngine.Object;
							PerformMatch(component, testObject, nestedMemberName, entryName, fieldInfo.IsPublic, childGameObjects);
						}
						else if (entryType.IsClass && (entryType.FullName != "System.String") && (arrayEntry != null) && !arrayEntry.Equals(null))
						{
							if (string.IsNullOrEmpty(nestedMemberName))
								ScanObjectForReferences(component, arrayEntry, childGameObjects, entryName);
							else
								ScanObjectForReferences(component, arrayEntry, childGameObjects, nestedMemberName + "." + entryName);
						}
					}
				}
			}
			// Handle generic types, such as lists
			else if (fieldInfo.FieldType.IsGenericType)
			{
				IEnumerable genericValue = fieldInfo.GetValue(scanObject) as IEnumerable;
				if (genericValue != null)
				{
					int genericIndex = 0;
					foreach (object genericEntry in genericValue)
					{
						Type entryType = (fieldInfo.FieldType.HasElementType) ? fieldInfo.FieldType.GetElementType() : genericEntry.GetType();
						string entryName = fieldInfo.Name + "[" + genericIndex.ToString() + "]";
						if (entryType.IsAssignableFrom(m_selectedObjectType) || gameObjectType.IsAssignableFrom(entryType) || componentType.IsAssignableFrom(entryType))
						{
							UnityEngine.Object testObject = genericEntry as UnityEngine.Object;
							PerformMatch(component, testObject, nestedMemberName, entryName, fieldInfo.IsPublic, childGameObjects);
						}
						else if (entryType.IsClass && (entryType.FullName != "System.String") && (genericEntry != null) && !genericEntry.Equals(null))
						{
							if (string.IsNullOrEmpty(nestedMemberName))
								ScanObjectForReferences(component, genericEntry, childGameObjects, entryName);
							else
								ScanObjectForReferences(component, genericEntry, childGameObjects, nestedMemberName + "." + entryName);
						}
						genericIndex++;
					}
				}
			}
			// Any other type
			else
			{
				Type entryType = fieldInfo.FieldType;
				if (entryType.IsAssignableFrom(m_selectedObjectType) || gameObjectType.IsAssignableFrom(entryType) || componentType.IsAssignableFrom(entryType))
				{
					UnityEngine.Object testObject = fieldInfo.GetValue(scanObject) as UnityEngine.Object;
					PerformMatch(component, testObject, nestedMemberName, fieldInfo.Name, fieldInfo.IsPublic, childGameObjects);
				}
				else if (entryType.IsClass && (entryType.FullName != "System.String") && !entryType.FullName.Contains("System.Reflection"))
				{
					object genericEntry = fieldInfo.GetValue(scanObject);
					if ((genericEntry != null) && !genericEntry.Equals(null))
					{
						string entryName = fieldInfo.Name;
						if (string.IsNullOrEmpty(nestedMemberName))
							ScanObjectForReferences(component, genericEntry, childGameObjects, entryName);
						else
							ScanObjectForReferences(component, genericEntry, childGameObjects, nestedMemberName + "." + entryName);
					}
				}
			}
		}

		// Loop through all properties of this object
		PropertyInfo[] properties = currentType.GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			// Specifically ignore certain elements, as Unity complains
			if (CullPropertyType(currentType, propertyInfo.Name))
				continue;

			// Handle arrays
			if (propertyInfo.PropertyType.IsArray)
			{
				Array arrayValue = propertyInfo.GetValue(scanObject, null) as Array;
				if (arrayValue != null)
				{
					for (int arrayIndex = 0; arrayIndex < arrayValue.Length; ++arrayIndex)
					{
						int[] rankIndices = GetArrayRankIndicesAtIndex(arrayValue, arrayIndex);
						object arrayEntry = arrayValue.GetValue(rankIndices);
						Type entryType = (propertyInfo.PropertyType.HasElementType) ? propertyInfo.PropertyType.GetElementType() : arrayEntry.GetType();
						string entryName = propertyInfo.Name + "[" + arrayIndex.ToString() + "]";
						if (entryType.IsAssignableFrom(m_selectedObjectType) || gameObjectType.IsAssignableFrom(entryType) || componentType.IsAssignableFrom(entryType))
						{
							UnityEngine.Object testObject = arrayEntry as UnityEngine.Object;
							PerformMatch(component, testObject, nestedMemberName, entryName, propertyInfo.PropertyType.IsPublic, childGameObjects);
						}
						else if (entryType.IsClass && (entryType.FullName != "System.String") && (arrayEntry != null) && !arrayEntry.Equals(null))
						{
							if (string.IsNullOrEmpty(nestedMemberName))
								ScanObjectForReferences(component, arrayEntry, childGameObjects, entryName);
							else
								ScanObjectForReferences(component, arrayEntry, childGameObjects, nestedMemberName + "." + entryName);
						}
					}
				}
			}
			// Handle generic types, such as lists
			else if (propertyInfo.PropertyType.IsGenericType)
			{
				IEnumerable genericValue = propertyInfo.GetValue(scanObject, null) as IEnumerable;
				if (genericValue != null)
				{
					int genericIndex = 0;
					foreach (object genericEntry in genericValue)
					{
						Type entryType = (propertyInfo.PropertyType.HasElementType) ? propertyInfo.PropertyType.GetElementType() : genericEntry.GetType();
						string entryName = propertyInfo.Name + "[" + genericIndex.ToString() + "]";
						if (entryType.IsAssignableFrom(m_selectedObjectType) || gameObjectType.IsAssignableFrom(entryType) || componentType.IsAssignableFrom(entryType))
						{
							UnityEngine.Object testObject = genericEntry as UnityEngine.Object;
							PerformMatch(component, testObject, nestedMemberName, entryName, propertyInfo.PropertyType.IsPublic, childGameObjects);
						}
						else if (entryType.IsClass && (entryType.FullName != "System.String") && (genericEntry != null) && !genericEntry.Equals(null))
						{
							if (string.IsNullOrEmpty(nestedMemberName))
								ScanObjectForReferences(component, genericEntry, childGameObjects, entryName);
							else
								ScanObjectForReferences(component, genericEntry, childGameObjects, nestedMemberName + "." + entryName);
						}
						genericIndex++;
					}
				}
			}
			// Any other type, that doesn't take index parameters (which we cannot know)
			else if (propertyInfo.GetIndexParameters().Length == 0)
			{
				Type entryType = propertyInfo.PropertyType;
				if (entryType.IsAssignableFrom(m_selectedObjectType) || gameObjectType.IsAssignableFrom(entryType) || componentType.IsAssignableFrom(entryType))
				{
					// Test this object to see if it is referenced
					UnityEngine.Object testObject = propertyInfo.GetValue(scanObject, null) as UnityEngine.Object;
					PerformMatch(component, testObject, nestedMemberName, propertyInfo.Name, entryType.IsPublic, childGameObjects);
				}
				// Cannot handle indexed properties, since we don't know what values they require :(
				else if (entryType.IsClass && (entryType.FullName != "System.String") && !entryType.FullName.Contains("System.Reflection"))
				{
					object genericEntry = propertyInfo.GetValue(scanObject, null);
					if ((genericEntry != null) && !genericEntry.Equals(null))
					{
						string entryName = propertyInfo.Name;
						if (string.IsNullOrEmpty(nestedMemberName))
							ScanObjectForReferences(component, genericEntry, childGameObjects, entryName);
						else
							ScanObjectForReferences(component, genericEntry, childGameObjects, nestedMemberName + "." + entryName);
					}
				}
			}
		}

		// If the current type is an Animation, need to hack in clip lists
		if (currentType == typeof(Animation))
		{
			Animation animation = scanObject as Animation;
			int animIndex = 0;
			foreach (AnimationState animState in animation)
			{
				if (animState != null)
				{
					string entryName = "Animation[" + animIndex.ToString() + "]";
					if (string.IsNullOrEmpty(nestedMemberName))
						ScanObjectForReferences(component, animState, childGameObjects, entryName);
					else
						ScanObjectForReferences(component, animState, childGameObjects, nestedMemberName + "." + entryName);
				}
				animIndex++;
			}
		}
	}
	
	// Specifically ignore certain elements, as Unity complains
	private bool CullPropertyType(Type propertyType, string propertyName)
	{
		foreach (IgnoreProperties ignore in ms_ignoredProperties)
		{
			if ((ignore.m_type.IsAssignableFrom(propertyType)) && ignore.m_properties.Contains(propertyName))
				return true;
		}
		return false;
	}

	private void PerformMatch(Component component, UnityEngine.Object testObject, string nestedMemberName, string fieldName, bool publicField, GameObject[] childGameObjects)
	{
		// Exact match
		if (testObject == m_selectedObject)
		{
			// Test to see if we want references from children
			Component testComponent = testObject as Component;
			GameObject testGO = (testComponent != null) ? testComponent.gameObject : (testObject as GameObject);
			if (!m_ignoreInternalReferences || !component.IsParentOf(testGO))
			{
				Reference reference = new Reference();
				reference.m_referringComponent = component;
				reference.m_referringMember = string.IsNullOrEmpty(nestedMemberName) ? fieldName : (nestedMemberName + "." + fieldName);
				reference.m_referringFieldPublic = publicField;
				m_matchListExact.Add(reference);
			}
		}
		else
		{
			// Matches the game object the selection is associated with
			if ((m_selectedGameObject != null) && (testObject == m_selectedGameObject))
			{
				// Test to see if we want references from children
				Component testComponent = testObject as Component;
				GameObject testGO = (testComponent != null) ? testComponent.gameObject : (testObject as GameObject);
				if (!m_ignoreInternalReferences || !component.IsParentOf(testGO))
				{
					Reference reference = new Reference();
					reference.m_referringComponent = component;
					reference.m_referringMember = string.IsNullOrEmpty(nestedMemberName) ? fieldName : (nestedMemberName + "." + fieldName);
					reference.m_referringFieldPublic = publicField;
					m_matchListGameObject.Add(reference);
				}
			}

			// Check if it matches any component on the game object the selection is associated with
			if (m_selectedComponents != null)
			{
				int index = Array.IndexOf(m_selectedComponents, testObject);
				if ((index >= 0) && (m_selectedComponents[index] != component))		// Don't allow self-references
				{
					// Test to see if we want references from children
					Component testComponent = testObject as Component;
					GameObject testGO = (testComponent != null) ? testComponent.gameObject : (testObject as GameObject);
					if (!m_ignoreInternalReferences || !component.IsParentOf(testGO))
					{
						Reference reference = new Reference();
						reference.m_referringComponent = component;
						reference.m_referringMember = string.IsNullOrEmpty(nestedMemberName) ? fieldName : (nestedMemberName + "." + fieldName);
						reference.m_referringFieldPublic = publicField;
						reference.m_referredType = m_selectedComponents[index].GetType();
						m_matchListComponent.Add(reference);
					}
				}
			}
		}

		// If also matching children of the selection
		if (m_testChildren)
		{
			// Get the GameObject associated with the current test object - if one exists
			Component testComponent = testObject as Component;
			GameObject testGO = (testComponent != null) ? testComponent.gameObject : (testObject as GameObject);
			if ((testGO != null) && childGameObjects.Contains(testGO))
			{
				if (!m_ignoreInternalReferences || (!childGameObjects.Contains(component.gameObject) && (component.gameObject != m_gameObjectForSelection)))
				{
					Reference reference = new Reference();
					reference.m_referringComponent = component;
					reference.m_referringMember = string.IsNullOrEmpty(nestedMemberName) ? fieldName : (nestedMemberName + "." + fieldName);
					reference.m_referringFieldPublic = publicField;
					reference.m_referredType = (testComponent != null) ? testComponent.GetType() : null;
					reference.m_referredRootGameObject = m_gameObjectForSelection;
					reference.m_referredGameObject = testGO;
					m_matchListChildGameObject.Add(reference);
				}
			}
		}
	}
}
