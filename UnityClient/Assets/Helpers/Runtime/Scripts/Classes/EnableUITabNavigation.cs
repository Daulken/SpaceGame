using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Tab Navigator for UI
[AddComponentMenu("UI/EnableUITabNavigation")]
public class EnableUITabNavigation : SceneSingleton<EnableUITabNavigation>
{
	private EventSystem m_eventSystem;

	// Guarantee this will be always a singleton only - make the constructor protected!
	protected EnableUITabNavigation()
	{
	}

	private void Start()
	{
		m_eventSystem = EventSystem.current;
	}

	private void Update()
	{
		if ((m_eventSystem.currentSelectedGameObject == null) || !Input.GetKeyDown(KeyCode.Tab))
			return;

		Selectable current = m_eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
		if (current == null)
			return;

		bool up = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		Selectable next = up ? current.FindSelectableOnUp() : current.FindSelectableOnDown();

		// We are at the end or the beginning, go to either, depends on the direction we are tabbing in
		// The previous version would take the logical 0 selector, which would be the highest up in your editor hierarchy
		// But not certainly the first item on your GUI, or last for that matter
		// This code tabs in the correct visual order
		if (next == null)
		{
			next = current;

			Selectable pnext;
			if (up)
			{
				while ((pnext = next.FindSelectableOnDown()) != null)
					next = pnext;
			}
			else
			{
				while ((pnext = next.FindSelectableOnUp()) != null)
					next = pnext;
			}
		}

		// Simulate Inputfield MouseClick
		InputField inputfield = next.GetComponent<InputField>();
		if (inputfield != null)
			inputfield.OnPointerClick(new PointerEventData(m_eventSystem));

		// Select the next item in the tab order of our direction
		m_eventSystem.SetSelectedGameObject(next.gameObject);
	}
}