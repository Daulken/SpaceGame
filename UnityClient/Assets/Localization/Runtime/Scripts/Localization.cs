using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
	using System.IO;
	using System.Text;
	using System.Collections;
#endif
using System;
using System.Collections.Generic;


[ExecuteInEditMode]
[AddComponentMenu("")]
public class Localization : Singleton<Localization>
{
	/// <summary>
	//// Full localisation cache data, in case more advanced things need doing
	/// </summary>
	public class LocalisationData
	{
		public Dictionary<SystemLanguage, Dictionary<string, string>> m_translations = null;
		public List<SystemLanguage> m_validLanguageList = new List<SystemLanguage>();
		public bool m_loaded = false;
		public List<string> m_editorKeys = null;
		public List<GUIContent> m_editorNamesWithTooltips = null;
		public DateTime m_lastSourceWriteTime = DateTime.MinValue;
		public string m_sourceAssetPath = null;
	};
		
	/// <summary>
	/// Should the current language be persisted across sessions, rather than reading the system language each time?
	/// Only set this if your game has a language selection screen, otherwise changing system language will not update
	/// </summary>
	public bool m_persistLanguage = false;

	/// <summary>
	/// Delegates that get called when changing language at runtime
	/// </summary>
	public delegate void OnLocalize(SystemLanguage language);
	public OnLocalize m_onLocalize;
		
	private SystemLanguage m_currentLanguage = SystemLanguage.English;
	private LocalisationData m_localisationData = new LocalisationData();

	// Guarantee this will be always a singleton only - make the constructor protected!
	protected Localization()
	{
	}

	// Awake is called at game startup, regardless of enable state
	protected virtual void Awake()
	{
		// If this is an actual run of the game, load the localisation data if present.
		// No need to get localisation data as the Editor scripts continuously update this for us
		if (Application.isPlaying)
			UpdateLocalisationData();

		// Get the language to use initially from the selected system language
		SetStartingLanguage();
	}

