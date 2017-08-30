using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Actions {

	[AddComponentMenu("SPACEJAM/Actions/Actions/Generic/OpenWebsite")]
	public class OpenWebsite : BaseAction
	{
		public string m_url = "";

		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// Check that there is a URL
			if (string.IsNullOrEmpty(m_url))
			{
				DebugHelpers.LogWarningContext("OpenWebsite action on \"{0}\" has no URL specified", this, gameObject.GetNameWithPath());
				return 0.0f;
			}

			// Open the URL
			Application.OpenURL(m_url);

			// This action is always instant
			return 0.0f;
		}
	}


}	// namespace Actions
