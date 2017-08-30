using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PropertyDrawers
{
	namespace EditorScripts
	{
		// Container for generic property drawers for automatic display in the inspector, without an editor script
		public static class GenericPropertyDrawers
		{
			// Helper to automatically render an enum bitmask, rather than an enum in the Inspector window
			[CustomPropertyDrawer(typeof(BitMaskAttribute))]
			private class EnumBitMaskPropertyDrawer : PropertyDrawer
			{
				public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
				{
					// Add the actual int value behind the field name
					BitMaskAttribute maskAttribute = attribute as BitMaskAttribute;

					// Get the enum names and values
					string[] itemNames = System.Enum.GetNames(maskAttribute.m_propType);
					int[] itemValues = System.Enum.GetValues(maskAttribute.m_propType) as int[];

					// Get a valid mask value from the input mask, constrained to the enum value range
					int validInputMaskValue = 0;
					for (int itemIndex = 0; itemIndex < itemValues.Length; ++itemIndex)
					{
						if (itemValues[itemIndex] != 0)
						{
							if ((property.intValue & itemValues[itemIndex]) == itemValues[itemIndex])
								validInputMaskValue |= 1 << itemIndex;
						}
						else if (property.intValue == 0)
						{
							validInputMaskValue |= 1 << itemIndex;
						}
					}

					// Display a mask field, storing any new value selection
					int newMaskVal = EditorGUI.MaskField(position, label, validInputMaskValue, itemNames);

					// Check for any differences in value
					int maskChanges = validInputMaskValue ^ newMaskVal;

					// Construct a new value mask from the input mask, allowing for keeping any values outside of the constrained value range
					int newValue = property.intValue;
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

					// Set the newly selected value
					property.intValue = newValue;
				}
			}


		}


	}   // namespace EditorScripts
}   // namespace PropertyDrawers
