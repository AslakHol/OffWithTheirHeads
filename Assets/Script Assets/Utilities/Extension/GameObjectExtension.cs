using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension
{
	/// <summary>
	/// Does the game object have the specified tag?
	/// The parameter can contain a single tag, or multiple tags, in which case true is only
	/// returned if it contains all of the tags.
	/// </summary>
	/// <param name="tag">The tag to check for. This can also be a combination of tags.</param>
	/// <returns>True if the game object has the specified tag(s), false otherwise.</returns>
	public static bool HasTag(this Component component, Tag tag)
	{
		MultiTag multiTag = component.GetComponent<MultiTag>();
		return multiTag != null && multiTag.HasTag(tag);
	}

	/// <summary>
	/// Returns the full hierarchy path of the game object.
	/// For example, an object called "Box" with a parent called "Environment" will return
	/// "Environment/Box".
	/// </summary>
	public static string GetFullPath(this Transform trans)
	{
		Stack<string> hierarchy = new Stack<string>();
		while (trans != null)
		{
			hierarchy.Push(trans.name);
			trans = trans.parent;
		}

		return string.Join("/", hierarchy.ToArray());
	}

	/// <summary>
	/// Get a list of all the children of this tramsform.
	/// Only immediate children are returned (no children of children etc).
	/// </summary>
	public static List<Transform> GetAllChildren(this Transform t)
	{
		List<Transform> children = new List<Transform>(t.childCount);
		for (int i = 0; i < t.childCount; i++)
		{
			children.Add(t.GetChild(i));
		}
		return children;
	}
}
