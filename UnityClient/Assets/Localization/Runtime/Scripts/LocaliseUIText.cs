using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple script that lets you localize a Text object.
/// </summary>
[RequireComponent(typeof(Text))]
[AddComponentMenu("UI/LocaliseUIText")]
public class LocaliseUIText : MonoBehaviour
{
	[LocalizationKey]
	public string key;

	private SystemLanguage m_language;
	private bool m_started = false;
	private Text m_text = null;

	/// <summary>
	/// Localize the widget on enable, but only if it has been started already.
	/// </summary>
	void OnEnable()
	{
		if (m_started)
			Localize();
	}

	/// <summary>
	/// Localize the Text on start.
	/// </summary>
	void Start()
	{
		// Cache off the Text
		m_text = GetComponent<Text>();

		m_started = true;
		Localize();
	}

	/// <summary>
	/// This function is called by the Localization manager via a broadcast SendMessage.
	/// </summary>
	void OnLocalize(SystemLanguage newLanguage)
	{
		if (m_language != newLanguage)
			Localize();
	}

	/// <summary>
	/// Force-localize the widget, allowing this in the editor.
	/// </summary>
	public void LocalizeInEditor()
	{
		Localize(true);
	}

	/// <summary>
	/// Force-localize the widget.
	/// </summary>
	public void Localize()
	{
		Localize(false);
	}

	private void Localize(bool inEditor)
	{
		m_language = Localization.Instance.GetCurrentLanguage();

#if UNITY_EDITOR
		// If in the editor, always ensure we have a valid instance
		if (!Application.isPlaying)
			m_text = GetComponent<Text>();
#endif

		if (m_text == null)
			return;

		// If we still don't have a key, leave the value as blank
		string val = inEditor ? Localization.Instance.LocalizeInEditor(key) : Localization.Instance.Localize(key);
		if (m_text.text != val)
			m_text.text = val;
	}
}
