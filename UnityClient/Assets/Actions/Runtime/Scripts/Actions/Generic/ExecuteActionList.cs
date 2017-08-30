using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Actions {

	
	[AddComponentMenu("SPACEJAM/Actions/Actions/Generic/ExecuteActionList")]
	public class ExecuteActionList : BaseAction
	{
		public GameObject m_actionListObject = null;
		public bool m_forceRealTime = false;
		public bool m_realTime = true;
		public bool m_immediate = false;
	
		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// If there is an action list specified, execute it over time
			if (m_actionListObject == null)
			{
				DebugHelpers.LogWarningContext("ExecuteActionList action on \"{0}\" has no ActionListObject specified", this, gameObject.GetNameWithPath());
				return 0.0f;
			}

			// Make sure we don't run ourselves!
			if (m_actionListObject == gameObject)
			{
				DebugHelpers.LogWarningContext("ExecuteActionList action on \"{0}\" is trying to execute itself. This is forbidden, as Unity will implode!", this, gameObject.GetNameWithPath());
				return 0.0f;
			}

			System.UInt64 executingActions = ActionListManager.Execute(m_actionListObject, immediate || m_immediate, m_forceRealTime ? m_realTime : realTime);
			dependantActionHandles.Add(executingActions);
	
			// This action is always instant. The dependant action list will be linked in automatically
			return 0.0f;
		}
	}


}	// namespace Actions
