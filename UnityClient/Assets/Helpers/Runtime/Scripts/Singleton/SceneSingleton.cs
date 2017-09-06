using UnityEngine;

/// <summary>
/// This finds a singleton MonoBehaviour in a scene, to provide easy access. It does not runtime create, which allows derived classes to have assigned public properties
/// <para>Be aware this will not prevent a non singleton constructor such as `TDerivedType myT = new TDerivedType();`. To prevent that, add `protected TDerivedType () {}` to your singleton class.</para>
/// </summary>
public class SceneSingleton<TDerivedType> : MonoBehaviour where TDerivedType : SceneSingleton<TDerivedType>
{
	private static TDerivedType ms_instance;
	private static object ms_lock = new object();

	public static TDerivedType Instance
	{
		get
		{
			lock (ms_lock)
			{
				if (ms_instance == null)
				{
					// Look for an instance of this type in the scene already, in case a singleton was added
					TDerivedType[] instances = HelperFunctions.FindAllComponentsOfType<TDerivedType>(true);

					// If no instance is found
					if (instances.Length == 0)
						return null;

					ms_instance = instances[0];

					// Error check that there is only one instance
					if (instances.Length > 1)
						DebugHelpers.LogError("[SceneSingleton<{0}>] Something went really wrong - there should never be more than 1 SceneSingleton! Reopening the scene might fix it.", typeof(TDerivedType).ToString());
				}

				return ms_instance;
			}
		}
	}
}
