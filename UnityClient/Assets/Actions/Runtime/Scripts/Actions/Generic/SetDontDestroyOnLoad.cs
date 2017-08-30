using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Actions {

	
	[AddComponentMenu("SPACEJAM/Actions/Actions/Generic/SetDontDestroyOnLoad")]
	public class SetDontDestroyOnLoad : BaseAction
	{
		public GameObject[] m_gameObjects;
	
		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// Go through all game objects and mark them as not to be destroyed when loading other scenes
			foreach (Object go in m_gameObjects)
			{
				if (go != null)
					DontDestroyOnLoad(go);
				else
					DebugHelpers.LogWarningContext("SetDontDestroyOnLoad action on \"{0}\" has been given a bad GameObject", this, gameObject.GetNameWithPath());
			}
	
			// This action is always instant
			return 0.0f;
		}
	}


}	// namespace Actions
