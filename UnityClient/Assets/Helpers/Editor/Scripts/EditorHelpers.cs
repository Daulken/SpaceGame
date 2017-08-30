using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// Generic Editor helper functions
public static class EditorHelpers
{
	/// <summary>
	/// Add a separator line
	/// </summary>
	public static void SeparatorLine()
	{
		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
	}

	// ------------------------------------------------------------------

	/// <summary>
	/// Helper function to increase an indent
	/// </summary>
	public static void IncreaseIndent()
	{
#if UNITY_4_3
		EditorGUI.indentLevel++;
#else
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(25);
		EditorGUILayout.BeginVertical();
#endif
	}

	/// <summary>
	/// Helper function to decrease an indent
	/// </summary>
	public static void DecreaseIndent()
	{
#if UNITY_4_3
		EditorGUI.indentLevel--;
#else
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
#endif
	}

	// ------------------------------------------------------------------

	/// <summary>
	/// Helper function to start boxing further controls
	/// </summary>
	public static void StartBox()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(3.0f);
		EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(10.0f));
		GUILayout.BeginVertical();
		GUILayout.Space(2.0f);
	}
	
	/// <summary>
	/// Helper function to start boxing further controls, with a collapsible title
	/// </summary>
	public static bool StartBox(string title, ref bool expanded)
	{
		// Show the title for this box
		GUILayout.Space(3.0f);
		if (!expanded)
			GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
		GUILayout.BeginHorizontal();
		GUILayout.Space(3.0f);
		if (!GUILayout.Toggle(true, "<b><size=11>" + title + "</size></b>", "dragtab"))
			expanded = !expanded;
		GUILayout.Space(2.0f);
		GUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
		if (!expanded)
			GUILayout.Space(3.0f);

		// If expanded, start a new box, and return open
		if (expanded)
		{
			StartBox();
			return true;
		}

		// Otherwise return closed
		return false;
	}

	/// <summary>
	/// Helper function to start boxing further controls, with a collapsible title
	/// </summary>
	public static bool StartBox(string title, string key)
	{
		// Get the current state
		bool expanded = EditorPrefs.GetBool(key, true);
	
		// Show the title for this box
		GUILayout.Space(3.0f);
		if (!expanded)
			GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
		GUILayout.BeginHorizontal();
		GUILayout.Space(3.0f);
		if (!GUILayout.Toggle(true, "<b><size=11>" + title + "</size></b>", "dragtab"))
			expanded = !expanded;
		GUILayout.Space(2.0f);
		GUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
		if (!expanded)
			GUILayout.Space(3.0f);

		// Set the new state
		EditorPrefs.SetBool(key, expanded);

		// If expanded, start a new box, and return open
		if (expanded)
		{
			StartBox();
			return true;
		}

		// Otherwise return closed
		return false;
	}
	
	/// <summary>
	/// Helper function to start boxing further controls, with a collapsable title
	/// </summary>
	public static bool StartBox(string title, SerializedProperty property)
	{
		// Check for a valid property
		if (property == null)
			return false;

		// Display the box
		bool isExpanded = property.isExpanded;
		bool isOpen = StartBox(title, ref isExpanded);

		// Update the expand state
		property.isExpanded = isExpanded;

		// Return whether open
		return isOpen;
	}
	
	/// <summary>
	/// Helper function to stop boxing further controls
	/// </summary>
	public static void EndBox()
	{
		GUILayout.Space(3.0f);
		GUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(3.0f);
		GUILayout.EndHorizontal();
		GUILayout.Space(3.0f);
	}

	// ------------------------------------------------------------------

	/// <summary>
	/// Helper function to display a Vector3 editing GUI that mimics the stock Transform position editor
	/// </summary>
	public static void Vector3WithReset(string resetText, ref Vector3 value, Vector3 resetValue)
	{
		// Align vector controls horizontally
		EditorGUILayout.BeginHorizontal();

		// Add a reset button, querying whether pressed
		bool resetVector = GUILayout.Button(resetText, GUILayout.ExpandWidth(false));

		// Add the vector values
		EditorGUIUtility.labelWidth = 15.0f;
		value.x = EditorGUILayout.FloatField("X", value.x, GUILayout.ExpandWidth(true));
		value.y = EditorGUILayout.FloatField("Y", value.y, GUILayout.ExpandWidth(true));
		value.z = EditorGUILayout.FloatField("Z", value.z, GUILayout.ExpandWidth(true));
		EditorGUIUtility.labelWidth = 0.0f;

		// Stop aligning
		EditorGUILayout.EndHorizontal();
		
		// If a reset is to be performed, do so
		if (resetVector)
			value = resetValue;
	}

	/// <summary>
	/// Helper function to display a Quaternion editing GUI that mimics the stock Transform rotation editor
	/// </summary>
	public static void QuaternionWithReset(string resetText, ref Quaternion value, Quaternion resetValue)
	{
		// Align vector controls horizontally
		EditorGUILayout.BeginHorizontal();

		// Add a reset button, querying whether pressed
		bool resetVector = GUILayout.Button(resetText, GUILayout.ExpandWidth(false));
		
		// Add the vector euler values for this rotation
		Vector3 eulerAngles = value.eulerAngles;
		EditorGUIUtility.labelWidth = 15.0f;
		eulerAngles.x = EditorGUILayout.FloatField("X", eulerAngles.x, GUILayout.ExpandWidth(true));
		eulerAngles.y = EditorGUILayout.FloatField("Y", eulerAngles.y, GUILayout.ExpandWidth(true));
		eulerAngles.z = EditorGUILayout.FloatField("Z", eulerAngles.z, GUILayout.ExpandWidth(true));
		EditorGUIUtility.labelWidth = 0.0f;

		// Stop aligning
		EditorGUILayout.EndHorizontal();
		
		// If a reset is to be performed, do so
		if (resetVector)
			value = resetValue;
		else if (GUI.changed)
			value = Quaternion.Euler(eulerAngles);
	}
	
	/// <summary>
	/// Helper function to display a Transform editing GUI that mimics the stock Transform editor
	/// </summary>
	public static void TransformWithReset(Transform value, ref bool showContents, string contentName)
	{
		// Get the built-in Transform icon
		Texture editorIcon = EditorGUIUtility.ObjectContent(null, typeof(Transform)).image;

		// Add a foldout of the given List-type variable
		EditorGUIUtility.labelWidth = 350.0f;
		GUIStyle customFoldout = new GUIStyle(EditorStyles.foldout);
		customFoldout.fixedHeight = 20.0f;
		showContents = EditorGUILayout.Foldout(showContents, new GUIContent("      " + contentName, editorIcon), customFoldout);
		EditorGUIUtility.labelWidth = 0.0f;
		if (showContents)
		{
			// Align Transform controls vertically
			EditorGUILayout.BeginVertical();
			
			// Get the values to edit
			Vector3 position = value.localPosition;
			Quaternion rotation = value.localRotation;
			Vector3 scale = value.localScale;
	
			// Show the position, rotation and scale editors
			Vector3WithReset("P", ref position, Vector3.zero);
			QuaternionWithReset("R", ref rotation, Quaternion.identity);
			Vector3WithReset("S", ref scale, Vector3.one);
	
			// If changed, set the new values
			if (GUI.changed)
			{
				value.localPosition = position;
				value.localRotation = rotation;
				value.localScale = scale;
			}
	
			// Stop aligning
			EditorGUILayout.EndVertical();
		}
	}

	// ------------------------------------------------------------------

	/// <summary>
	/// Ensure array data is the correct size for the enum
	/// </summary>
	public static void EnsurePropertyArraySize(SerializedProperty property, int size)
	{
		if ((property == null) || !property.isArray)
			return;

		// Ensure the array is of the correct size
		bool changed = false;
		while (size > property.arraySize)
		{
			property.InsertArrayElementAtIndex(property.arraySize);
			changed = true;
		}
		while (size < property.arraySize)
		{
			property.DeleteArrayElementAtIndex(property.arraySize - 1);
			changed = true;
		}
		if (changed)
			property.serializedObject.ApplyModifiedProperties();
	}

	/// <summary>
	/// Get the parent serialized property for a give serialized property. Returns null if not a child property.
	/// </summary>
	public static SerializedProperty GetParentSerializedProperty(SerializedProperty property)
	{
		// Initialise search values
		SerializedObject owner = property.serializedObject;
		SerializedProperty arrayProperty = null;

		// Iterate through all but the final variable
		string path = property.propertyPath.Replace(".Array.data[", "[");
		string[] elements = path.Split('.');
		for (int elementIndex = 0; elementIndex < elements.Length - 1; ++elementIndex)
		{
			string element = elements[elementIndex];

			if (element.Contains("["))
			{
				string elementName = element.Substring(0, element.IndexOf("["));
				int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));

				SerializedProperty childProperty = (arrayProperty == null) ? owner.FindProperty(elementName) : arrayProperty.FindPropertyRelative(elementName);
				arrayProperty = childProperty.GetArrayElementAtIndex(index);
			}
			else
			{
				arrayProperty = (arrayProperty == null) ? owner.FindProperty(element) : arrayProperty.FindPropertyRelative(element);
			}
		}

		// We should now be on the parent property, unless this last element is also an array, in which case
		// we should get the parent array variable property, rather than the specific child index one.
		string lastElement = elements[elements.Length - 1];
		if (lastElement.Contains("["))
		{
			string lastElementName = lastElement.Substring(0, lastElement.IndexOf("["));
			arrayProperty = (arrayProperty == null) ? owner.FindProperty(lastElementName) : arrayProperty.FindPropertyRelative(lastElementName);
		}

		// Return the found property
		return arrayProperty;
	}

	// Get the variable associated with a serialized property
	private static System.Object GetObjectAssociatedWithProperty(SerializedProperty property)
	{
		// Initialise search values
		System.Object foundObject = property.serializedObject.targetObject;
		if (foundObject == null)
			return null;

		// Iterate through all but the final variable
		string path = property.propertyPath.Replace(".Array.data[", "[");
		string[] elements = path.Split('.');
		foreach (string element in elements)
		{
			if (element.Contains("["))
			{
				string elementName = element.Substring(0, element.IndexOf("["));
				int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));

				// If this is a field, get the parent enumerable value from the field
				IEnumerable enumerable = null;
				System.Type type = foundObject.GetType();
				FieldInfo fieldInfo = type.GetField(elementName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if (fieldInfo != null)
				{
					enumerable = fieldInfo.GetValue(foundObject) as IEnumerable;
				}
				// Otherwise if this is a property, get the parent enumerable value from the property
				else
				{
					PropertyInfo propertyInfo = type.GetProperty(elementName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
					if (propertyInfo == null)
						return null;
					enumerable = propertyInfo.GetValue(foundObject, null) as IEnumerable;
				}

				// Iterate through the number of indices to get to the correct child
				IEnumerator enumerator = enumerable.GetEnumerator();
				while (index-- >= 0)
					enumerator.MoveNext();
				foundObject = enumerator.Current;
			}
			else
			{
				// If this is a field, get the value from the field
				System.Type type = foundObject.GetType();
				FieldInfo fieldInfo = type.GetField(element, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if (fieldInfo != null)
				{
					foundObject = fieldInfo.GetValue(foundObject);
				}
				// Otherwise if this is a property, get the value from the property
				else
				{
					PropertyInfo propertyInfo = type.GetProperty(element, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
					if (propertyInfo == null)
						return null;
					foundObject = propertyInfo.GetValue(foundObject, null);
				}
			}
		}
		return foundObject;
	}
	
	// Set the variable associated with a serialized property
	private static bool SetObjectAssociatedWithProperty(SerializedProperty property, System.Object newObject)
	{
		// Initialise search values
		System.Object foundObject = property.serializedObject.targetObject;
		if (foundObject == null)
			return false;

		// Iterate through all but the final variable
		string path = property.propertyPath.Replace(".Array.data[", "[");
		string[] elements = path.Split('.');
		int maxIndex = elements.Length - 1;
		for (int elementIndex = 0; elementIndex < elements.Length; ++elementIndex)
		{
			string element = elements[elementIndex];
			if (element.Contains("["))
			{
				string elementName = element.Substring(0, element.IndexOf("["));
				int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));

				// If this is a field, get the parent enumerable value from the field
				IList listEntry = null;
				System.Type type = foundObject.GetType();
				FieldInfo fieldInfo = type.GetField(elementName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if (fieldInfo != null)
				{
					listEntry = fieldInfo.GetValue(foundObject) as IList;
				}
				// Otherwise if this is a property, get the parent enumerable value from the property
				else
				{
					PropertyInfo propertyInfo = type.GetProperty(elementName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
					if (propertyInfo == null)
						return false;
					listEntry = propertyInfo.GetValue(foundObject, null) as IList;
				}

				// Iterate through the number of indices to get to the correct child
				if (elementIndex == maxIndex)
					listEntry[index] = newObject;
				else
					foundObject = listEntry[index];
			}
			else
			{
				// If this is a field, get the value from the field
				System.Type type = foundObject.GetType();
				FieldInfo fieldInfo = type.GetField(element, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if (fieldInfo != null)
				{
					if (elementIndex == maxIndex)
						fieldInfo.SetValue(foundObject, newObject);
					else
						foundObject = fieldInfo.GetValue(foundObject);
				}
				// Otherwise if this is a property, get the value from the property
				else
				{
					PropertyInfo propertyInfo = type.GetProperty(element, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
					if (propertyInfo == null)
						return false;
					if (elementIndex == maxIndex)
						propertyInfo.SetValue(foundObject, newObject, null);
					else
						foundObject = propertyInfo.GetValue(foundObject, null);
				}
			}
		}

		return true;
	}
	
	/// <summary>
	/// Templated function to allow generic list variables to create their own controls
	/// </summary>
	public static void GenericListEditorGUI<T>(SerializedProperty serializedProperty, string contentName, string helpText) where T: new()
	{
		// Check that this is an array property
		if (!serializedProperty.isArray)
		{
			EditorGUILayout.HelpBox("Property '" + serializedProperty.name + "' is not an array type", MessageType.Error);
			return;
		}

		// Get the owning object from the serialized property using reflection
		System.Object associatedVariable = GetObjectAssociatedWithProperty(serializedProperty);
		List<T> listVariable = associatedVariable as List<T>;
		T[] arrayVariable = associatedVariable as T[];
		if ((listVariable == null) && (arrayVariable == null))
		{
			EditorGUILayout.HelpBox("Property '" + serializedProperty.name + "' is not pointing to a List<T> or T[] object", MessageType.Error);
			return;
		}
		
		// Get the type of the template parameter T, since we use this a lot
		System.Type typeofT = typeof(T);
		
		// Add a foldout box of the given List-type variable
		if (StartBox(contentName, serializedProperty))
		{
			// Show help text if there is any
			if (!string.IsNullOrEmpty(helpText))
				EditorGUILayout.HelpBox(helpText, MessageType.Info);
			
			// Go through each entry in the list
			int maxIndex = serializedProperty.arraySize - 1;
			for (int rowIndex = 0; rowIndex < serializedProperty.arraySize; ++rowIndex)
			{
				// Add a horizontal row of editing buttons for this entry
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(3.0f);
				if (GUILayout.Button(String.Format("Remove {0}", rowIndex), GUILayout.ExpandWidth(false)))
				{
					// Move array elements down, and delete the last. We can't use DeleteArrayElementAtIndex, since the
					// generic list could be a reference type, in which case the entry if null'd rather than deleted,
					// before then allowing the entry to be deleted the NEXT time the element is deleted. Confusing!
					for (int shuffleIndex = rowIndex + 1; shuffleIndex < serializedProperty.arraySize; ++shuffleIndex)
					{
						bool isExpanded = serializedProperty.GetArrayElementAtIndex(shuffleIndex).isExpanded;
						serializedProperty.MoveArrayElement(shuffleIndex, shuffleIndex - 1);
						serializedProperty.GetArrayElementAtIndex(shuffleIndex - 1).isExpanded = isExpanded;
					}
					serializedProperty.arraySize--;
					break;
				}
				if (rowIndex > 0)
				{
					if (GUILayout.Button(String.Format("Move {0} Up", rowIndex), GUILayout.ExpandWidth(false)))
					{
						int swapIndex = (Event.current.shift) ? 0 : (rowIndex - 1);
						bool isExpandedRow = serializedProperty.GetArrayElementAtIndex(rowIndex).isExpanded;
						bool isExpandedSwap = serializedProperty.GetArrayElementAtIndex(swapIndex).isExpanded;
						serializedProperty.MoveArrayElement(rowIndex, swapIndex);
						serializedProperty.GetArrayElementAtIndex(rowIndex).isExpanded = isExpandedSwap;
						serializedProperty.GetArrayElementAtIndex(swapIndex).isExpanded = isExpandedRow;
						break;
					}
				}
				if (rowIndex < maxIndex)
				{
					if (GUILayout.Button(String.Format("Move {0} Down", rowIndex), GUILayout.ExpandWidth(false)))
					{
						int swapIndex = (Event.current.shift) ? maxIndex : (rowIndex + 1);
						bool isExpandedRow = serializedProperty.GetArrayElementAtIndex(rowIndex).isExpanded;
						bool isExpandedSwap = serializedProperty.GetArrayElementAtIndex(swapIndex).isExpanded;
						serializedProperty.MoveArrayElement(rowIndex, swapIndex);
						serializedProperty.GetArrayElementAtIndex(rowIndex).isExpanded = isExpandedSwap;
						serializedProperty.GetArrayElementAtIndex(swapIndex).isExpanded = isExpandedRow;
						break;
					}
				}
				EditorGUILayout.EndHorizontal();

				// Get the property for this row
				SerializedProperty rowProperty = serializedProperty.GetArrayElementAtIndex(rowIndex);
				
				// Show a header for this row
				if (StartBox("Element " + rowIndex.ToString(), rowProperty))
				{
					// Display the controls for this row, depending on whether simple or complex array type
					FieldInfo[] info = typeofT.GetFields();
					if (info.Length != 0)
					{
						// Skip array size property
						rowProperty.Next(true);

						// Get the number of visible children
						int noofVisibleChildren = info.Length;

						// Display children
						for (int fieldIndex = 0; fieldIndex < noofVisibleChildren; ++fieldIndex)
						{
							HideInInspector[] hideAttributes = info[fieldIndex].GetCustomAttributes(typeof(HideInInspector), true) as HideInInspector[];
							if (hideAttributes.Length > 0)
								noofVisibleChildren--;
							else
								EditorGUILayout.PropertyField(rowProperty, true);
							if (!rowProperty.NextVisible(false))
								break;
						}
					}
					else
					{
						EditorGUILayout.PropertyField(rowProperty, new GUIContent(""), true);
					}
	
					// Stop boxing the entry
					EndBox();
				}
			}

			// Add a 'add' button at the bottom to add a new row
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(3.0f);
			if (GUILayout.Button("Add Row"))
			{
				// Construct and assign a new entry, depending on whether simple or complex array type
				FieldInfo[] info = typeofT.GetFields();
				T newEntry = default(T);
				if ((info.Length != 0) && !typeof(MonoBehaviour).IsAssignableFrom(typeofT) && !typeof(GameObject).IsAssignableFrom(typeofT))
					newEntry = new T();
				if (listVariable != null)
				{
					listVariable.Add(newEntry);
				}
				else
				{
					// Allocate new array with a greater size
					T[] newEntries = new T[arrayVariable.Length + 1];
			
					// Copy over existing entries that fit, and initialize new entries
					for (int rowIndex = 0; rowIndex < arrayVariable.Length; ++rowIndex)
						newEntries[rowIndex] = arrayVariable[rowIndex];
					newEntries[arrayVariable.Length] = newEntry;
					SetObjectAssociatedWithProperty(serializedProperty, newEntries);
				}
			}
			EditorGUILayout.EndHorizontal();

			// Stop boxing list
			EndBox();
		}
	}

	// ------------------------------------------------------------------

	/// <summary>
	/// Render a bitmask field for given names and values
	/// </summary>
	public static int BitMaskField(GUIContent label, string[] itemNames, int[] itemValues, int maskValue, params GUILayoutOption[] options)
	{
		// Get a valid mask value from the input mask, constrained to the enum value range
		int validInputMaskValue = 0;
		for (int itemIndex = 0; itemIndex < itemValues.Length; ++itemIndex)
		{
			if (itemValues[itemIndex] != 0)
			{
				if ((maskValue & itemValues[itemIndex]) == itemValues[itemIndex])
					validInputMaskValue |= 1 << itemIndex;
			}
			else if (maskValue == 0)
			{
				validInputMaskValue |= 1 << itemIndex;
			}
		}

		// Display a mask field, storing any new value selection
		int newMaskVal = EditorGUILayout.MaskField(label, validInputMaskValue, itemNames, options);

		// Check for any differences in value
		int maskChanges = validInputMaskValue ^ newMaskVal;

		// Construct a new value mask from the input mask, allowing for keeping any values outside of the constrained value range
		int newValue = maskValue;
		for (int itemIndex = 0; itemIndex < itemValues.Length; ++itemIndex)
		{
			// If not changed, ignore this index
			if ((maskChanges & (1 << itemIndex)) == 0)
				continue;

			// Has the value been removed?
			if ((newMaskVal & (1 << itemIndex)) == 0)
			{
				newValue &= ~itemValues[itemIndex];
				continue;
			}

			// Special case: if "0" is set, just set the val to 0
			if (itemValues[itemIndex] == 0)
			{
				newValue = 0;
				break;
			}

			// Add this item on
			newValue |= itemValues[itemIndex];
		}

		// Return the new value
		return newValue;
	}
	
	/// <summary>
	/// Render a bitmask field for an enum
	/// </summary>
	public static int BitMaskField(GUIContent label, System.Type enumType, int maskValue, params GUILayoutOption[] options)
	{
		// If this isn't an enum type, don't display anything, and leave the value unchanged
		if (!enumType.IsEnum)
			return maskValue;

		// Get the enum names and values
		string[] itemNames = System.Enum.GetNames(enumType);
		int[] itemValues = System.Enum.GetValues(enumType) as int[];
		
		// Return the selected value of the bitmask field with these names and values
		return BitMaskField(label, itemNames, itemValues, maskValue, options);
	}
	
	/// <summary>
	/// Render a bitmask field for an enum
	/// </summary>
	public static int BitMaskField<T>(GUIContent label, int maskValue, params GUILayoutOption[] options) where T : struct, IComparable, IConvertible, IFormattable
	{
		return BitMaskField(label, typeof(T), maskValue, options);
	}

	// ------------------------------------------------------------------

	/// <summary>
	/// Render a position handle, but with a custom scale
	/// </summary>
    public static Vector3 ScaledPositionHandle(float scale, Vector3 position, Quaternion rotation)
    {
		float snapX = EditorPrefs.GetFloat("MoveSnapX");
		float snapY = EditorPrefs.GetFloat("MoveSnapY");
		float snapZ = EditorPrefs.GetFloat("MoveSnapZ");
	    float handleSize = HandleUtility.GetHandleSize(position) * scale;
	    Color color = Handles.color;

	    Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.93f);
	    position = Handles.Slider(position, rotation * Vector3.right, handleSize, new Handles.CapFunction(Handles.ArrowHandleCap), snapX);

	    Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.93f);
	    position = Handles.Slider(position, rotation * Vector3.up, handleSize, new Handles.CapFunction(Handles.ArrowHandleCap), snapY);

	    Handles.color = new Color(0.0f, 0.0f, 1.0f, 0.93f);
	    position = Handles.Slider(position, rotation * Vector3.forward, handleSize, new Handles.CapFunction(Handles.ArrowHandleCap), snapZ);

	    Handles.color = new Color(0.8f, 0.8f, 0.8f, 0.93f);
	    position = Handles.FreeMoveHandle(position, rotation, handleSize * 0.15f, new Vector3(snapX, snapY, snapZ), new Handles.CapFunction(Handles.RectangleHandleCap));

	    Handles.color = color;
	    return position;
    }	

	/// <summary>
	/// Render a rotation handle, but with a custom scale
	/// </summary>
	public static Quaternion ScaledRotationHandle(float scale, Quaternion rotation, Vector3 position)
	{
		float snapRotate = EditorPrefs.GetFloat("RotationSnap");
		float handleSize = HandleUtility.GetHandleSize(position) * scale * 1.435f;
		Color color = Handles.color;

		Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.93f);
		rotation = Handles.Disc(rotation, position, rotation * Vector3.right, handleSize, true, snapRotate);

		Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.93f);
		rotation = Handles.Disc(rotation, position, rotation * Vector3.up, handleSize, true, snapRotate);

		Handles.color = new Color(0.0f, 0.0f, 1.0f, 0.93f);
		rotation = Handles.Disc(rotation, position, rotation * Vector3.forward, handleSize, true, snapRotate);

		Handles.color = new Color(0.8f, 0.8f, 0.8f, 0.93f);
		rotation = Handles.Disc(rotation, position, Camera.current.transform.forward, handleSize * 1.1f, false, 0f);
		rotation = Handles.FreeRotateHandle(rotation, position, handleSize);

		Handles.color = color;
		return rotation;
	}
		
	// ------------------------------------------------------------------

	private static PropertyInfo ms_systemCopyBufferProperty = null;
	private static PropertyInfo GetSystemCopyBufferProperty()
	{
		if (ms_systemCopyBufferProperty == null)
		{
			Type T = typeof(GUIUtility);
			ms_systemCopyBufferProperty = T.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
			if (ms_systemCopyBufferProperty == null)
				throw new Exception("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
		}
		return ms_systemCopyBufferProperty;
	}
	
	/// <summary>
	/// Get or set text on the clipboard
	/// </summary>
	public static string clipBoard
	{
		get
		{
			PropertyInfo systemCopyBufferProperty = GetSystemCopyBufferProperty();
			return (string)systemCopyBufferProperty.GetValue(null, null);
		}
		set
		{
			PropertyInfo systemCopyBufferProperty = GetSystemCopyBufferProperty();
			systemCopyBufferProperty.SetValue(null, value, null);
		}
	}
	
}
