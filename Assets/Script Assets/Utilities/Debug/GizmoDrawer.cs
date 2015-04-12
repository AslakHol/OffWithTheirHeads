using UnityEngine;

[ExecuteInEditMode]
public class GizmoDrawer : MonoBehaviour
{
	public Vector3 start;
	public Vector3 end;


	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(start, end);
		const float radius = 0.1f;
		Gizmos.DrawSphere(start, radius);
		Gizmos.DrawSphere(end, radius);
	}
}
