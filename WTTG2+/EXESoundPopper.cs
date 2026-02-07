using UnityEngine;

public static class EXESoundPopper
{
	[Range(0f, 10000f)]
	public static int PoppedSounds;

	public static void PopSound(int amount)
	{
		PoppedSounds += amount;
	}
}
