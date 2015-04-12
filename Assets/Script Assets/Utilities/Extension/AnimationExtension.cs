using UnityEngine;

public static class AnimationExtension
{
	/// <summary>
	/// Resets (and disables) an animation state.
	/// This sets speed to 1, time to 0, weight to 0, and disabled the state.
	/// </summary>
	public static void Reset(this AnimationState animationState)
	{
		DebugUtils.Assert(animationState != null, "Parameter cannot be null.");
		animationState.speed = 1.0f;
		animationState.time = 0.0f;
		animationState.weight = 0.0f;
		animationState.enabled = false;
	}

	public static void GhettoPlay(this AnimationState animationState)
	{
		animationState.speed = 1.0f;
		animationState.time = 0.0f;
		animationState.weight = 1.0f;
		animationState.enabled = true;
	}

	/// <summary>
	/// Fades in the animation over some time. It uses Animation.CrossFade, but additionally
	/// ensures that the animation state is enabled and resets its time to 0.
	/// </summary>
	public static void CrossFade2(this Animation anim, AnimationState state,
		float fadeLength = 0.3f, PlayMode mode = PlayMode.StopSameLayer)
	{
		anim.CrossFade(state.name, fadeLength, mode);
		state.enabled = true;
		state.time = 0.0f;
	}
}
