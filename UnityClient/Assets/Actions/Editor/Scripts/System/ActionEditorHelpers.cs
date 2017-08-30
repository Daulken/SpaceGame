using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// Action Editor helper functions
public static class ActionEditorHelpers
{
	/// <summary>
	/// Function to add common fields for all actions, in a unified and consistent manner
	/// </summary>
	public static void CommonActionGUI(SerializedObject serializedObject)
	{
		// Add the base action fields
		SerializedProperty delayProperty = serializedObject.FindProperty("m_delaySeconds");
		if (delayProperty != null)
			delayProperty.floatValue = EditorGUILayout.FloatField(new GUIContent("Delay", "The time in seconds to pause before executing this, and following, actions"), delayProperty.floatValue);
	}
	
	/// <summary>
	/// Function to add fields for all common iTween-based action properties, in a unified and consistent manner
	/// </summary>
	public static void CommonITweenGUI(SerializedObject serializedObject)
	{
		// Add colour channels property if present
		SerializedProperty channelsProperty = serializedObject.FindProperty("m_channels");
		if (channelsProperty != null)
			channelsProperty.intValue = (int)((Actions.ColourChannels)EditorGUILayout.EnumMaskField(new GUIContent("Channels", "Which colour channels to change"), (Actions.ColourChannels)channelsProperty.intValue));

		// Add recursive property if present
		SerializedProperty recursiveProperty = serializedObject.FindProperty("m_recursive");
		if (recursiveProperty != null)
			recursiveProperty.boolValue = EditorGUILayout.Toggle(new GUIContent("Affect Children?", "Should only this object be changed, or its children also?"), recursiveProperty.boolValue);
		
		// Add the generic iTween duration field if present
		SerializedProperty durationProperty = serializedObject.FindProperty("m_duration");
		if (durationProperty != null)
		{
			durationProperty.floatValue = EditorGUILayout.FloatField(new GUIContent("Duration", "The time in seconds to spread the change out over"), durationProperty.floatValue);
			if (durationProperty.floatValue > 0.0f)
			{
				// Check for any duration-based properties
				SerializedProperty easeTypeProperty = serializedObject.FindProperty("m_easeType");
				SerializedProperty shakeTypeProperty = serializedObject.FindProperty("m_shakeType");
				SerializedProperty loopTypeProperty = serializedObject.FindProperty("m_loopType");
				SerializedProperty loopCountProperty = serializedObject.FindProperty("m_loopCount");
				SerializedProperty delayProperty = serializedObject.FindProperty("m_iTweenDelay");
				if ((easeTypeProperty != null) ||
					(loopTypeProperty != null) ||
					(loopCountProperty != null) ||
					(delayProperty != null))
				{
					// Indent all duration-based properties
					EditorHelpers.IncreaseIndent();

					// Add the easeType property if found
					if (easeTypeProperty != null)
						EditorGUILayout.PropertyField(easeTypeProperty, new GUIContent("Ease Type", "The method to change the value by"));

					// Add the shakeType property if found
					if (shakeTypeProperty != null)
						EditorGUILayout.PropertyField(shakeTypeProperty, new GUIContent("Shake Type", "The method to change the value by"));

					// Add the shakeFrequency property if found
					SerializedProperty shakeFrequencyProperty = serializedObject.FindProperty("m_shakeFrequency");
					if (shakeFrequencyProperty != null)
						EditorGUILayout.PropertyField(shakeFrequencyProperty, new GUIContent("Shake Frequency", "The frequency at which the shake occurs"));
					
					// Add the loopType property if found
					if (loopTypeProperty != null)
					{
						EditorGUILayout.PropertyField(loopTypeProperty, new GUIContent("Loop Type", "What to do when the change finishes"));
						if (loopTypeProperty.intValue != (int)iTween.LoopType.none)
						{
							// Check for any loop-based properties
							if (loopCountProperty != null)
							{
								// Indent all loop-based properties
								EditorHelpers.IncreaseIndent();

								if (loopCountProperty != null)
								{
									string loopCountTooltip = "The number of loops to perform. Count of -1 is infinite. Count of 0 is the same as no loop type. After that, it's the number of EXTRA loops to perform. For ping-pong, it's the number of pings OR pongs to add. For example, a ping-pong loop count of 0 will just ping, and a loop count of 4 will give A->B->A->B->A->B";
									loopCountProperty.intValue = EditorGUILayout.IntSlider(new GUIContent("Loop Count", loopCountTooltip), loopCountProperty.intValue, -1, 20);
								}

								// End loop-based indent
								EditorHelpers.DecreaseIndent();
							}
						}
					}

					// Add the iTweenDelay property if found
					if (delayProperty != null)
						delayProperty.floatValue = EditorGUILayout.FloatField(new GUIContent("iTween Delay", "The time in seconds to pause before executing a tween, including between each loop or pingpong"), delayProperty.floatValue);
	
					// End duration-based indent
					EditorHelpers.DecreaseIndent();
				}
			}
		}
	}
	
}
