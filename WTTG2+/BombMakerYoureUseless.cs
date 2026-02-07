using DG.Tweening;
using UnityEngine;

public class BombMakerYoureUseless : MonoBehaviour
{
	public GameObject BMMouth;

	public GameObject BMNeck;

	public GameObject BMShoulder;

	public AudioHubObject MouthTalkHub;

	public GameObject Bullet;

	public void StagePCKill()
	{
		BMNeck.transform.DOLocalRotate(new Vector3(-8.264f, 5.885f, -28.896f), 0.4f);
		BMShoulder.transform.DOLocalRotate(new Vector3(-164.6f, -245.8f, 184.5f), 0.8f).OnComplete(delegate
		{
			StageYouAreUseless();
		});
	}

	public void StageYouAreUseless()
	{
		AnimStage1();
		BMMouth.SetActive(value: true);
		MouthTalkHub.PlaySound(CustomSoundLookUp.youreuseless);
	}

	public void StageGunRecoil()
	{
		Bullet.SetActive(value: true);
		BMShoulder.transform.DOLocalMoveY(0.03f, 0.1f).OnComplete(delegate
		{
			HitmanBehaviour.Ins.GunFlashBombMaker();
			gunRecoilBack();
		});
	}

	private void gunRecoilBack()
	{
		BMShoulder.transform.DOLocalMoveY(0f, 0.1f).OnComplete(delegate
		{
			Bullet.SetActive(value: false);
		});
	}

	private void AnimStage1()
	{
		BMMouth.transform.DOScaleX(0.01f, 0.5f).OnComplete(delegate
		{
			AnimStage2();
		});
	}

	private void AnimStage2()
	{
		BMMouth.transform.DOScaleX(-0.01f, 0.5f).OnComplete(delegate
		{
			AnimStage3();
		});
	}

	private void AnimStage3()
	{
		BMMouth.transform.DOScaleX(0.01f, 0.2f).OnComplete(delegate
		{
			AnimStage4();
		});
	}

	private void AnimStage4()
	{
		BMMouth.transform.DOScaleX(-0.01f, 0.2f).OnComplete(delegate
		{
			AnimStageFinish();
		});
	}

	private void AnimStageFinish()
	{
		BMMouth.transform.DOScaleX(0f, 0.3f).OnComplete(delegate
		{
			BMMouth.SetActive(value: false);
			GameManager.TimeSlinger.FireTimer(0.5f, StageGunRecoil);
		});
	}
}
