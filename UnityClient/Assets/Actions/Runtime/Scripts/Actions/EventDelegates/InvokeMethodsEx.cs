using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Actions {


	[AddComponentMenu("SPACEJAM/Actions/Actions/EventDelegates/InvokeMethodsEx")]
	public class InvokeMethodsEx : BaseAction
	{
		[SerializeField] public List<EventDelegates.EventDelegateEx> m_methodList = new List<EventDelegates.EventDelegateEx>();
	
		// Called whenever values are changed in the editor, or at startup, to cache values that can then be used
		// multiple times. For example, to create lists or arrays that would be expensive to create each perform
		protected override void RecacheValues()
		{
			// Precache any methods and parameters
			EventDelegates.EventDelegateEx.PreCacheExecute(m_methodList);
		}
		
		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// Invoke the selected methods
			EventDelegates.EventDelegateEx.Execute(m_methodList);
	
			// This action is always instant
			return 0.0f;
		}
	}


}	// namespace Actions
