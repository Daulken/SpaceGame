using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
	using System.IO;
	using System.Text;
#endif
using System;
using System.Collections.Generic;

namespace Localization {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class Localization : MonoBehaviour
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
		
		private static Localization ms_instance = null;
		private SystemLanguage m_currentLanguage = SystemLanguage.English;
		private LocalisationData m_localisationData = new LocalisationData();

		// Awake is called at game startup, regardless of enable state
		protected virtual void Awake()
		{
			// If no instance exists, use this one
			if (ms_instance == null)
				ms_instance = this;

#if UNITY_EDITOR
			// If we're not the instance that things use
			if (ms_instance != this)
			{
				Debug.LogError("Somehow creating multiple instances of the Localization object");
			}
			// This is our instance
			else
			{
				// Get the language to use initially from the selected system language
				SetStartingLanguage();
			}
#else
			// If we're not the instance that things use
			if (ms_instance != this)
			{
				// Remove ourself, as we're duplicate
				bool destroyGameObject = true;
				Component[] components = gameObject.GetComponents(typeof(Component));
				foreach (Component component in components)
				{
					if (component is Transform)
						continue;
					if (component is Localization)
						continue;
					destroyGameObject = false;
					break;
				}

				if (destroyGameObject)
					Destroy(gameObject);
				else
					Destroy(this);
			}
			// This is our instance
			else
			{
				// Ensure we don't get destroyed between scenes
				DontDestroyOnLoad(gameObject);

				// Load the localisation data if present
				UpdateLocalisationData();

				// Get the language to use initially from the selected system language
				SetStartingLanguage();
			}
#endif
		}

		// Called when this object is destroyed
		protected virtual void OnDestroy()
		{
			// Remove our global instance pointer
			if (ms_instance == this)
				ms_instance = null;
		}

