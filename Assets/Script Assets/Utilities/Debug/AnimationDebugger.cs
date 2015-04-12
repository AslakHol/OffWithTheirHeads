using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class AnimationDebugger
{
	public struct AnimationAndClippedName
	{
		public AnimationState animationState;
		public string clippedAnimationName;
	}

	private readonly StringBuilder stringBuilder = new StringBuilder();
	private List<AnimationAndClippedName> animStates = new List<AnimationAndClippedName>();
	private readonly Comparer2 comparer = new Comparer2();


	private class Comparer2
	{
		public Comparison<AnimationState> Comparer { get; set; }

		public int Compare(AnimationAndClippedName lhs, AnimationAndClippedName rhs)
		{
			return Comparer.Invoke(lhs.animationState, rhs.animationState);
		}
	}

	private class Comparer3 : IComparer<AnimationState>
	{
		private Comparer2 comp;

		public Comparer3(Comparer2 comp)
		{
			this.comp = comp;
		}

		public int Compare(AnimationState lhs, AnimationState rhs)
		{
			return comp.Comparer.Invoke(lhs, rhs);
		}
	}

	private UnityEngine.Animation animation;
	public UnityEngine.Animation Animation
	{
		get
		{
			return animation;
		}
		set
		{
			animation = value;
			AddAndFilterAnimations();
		}
	}

	private string animationNameFilter;
	public string AnimationNameFilter
	{
		get { return animationNameFilter; }
		set
		{
			//Re-add animations to list, using filter.
			animStates.Clear();

			animationNameFilter = value;
			AddAndFilterAnimations();
		}
	}
	public Comparison<AnimationState> SortPredicate
	{
		get { return comparer.Comparer; }
		set
		{
			comparer.Comparer = value;
		}
	}

	public List<AnimationAndClippedName> AnimationStates { get { return animStates; } }

	public bool EnabledAnimationsOnly { get; set; }

	public bool PlayingAnimationsOnly { get; set; }

	public bool ContributingOnly { get; set; }

	public bool HighPrecisionOutput { get; set; }

	public string NamePredicate { get; set; }

	public enum Match
	{
		All,
		One,
	}
	public Match NamePredicateMatching = Match.All;

	public AnimationDebugger(UnityEngine.Animation animation)
	{
		this.animation = animation;

		//Default to lexicographic comparison.
		SortPredicate = (AnimationState lhs, AnimationState rhs)
			=> String.CompareOrdinal(lhs.name, rhs.name);

		//Default to no filter.
		animationNameFilter = string.Empty;

		EnabledAnimationsOnly = true;
		ContributingOnly = true;

		NamePredicate = string.Empty;
	}
	
	private static string ClipAnimationName(string fullAnimationName, int maxNameSize = 22)
	{
		if (fullAnimationName.Length > maxNameSize)
		{
			return fullAnimationName.Substring(0, maxNameSize);
		}
		else if (fullAnimationName.Length < maxNameSize)
		{
			return fullAnimationName.PadRight(maxNameSize);
		}
		else
		{
			return fullAnimationName;
		}
	}

	private void AddAndFilterAnimations()
	{
		if (animationNameFilter != string.Empty)
		{
			foreach (AnimationState s in from AnimationState s in Animation
				where s.name.ToLower().Contains(animationNameFilter) select s)
			{
				animStates.Add(new AnimationAndClippedName
				{
					animationState = s,
					clippedAnimationName = ClipAnimationName(s.name)
				});
			}
		}
		else
		{
			foreach (AnimationState s in Animation)
			{
				animStates.Add(new AnimationAndClippedName
				{
					animationState = s,
					clippedAnimationName = ClipAnimationName(s.name)
				});
			}
		}

		SortAnimationStates();
	}

	private void SortAnimationStates()
	{
		if (comparer != null)
		{
			//Can't use List.Sort here since it's not stable, which would cause elements
			//to jump around on the generated list from frame to frame.
			var sortedAnimations = animStates.OrderBy((AnimationAndClippedName anim) =>
			{
				return anim.animationState;
			}, new Comparer3(comparer));

			animStates = new List<AnimationAndClippedName>(sortedAnimations);
		}
	}

	private bool MatchesFilters(AnimationState animationState)
	{
		if (EnabledAnimationsOnly && !animationState.enabled)
		{
			return false;
		}
		if (PlayingAnimationsOnly && !Animation.IsPlaying(animationState.name))
		{
			return false;
		}
		if (ContributingOnly && animationState.weight < 0.001f)
		{
			return false;
		}

		if (NamePredicateMatching == Match.All)
		{
			foreach (string word in NamePredicate.Split(' '))
			{
				if (!animationState.name.ToLower().Contains(word))
				{
					//Found a word the name didn't match, and needs to match all words.
					return false;
				}
			}
		}
		else
		{
			bool foundMatch = false;
			foreach (string word in NamePredicate.Split(' '))
			{
				if (animationState.name.ToLower().Contains(word))
				{
					foundMatch = true;
				}
			}
			if (!foundMatch)
			{
				//Found no matching words.
				return false;
			}
		}

		return true;
	}

	public AnimationState[] GetSortedAnimationStates()
	{
		if (animation == null)
		{
			return new AnimationState[0];
		}
		if (animStates.Count == 0)
		{
			AddAndFilterAnimations();
		}
		animStates.RemoveAll(state => state.animationState == null);

		SortAnimationStates();

		return animStates
			.Select(anim => anim.animationState)
			.Where(MatchesFilters)
			.ToArray();
	}

	public void Get(ref List<string> strings)
	{
		SortAnimationStates();

		//Some weird combination of entering/leaving play mode resets the list of animations, so
		//have to re-add them if that happens.
		if (animStates.Count == 0)
		{
			AddAndFilterAnimations();
		}

		for (int i = 0; i < animStates.Count; ++i)
		{
			GetForID(i, ref strings);
		}
	}

	public void GetForID(int id, ref List<string> strings)
	{
		AnimationState animationState = animStates[id].animationState;
		if (EnabledAnimationsOnly && !animationState.enabled)
		{
			return;
		}
		if (PlayingAnimationsOnly && !Animation.IsPlaying(animationState.name))
		{
			return;
		}
		if (ContributingOnly && animationState.weight < 0.001f)
		{
			return;
		}

		if (NamePredicateMatching == Match.All)
		{
			foreach (string word in NamePredicate.Split(' '))
			{
				if (!animationState.name.ToLower().Contains(word))
				{
					return;
				}
			}
		}
		else
		{
			bool foundMatch = false;
			foreach (string word in NamePredicate.Split(' '))
			{
				if (animationState.name.ToLower().Contains(word))
				{
					foundMatch = true;
				}
			}
			if (!foundMatch)
			{
				return;
			}
		}

		string clippedAnimationName = animStates[id].clippedAnimationName;

		strings.Add(clippedAnimationName);
		strings.Add(animationState.enabled ? "True" : "False");
		strings.Add(Animation.IsPlaying(animationState.name) ? "True" : "False");
		strings.Add(animationState.normalizedSpeed.ToString(HighPrecisionOutput ? "N5" : "N2"));
		strings.Add(animationState.weight.ToString(HighPrecisionOutput ? "N5" : "N2"));
		strings.Add(animationState.normalizedTime.ToString(HighPrecisionOutput ? "N5" : "N2"));
		strings.Add(animationState.layer.ToString());
	}

	/// <summary>
	/// Generates a debug string representing information about all animations matching
	/// the name filter, in sorted order using the sorting predicate (lexicographically
	/// by default).
	/// </summary>
	/// <returns></returns>
	public string GenerateDebugString()
	{
		SortAnimationStates();

		stringBuilder.Length = 0;
		for (int i = 0; i < animStates.Count; ++i)
		{
			//animationStates2[i]
			AnimationState animationState = animStates[i].animationState;
			if (EnabledAnimationsOnly && !animationState.enabled)
			{
				continue;
			}
			if (PlayingAnimationsOnly && !Animation.IsPlaying(animationState.name))
			{
				continue;
			}
			string clippedAnimationName = animStates[i].clippedAnimationName;

			//Output animation stats with the following format:
			//[name   ] enabled: 1, speed: 0.44, weight: 1.00, time: 0.73
			stringBuilder.AppendFormat("[{0}] enabled: {1}, playing: {2}, speed: {3:N2}, weight: {4:N2}, time: {5:N2}, layer: {6}\n",
				clippedAnimationName, animationState.enabled ? 1 : 0, Animation.IsPlaying(animationState.name) ? 1 : 0,
				animationState.normalizedSpeed, animationState.weight, animationState.normalizedTime, animationState.layer);
		}
		/*
		foreach (AnimationState state in animationStates)
		{
			//Make every animation name the same length to make the stats aligned properly.
			string clippedAnimationName = state.name;

			if (clippedAnimationName.Length > nameMaxSize)
			{
				clippedAnimationName = clippedAnimationName.Substring(0, nameMaxSize);
			}
			else if (clippedAnimationName.Length < nameMaxSize)
			{
				clippedAnimationName = clippedAnimationName.PadRight(nameMaxSize);
			}

			//Output animation stats with the following format:
			//[name   ] enabled: 1, speed: 0.44, weight: 1.00, time: 0.73
			stringBuilder.AppendFormat("[{0}] enabled: {1}, playing: {2}, speed: {3}, weight: {4}, time: {5}\n",
				clippedAnimationName, state.enabled ? 1 : 0, animation.IsPlaying(clippedAnimationName) ? 1 : 0,
				state.normalizedSpeed.ToString("N2"), state.weight.ToString("N2"),
				state.normalizedTime.ToString("N2"));
		}
		*/
		return stringBuilder.ToString();
	}
}
