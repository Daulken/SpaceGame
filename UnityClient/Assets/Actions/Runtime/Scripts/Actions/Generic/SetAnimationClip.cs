using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Actions {


	[AddComponentMenu("SPACEJAM/Actions/Actions/Generic/SetAnimationClip")]
	public class SetAnimationClip : BaseAction
	{
		public Animation m_animation = null;
		public AnimationClip m_clip = null;
		public float m_duration = 0.0f;
	
		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// If there is an animation clip to play, and an animation to play on
			if ((m_animation != null) && (m_clip != null))
			{
				// If there is a duration, cross-fade the clip, otherwise just set it
				if ((m_duration > 0.0f) && !immediate)
					m_animation.CrossFade(m_clip.name, m_duration);
				else
					m_animation.Play(m_clip.name);
			}
			else
			{
				DebugHelpers.LogWarningContext("SetAnimationClip action on \"{0}\" has either no Animation or AnimationClip specified", this, gameObject.GetNameWithPath());
			}
	
			// This action is always instant
			return 0.0f;
		}
	}


}	// namespace Actions
