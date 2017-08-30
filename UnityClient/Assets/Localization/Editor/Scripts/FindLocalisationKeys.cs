using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;


namespace Localization {
	namespace EditorScript {

		public class FindLocalisationKeys : EditorWindow
		{
			private struct Reference
			{
				public Component m_referringComponent;
				public string m_referringMember;
				public string m_referredKey;
				public bool m_referredKeyMissing;
				public Dictionary<SystemLanguage, string> m_translations;
				
				public static string HelpFormat()
				{
					return
						"Reference format is \"GameObject [Component.Member] -> \"Key\"\".\n" +
						"References displayed in bold are missing references, otherwise they are valid.\n" +
						"Click on a reference to select the object in the project hierarchy views. The tooltip for each reference shows the localisation strings for the key.\n";
				}

				public override string ToString()
				{
					if (m_referringComponent == null)
						return "";

					StringBuilder sb = new StringBuilder();
					sb.Append(m_referringComponent.gameObject.GetNameWithPath());
					sb.Append(" [");
					sb.Append(m_referringComponent.GetType().Name);
					sb.Append(".");
					sb.Append(m_referringMember);
					sb.Append("] -> \"");
					sb.Append(m_referredKey);
					sb.Append("\"");

					return sb.ToString();
				}

				public GUIContent ToGUIContent()
				{
					if ((m_referringComponent == null) || m_referredKeyMissing || (m_translations == null))
						return new GUIContent(ToString());

					StringBuilder builder = new StringBuilder();
					foreach (SystemLanguage language in m_translations.Keys)
						builder.AppendLine(string.Format("{0}: {1}\n", language.ToString(), m_translations[language]));
					string tooltip = builder.ToString();
					return new GUIContent(ToString(), tooltip);
				}

