using UnityEngine;

/// <summary>
/// Simple singleton class which provides global, cached access to a component.
/// This script does not perform any kind of initialization; it is expected that an instance
/// of the class is created elsewhere.
/// Example class definition: public class Goat : Singleton&lt;Goat&gt;
/// </summary>
/// <typeparam name="T">The component type which should act like a singleton.</typeparam>
public class Singleton<T> : MonoBehaviour where T : UnityEngine.Object
{
	/// <summary>
	/// Cached reference to the instance.
	/// </summary>
	private static T instance;

	/// <summary>
	/// Reference to the single instance of T.
	/// </summary>
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				//This is the first call to Instance, or the level changed (in which case
				//instance points to an object which has now been destroyed).

				instance = Find.FirstInScene<T>();
#if UNITY_EDITOR
				DebugUtils.Assert(Find.InScene<T>().Length <= 1, string.Format("More than one instance of '{0}'"
					+ " exists. Only a single instance of this type should exist at any given time.", typeof(T).Name));

				DebugUtils.Assert(Find.InScene<T>().Length != 0, string.Format("No instances of '{0}'"
					+ " exists. Only a single instance of this type should exist at any given time.", typeof(T).Name));
#endif
			}
#if UNITY_EDITOR
			DebugUtils.Assert(instance != null, string.Format("An instance of class type '{0}'"
				+ " does not exist in the current scene.", typeof(T).Name));
#endif

			return instance;
		}
		protected set { instance = value; }
	}
}
