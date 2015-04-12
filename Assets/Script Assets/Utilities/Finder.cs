using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Find
{
#if UNITY_EDITOR
	public static T[] InProjectAndScene<T>() where T : Object
	{
		return (T[])Resources.FindObjectsOfTypeAll(typeof(T));
	}

	public static List<T> AllObjectsInScene<T>() where T : Component
	{
		T[] allObjects = Find.InProjectAndScene<T>();

		List<T> objects = new List<T>();

		foreach (T t in allObjects)
		{
			if (t.hideFlags == HideFlags.NotEditable || t.hideFlags == HideFlags.HideAndDontSave ||
				t.gameObject.hideFlags == HideFlags.NotEditable || t.gameObject.hideFlags == HideFlags.HideAndDontSave)
			{
				continue;
			}

			if (AssetDatabase.Contains(t.transform.root.gameObject))
			{
				//The thing is an asset/prefab etc, not in the scene.
				continue;
			}

			objects.Add(t);
		}

		return objects;
	}

	public static GameObject[] AllGameObjectsInScene()
	{
		return AllObjectsInScene<Transform>().Select(t => t.gameObject).ToArray();
	}
#else
	public static GameObject[] AllGameObjectsInScene()
	{
		return InScene<Transform>().Select(t => t.gameObject).ToArray();
	}
#endif

	/// <summary>
	/// Find all instances of type in the scene. Disabled instances are not found.
	/// </summary>
	/// <typeparam name="T">Type of component (or GameObject) to find.</typeparam>
	public static T[] InScene<T>() where T : Object
	{
		return (T[])GameObject.FindObjectsOfType(typeof(T));
	}

	/// <summary>
	/// Find one instance of type in the scene. Disabled instances are not found.
	/// </summary>
	/// <typeparam name="T">Type of component (or GameObject) to find.</typeparam>
	public static T FirstInScene<T>() where T : Object
	{
		return (T)GameObject.FindObjectOfType(typeof(T));
	}

	public static T ByName<T>(string gameObjectName) where T : Component
	{
		GameObject go = GameObject.Find(gameObjectName);
		if (go == null)
		{
			return null;
		}
		return go.GetComponent<T>();
	}
}
