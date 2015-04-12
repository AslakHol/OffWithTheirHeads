using UnityEngine;

public class MonoBehaviourBase : MonoBehaviour
{
	/// <summary>
	/// Put everything that need to be reset inside of this function.
	/// </summary>
	public virtual void Revert(){}
}
