using DG.Tweening;
using UnityEngine;

public class SecretManager : MonoBehaviour
{
	[SerializeField]
	private AudioFileDefinition SecretMusic;

	[SerializeField]
	private DOTweenAnimation[] HypeAnis;

	[SerializeField]
	private CanvasGroup PauseScreenCG;

	private DOTweenAnimation currentHypeAni;

	private void Awake()
	{
		GameManager.PauseManager.GamePaused += gameWasPaused;
		GameManager.PauseManager.GameUnPaused += gameWasUnPaused;
	}

	private void Start()
	{
		if (!(SecretController.Ins != null))
		{
			return;
		}
		GameManager.TimeSlinger.FireTimer(0.5f, delegate
		{
			PauseManager.UnLockPause();
			SecretController.Ins.Release();
			if (PostCreditsManager.megasex)
			{
				GameObject.Find("inthewaiting").GetComponent<AudioSource>().Play();
			}
			else
			{
				GameManager.AudioSlinger.PlaySound(SecretMusic);
				ayanaRemover();
				rollHypeText();
			}
		});
		GameManager.TimeSlinger.FireTimer(3f, delegate
		{
			if (SteamSlinger.Ins != null)
			{
				SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.GOD_GAMER);
			}
			StatisticsManager.Ins.BeatRun(Difficulty.LEET);
		});
	}

	public void HypeTextEnd()
	{
		currentHypeAni.DORewind();
		currentHypeAni.gameObject.SetActive(value: false);
		float duration = Random.Range(0.5f, 8f);
		GameManager.TimeSlinger.FireTimer(duration, rollHypeText);
	}

	private void rollHypeText()
	{
		int num = Random.Range(0, HypeAnis.Length);
		currentHypeAni = HypeAnis[num];
		currentHypeAni.gameObject.SetActive(value: true);
		currentHypeAni.DOPlay();
	}

	private void gameWasPaused()
	{
		if (!PostCreditsManager.megasex)
		{
			PauseScreenCG.alpha = 1f;
		}
	}

	private void gameWasUnPaused()
	{
		if (!PostCreditsManager.megasex)
		{
			PauseScreenCG.alpha = 0f;
		}
	}

	private void ayanaRemover()
	{
		if (TenantTrackManager.DidAyana)
		{
			Object.Destroy(GameObject.Find("NympSecretSending"));
		}
		GameObject gameObject = Object.Instantiate(CustomObjectLookUp.TheTanner);
		gameObject.transform.position = new Vector3(3.7773f, 0f, -1.0261f);
		gameObject.transform.rotation = Quaternion.Euler(0f, 313.6545f, 0f);
		GameObject.Find("TannerHeadLight").SetActive(value: false);
		gameObject.GetComponent<Animator>().runtimeAnimatorController = CustomObjectLookUp.CustomTannerAC;
		gameObject.GetComponent<Animator>().avatar = CustomObjectLookUp.TannerTPoseAvatar;
		gameObject.transform.Find("SK_Tanner_01").gameObject.SetActive(value: false);
		gameObject.transform.Find("WorldJoint").gameObject.SetActive(value: false);
		gameObject.transform.Find("SK_Tanner").gameObject.SetActive(value: true);
		gameObject.transform.Find("mixamorig:Hips").gameObject.SetActive(value: true);
		gameObject.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/CustomTannerHeadLight").gameObject.SetActive(value: false);
		GameObject gameObject2 = Object.Instantiate(CustomObjectLookUp.ExecutionerCustomRig);
		gameObject2.transform.position = new Vector3(-2.8809f, 0f, -0.0625f);
		gameObject2.transform.rotation = Quaternion.Euler(0f, 51.7272f, 0f);
		gameObject.GetComponent<Animator>().SetTrigger("Dance");
		gameObject2.GetComponent<Animator>().SetBool("Dancing", value: true);
		gameObject.AddComponent<BoxCollider>();
		gameObject2.AddComponent<BoxCollider>();
	}
}
