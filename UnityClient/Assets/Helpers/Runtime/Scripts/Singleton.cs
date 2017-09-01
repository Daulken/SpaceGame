using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is a MonoBehaviour, to allow the singleton to use Coroutines.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T ms_instance;
	private static object ms_lock = new object();
	private static bool ms_applicationIsQuitting = false;

	public static T Instance
	{
		get
		{
			if (ms_applicationIsQuitting)
			{
				DebugHelpers.LogWarning("[Singleton] Instance '{0}' already destroyed on application quit. Won't create again - returning null.", typeof(T));
				return null;
			}

			lock (ms_lock)
			{
				if (ms_instance == null)
				{
					// Look for an instance of this type in the scene already, in case a singleton was added
					ms_instance = (T)FindObjectOfType(typeof(T));
					if (FindObjectsOfType(typeof(T)).Length > 1)
					{
						DebugHelpers.LogError("[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
						return ms_instance;
					}

					// Automatically create one if not found
					if (ms_instance == null)
					{
#if UNITY_EDITOR
						GameObject singleton = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("_Singleton_" + typeof(T).ToString(), HideFlags.HideAndDontSave);
#else
						GameObject singleton = new GameObject("_Singleton_" + typeof(T).ToString());
						singleton.hideFlags = HideFlags.HideAndDontSave;
#endif
						ms_instance = singleton.AddComponent<T>();

						// Ensure we don't get destroyed between scenes
						if (Application.isPlaying)
							DontDestroyOnLoad(singleton);
					}
				}

				return ms_instance;
			}
		}
	}

	/// <summary>
	/// When Unity quits, it destroys objects in a random order. In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, it will create a buggy ghost object that will stay on the Editor scene
	/// even after stopping playing the Application. Really bad! So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
	public void OnDestroy()
	{
		ms_applicationIsQuitting = true;
	}
}
