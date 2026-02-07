using System.Collections.Generic;
using UnityEngine;

public class MicManager : MonoBehaviour
{
	public delegate void MicActions();

	public delegate void MicFloatOut(int intValue);

	[Range(0.5f, 5.5f)]
	public float micWarmCheckTime = 3f;

	[Range(1f, 10f)]
	public int maxMicCheckCount = 5;

	public int currentDBLevel;

	private int avgCheckCount;

	private float avgDecibels = 100f;

	private bool checkForMic;

	private bool checkForSubDBLevel;

	private float DecibelRef = 0.1f;

	private float Decibels = -165f;

	private string defaultMic;

	private bool ignoreAvg;

	private bool listenToPlayer;

	private int micCheckCount;

	private List<string> micDevs;

	private string micDevToTest;

	private int micIndex;

	private float micTimeStamp;

	private AudioSource myAS;

	private float RMS;

	private float SampleSize = 1024f;

	private int subDBAvg;

	private float Sum;

	public event MicActions NoMic;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