	// Set the starting language from the selected system language
	private void SetStartingLanguage()
	{
		// Get the game language to start with
		m_currentLanguage = Application.systemLanguage;
			
		// If unknown, default to English
		if (m_currentLanguage == SystemLanguage.Unknown)
			m_currentLanguage = SystemLanguage.English;

		// If the language is to persist from the last session
		if (m_persistLanguage)
		{
			// Get the current language from the previous language selection, or current (system language) if not found
			string lastSelectedLanguage = PlayerPrefs.GetString("LastSelectedLanguage", m_currentLanguage.ToString());
			if (Enum.IsDefined(typeof(SystemLanguage), lastSelectedLanguage))
				m_currentLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), lastSelectedLanguage);
		}

		// If this language is not a valid language in the stringtable, default to the first valid language
		if (!m_localisationData.m_validLanguageList.Contains(m_currentLanguage))
			m_currentLanguage = (m_localisationData.m_validLanguageList.Count > 0) ? m_localisationData.m_validLanguageList[0] : SystemLanguage.English;
	}
		
	/// <summary>
	/// Update and cache the localisation data
	/// </summary>
	public LocalisationData UpdateLocalisationData()
	{
		// Check to see if the file has been updated, and needs reloading
		bool needsReCache = false;
#if UNITY_EDITOR
		DateTime currentTableWriteTime = DateTime.MinValue;
#endif
		if (!m_localisationData.m_loaded)
		{
			needsReCache = true;
		}
#if UNITY_EDITOR
		// In the editor, check if the source file has changed
		else if (!Application.isPlaying && !string.IsNullOrEmpty(m_localisationData.m_sourceAssetPath))
		{
			FileInfo fileInfo = new FileInfo(Application.dataPath + m_localisationData.m_sourceAssetPath);
			currentTableWriteTime = fileInfo.LastWriteTime;
			if (currentTableWriteTime != m_localisationData.m_lastSourceWriteTime)
				needsReCache = true;
		}
#endif

		// If no re-cache is required, just return the current cache
		if (!needsReCache)
			return m_localisationData;

		// Load the stringtable asset resource
		string stringtableResourceName = PlayerPrefs.GetString("LocalisationStringTableResourceName", "Strings_Stringtable");
		TextAsset localisationTable = Resources.Load(stringtableResourceName) as TextAsset;

		// Set stringtable resource as loaded
		m_localisationData.m_loaded = true;

		// If no localisation table was found
		if (localisationTable == null)
		{
			// Reset the cached data, and return this
			m_localisationData.m_translations = null;
			m_localisationData.m_validLanguageList.Clear();
			m_localisationData.m_editorKeys = null;
			m_localisationData.m_editorNamesWithTooltips = null;
			m_localisationData.m_sourceAssetPath = null;
			m_localisationData.m_lastSourceWriteTime = DateTime.MinValue;
			return m_localisationData;
		}

#if UNITY_EDITOR
		// Set the path of the loaded localisation table, so we can check for write timestamp changes
		m_localisationData.m_sourceAssetPath = AssetDatabase.GetAssetPath(localisationTable).Substring("Assets".Length);

		// Update the last file write time, so we can detect changes
		m_localisationData.m_lastSourceWriteTime = currentTableWriteTime;
#endif

		try
		{
			// Deserialise the currentKey stringtable
			m_localisationData.m_translations = Storage.Deserialize<Dictionary<SystemLanguage, Dictionary<string, string>>>(localisationTable.text);
				
			// Get the list of valid languages from the translations, in the order found in the stringtable.
			// This allows the first language found to be the default language for unsupported system languages
			foreach (SystemLanguage language in m_localisationData.m_translations.Keys)
			{
				if (!m_localisationData.m_validLanguageList.Contains(language))
					m_localisationData.m_validLanguageList.Add(language);
			}
		}
		catch (Exception /*ex*/)
		{
		}

#if UNITY_EDITOR
		// Get the keys available in the translations
		m_localisationData.m_editorKeys = new List<string>();
		m_localisationData.m_editorKeys.Add(String.Empty);
		if (m_localisationData.m_translations != null)
		{
			foreach (SystemLanguage language in m_localisationData.m_translations.Keys)
			{
				foreach (string key in m_localisationData.m_translations[language].Keys)
					m_localisationData.m_editorKeys.Add(key);
				m_localisationData.m_editorKeys.Sort();
				break;
			}
		}

		// Get the tooltip display strings available in the translations
		m_localisationData.m_editorNamesWithTooltips = new List<GUIContent>();
		foreach (string key in m_localisationData.m_editorKeys)
		{
			if (string.IsNullOrEmpty(key))
			{
				m_localisationData.m_editorNamesWithTooltips.Add(new GUIContent("<blank>", "No localisation key"));
			}
			else
			{
				StringBuilder builder = new StringBuilder();
				foreach (SystemLanguage language in m_localisationData.m_translations.Keys)
					builder.AppendLine(string.Format("{0}: {1}\n", language.ToString(), m_localisationData.m_translations[language][key]));
				m_localisationData.m_editorNamesWithTooltips.Add(new GUIContent(string.Format("{0}/{1}", key[0], key), builder.ToString()));
			}
		}
#endif

		// Return the newly cached data
		return m_localisationData;
	}

