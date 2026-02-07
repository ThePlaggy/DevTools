using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DancingLoader : MonoBehaviour
{
	public static DancingLoader Ins;

	public GameObject MaleCult;

	public GameObject FemaleCult;

	private Transform DancePool;

	private List<GameObject> instances = new List<GameObject>();

	[NonSerialized]
	public GameObject DancingBreather;

	[NonSerialized]
	public GameObject noirChips;

	[NonSerialized]
	public GameObject shitman;

	[NonSerialized]
	public GameObject chipman;

	[NonSerialized]
	public GameObject dancingExecutioner;

	[NonSerialized]
	public GameObject dude;

	private void Awake()
	{
		Ins = this;
		DancePool = new GameObject("DancePool").transform;
		BuildMaleCult();
		BuildFemaleCult();
		BuildDancingBreather();
		BuildChipsNoir();
		BuildShitman();
		BuildChipman();
		BuildEXE();
		BuildDude();
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public void SpawnNoir(Vector3 Pos, Vector3 Rot, SEX Sex = SEX.MALE)
	{
		GameObject gameObject = ((Sex == SEX.MALE) ? MaleCult : FemaleCult);
		gameObject.SetActive(value: true);
		gameObject.transform.position = Pos;
		gameObject.transform.rotation = Quaternion.Euler(Rot);
	}

	public void DeSpawnNoir(SEX Sex = SEX.MALE)
	{
		GameObject gameObject = ((Sex == SEX.MALE) ? MaleCult : FemaleCult);
		gameObject.SetActive(value: false);
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
	}

	public void DeSpawnPair()
	{
		DeSpawnNoir();
		DeSpawnNoir(SEX.FEMALE);
	}

	public void KillAll()
	{
		DeSpawnPair();
		foreach (GameObject instance in instances)
		{
			UnityEngine.Object.Destroy(instance.gameObject);
		}
	}

	public GameObject InstantinateAll(Vector3 Pos, Vector3 Rot)
	{
		GameObject gameObject = null;
		int num = UnityEngine.Random.Range(-1, 8);
		switch (num)
		{
		case -1:
			gameObject = UnityEngine.Object.Instantiate(MaleCult, DancePool, worldPositionStays: true);
			break;
		case 0:
			gameObject = UnityEngine.Object.Instantiate(FemaleCult, DancePool, worldPositionStays: true);
			break;
		case 1:
			gameObject = UnityEngine.Object.Instantiate(DancingBreather, DancePool, worldPositionStays: true);
			break;
		case 2:
			gameObject = UnityEngine.Object.Instantiate(noirChips, DancePool, worldPositionStays: true);
			break;
		case 3:
			gameObject = UnityEngine.Object.Instantiate(shitman, DancePool, worldPositionStays: true);
			break;
		case 4:
			gameObject = UnityEngine.Object.Instantiate(chipman, DancePool, worldPositionStays: true);
			break;
		case 5:
			gameObject = UnityEngine.Object.Instantiate(dancingExecutioner, DancePool, worldPositionStays: true);
			gameObject.GetComponent<Animator>().SetBool("Dancing", value: true);
			break;
		case 6:
			gameObject = UnityEngine.Object.Instantiate(CustomObjectLookUp.TheTanner, GameObject.Find("EnemyPool").transform, worldPositionStays: true);
			gameObject.AddComponent<DancingTanner>().TriggerDancing();
			break;
		case 7:
			gameObject = AdamLOLHook.Ins.InstantiateAt(Pos, Rot);
			break;
		}
		gameObject.SetActive(value: true);
		gameObject.transform.position = Pos;
		gameObject.transform.rotation = Quaternion.Euler(Rot);
		if (num == 5)
		{
			gameObject.GetComponent<Animator>().SetBool("Dancing", value: true);
		}
		instances.Add(gameObject);
		return gameObject;
	}

	private void BuildMaleCult()
	{
		GameObject gameObject = GameObject.Find("CultMaleGame");
		MaleCult = UnityEngine.Object.Instantiate(CustomObjectLookUp.CultMaleDance, DancePool, worldPositionStays: true);
		MaleCult.GetComponent<Animator>().avatar = gameObject.GetComponent<Animator>().avatar;
		SkinnedMeshRenderer component = gameObject.transform.Find("cultMale").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer component2 = MaleCult.transform.Find("cultMale").GetComponent<SkinnedMeshRenderer>();
		component2.sharedMesh = component.sharedMesh;
		component2.materials = component.materials;
		MaleCult.SetActive(value: false);
	}

	private void BuildFemaleCult()
	{
		GameObject gameObject = GameObject.Find("CultFemaleGame");
		FemaleCult = UnityEngine.Object.Instantiate(CustomObjectLookUp.CultFemaleDance, DancePool, worldPositionStays: true);
		FemaleCult.GetComponent<Animator>().avatar = gameObject.GetComponent<Animator>().avatar;
		SkinnedMeshRenderer component = gameObject.transform.Find("cultFemale").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer component2 = FemaleCult.transform.Find("cultFemale").GetComponent<SkinnedMeshRenderer>();
		component2.sharedMesh = component.sharedMesh;
		component2.materials = component.materials;
		SkinnedMeshRenderer component3 = gameObject.transform.Find("Hammer").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer component4 = FemaleCult.transform.Find("Hammer").GetComponent<SkinnedMeshRenderer>();
		component4.sharedMesh = component3.sharedMesh;
		component4.materials = component3.materials;
		FemaleCult.SetActive(value: false);
	}

	public void SpawnMaleNoir(Vector3 POS, Vector3 ROT, Vector3 SCL)
	{
		GameObject maleCult = MaleCult;
		maleCult.SetActive(value: true);
		maleCult.transform.position = POS;
		maleCult.transform.rotation = Quaternion.Euler(ROT);
		maleCult.transform.localScale = SCL;
	}

	public void DeSpawnMaleNoir()
	{
		GameObject maleCult = MaleCult;
		maleCult.SetActive(value: false);
		maleCult.transform.position = Vector3.zero;
		maleCult.transform.rotation = Quaternion.Euler(Vector3.zero);
		maleCult.transform.localScale = Vector3.zero;
	}

	public void SpawnFemaleNoir(Vector3 POS, Vector3 ROT, Vector3 SCL)
	{
		GameObject femaleCult = FemaleCult;
		femaleCult.SetActive(value: true);
		femaleCult.transform.position = POS;
		femaleCult.transform.rotation = Quaternion.Euler(ROT);
		femaleCult.transform.localScale = SCL;
	}

	public void DeSpawnFemaleNoir()
	{
		GameObject femaleCult = FemaleCult;
		femaleCult.SetActive(value: false);
		femaleCult.transform.position = Vector3.zero;
		femaleCult.transform.rotation = Quaternion.Euler(Vector3.zero);
		femaleCult.transform.localScale = Vector3.zero;
	}

	public void DOMoveMale(Vector3 POS, Vector3 ROT, Vector3 SCL, float duration)
	{
		MaleCult.transform.DOMove(POS, duration).SetEase(Ease.Linear);
		MaleCult.transform.DORotate(ROT, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		MaleCult.transform.DOScale(SCL, duration).SetEase(Ease.Linear);
	}

	public void DOMoveFeMale(Vector3 POS, Vector3 ROT, Vector3 SCL, float duration)
	{
		FemaleCult.transform.DOMove(POS, duration).SetEase(Ease.Linear);
		FemaleCult.transform.DORotate(ROT, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		FemaleCult.transform.DOScale(SCL, duration).SetEase(Ease.Linear);
	}

	private void BuildDancingBreather()
	{
		DancingBreather = UnityEngine.Object.Instantiate(CustomObjectLookUp.BreatherSecret);
		DancingBreather.SetActive(value: false);
	}

	public void SpawnDancingBreather(Vector3 POS, Vector3 ROT, Vector3 SCL)
	{
		GameObject dancingBreather = DancingBreather;
		dancingBreather.SetActive(value: true);
		dancingBreather.transform.position = POS;
		dancingBreather.transform.rotation = Quaternion.Euler(ROT);
		dancingBreather.transform.localScale = SCL;
	}

	public void DeSpawnDancingBreather()
	{
		GameObject dancingBreather = DancingBreather;
		dancingBreather.SetActive(value: false);
		dancingBreather.transform.position = Vector3.zero;
		dancingBreather.transform.rotation = Quaternion.Euler(Vector3.zero);
		dancingBreather.transform.localScale = Vector3.zero;
	}

	public void DOMoveDancingBreather(Vector3 POS, Vector3 ROT, Vector3 SCL, float duration)
	{
		DancingBreather.transform.DOMove(POS, duration).SetEase(Ease.Linear);
		DancingBreather.transform.DORotate(ROT, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		DancingBreather.transform.DOScale(SCL, duration).SetEase(Ease.Linear);
	}

	private void BuildChipsNoir()
	{
		noirChips = UnityEngine.Object.Instantiate(CustomObjectLookUp.NoirChips);
		noirChips.SetActive(value: false);
	}

	private void BuildDude()
	{
		dude = UnityEngine.Object.Instantiate(CustomObjectLookUp.dude);
		dude.SetActive(value: false);
	}

	public void SpawnChips(Vector3 POS, Vector3 ROT, Vector3 SCL)
	{
		GameObject gameObject = noirChips;
		gameObject.SetActive(value: true);
		gameObject.transform.position = POS;
		gameObject.transform.rotation = Quaternion.Euler(ROT);
		gameObject.transform.localScale = SCL;
	}

	public void DeSpawnChips()
	{
		GameObject gameObject = noirChips;
		gameObject.SetActive(value: false);
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
		gameObject.transform.localScale = Vector3.zero;
	}

	public void DOMoveChips(Vector3 POS, Vector3 ROT, Vector3 SCL, float duration)
	{
		noirChips.transform.DOMove(POS, duration).SetEase(Ease.Linear);
		noirChips.transform.DORotate(ROT, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		noirChips.transform.DOScale(SCL, duration).SetEase(Ease.Linear);
	}

	private void BuildShitman()
	{
		shitman = UnityEngine.Object.Instantiate(CustomObjectLookUp.Shitman);
		shitman.SetActive(value: false);
	}

	public void SpawnShitman(Vector3 POS, Vector3 ROT, Vector3 SCL)
	{
		GameObject gameObject = shitman;
		gameObject.SetActive(value: true);
		gameObject.transform.position = POS;
		gameObject.transform.rotation = Quaternion.Euler(ROT);
		gameObject.transform.localScale = SCL;
	}

	public void DeSpawnShitman()
	{
		GameObject gameObject = shitman;
		gameObject.SetActive(value: false);
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
		gameObject.transform.localScale = Vector3.zero;
	}

	public void DOMoveShitman(Vector3 POS, Vector3 ROT, Vector3 SCL, float duration)
	{
		shitman.transform.DOMove(POS, duration).SetEase(Ease.Linear);
		shitman.transform.DORotate(ROT, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		shitman.transform.DOScale(SCL, duration).SetEase(Ease.Linear);
	}

	private void BuildChipman()
	{
		chipman = UnityEngine.Object.Instantiate(CustomObjectLookUp.Chipman);
		GameObject.Find("Chipman(Clone)/Hitman/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/Pistol").SetActive(value: false);
		chipman.SetActive(value: false);
	}

	public void SpawnChipman(Vector3 POS, Vector3 ROT, Vector3 SCL)
	{
		GameObject gameObject = chipman;
		gameObject.SetActive(value: true);
		gameObject.transform.position = POS;
		gameObject.transform.rotation = Quaternion.Euler(ROT);
		gameObject.transform.localScale = SCL;
	}

	public void DeSpawnChipman()
	{
		GameObject gameObject = chipman;
		gameObject.SetActive(value: false);
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
		gameObject.transform.localScale = Vector3.zero;
	}

	public void DOMoveChipman(Vector3 POS, Vector3 ROT, Vector3 SCL, float duration)
	{
		chipman.transform.DOMove(POS, duration).SetEase(Ease.Linear);
		chipman.transform.DORotate(ROT, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		chipman.transform.DOScale(SCL, duration).SetEase(Ease.Linear);
	}

	private void BuildEXE()
	{
		dancingExecutioner = UnityEngine.Object.Instantiate(CustomObjectLookUp.ExecutionerCustomRig);
		dancingExecutioner.SetActive(value: false);
	}

	public void SpawnEXE(Vector3 POS, Vector3 ROT, Vector3 SCL)
	{
		GameObject gameObject = dancingExecutioner;
		gameObject.SetActive(value: true);
		gameObject.GetComponent<Animator>().SetBool("Dancing", value: true);
		gameObject.transform.position = POS;
		gameObject.transform.rotation = Quaternion.Euler(ROT);
		gameObject.transform.localScale = SCL;
	}

	public void DeSpawnEXE()
	{
		GameObject gameObject = dancingExecutioner;
		gameObject.SetActive(value: false);
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
		gameObject.transform.localScale = Vector3.zero;
	}

	public void DOMoveEXE(Vector3 POS, Vector3 ROT, Vector3 SCL, float duration)
	{
		dancingExecutioner.transform.DOMove(POS, duration).SetEase(Ease.Linear);
		dancingExecutioner.transform.DORotate(ROT, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		dancingExecutioner.transform.DOScale(SCL, duration).SetEase(Ease.Linear);
	}

	public void SpawnDude(Vector3 POS, Vector3 ROT, Vector3 SCL)
	{
		GameObject gameObject = dude;
		gameObject.SetActive(value: true);
		gameObject.transform.position = POS;
		gameObject.transform.rotation = Quaternion.Euler(ROT);
		gameObject.transform.localScale = SCL;
	}

	public void DeSpawnDude()
	{
		GameObject gameObject = dude;
		gameObject.SetActive(value: false);
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
		gameObject.transform.localScale = Vector3.zero;
	}

	public void DOMoveDude(Vector3 POS, Vector3 ROT, Vector3 SCL, float duration)
	{
		dude.transform.DOMove(POS, duration).SetEase(Ease.Linear);
		dude.transform.DORotate(ROT, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
		dude.transform.DOScale(SCL, duration).SetEase(Ease.Linear);
	}
}
