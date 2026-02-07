using UnityEngine;

public class DeepWebRadioManager : MonoBehaviour
{
	public static int currentClip;

	public static AudioSource RadioAS;

	public static int DWRTimeStamp;

	public static bool DWRDizzy;

	public static RADIO_TYPE radio;

	public static int currentClip2;

	public static AudioSource RadioAS2;

	private void Awake()
	{
		currentClip = Random.Range(0, 4);
		currentClip2 = Random.Range(0, 4);
		RadioAS = GameObject.Find("computerAudioHub").AddComponent<AudioSource>();
		RadioAS.volume = 0f;
		RadioAS.spatialBlend = 1f;
		RadioAS.dopplerLevel = 0f;
		RadioAS.clip = RadioLookUp.anonyjazz[currentClip];
		RadioAS.time = Random.Range(30f, 60f);
		RadioAS.Play();
		RadioAS2 = GameObject.Find("computerAudioHub").AddComponent<AudioSource>();
		RadioAS2.volume = 0f;
		RadioAS2.spatialBlend = 1f;
		RadioAS2.dopplerLevel = 0f;
		RadioAS2.clip = RadioLookUp.dsbm[currentClip2];
		RadioAS2.time = Random.Range(30f, 60f);
		RadioAS2.Play();
		radio = RADIO_TYPE.NONE;
	}

	private void Update()
	{
		UpdateDW1();
		UpdateDW2();
	}

	private void FixedUpdate()
	{
		if (RadioAS.volume >= 0.01f)
		{
			DWRTimeStamp++;
		}
		else if (RadioAS2.volume >= 0.01f)
		{
			DWRTimeStamp++;
		}
		else
		{
			DWRTimeStamp--;
		}
		if (DWRTimeStamp <= 0)
		{
			DWRTimeStamp = 0;
			toNoDizzy();
		}
		else if (DWRTimeStamp >= 30000)
		{
			DWRTimeStamp = 30000;
			ToDizzy();
		}
	}

	private void OnDestroy()
	{
		RadioAS = null;
		RadioAS2 = null;
	}

	private void ToDizzy()
	{
		if (!DWRDizzy)
		{
			DWRDizzy = true;
			TannerRoamJumper.Ins.SetDruggedPPVol(2.5f);
			Debug.Log("Applied DWR Dizzy");
		}
	}

	private void toNoDizzy()
	{
		if (DWRDizzy)
		{
			DWRDizzy = false;
			TannerRoamJumper.Ins.ClearPPVol();
			Debug.Log("De-applied DWR Dizzy");
		}
	}

	private void UpdateDW1()
	{
		if (!RadioAS.isPlaying)
		{
			currentClip++;
			if (currentClip >= RadioLookUp.anonyjazz.Length)
			{
				currentClip = 0;
			}
			RadioAS.clip = RadioLookUp.anonyjazz[currentClip];
			RadioAS.time = 0f;
			RadioAS.Play();
		}
		if (GameManager.PauseManager.Paused)
		{
			RadioAS.volume = 0f;
		}
		else if (radio == RADIO_TYPE.ANONYJAZZ && !ComputerMuteBehaviour.Ins.Muted)
		{
			RadioAS.volume = 0.33f;
		}
		else
		{
			RadioAS.volume = 0f;
		}
	}

	private void UpdateDW2()
	{
		if (!RadioAS2.isPlaying)
		{
			currentClip2++;
			if (currentClip2 >= RadioLookUp.dsbm.Length)
			{
				currentClip2 = 0;
			}
			RadioAS2.clip = RadioLookUp.dsbm[currentClip2];
			RadioAS2.time = 0f;
			RadioAS2.Play();
		}
		if (GameManager.PauseManager.Paused)
		{
			RadioAS2.volume = 0f;
		}
		else if (radio == RADIO_TYPE.DSBM && !ComputerMuteBehaviour.Ins.Muted)
		{
			RadioAS2.volume = 0.33f;
		}
		else
		{
			RadioAS2.volume = 0f;
		}
	}
}
