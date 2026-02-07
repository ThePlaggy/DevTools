using DG.Tweening;
using UnityEngine;

public class DancingTanner : MonoBehaviour
{
	public static DancingTanner dancingTanner;

	private Animator myAC;

	private void Awake()
	{
		base.gameObject.name = "DancingTanner";
		myAC = GetComponent<Animator>();
		BuildCustomPool();
	}

	public void Spawn(Vector3 SpawnPos, Vector3 SpawnRot)
	{
		base.gameObject.transform.position = SpawnPos;
		base.gameObject.transform.rotation = Quaternion.Euler(SpawnRot);
	}

	public void DeSpawn()
	{
		Spawn(new Vector3(0f, -20f, 0f), Vector3.zero);
		myAC.SetTrigger("Despawn");
	}

	public void TriggerDancing()
	{
		myAC.SetTrigger("Dance");
	}

	private void BuildCustomPool()
	{
		myAC.runtimeAnimatorController = CustomObjectLookUp.CustomTannerAC;
		myAC.avatar = CustomObjectLookUp.TannerTPoseAvatar;
		base.transform.Find("SK_Tanner_01").gameObject.SetActive(value: false);
		base.transform.Find("WorldJoint").gameObject.SetActive(value: false);
		base.transform.Find("SK_Tanner").gameObject.SetActive(value: true);
		base.transform.Find("mixamorig:Hips").gameObject.SetActive(value: true);
		base.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/CustomTannerHeadLight").gameObject.SetActive(value: false);
	}

	public static DancingTanner GetDancingTanner()
	{
		if (dancingTanner == null)
		{
			dancingTanner = Object.Instantiate(CustomObjectLookUp.TheTanner, GameObject.Find("EnemyPool").transform, worldPositionStays: true).AddComponent<DancingTanner>();
		}
		return dancingTanner;
	}

	public void Spawn(Vector3 POS, Vector3 ROT, Vector3 SCL)
	{
		base.gameObject.transform.position = POS;
		base.gameObject.transform.rotation = Quaternion.Euler(ROT);
		base.gameObject.transform.localScale = SCL;
	}

	public void DeSpawn2()
	{
		Spawn(new Vector3(0f, -20f, 0f), Vector3.zero, Vector3.one);
		myAC.SetTrigger("Despawn");
	}

	public void DOMoveTanner(Vector3 POS, Vector3 ROT, Vector3 SCL, float duration)
	{
		base.gameObject.transform.DOMove(POS, duration).SetEase(Ease.Linear);
		base.gameObject.transform.DORotate(POS, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		base.gameObject.transform.DOScale(SCL, duration).SetEase(Ease.Linear);
	}
}
