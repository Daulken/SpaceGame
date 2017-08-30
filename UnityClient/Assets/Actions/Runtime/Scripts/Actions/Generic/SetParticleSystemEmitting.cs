using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Actions {


	[AddComponentMenu("SPACEJAM/Actions/Actions/Generic/SetParticleSystemEmitting")]
	public class SetParticleSystemEmitting : BaseAction
	{
		public ParticleSystem m_particleSystem = null;
		public bool m_toggle = false;
		public bool m_emitting = true;
	
		// Called when the action is executed. The given flags indicate whether the action is being run as part of an immediate or realtime
		// action list, and a list of action list handles that this action should add to if it executes any. Returns action duration in seconds.
		protected override float PerformAction(bool immediate, bool realTime, ref List<System.UInt64> dependantActionHandles)
		{
			// Test validity
			if (m_particleSystem == null)
			{
				DebugHelpers.LogWarningContext("SetParticleSystemEmitting action on \"{0}\" has no ParticleSystem specified", this, gameObject.GetNameWithPath());
				return 0.0f;
			}

			ParticleSystem.EmissionModule emission = m_particleSystem.emission;

			// Set the emittance state of the particle system
			if (m_toggle)
				emission.enabled = !emission.enabled;
			else
				emission.enabled = m_emitting;
	
			// This action is always instant
			return 0.0f;
		}
	}


}	// namespace Actions
