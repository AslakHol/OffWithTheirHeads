using System;
using System.Diagnostics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Debug = UnityEngine.Debug;


/// <summary>
/// Contains various functions for debugging which will be removed by the compiler when
/// building a non-editor build.
/// </summary>
public static class DebugUtils
{
	private const int LineCircleSegments = 16;
	private const float LineCircleAngleDelta = 180.0f / LineCircleSegments;

	[Serializable]
	public class RuntimeError : Exception
	{
		public RuntimeError(string message) : base(message) { }

		public override string StackTrace
		{
			get
			{
				//This is thrown from the Assert function, so remove its entry in the
				//stack trace to make double clicking the log take the user to the line
				//the assert is on rather than to the assert function itself.
				try
				{
					string fullStackTrace = base.StackTrace;
					int indexOfNewLine = fullStackTrace.IndexOf(Environment.NewLine);
					if (indexOfNewLine != -1)
					{
						return fullStackTrace.Substring(indexOfNewLine);
					}
					//In the extremely unlikely case that the stack trace contains no newlines, we
					//simply return the whole stack trace.
					return fullStackTrace;
				}
				catch (Exception ex)
				{
					string stackTrace = base.StackTrace;
					UnityEngine.Debug.LogError("Exception was thrown when modifying stack trace: "
						+ ex + "\nCurrent stack trace: " + stackTrace);
					return stackTrace;
				}
			}
		}
	}

	[Serializable]
	public class AssertionFailedException : RuntimeError
	{
		public AssertionFailedException(string message) : base(message) { }
	}


	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, string message)
	{
		if (!condition)
		{
#if BREAK_ON_ASSERT
			UnityEngine.Debug.Break();
#endif
			throw new AssertionFailedException(message);
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, string message, params object[] args)
	{
		if (!condition)
		{
#if BREAK_ON_ASSERT
			UnityEngine.Debug.Break();
#endif
			throw new AssertionFailedException(string.Format(message, args));
		}
	}


	/// <summary>
	/// Draws a bunch of lines intersecting a point, resulting in a sphere-like shape being drawn.
	/// </summary>
	/// <param name="point">The center point which all lines will intersect.</param>
	/// <param name="color">The color of the lines.</param>
	/// <param name="radius">The radius of the sphere, decides the length of individual lines.</param>
	/// <param name="duration">The duration for which the sphere should be drawn, in seconds.
	/// If left at default (0), it will only be drawn for one frame.</param>
	public static void DrawLineSphereAroundPoint(Vector3 point, Color color, float radius = 0.1f,
		float duration = 0.0f, bool depthTest = true)
	{
		for (int i = 0; i < LineCircleSegments; ++i)
		{
			Vector3 offset = Quaternion.AngleAxis(i * LineCircleAngleDelta, Vector3.up) * new Vector3(0.0f, 0.0f, radius);
			UnityEngine.Debug.DrawLine(point + offset, point - offset, color, duration, depthTest);
		}
		for (int i = 0; i < LineCircleSegments; ++i)
		{
			Vector3 offset = Quaternion.AngleAxis(i * LineCircleAngleDelta, Vector3.forward) * new Vector3(0.0f, 0.0f, radius);
			UnityEngine.Debug.DrawLine(point + offset, point - offset, color, duration, depthTest);
		}
		for (int i = 0; i < LineCircleSegments; ++i)
		{
			Vector3 offset = Quaternion.AngleAxis(i * LineCircleAngleDelta, Vector3.left) * new Vector3(0.0f, 0.0f, radius);
			UnityEngine.Debug.DrawLine(point + offset, point - offset, color, duration, depthTest);
		}
	}

#if UNITY_EDITOR
	public static void DrawLineSphereAroundPointHandles(Vector3 point, Color color, float radius = 0.1f)
	{
		Color originalColor = Handles.color;
		Handles.color = color;

		for (int i = 0; i < LineCircleSegments; ++i)
		{
			Vector3 offset = Quaternion.AngleAxis(i * LineCircleAngleDelta, Vector3.up) * new Vector3(0.0f, 0.0f, radius);
			Handles.DrawLine(point + offset, point - offset);
		}
		for (int i = 0; i < LineCircleSegments; ++i)
		{
			Vector3 offset = Quaternion.AngleAxis(i * LineCircleAngleDelta, Vector3.forward) * new Vector3(0.0f, 0.0f, radius);
			Handles.DrawLine(point + offset, point - offset);
		}
		for (int i = 0; i < LineCircleSegments; ++i)
		{
			Vector3 offset = Quaternion.AngleAxis(i * LineCircleAngleDelta, Vector3.left) * new Vector3(0.0f, 0.0f, radius);
			Handles.DrawLine(point + offset, point - offset);
		}

		Handles.color = originalColor;
	}
#endif

	public static void DrawAxis(Vector3 position, Quaternion axisRotation,
		float duration = 0.0f, bool depthTest = true, float lineLength = 1.0f)
	{
		Debug.DrawRay(position, axisRotation * Vector3.up * lineLength, Color.green, duration, depthTest);
		Debug.DrawRay(position, axisRotation * Vector3.right * lineLength, Color.red, duration, depthTest);
		Debug.DrawRay(position, axisRotation * Vector3.forward * lineLength, Color.blue, duration, depthTest);
	}
}
