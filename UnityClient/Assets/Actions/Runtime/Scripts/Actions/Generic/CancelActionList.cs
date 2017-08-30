using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Actions {

	
	[AddComponentMenu("SPACEJAM/Actions/Actions/Generic/CancelActionList")]
	public class CancelActionList : BaseAction
	{
		public GameObject m_actionListObject = null;
	
		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// If there is an action list specified, execute it over time
			if (m_actionListObject == null)
			{
				DebugHelpers.LogWarningContext("CancelActionList action on \"{0}\" has no ActionListObject specified", this, gameObject.GetNameWithPath());
				return 0.0f;
			}

			// Cancel the selected action list
			ActionListManager.Cancel(m_actionListObject);

			// This action is always instant.
			return 0.0f;
		}
	}


}	// namespace Actions
