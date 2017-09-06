//#define HELPER_SUPPORTS_NGUI

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public static class HelperFunctions
{
	/// <summary>
	/// Ensure an array of new-able items is the correct size
	/// </summary>
	public static void EnsureArraySize<T>(ref T[] table, int size) where T: new()
	{
		// Exists and same size, do nothing
		if ((table != null) && (table.Length == size))
			return;

		// Allocate new array
		T[] newEntries = new T[size];

		// Copy over existing entries that fit, and initialize new entries
		for (int index = 0; index < size; ++index)
		{
			if ((table != null) && (index < table.Length))
				newEntries[index] = table[index];
			else
				newEntries[index] = new T();
		}

		// Assign new entries
		table = newEntries;
	}

	/// <summary>
	/// Ensure an array of value-type items is the correct size
	/// </summary>
	public static void EnsureArraySize<T>(ref T[] table, int size, T newValue)
	{
		// Exists and same size, do nothing
		if ((table != null) && (table.Length == size))
			return;

		// Allocate new array
		T[] newEntries = new T[size];

		// Copy over existing entries that fit, and initialize new entries
		for (int index = 0; index < size; ++index)
		{
			if ((table != null) && (index < table.Length))
				newEntries[index] = table[index];
			else
				newEntries[index] = newValue;
		}

		// Assign new entries
		table = newEntries;
	}
	
	//---------------------------------------------------

	/// <summary>
	/// If an array of new-able items is the wrong size, reinitialize it with defaults
	/// </summary>
	public static void ReinitializeArrayIfSizeChanged<T>(ref T[] table, int size) where T: new()
	{
		// Exists and same size, do nothing
		if ((table != null) && (table.Length == size))
			return;

		// Allocate new array, without copying old values
		T[] newEntries = new T[size];
		for (int index = 0; index < size; ++index)
			newEntries[index] = new T();

		// Assign new entries
		table = newEntries;
	}

	/// <summary>
	/// If an array of value-type items is the wrong size, reinitialize it with defaults
	/// </summary>
	public static void ReinitializeArrayIfSizeChanged<T>(ref T[] table, int size, T newValue)
	{
		// Exists and same size, do nothing
		if ((table != null) && (table.Length == size))
			return;

		// Allocate new array, without copying old values
		T[] newEntries = new T[size];
		for (int index = 0; index < size; ++index)
			newEntries[index] = newValue;

		// Assign new entries
		table = newEntries;
	}

	//---------------------------------------------------

	/// <summary>
	/// Find all scene components, active or inactive.
	/// </summary>
	public static T[] FindAllComponentsOfType<T>(bool includeInactive) where T : Component
	{
		// If we want to include inactive objects
		if (includeInactive)
		{
			// Find all resources of this type, anywhere
			T[] components = Resources.FindObjectsOfTypeAll(typeof(T)) as T[];

			// Get a list of all non-hidden components, so we only get those visible in the scene, not hidden or in project prefabs
			List<T> list = new List<T>();
			foreach (T component in components)
			{
				if (component.gameObject.hideFlags == 0)
					list.Add(component);
			}

			// Return this list as an array
			return list.ToArray();
		}
		// Just active objects
		else
		{
			// Use the built-in find
			return GameObject.FindObjectsOfType(typeof(T)) as T[];
		}
	}
	
	/// <summary>
	/// Find all scene gameobjects, active or inactive.
	/// </summary>
	public static GameObject[] FindAllGameObjects(bool includeInactive)
	{
		// If we want to include inactive objects
		if (includeInactive)
		{
			// Find all GameObject resources, anywhere
			GameObject[] gameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];

			// Get a list of all non-hidden gameobjects, so we only get those visible in the scene, not hidden or in project prefabs
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject gameObject in gameObjects)
			{
				if (gameObject.hideFlags == 0)
					list.Add(gameObject);
			}

			// Return this list as an array
			return list.ToArray();
		}
		// Just active objects
		else
		{
			// Use the built-in find
			return GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		}
	}
	
	//---------------------------------------------------

	/// <summary>
	/// Create a new array of enum names from a given type
	/// </summary>
	public static T[] CreateEnumNameArray<T>() where T : struct, IConvertible
	{
		// Ensure the type is valid
		if (!typeof(T).IsEnum)
			throw new ArgumentException("T must be an enumerated type");

		// Get the names for this type
		System.Array enumNames = Enum.GetNames(typeof(T));

		// Create an array of the correct size, and fill in the names
		int noofNames = enumNames.Length;
		T[] nameArray = new T[noofNames];
		for (int enumIndex = 0; enumIndex < noofNames; ++enumIndex)
			nameArray[enumIndex] = (T)enumNames.GetValue(enumIndex);

		// Return this new array
		return nameArray;
	}

	/// <summary>
	/// Create a new array of enum values from a given type
	/// </summary>
	public static T[] CreateEnumValueArray<T>() where T : struct, IConvertible
	{
		// Ensure the type is valid
		if (!typeof(T).IsEnum)
			throw new ArgumentException("T must be an enumerated type");

		// Get the values for this type
		System.Array enumValues = Enum.GetValues(typeof(T));

		// Create an array of the correct size, and fill in the values
		int noofValues = enumValues.Length;
		T[] valueArray = new T[noofValues];
		for (int enumIndex = 0; enumIndex < noofValues; ++enumIndex)
			valueArray[enumIndex] = (T)enumValues.GetValue(enumIndex);

		// Return this new array
		return valueArray;
	}

	//---------------------------------------------------

	/// <summary>
	/// Create a new array of enum names from a given type
	/// </summary>
	public static Dictionary<string, T> CreateNameToEnumDictionary<T>() where T : struct, IConvertible
	{
		// Ensure the type is valid
		if (!typeof(T).IsEnum)
			throw new ArgumentException("T must be an enumerated type");

		// Get the values for this type
		System.Array enumValues = Enum.GetValues(typeof(T));

		// Create a dictionary of the correct size, and fill in the values
		int noofValues = enumValues.Length;
		Dictionary<string, T> valueDict = new Dictionary<string, T>(noofValues);
		for (int enumIndex = 0; enumIndex < noofValues; ++enumIndex)
		{
			T enumValue = (T)enumValues.GetValue(enumIndex);
			valueDict[enumValue.ToString()] = enumValue;
		}

		// Return this new dictionary
		return valueDict;
	}
	
	//---------------------------------------------------

	// Modify the alpha channel of a colour, and return the new colour
	private static Color ModifyAlpha(Color colour, float alpha)
	{
		colour.a = alpha;
		return colour;
	}

	/// <summary>
	/// Apply the given alpha to the GameObject belonging to a transform
	/// </summary>
	public static void ApplyAlpha(Transform transform, float alpha)
	{
#if HELPER_SUPPORTS_NGUI
		UIWidget uiWidget = transform.GetComponent<UIWidget>();
		if (uiWidget != null)
		{
			uiWidget.alpha = alpha;
		}
		else
#endif
		{
			Renderer renderer = transform.GetComponent<Renderer>();
			if (renderer != null)
			{
				Material[] rendererMaterials = (renderer != null) ? renderer.materials : null;
				for (int materialIndex = 0; materialIndex < rendererMaterials.Length; ++materialIndex)
				{
					if (rendererMaterials[materialIndex] != null)
						rendererMaterials[materialIndex].color = ModifyAlpha(rendererMaterials[materialIndex].color, alpha);
				}
			}
			else
			{
				Light light = transform.GetComponent<Light>();
				if (light != null)
				{
					light.color = ModifyAlpha(light.color, alpha);
				}
				else
				{
					GUIText guiText = transform.GetComponent<GUIText>();
					if (guiText != null)
					{
						guiText.material.color = ModifyAlpha(guiText.material.color, alpha);
					}
					else
					{
						GUITexture guiTexture = transform.GetComponent<GUITexture>();
						if (guiTexture != null)
						{
							guiTexture.color = ModifyAlpha(guiTexture.color, alpha);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Recursively apply the given alpha to the GameObject belonging to a transform, and all child GameObject's
	/// </summary>
	public static void RecursiveApplyAlpha(Transform transform, float alpha)
	{
		ApplyAlpha(transform, alpha);
		int childCount = transform.childCount;
		for (int childIndex = 0; childIndex < childCount; ++childIndex)
		{
			Transform child = transform.GetChild(childIndex);
			RecursiveApplyAlpha(child, alpha);
		}
	}

	//---------------------------------------------------

	// Modify the first colour by the selected colour channels of the second colour
	private static Color ModifyColourChannels(Color colour, Color applicationColour, bool applyRed, bool applyGreen, bool applyBlue, bool applyAlpha)
	{
		if (applyRed)
			colour.r = applicationColour.r;
		if (applyGreen)
			colour.g = applicationColour.g;
		if (applyBlue)
			colour.b = applicationColour.b;
		if (applyAlpha)
			colour.a = applicationColour.a;
		return colour;
	}

	/// <summary>
	/// Apply the given colour to the GameObject belonging to a transform
	/// </summary>
	public static void ApplyColour(Transform transform, Color colour, bool applyRed, bool applyGreen, bool applyBlue, bool applyAlpha)
	{
#if HELPER_SUPPORTS_NGUI
		UIWidget uiWidget = transform.GetComponent<UIWidget>();
		if (uiWidget != null)
		{
			uiWidget.color = ModifyColourChannels(uiWidget.color, colour, applyRed, applyGreen, applyBlue, applyAlpha);
		}
		else
#endif
		{
			Renderer renderer = transform.GetComponent<Renderer>();
			if (renderer != null)
			{
				Material[] rendererMaterials = (renderer != null) ? renderer.materials : null;
				for (int materialIndex = 0; materialIndex < rendererMaterials.Length; ++materialIndex)
				{
					if (rendererMaterials[materialIndex] != null)
						rendererMaterials[materialIndex].color = ModifyColourChannels(rendererMaterials[materialIndex].color, colour, applyRed, applyGreen, applyBlue, applyAlpha);
				}
			}
			else
			{
				Light light = transform.GetComponent<Light>();
				if (light != null)
				{
					light.color = ModifyColourChannels(light.color, colour, applyRed, applyGreen, applyBlue, applyAlpha);
				}
				else
				{
					GUIText guiText = transform.GetComponent<GUIText>();
					if (guiText != null)
					{
						guiText.material.color = ModifyColourChannels(guiText.material.color, colour, applyRed, applyGreen, applyBlue, applyAlpha);
					}
					else
					{
						GUITexture guiTexture = transform.GetComponent<GUITexture>();
						if (guiTexture != null)
						{
							guiTexture.color = ModifyColourChannels(guiTexture.color, colour, applyRed, applyGreen, applyBlue, applyAlpha);
						}
					}
				}
			}
		}
	}
	
#if HELPER_SUPPORTS_NGUI
	/// <summary>
	/// Apply the given colour to the UIWidget
	/// </summary>
	public static void ApplyColour(UIWidget uiWidget, Color colour, bool applyRed, bool applyGreen, bool applyBlue, bool applyAlpha)
	{
		uiWidget.color = ModifyColourChannels(uiWidget.color, colour, applyRed, applyGreen, applyBlue, applyAlpha);
	}
#endif
	
	/// <summary>
	/// Recursively apply the given colour to the GameObject belonging to a transform, and all child GameObject's, masked by flags
	/// </summary>
	public static void RecursiveApplyColour(Transform transform, Color colour, bool applyRed, bool applyGreen, bool applyBlue, bool applyAlpha)
	{
		ApplyColour(transform, colour, applyRed, applyGreen, applyBlue, applyAlpha);
		int childCount = transform.childCount;
		for (int childIndex = 0; childIndex < childCount; ++childIndex)
		{
			Transform child = transform.GetChild(childIndex);
			RecursiveApplyColour(child, colour, applyRed, applyGreen, applyBlue, applyAlpha);
		}
	}
	
}
