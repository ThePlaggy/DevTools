using UnityEngine;

public class TrophyManager : MonoBehaviour
{
	[SerializeField]
	private GameObject bronzeHackerModeTrophy;

	[SerializeField]
	private GameObject silverHackerModeTrophy;

	[SerializeField]
	private GameObject goldHackerModeTrophy;

	public GameObject nightmareTrophy;

	public GameObject christmasTrophy;

	public GameObject easterTrophy;

	public GameObject halloweenTrophy;

	public static TrophyManager Ins;

	private void Awake()
	{
		Ins = this;
		base.transform.position = new Vector3(-6.4f, 39.6f, -4.6f);
		RefreshTrophies();
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public void RefreshTrophies()
	{
		nightmareTrophy.SetActive(PlayerPrefs.HasKey("[Stats]NightmareBeaten"));
		christmasTrophy.SetActive(PlayerPrefs.HasKey("EventXMASTrophy"));
		easterTrophy.SetActive(PlayerPrefs.HasKey("EventEasterTrophy"));
		halloweenTrophy.SetActive(PlayerPrefs.HasKey("EventHalloTrophy"));
		bronzeHackerModeTrophy.SetActive(PlayerPrefs.HasKey("HackermodeTrophy1"));
		silverHackerModeTrophy.SetActive(PlayerPrefs.HasKey("HackermodeTrophy2"));
		goldHackerModeTrophy.SetActive(PlayerPrefs.HasKey("HackermodeTrophy3"));
	}
}