#if UNITY_EDITOR
	private IEnumerator m_translationPerformer;

	public void AutoTranslateNonEnglishLanguages(Action<Dictionary<string, Dictionary<SystemLanguage, string>>> result)
	{
		// If already translating, do nothing
		if (m_translationPerformer != null)
			return;

		// Update the cached localisation keys for this stringtable if required, and return the cached data
		LocalisationData cachedData = Instance.UpdateLocalisationData();

		// Perform translations
		m_translationPerformer = AutoTranslateNonEnglishLanguagesInternal(cachedData, (changes) =>
				{
					m_translationPerformer = null;
					result(changes);
				}
			);
		StartCoroutine(m_translationPerformer);
	}

	public void CancelAutoTranslate()
	{
		if (m_translationPerformer != null)
		{
			StopCoroutine(m_translationPerformer);
			m_translationPerformer = null;
		}
	}

	private IEnumerator AutoTranslateNonEnglishLanguagesInternal(LocalisationData cachedData, Action<Dictionary<string, Dictionary<SystemLanguage, string>>> result)
	{
		Dictionary<SystemLanguage, string> translationLanguageCodes = new Dictionary<SystemLanguage, string>
				{
					{  SystemLanguage.Afrikaans, "af" },
					{  SystemLanguage.Arabic, "ar" },
					{  SystemLanguage.Basque, "eu" },
					{  SystemLanguage.Belarusian, "be" },
					{  SystemLanguage.Bulgarian, "bg" },
					{  SystemLanguage.Catalan, "ca" },
					{  SystemLanguage.Chinese, "zh-CN" },
					{  SystemLanguage.Czech, "cs" },
					{  SystemLanguage.Danish, "da" },
					{  SystemLanguage.Dutch, "nl" },
					{  SystemLanguage.English, "en" },
					{  SystemLanguage.Estonian, "et" },
					{  SystemLanguage.Finnish, "fi" },
					{  SystemLanguage.French, "fr" },
					{  SystemLanguage.German, "de" },
					{  SystemLanguage.Greek, "el" },
					{  SystemLanguage.Hebrew, "iw" },
					{  SystemLanguage.Hungarian, "hu" },
					{  SystemLanguage.Icelandic, "is" },
					{  SystemLanguage.Indonesian, "id" },
					{  SystemLanguage.Italian, "it" },
					{  SystemLanguage.Japanese, "ja" },
					{  SystemLanguage.Korean, "ko" },
					{  SystemLanguage.Latvian, "lv" },
					{  SystemLanguage.Lithuanian, "lt" },
					{  SystemLanguage.Norwegian, "no" },
					{  SystemLanguage.Polish, "pl" },
					{  SystemLanguage.Portuguese, "pt" },
					{  SystemLanguage.Romanian, "ro" },
					{  SystemLanguage.Russian, "ru" },
					{  SystemLanguage.SerboCroatian, "sr" },
					{  SystemLanguage.Slovak, "sk" },
					{  SystemLanguage.Slovenian, "sl" },
					{  SystemLanguage.Spanish, "es" },
					{  SystemLanguage.Swedish, "sv" },
					{  SystemLanguage.Thai, "th" },
					{  SystemLanguage.Turkish, "tr" },
					{  SystemLanguage.Ukrainian, "uk" },
					{  SystemLanguage.Vietnamese, "vi" },
				};

		Dictionary<string, Dictionary<SystemLanguage, string>> changes = new Dictionary<string, Dictionary<SystemLanguage, string>>();

		// Translate all missing non-English strings that have an English equivalent
		Dictionary<string, string> englishTranslations = cachedData.m_translations[SystemLanguage.English];
		foreach (SystemLanguage language in cachedData.m_translations.Keys)
		{
			if (language == SystemLanguage.English)
				continue;

			foreach (KeyValuePair<string, string> keyStringPair in cachedData.m_translations[language])
			{
				if (string.IsNullOrEmpty(keyStringPair.Value))
				{
					string url = String.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl={0}&dt=t&q={1}", translationLanguageCodes[language], WWW.EscapeURL(englishTranslations[keyStringPair.Key]));
					WWW www = new WWW(url);
					yield return www;

					if (www.isDone && string.IsNullOrEmpty(www.error))
					{
						List<object> response = (List<object>)MiniJSON.Json.Deserialize(www.text);
						List<object> translations = (List<object>)response[0];
						string translation = "";
						foreach (object block in translations)
							translation += ((List<object>)block)[0];

						if (!changes.ContainsKey(keyStringPair.Key))
							changes[keyStringPair.Key] = new Dictionary<SystemLanguage, string>();
						changes[keyStringPair.Key][language] = translation.Trim();
					}
				}
			}
		}

		// If changes were made
		if (changes.Count > 0)
		{
			// Update the cached translations
			foreach (KeyValuePair<string, Dictionary<SystemLanguage, string>> keyDictPair in changes)
			{
				foreach (KeyValuePair<SystemLanguage, string> languageStringPair in keyDictPair.Value)
				{
					cachedData.m_translations[languageStringPair.Key][keyDictPair.Key] = languageStringPair.Value;
				}
			}

			// Create the stringtable contents
			string content = Storage.SerializeToString(cachedData.m_translations);

			// Write the binary stringtable contents
			string absoluteStringtableFilename = Application.dataPath + m_localisationData.m_sourceAssetPath;
			string absoluteStringtablePath = System.IO.Path.GetDirectoryName(absoluteStringtableFilename);
			Directory.CreateDirectory(absoluteStringtablePath);
			StreamWriter writer = File.CreateText(absoluteStringtableFilename);
			writer.Write(content);
			writer.Flush();
			writer.Close();

			// Refresh the database, so the new file is loaded
			AssetDatabase.Refresh();
		}

		result(changes);
	}
