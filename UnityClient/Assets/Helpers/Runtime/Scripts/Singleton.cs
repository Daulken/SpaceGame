using UnityEngine;

/// <summary>
/// This instantiates a singleton at runtime. This is a MonoBehaviour, to allow the singleton to use Coroutines.
/// <para>Be aware this will not prevent a non singleton constructor such as `TDerivedType myT = new TDerivedType();`. To prevent that, add `protected TDerivedType () {}` to your singleton class.</para>
/// </summary>
public abstract class Singleton<TDerivedType> : MonoBehaviour where TDerivedType : Singleton<TDerivedType>
{
	private static TDerivedType ms_instance;
	private static object ms_lock = new object();
	private static bool ms_applicationIsQuitting = false;

	public static TDerivedType Instance
	{
		get
		{
			lock (ms_lock)
			{
				if (ms_instance == null)
				{
					if (ms_applicationIsQuitting)
					{
						DebugHelpers.LogWarning("[Singleton<{0}>] Instance already destroyed on application quit. Won't create again. Returning null.", typeof(TDerivedType).ToString());
						return null;
					}

					// Look for an instance of this type in the scene already, in case a Singleton was added
					ms_instance = (TDerivedType)FindObjectOfType(typeof(TDerivedType));

					// If no instance is found
					if (ms_instance == null)
					{
						// Automatically create one
#if UNITY_EDITOR
						GameObject singleton = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("_Singleton_" + typeof(TDerivedType).ToString(), HideFlags.HideAndDontSave);
#else
						GameObject singleton = new GameObject("_Singleton_" + typeof(TDerivedType).ToString());
						singleton.hideFlags = HideFlags.HideAndDontSave;
#endif
						ms_instance = singleton.AddComponent<TDerivedType>();

						// Ensure we don't get destroyed between scenes
						if (Application.isPlaying)
							DontDestroyOnLoad(singleton);
					}

					// Error check that there is only one instance
					if (FindObjectsOfType(typeof(TDerivedType)).Length > 1)
						DebugHelpers.LogError("[Singleton<{0}>] Something went really wrong - there should never be more than 1 Singleton! Reopening the scene might fix it.", typeof(TDerivedType).ToString());
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
