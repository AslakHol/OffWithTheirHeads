using System.Collections.Generic;

public static class ListExtension
{
	/// <summary>
	/// Removes an element from a list without keeping the list in sorted order.
	/// The complexity of this operation is O(1) compared to List.RemoveAt's O(n).
	/// </summary>
	/// <param name="list">The list from which an element will be removed.</param>
	/// <param name="index">The index of the element to remove.</param>
	public static void UnstableRemoveAt<T>(this List<T> list, int index)
	{
		int lastElementIndex = list.Count - 1;
		list[index] = list[lastElementIndex];
		list.RemoveLast();
	}

	/// <summary>
	/// Removes the last element in a list. Throws if the list is empty.
	/// </summary>
	/// <param name="list">The list from which an element will be removed.</param>
	public static void RemoveLast<T>(this List<T> list)
	{
		list.RemoveAt(list.Count - 1);
	}

	/// <summary>
	/// This function makes it possible to write code like this:
	/// List&lt;string&gt; animals = ...;
	/// animals.AddRange("dog", "cat", "fish");
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <param name="objects"></param>
	public static void AddRange<T>(this List<T> list, params T[] objects)
	{
		list.AddRange(objects);
	}

	/// <summary>
	/// Get a random element from a list.
	/// </summary>
	public static T GetRandomElement<T>(this System.Collections.Generic.List<T> list)
	{
		if (list.Count == 0)
		{
			return default(T);
		}

		return list[UnityEngine.Random.Range(0, list.Count)];
	}
}
