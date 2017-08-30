using UnityEngine;
using System.Collections;

namespace Actions {


	[AddComponentMenu("SPACEJAM/Actions/Triggers/OnStartPerformActions")]
	public class OnStartPerformActions : MonoBehaviour
	{
		public GameObject m_actionListObject = null;
		public bool m_realTime = true;
		
		// This is non-zero if there are actions executing
		private System.UInt64 m_executingActions = 0;
		private bool m_shuttingDown = false;

		// This is called once on first enable, but never again
		protected virtual void Start()
		{
			// Cancel any current actions playing
			ActionListManager.Cancel(m_executingActions);

			// If there is an action list specified, execute it over time
			m_executingActions = ActionListManager.Execute(m_actionListObject, false, m_realTime);
		}
		
		// Called when the application shuts down
		protected virtual void OnApplicationQuit()
		{
			// Set our flag to indicate we are shutting down
			m_shuttingDown = true;
		}
		
		// Called when this object is destroyed
		protected virtual void OnDestroy()
		{
			// Cancel any current actions playing
			if (!m_shuttingDown)
			{
				ActionListManager.Cancel(m_executingActions);
				m_executingActions = 0;
			}
		}
	}


}	// namespace Actions
