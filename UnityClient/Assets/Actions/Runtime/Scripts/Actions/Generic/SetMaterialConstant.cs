using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Actions {


	[AddComponentMenu("SPACEJAM/Actions/Actions/Generic/SetMaterialConstant")]
	public class SetMaterialConstant : BaseAction
	{
		public enum ConstantType
		{
			Float,
			Vector4,
			Color,
		};

		public Material m_material = null;
		public string m_name = String.Empty;
		public ConstantType m_type = ConstantType.Float;
		public float m_constantFloat = 0.0f;
		public Vector4 m_constantVector = Vector4.zero;
		public Color m_constantColour = Color.white;

		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// Check for missing material
			if (m_material == null)
			{
				DebugHelpers.LogWarningContext("SetMaterialConstant action on \"{0}\" has no Material specified", this, gameObject.GetNameWithPath());
				return 0.0f;
			}

			// Check for missing constant
			if (string.IsNullOrEmpty(m_name))
			{
				DebugHelpers.LogWarningContext("SetMaterialConstant action on \"{0}\" has no constant name specified", this, gameObject.GetNameWithPath());
				return 0.0f;
			}
			
			// Handle different constant types
			switch (m_type)
			{
			case ConstantType.Float: m_material.SetFloat(m_name, m_constantFloat); break;
			case ConstantType.Vector4: m_material.SetVector(m_name, m_constantVector); break;
			case ConstantType.Color: m_material.SetColor(m_name, m_constantColour); break;
			}
	
			// This action is always instant
			return 0.0f;
		}
	}


}	// namespace Actions
