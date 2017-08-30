using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Actions {


	[AddComponentMenu("SPACEJAM/Actions/Actions/Generic/SetActive")]
	public class SetActive : BaseAction
	{
		public GameObject m_gameObject = null;
		public bool m_setActive = true;
	
		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// Set the active state of the given game object
			if (m_gameObject != null)
				m_gameObject.SetActive(m_setActive);
			else
				DebugHelpers.LogWarningContext("SetActive action on \"{0}\" has no GameObject specified", this, gameObject.GetNameWithPath());
	
			// This action is always instant
			return 0.0f;
		}
	}


}	// namespace Actions