		// Ensures that there is an instance available to work with
		private static void EnsureInstanceExists()
		{
			// If an instance exists already, do nothing
			if ((ms_instance != null) && (ms_instance.gameObject != null))
				return;	

			// Look for an instance of this type in the scene already
			ms_instance = (Localization)GameObject.FindObjectOfType(typeof(Localization));

			// Create one if not found
			if (ms_instance == null)
			{
	#if UNITY_EDITOR
				GameObject go = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("_Global_Localization", HideFlags.HideAndDontSave);
	#else
				GameObject go = new GameObject("_Global_Localization");
				go.hideFlags = HideFlags.HideAndDontSave;
	#endif
				ms_instance = go.AddComponent<Localization>();
			}
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
		public static LocalisationData UpdateLocalisationData()
		{
			// Ensure that there is an instance of the singleton available
			EnsureInstanceExists();
			
			// Check to see if the file has been updated, and needs reloading
			bool needsReCache = false;
	#if UNITY_EDITOR
			DateTime currentTableWriteTime = DateTime.MinValue;
	#endif
			if (!ms_instance.m_localisationData.m_loaded)
			{
				needsReCache = true;
			}
	#if UNITY_EDITOR
			// In the editor, check if the source file has changed
			else if (!Application.isPlaying && !string.IsNullOrEmpty(ms_instance.m_localisationData.m_sourceAssetPath))
			{
				FileInfo fileInfo = new FileInfo(Application.dataPath + ms_instance.m_localisationData.m_sourceAssetPath);
				currentTableWriteTime = fileInfo.LastWriteTime;
				if (currentTableWriteTime != ms_instance.m_localisationData.m_lastSourceWriteTime)
					needsReCache = true;
			}
	#endif

			// If no re-cache is required, just return the current cache
			if (!needsReCache)
				return ms_instance.m_localisationData;

			// Load the stringtable asset resource
			string stringtableResourceName = PlayerPrefs.GetString("LocalisationStringTableResourceName", "Strings_Stringtable");
			TextAsset localisationTable = Resources.Load(stringtableResourceName) as TextAsset;

			// Set stringtable resource as loaded
			ms_instance.m_localisationData.m_loaded = true;

			// If no localisation table was found
			if (localisationTable == null)
			{
				// Reset the cached data, and return this
				ms_instance.m_localisationData.m_translations = null;
				ms_instance.m_localisationData.m_validLanguageList.Clear();
				ms_instance.m_localisationData.m_editorKeys = null;
				ms_instance.m_localisationData.m_editorNamesWithTooltips = null;
				ms_instance.m_localisationData.m_sourceAssetPath = null;
				ms_instance.m_localisationData.m_lastSourceWriteTime = DateTime.MinValue;
				return ms_instance.m_localisationData;
			}

	#if UNITY_EDITOR
			// Set the path of the loaded localisation table, so we can check for write timestamp changes
			ms_instance.m_localisationData.m_sourceAssetPath = AssetDatabase.GetAssetPath(localisationTable).Substring("Assets".Length);

			// Update the last file write time, so we can detect changes
			ms_instance.m_localisationData.m_lastSourceWriteTime = currentTableWriteTime;
	#endif

			try
			{
				// Deserialise the currentKey stringtable
				ms_instance.m_localisationData.m_translations = Storage.Deserialize<Dictionary<SystemLanguage, Dictionary<string, string>>>(localisationTable.text);
				
				// Get the list of valid languages from the translations, in the order found in the stringtable.
				// This allows the first language found to be the default language for unsupported system languages
				foreach (SystemLanguage language in ms_instance.m_localisationData.m_translations.Keys)
				{
					if (!ms_instance.m_localisationData.m_validLanguageList.Contains(language))
						ms_instance.m_localisationData.m_validLanguageList.Add(language);
				}
			}
			catch (Exception /*ex*/)
			{
			}

	#if UNITY_EDITOR
			// Get the keys available in the translations
			ms_instance.m_localisationData.m_editorKeys = new List<string>();
			ms_instance.m_localisationData.m_editorKeys.Add(String.Empty);
			if (ms_instance.m_localisationData.m_translations != null)
			{
				foreach (SystemLanguage language in ms_instance.m_localisationData.m_translations.Keys)
				{
					foreach (string key in ms_instance.m_localisationData.m_translations[language].Keys)
						ms_instance.m_localisationData.m_editorKeys.Add(key);
					ms_instance.m_localisationData.m_editorKeys.Sort();
					break;
				}
			}

			// Get the tooltip display strings available in the translations
			ms_instance.m_localisationData.m_editorNamesWithTooltips = new List<GUIContent>();
			foreach (string key in ms_instance.m_localisationData.m_editorKeys)
			{
				if (string.IsNullOrEmpty(key))
				{
					ms_instance.m_localisationData.m_editorNamesWithTooltips.Add(new GUIContent("<blank>", "No localisation key"));
				}
				else
				{
					StringBuilder builder = new StringBuilder();
					foreach (SystemLanguage language in ms_instance.m_localisationData.m_translations.Keys)
						builder.AppendLine(string.Format("{0}: {1}\n", language.ToString(), ms_instance.m_localisationData.m_translations[language][key]));
					ms_instance.m_localisationData.m_editorNamesWithTooltips.Add(new GUIContent(string.Format("{0}/{1}", key[0], key), builder.ToString()));
				}
			}
	#endif

			// Return the newly cached data
			return ms_instance.m_localisationData;
		}
					
		/// <summary>
		/// Gets the valid languages to select from.
		/// </summary>
		public static List<SystemLanguage> GetValidLanguages()
		{
			// Ensure that there is an instance of the singleton available
			EnsureInstanceExists();

			// Return the valid language list
			return ms_instance.m_localisationData.m_validLanguageList;
		}

		/// <summary>
		/// Gets the currently active language.
		/// </summary>
		public static SystemLanguage GetCurrentLanguage()
		{
			// Ensure that there is an instance of the singleton available
			EnsureInstanceExists();

			// Return the current language
			return ms_instance.m_currentLanguage;
		}
		
		/// <summary>
		/// Set the currently active language.
		/// </summary>
		public static void SetCurrentLanguage(SystemLanguage language)
		{
			// Ensure that there is an instance of the singleton available
			EnsureInstanceExists();

			// If this language isn't valid, ignore it
			if (!ms_instance.m_localisationData.m_validLanguageList.Contains(language))
				return;

			// If the language is the same, do nothing
			if (ms_instance.m_currentLanguage == language)
				return;
			
			// Set the new current language
			ms_instance.m_currentLanguage = language;
			PlayerPrefs.SetString("LastSelectedLanguage", ms_instance.m_currentLanguage.ToString());

			// Inform listeners that the language has changed
			if (ms_instance.m_onLocalize != null)
				ms_instance.m_onLocalize(ms_instance.m_currentLanguage);
		}
		
		/// <summary>
		/// Localize the specified value, with formatted variable arguments
		/// </summary>
		public static string Localize(string key, params object[] formatArgs)
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
		public static string LocalizeInEditor(string key, params object[] formatArgs)
		{
			// Return the localised string
			return DoLocalize(key, formatArgs);
		}
			
		/// <summary>
		/// Localize the specified value, with formatted variable arguments
		/// </summary>
		private static string DoLocalize(string key, params object[] formatArgs)
		{
			// If there is no key, return no text
			if (string.IsNullOrEmpty(key))
				return System.String.Empty;

			// Ensure that there is an instance of the singleton available
			EnsureInstanceExists();
			
			// If the translation table is invalid, can't do anything
			if (ms_instance.m_localisationData.m_translations == null)
				return key;

			// Get the translation table for the current language
			Dictionary<string, string> currentTranslations = null;
			bool languageFound = ms_instance.m_localisationData.m_translations.TryGetValue(ms_instance.m_currentLanguage, out currentTranslations);
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

}   // namespace Localization
