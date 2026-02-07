using DG.Tweening;
using UnityEngine;

public class AdamLOLHook : MonoBehaviour
{
	public static AdamLOLHook Ins;

	[SerializeField]
	private SkinnedMeshRenderer[] Renderes = new SkinnedMeshRenderer[0];

	private Vector3 spawnPOS = new Vector3(3.266f, 39.582f, -1.240706f);

	private Vector3 spawnROT = new Vector3(0f, 180f, 0f);

	private void Awake()
	{
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.Euler(Vector3.zero);
		for (int i = 0; i < Renderes.Length; i++)
		{
			Renderes[i].enabled = false;
		}
		Ins = this;
	}

	public void Spawn()
	{
		base.transform.localPosition = spawnPOS;
		base.transform.localRotation = Quaternion.Euler(spawnROT);
		for (int i = 0; i < Renderes.Length; i++)
		{
			Renderes[i].enabled = true;
		}
	}

	public void DeSpawn()
	{
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.Euler(Vector3.zero);
		for (int i = 0; i < Renderes.Length; i++)
		{
			Renderes[i].enabled = false;
		}
	}

	public void SpawnAt(Vector3 POS, Vector3 ROT, Vector3 SCL)
	{
		base.transform.localPosition = POS;
		base.transform.localRotation = Quaternion.Euler(ROT);
		base.transform.localScale = SCL;
		for (int i = 0; i < Renderes.Length; i++)
		{
			Renderes[i].enabled = true;
		}
	}

	public void DOMoveMe(Vector3 POS, Vector3 ROT, Vector3 SCL, float duration)
	{
		base.transform.DOMove(POS, duration).SetEase(Ease.Linear);
		base.transform.DORotate(ROT, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		base.transform.DOScale(SCL, duration).SetEase(Ease.Linear);
	}

	public GameObject InstantiateAt(Vector3 POS, Vector3 ROT)
	{
		for (int i = 0; i < Renderes.Length; i++)
		{
			Renderes[i].enabled = true;
		}
		GameObject gameObject = Object.Instantiate(base.gameObject, base.transform.parent, worldPositionStays: true);
		gameObject.transform.position = POS;
		gameObject.transform.position = ROT;
		return gameObject;
	}
}
