using System.Collections.Generic;
using UnityEngine;

public class AudioClipComparer : IEqualityComparer<AudioClip>
{
	public static AudioClipComparer Ins = new AudioClipComparer();

	public bool Equals(AudioClip x, AudioClip y)
	{
		return x == y;
	}

	public int GetHashCode(AudioClip obj)
	{
		return obj.GetHashCode();
	}
}
