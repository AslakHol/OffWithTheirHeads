using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;


/// <summary>
/// This class exposes some of the functionality Unity's built-in animation editor window.
/// The UnityEditor.AnimationWindow class is internal, and completely undocumented, and
/// probably not meant to be used this way. 
/// Because of this, reflection is required to communicate with it. This also means that
/// future updates to Unity may break this class.
/// </summary>
public static class AnimationWindow
{
	private const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance
		| BindingFlags.Public;

	/// <summary>
	/// The type of the animation window.
	/// </summary>
	public static Type AnimationWindowType
	{ get { return typeof(Editor).Assembly.GetType("UnityEditor.AnimationWindow"); } }

	private static EditorWindow cachedWindow;
	/// <summary>
	/// A reference to the animation window if it is open, null otherwise.
	/// </summary>
	public static EditorWindow Window
	{
		get
		{
			if (cachedWindow == null)
			{
				UnityEngine.Object[] animationWindows = Resources.FindObjectsOfTypeAll(AnimationWindowType);
				if (animationWindows.Length != 0)
				{
					cachedWindow = (EditorWindow)animationWindows[0];
				}
			}
			return cachedWindow;
		}
	}

	/// <summary>
	/// Is the animation window open?
	/// </summary>
	public static bool IsWindowOpen { get { return Window != null; } }

	/// <summary>
	/// Is auto recording enabled?
	/// This is the same value as the red record button in the window.
	/// </summary>
	public static bool IsAutoRecording
	{
		get
		{
			FieldInfo field = AnimationWindowType.GetField("m_AutoRecord", bindingFlags);
			return (bool)field.GetValue(Window);
		}
	}

	/// <summary>
	/// Get or set the current frame of the selected animation.
	/// </summary>
	public static int Frame
	{
		get
		{
			FieldInfo field = AnimationWindowType.GetField("m_Frame", bindingFlags);
			return (int)field.GetValue(Window);
		}
		set
		{
			PreviewFrame(value);
		}
	}

	/// <summary>
	/// Get or set the current time of the selected animation.
	/// </summary>
	public static float Time
	{
		get
		{
			return FrameToTime(Frame);
		}
		set
		{
			Frame = (int)TimeToFrame(value);
		}
	}

	/// <summary>
	/// Get the current selection. Type: UnityEditor.AnimationSelection
	/// </summary>
	private static object Selected
	{
		get
		{
			if (Window == null)
			{
				return null;
			}

			FieldInfo selectedField = AnimationWindowType.GetField("m_Selected", bindingFlags);
			object[] selectedAnimations = selectedField.GetValue(Window) as object[];
			if (selectedAnimations != null && selectedAnimations.Length != 0)
			{
				return selectedAnimations[0];
			}
			return null;
		}
	}

	/// <summary>
	/// Get a reference to the currently active animation clip, or null if there is none.
	/// </summary>
	public static AnimationClip CurrentAnimationClip
	{
		get
		{
			object selected = Selected;
			FieldInfo clipField = selected.GetType().GetField("m_Clip", bindingFlags);
			AnimationClip clip = (AnimationClip)clipField.GetValue(selected);
			return clip;
		}
	}

	public static GameObject AnimatedObject
	{
		get
		{
			object selected = Selected;
			if (selected == null)
			{
				return null;
			}
			PropertyInfo animatedObjectProperty = selected.GetType().GetProperty("animatedObject", bindingFlags);
			GameObject animatedObject = (GameObject)animatedObjectProperty.GetValue(selected, null);
			return animatedObject;
		}
	}

	private static float FrameToTime(float frame)
	{
		MethodInfo method = AnimationWindowType.GetMethod("FrameToTime", bindingFlags);
		return (float)method.Invoke(Window, new object[] { frame });
	}

	private static float TimeToFrame(float time)
	{
		MethodInfo method = AnimationWindowType.GetMethod("TimeToFrame", bindingFlags);
		return (float)method.Invoke(Window, new object[] { time });
	}

	private static void PreviewFrame(int frame)
	{
		MethodInfo method = AnimationWindowType.GetMethod("PreviewFrame", bindingFlags);
		method.Invoke(Window, new object[] { frame });
	}
}