#endif

	/// <summary>
	/// Gets the valid languages to select from.
	/// </summary>
	public List<SystemLanguage> GetValidLanguages()
	{
		// Return the valid language list
		return m_localisationData.m_validLanguageList;
	}

	/// <summary>
	/// Gets the currently active language.
	/// </summary>
	public SystemLanguage GetCurrentLanguage()
	{
		// Return the current language
		return m_currentLanguage;
	}
		
	/// <summary>
	/// Set the currently active language.
	/// </summary>
	public void SetCurrentLanguage(SystemLanguage language)
	{
		// If this language isn't valid, ignore it
		if (!m_localisationData.m_validLanguageList.Contains(language))
			return;

		// If the language is the same, do nothing
		if (m_currentLanguage == language)
			return;

		// Set the new current language
		m_currentLanguage = language;
		PlayerPrefs.SetString("LastSelectedLanguage", m_currentLanguage.ToString());

		// Inform listeners that the language has changed
		if (m_onLocalize != null)
			m_onLocalize(m_currentLanguage);
	}
		
	/// <summary>
	/// Localize the specified value, with formatted variable arguments
	/// </summary>
	public string Localize(string key, params object[] formatArgs)
	{
#if UNITY_EDITOR
		// If in the editor, always use the key
		if (!Application.isPlaying)
			return key;
#endif
			
		// Return the localised string
		return DoLocalize(key, formatArgs);
	}
			
	/// <summary>
	/// Localize the specified value, with formatted variable arguments
	/// </summary>
	public string LocalizeInEditor(string key, params object[] formatArgs)
	{
		// Return the localised string
		return DoLocalize(key, formatArgs);
	}
			
	/// <summary>
	/// Localize the specified value, with formatted variable arguments
	/// </summary>
	private string DoLocalize(string key, params object[] formatArgs)
	{
		// If there is no key, return no text
		if (string.IsNullOrEmpty(key))
			return System.String.Empty;

		// If the translation table is invalid, can't do anything
		if (m_localisationData.m_translations == null)
			return key;

		// Get the translation table for the current language
		Dictionary<string, string> currentTranslations = null;
		bool languageFound = m_localisationData.m_translations.TryGetValue(m_currentLanguage, out currentTranslations);
		if (!languageFound || (currentTranslations == null))
			return key;

		// Localise the string if found in the dictionary
		string translation = null;
		bool keyFound = currentTranslations.TryGetValue(key, out translation);
		if (!keyFound)
		{
			DebugHelpers.LogWarning("Localization key not found: '" + key + "'");
			return key;
		}

		// Format the string with given arguments
		string formatted = ((formatArgs != null) && (formatArgs.Length > 0)) ? string.Format(translation, formatArgs) : translation;
		return formatted;
	}
}

// Turns a string into a localisation key in the Inspector
public class LocalizationKeyAttribute : PropertyAttribute
{
}

