using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Actions {


	[AddComponentMenu("SPACEJAM/Actions/Actions/Generic/DestroyComponent")]
	public class DestroyComponent : BaseAction
	{
		public MonoBehaviour m_component = null;
	
		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// Destroy the given component
			if (m_component != null)
				Destroy(m_component);
			else
				DebugHelpers.LogWarningContext("DestroyComponent action on \"{0}\" has no Component specified", this, gameObject.GetNameWithPath());
	
			// This action is always instant
			return 0.0f;
		}
	}


}	// namespace Actions
