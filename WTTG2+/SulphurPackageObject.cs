using UnityEngine;

public class SulphurPackageObject : MonoBehaviour
{
	public static SulphurPackageObject Ins;

	[HideInInspector]
	public GameObject[] packages = new GameObject[5];

	private void Awake()
	{
		Ins = this;
		if (!DifficultyManager.HackerMode)
		{
			for (int i = 0; i < packages.Length; i++)
			{
				packages[i] = Object.Instantiate(CustomObjectLookUp.PackageBox);
			}
			packages[0].transform.position = new Vector3(-0.808f, 40.648f, -0.224f);
			packages[1].transform.position = new Vector3(-0.813f, 40.849f, -0.224f);
			packages[2].transform.position = new Vector3(-0.794f, 41.049f, -0.224f);
			packages[3].transform.position = new Vector3(-0.578f, 40.687f, -0.224f);
			packages[4].transform.position = new Vector3(-0.632f, 40.654f, -0.013f);
			packages[2].transform.Rotate(new Vector3(0f, 0f, -1.13f));
			packages[3].transform.Rotate(new Vector3(0f, 0f, -84.25f));
			packages[2].transform.Rotate(new Vector3(85.98f, 0f, 0f));
			for (int j = 0; j < packages.Length; j++)
			{
				packages[j].transform.localScale = new Vector3(0.413f, 0.323f, 0.35f);
				packages[j].SetActive(value: false);
			}
		}
	}

	public void UpdateSulphurPackages()
	{
		if (!DifficultyManager.HackerMode)
		{
			packages[0].SetActive(SulphurInventory.SulphurAmount >= 1);
			packages[1].SetActive(SulphurInventory.SulphurAmount >= 2);
			packages[2].SetActive(SulphurInventory.SulphurAmount >= 3);
			packages[3].SetActive(SulphurInventory.SulphurAmount >= 4);
			packages[4].SetActive(SulphurInventory.SulphurAmount >= 5);
		}
	}
}
