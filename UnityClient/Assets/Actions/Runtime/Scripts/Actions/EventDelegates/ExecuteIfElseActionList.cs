using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Actions {

	
	[AddComponentMenu("SPACEJAM/Actions/Actions/EventDelegates/ExecuteIfElseActionList")]
	public class ExecuteIfElseActionList : BaseAction
	{
		public EventDelegates.FunctionDelegateList m_if = new EventDelegates.FunctionDelegateList();
		public GameObject m_actionListObjectIf = null;
		public GameObject m_actionListObjectElse = null;
		public bool m_forceRealTime = false;
		public bool m_realTime = true;
		public bool m_immediate = false;
	
		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// Get the action list to use, depending on the if test
			GameObject actionList = EventDelegates.FunctionDelegateList.Execute(m_if) ? m_actionListObjectIf : m_actionListObjectElse;
			
			// If there is an action list specified, execute it over time
			System.UInt64 executingActions = ActionListManager.Execute(actionList, immediate || m_immediate, m_forceRealTime ? m_realTime : realTime);
			dependantActionHandles.Add(executingActions);
	
			// This action is always instant. The dependant action list will be linked in automatically
			return 0.0f;
		}
	}


}	// namespace Actions
