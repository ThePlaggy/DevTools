using UnityEngine;
using UnityEngine.SceneManagement;

public class BabyEnding : MonoBehaviour
{
	[SerializeField]
	private GameObject babyL;

	[SerializeField]
	private GameObject babyR;

	[SerializeField]
	private AudioClip babyClip;

	private bool readyToLeave;

	private bool mode;

	private void Start()
	{
		Change();
	}

	private void Change()
	{
		mode = !mode;
		babyL.SetActive(mode);
		babyR.SetActive(!mode);
		Invoke("Change", babyClip.length);
		Invoke("ReadyToLeave", 10f);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && readyToLeave)
		{
			readyToLeave = false;
			SceneManager.LoadScene(0);
		}
	}

	private void ReadyToLeave()
	{
		readyToLeave = true;
	}
}