				public GUIStyle DisplayStyle()
				{
					GUIStyle displayStyle = new GUIStyle(EditorStyles.textField);
					displayStyle.alignment = TextAnchor.MiddleLeft;
					displayStyle.wordWrap = false;
					if (m_referredKeyMissing)
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
			
			
			private Vector2 m_scrollPositionMatch = Vector2.zero;
			private Vector2 m_scrollPositionUnused = Vector2.zero;
			[SerializeField] private List<Reference> m_matchList = new List<Reference>();
			[SerializeField] private List<string> m_keyList = new List<string>();
			[SerializeField] private List<string> m_unusedKeyList = new List<string>();
			private List<object> m_visitedObjects = new List<object>();
			private Localization.LocalisationData m_localisationData = null;

			
			[MenuItem("SPACEJAM/Localization/Find Localisation Keys", false, 100)]
			private static void OnSearchForReferences()
			{
				// Show the window with the selected object
				FindLocalisationKeys window = GetWindow<FindLocalisationKeys>(false, "Localise Keys", true);
				window.Show();
			}

			public void OnGUI()
			{
				EditorGUIUtility.labelWidth = 180.0f;

				// Allow key lists to be reset for speed
				if (GUILayout.Button("Reset to Unscanned"))
				{
					m_keyList.Clear();
					m_matchList.Clear();
					m_unusedKeyList.Clear();
				}
					

				// Find references for localisation keys
				if (GUILayout.Button("Find Localisation Keys"))
					FindLocalisationReferences();

				EditorGUILayout.Space();

				// Display help text to make it easier to use
				EditorGUILayout.HelpBox(Reference.HelpFormat(), MessageType.Info);

				// Show references to localisation keys
				if (m_matchList.Count > 0)
				{
					GUILayout.Label("References to localisation keys:");
					m_scrollPositionMatch = GUILayout.BeginScrollView(m_scrollPositionMatch, GUILayout.ExpandHeight(true));
					EditorHelpers.StartBox();
					foreach (Reference reference in m_matchList)
					{
						if (GUILayout.Button(reference.ToGUIContent(), reference.DisplayStyle(), GUILayout.ExpandWidth(true)))
						{
							Selection.activeObject = reference.m_referringComponent;
							EditorGUIUtility.PingObject(reference.m_referringComponent);
						}
					}
					EditorHelpers.EndBox();
					GUILayout.EndScrollView();
				}

				// Show unused localisation keys
				if (m_unusedKeyList.Count > 0)
				{
					GUILayout.Label("Unused localisation keys:");
					m_scrollPositionUnused = GUILayout.BeginScrollView(m_scrollPositionUnused, GUILayout.ExpandHeight(true));
					EditorHelpers.StartBox();
					foreach (string key in m_unusedKeyList)
					{
						GUIStyle displayStyle = new GUIStyle(EditorStyles.textField);
						displayStyle.alignment = TextAnchor.MiddleLeft;
						displayStyle.wordWrap = false;

						if (m_localisationData != null)
						{
							StringBuilder builder = new StringBuilder();
							foreach (SystemLanguage language in m_localisationData.m_translations.Keys)
							{
								Dictionary<string, string> languageIDs = m_localisationData.m_translations[language];
								builder.AppendLine(string.Format("{0}: {1}\n", language.ToString(), languageIDs[key]));
							}
							string tooltip = builder.ToString();
							
							GUILayout.Label(new GUIContent(key, tooltip), displayStyle, GUILayout.ExpandWidth(true));
						}
						else
						{
							GUILayout.Label(key, displayStyle, GUILayout.ExpandWidth(true));
						}
					}
					EditorHelpers.EndBox();
					GUILayout.EndScrollView();
					
					// Copy to clipboard
					if (GUILayout.Button("Copy Unused Keys"))
						EditorHelpers.clipBoard = string.Join("\n", m_unusedKeyList.ToArray());
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

			// Specifically ignore certain elements, as Unity complains about memory leaks on internal instantiation
			private bool IsCulledProperty(Type propertyType, string propertyName)
			{
				foreach (IgnoreProperties ignore in ms_ignoredProperties)
				{
					if ((ignore.m_type.IsAssignableFrom(propertyType)) && ignore.m_properties.Contains(propertyName))
						return true;
				}
				return false;
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

			private void FindLocalisationReferences()
			{
				// Ensure we clear the last results incase no matches are found
				m_matchList.Clear();

				// Clear the current visit list
				m_visitedObjects.Clear();

				// Get up to date localisation data
				m_localisationData = Localization.UpdateLocalisationData();
				
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
					if (EditorUtility.DisplayCancelableProgressBar("Scanning...", "Scanning for localisation keys in the current scene", (validComponentIndex + 1) / (float)noofValidComponents))
						break;

					// Scan this component for references
					ScanObjectForReferences(component, component, "");
					validComponentIndex++;
				}

				// Clear the progress bar
				EditorUtility.ClearProgressBar();

				// Sort matches to make them easier to read
				m_matchList.Sort(new MatchComparer());

				// Update the unused keys
				if (m_localisationData != null)
				{
					m_unusedKeyList = new List<string>(m_localisationData.m_editorKeys);
					foreach (string key in m_keyList)
						m_unusedKeyList.Remove(key);
					m_unusedKeyList.Remove("");
				}
				else
				{
					m_unusedKeyList.Clear();
				}
			}

			// Scan this object for references
			private void ScanObjectForReferences(Component component, object scanObject, string nestedMemberName)
			{
				// If we've already visited this object, ignore it, so we don't get circular references and infinite loops
				try
				{
					if (m_visitedObjects.Contains(scanObject))
						return;
				}
				catch
				{
					return;
				}

				// Add the scan object
				m_visitedObjects.Add(scanObject);

				// Cache the types to check
				Type currentType = scanObject.GetType();
				Type stringType = typeof(string);
				Type localisationKeyAttributeType = typeof(LocalizationKeyAttribute);
				
				// Loop through all fields of this object
				IEnumerable<FieldInfo> fields = GetAllFieldsFromType(currentType, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				foreach (FieldInfo fieldInfo in fields)
				{
					if (IsCulledProperty(currentType, fieldInfo.Name))
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
								if (stringType.IsAssignableFrom(entryType))
								{
									object[] attributes = fieldInfo.GetCustomAttributes(localisationKeyAttributeType, false);
									if (attributes.Length > 0)
									{
										string testKey = arrayEntry as string;
										PerformKeyCheck(component, nestedMemberName, entryName, testKey);
									}
								}
								else if (entryType.IsClass && (arrayEntry != null) && !arrayEntry.Equals(null))
								{
									if (string.IsNullOrEmpty(nestedMemberName))
										ScanObjectForReferences(component, arrayEntry, entryName);
									else
										ScanObjectForReferences(component, arrayEntry, nestedMemberName + "." + entryName);
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
								if (stringType.IsAssignableFrom(entryType))
								{
									object[] attributes = fieldInfo.GetCustomAttributes(localisationKeyAttributeType, false);
									if (attributes.Length > 0)
									{
										string testKey = genericEntry as string;
										PerformKeyCheck(component, nestedMemberName, entryName, testKey);
									}
								}
								else if (entryType.IsClass && (genericEntry != null) && !genericEntry.Equals(null))
								{
									if (string.IsNullOrEmpty(nestedMemberName))
										ScanObjectForReferences(component, genericEntry, entryName);
									else
										ScanObjectForReferences(component, genericEntry, nestedMemberName + "." + entryName);
								}
								genericIndex++;
							}
						}
					}
					// Any other type
					else
					{
						Type entryType = fieldInfo.FieldType;
						string entryName = fieldInfo.Name;
						if (stringType.IsAssignableFrom(entryType))
						{
							object[] attributes = fieldInfo.GetCustomAttributes(localisationKeyAttributeType, false);
							if (attributes.Length > 0)
							{
								string testKey = fieldInfo.GetValue(scanObject) as string;
								PerformKeyCheck(component, nestedMemberName, entryName, testKey);
							}
						}
						else if (entryType.IsClass && !entryType.FullName.Contains("System.Reflection"))
						{
							object genericEntry = fieldInfo.GetValue(scanObject);
							if ((genericEntry != null) && !genericEntry.Equals(null))
							{
								if (string.IsNullOrEmpty(nestedMemberName))
									ScanObjectForReferences(component, genericEntry, entryName);
								else
									ScanObjectForReferences(component, genericEntry, nestedMemberName + "." + entryName);
							}
						}
					}
				}

				// Loop through all properties of this object
				PropertyInfo[] properties = currentType.GetProperties();
				foreach (PropertyInfo propertyInfo in properties)
				{
					if (IsCulledProperty(currentType, propertyInfo.Name))
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
								if (stringType.IsAssignableFrom(entryType))
								{
									object[] attributes = propertyInfo.GetCustomAttributes(localisationKeyAttributeType, false);
									if (attributes.Length > 0)
									{
										string testKey = arrayEntry as string;
										PerformKeyCheck(component, nestedMemberName, entryName, testKey);
									}
								}
								else if (entryType.IsClass && (arrayEntry != null) && !arrayEntry.Equals(null))
								{
									if (string.IsNullOrEmpty(nestedMemberName))
										ScanObjectForReferences(component, arrayEntry, entryName);
									else
										ScanObjectForReferences(component, arrayEntry, nestedMemberName + "." + entryName);
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
								if (stringType.IsAssignableFrom(entryType))
								{
									object[] attributes = propertyInfo.GetCustomAttributes(localisationKeyAttributeType, false);
									if (attributes.Length > 0)
									{
										string testKey = genericEntry as string;
										PerformKeyCheck(component, nestedMemberName, entryName, testKey);
									}
								}
								else if (entryType.IsClass && (genericEntry != null) && !genericEntry.Equals(null))
								{
									if (string.IsNullOrEmpty(nestedMemberName))
										ScanObjectForReferences(component, genericEntry, entryName);
									else
										ScanObjectForReferences(component, genericEntry, nestedMemberName + "." + entryName);
								}
								genericIndex++;
							}
						}
					}
					// Any other type, that doesn't take index parameters (which we cannot know)
					else if (propertyInfo.GetIndexParameters().Length == 0)
					{
						Type entryType = propertyInfo.PropertyType;
						if (stringType.IsAssignableFrom(entryType))
						{
							object[] attributes = propertyInfo.GetCustomAttributes(localisationKeyAttributeType, false);
							if (attributes.Length > 0)
							{
								string entryName = propertyInfo.Name;
								string testKey = propertyInfo.GetValue(scanObject, null) as string;
								PerformKeyCheck(component, nestedMemberName, entryName, testKey);
							}
						}
						// Cannot handle indexed properties, since we don't know what values they require :(
						else if (entryType.IsClass && !entryType.FullName.Contains("System.Reflection"))
						{
							object genericEntry = propertyInfo.GetValue(scanObject, null);
							if ((genericEntry != null) && !genericEntry.Equals(null))
							{
								string entryName = propertyInfo.Name;
								if (string.IsNullOrEmpty(nestedMemberName))
									ScanObjectForReferences(component, genericEntry, entryName);
								else
									ScanObjectForReferences(component, genericEntry, nestedMemberName + "." + entryName);
							}
						}
					}
				}
			}
			
			private void PerformKeyCheck(Component component, string nestedMemberName, string fieldName, string key)
			{
				Reference reference;
				reference.m_referringComponent = component;
				if (string.IsNullOrEmpty(nestedMemberName))
					reference.m_referringMember = fieldName;
				else
					reference.m_referringMember = nestedMemberName + "." + fieldName;
				reference.m_referredKey = key;
				reference.m_referredKeyMissing = (m_localisationData == null) || string.IsNullOrEmpty(key) || !m_localisationData.m_editorKeys.Contains(key);
				if (reference.m_referredKeyMissing)
				{
					reference.m_translations = null;
				}
				else
				{
					reference.m_translations = new Dictionary<SystemLanguage, string>();
					foreach (SystemLanguage language in m_localisationData.m_translations.Keys)
						reference.m_translations[language] = m_localisationData.m_translations[language][key];
				}
				m_matchList.Add(reference);
				if (!m_keyList.Contains(key))
					m_keyList.Add(key);
			}
		}

	}   // namespace EditorScript
}   // namespace Localization
