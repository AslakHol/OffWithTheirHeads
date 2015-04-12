using System;
using UnityEngine;


[Flags]
public enum Tag
{
	Climbable = 1 << 0,
	Pushable = 1 << 1,
	Drawer = 1 << 2,
	GravityGunPickup = 1 << 3, // Can be held with gravity gun.
	GravityGunNoRotate = 1 << 4, // Will not be rotated by gravity gun.
	Pillow = 1 << 5, // Needs special treatment by gravity gun.
	DynamicWalkable = 1 << 6, // This is a dynamic object the player can walk on, i.e. level 2 seesaw.
	SleepOnStart = 1 << 7, // This game object's rigid body is immediately put to sleep when starting.
}

public class MultiTag : MonoBehaviour
{
	public Tag tags;
	public Vector3 startRotation;

	private void Start()
	{
		if (HasTag(Tag.SleepOnStart))
		{
			DebugUtils.Assert(this.GetComponent<Rigidbody>() != null, "Must have a rigid body if using the tag 'SleepOnStart'.");
			this.GetComponent<Rigidbody>().Sleep();
		}
		var rb = GetComponent<Rigidbody>();

		if(rb != null)
		{
			startRotation = rb.transform.rotation.eulerAngles;
		}
	}

#if UNITY_EDITOR
	private void OnEnable()
	{
		DebugUtils.Assert(this.GetComponents<MultiTag>().Length == 1,
			string.Format("'{0}' has multiple MultiTags!", this.name));

		if (HasTag(Tag.GravityGunPickup | Tag.Pushable))
		{
			Debug.LogWarning(string.Format("'{0}' is tagged with both gravity gun pickup and pushable."
				+ " This is probably not intended as the two systems will cause conflicts", this.name),
				gameObject);
		}
	}
#endif

#if !HACKS
	void LateUpdate()
	{
		if (HasTag(Tag.Pushable))
		{
			var rb = GetComponent<Rigidbody>();
			if (rb != null)
			{
				var constraints = rb.constraints;
				if ( (rb.constraints & RigidbodyConstraints.FreezeRotationX) != 0 ||
					 (rb.constraints & RigidbodyConstraints.FreezeRotationY) != 0 ||
					 (rb.constraints & RigidbodyConstraints.FreezeRotationZ) != 0)
				{
					rb.angularVelocity = Vector3.zero;
				}
			}
		}
	}
#endif


	/// <summary>
	/// Does the game object have the specified tag?
	/// The parameter can contain a single tag, or multiple tags, in which case true is only
	/// returned if it contains all of the tags.
	/// </summary>
	/// <param name="tag">The tag to check for. This can also be a combination of tags.</param>
	/// <returns>True if the game object has the specified tag(s), false otherwise.</returns>
	public bool HasTag(Tag tag)
	{
		return (tags & tag) == tag;
	}

	/// <summary>
	/// Adds the specified tag to the current tags.
	/// If the game object has the specified tag, no change occurs.
	/// </summary>
	/// <param name="tagToAdd">The tag to add.</param>
	public void AddTag(Tag tagToAdd)
	{
		tags |= tagToAdd;
	}

	/// <summary>
	/// Removes the specified tag.
	/// If the game object does not have the specified tag, no change occurs.
	/// This can remove multiple tags at once if the parameter is a combination of multiple tags.
	/// </summary>
	/// <param name="tagToRemove">The tag(s) which should be removed.</param>
	public void RemoveTag(Tag tagToRemove)
	{
		DebugUtils.Assert(HasTag(tagToRemove), "Trying to remove a tag which the this game"
			+ " object does not have.");

		tags &= ~tagToRemove;
	}
}
