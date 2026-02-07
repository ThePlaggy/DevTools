using System;
using UnityEngine;

public static class EventSlinger
{
	public static bool AprilFoolsEvent { get; set; }

	public static bool EasterEvent { get; set; }

	public static bool HalloweenEvent { get; set; }

	public static bool ChristmasEvent { get; set; }

	public static bool NewYearsEvent { get; set; }

	public static void SetEvents()
	{
		AprilFoolsEvent = DateTime.Now.Month == 4 && DateTime.Now.Day == 1;
		EasterEvent = (DateTime.Now.Month == 3 && DateTime.Now.Day >= 22) || DateTime.Now.Month == 4 || (DateTime.Now.Month == 4 && DateTime.Now.Day <= 20);
		HalloweenEvent = (DateTime.Now.Month == 10 && DateTime.Now.Day >= 30) || (DateTime.Now.Month == 11 && DateTime.Now.Day == 1);
		ChristmasEvent = (DateTime.Now.Month == 12 && DateTime.Now.Day >= 19) || (DateTime.Now.Month == 1 && DateTime.Now.Day <= 7);
		NewYearsEvent = (DateTime.Now.Month == 12 && DateTime.Now.Day == 31) || (DateTime.Now.Month == 1 && DateTime.Now.Day == 1);
		Debug.Log($"[EventSlinger] Events initialized: AprilFools {AprilFoolsEvent}, Easter {EasterEvent}, Halloween {HalloweenEvent}, Christmas {ChristmasEvent}, NewYears {NewYearsEvent}");
	}
}
