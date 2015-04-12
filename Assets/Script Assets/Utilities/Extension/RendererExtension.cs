using UnityEngine;

public static class RendererExtensions
{
	/// <summary>
	/// Is the renderer visible from the specified camera?
	/// Note: This only performs frustum checks, occlusion by other renderers is not considered.
	/// Note: This uses the renderer's bounding box, and is therefore not 100% accurate.
	/// </summary>
	public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
		return renderer.IsVisibleFrom(planes);
	}

	public static bool IsVisibleFrom(this Renderer renderer, Plane[] frustumPlanes)
	{
		return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
	}
}
