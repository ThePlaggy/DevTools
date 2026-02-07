using DG.Tweening;
using UnityEngine;

public class LucasLaserFailTrigger : MonoBehaviour
{
	[HideInInspector]
	public Vector3 laserEndPointPOS;

	[HideInInspector]
	public Vector3 laserEndPointROT;

	public void SetEndPoint(Vector3 POS, Vector3 ROT)
	{
		laserEndPointPOS = POS;
		laserEndPointROT = ROT;
	}

	private void OnTriggerEnter(Collider other)
	{
		LucasLaserTrigger.jumpmode = 1;
		LucasLaserManager.Ins.CancelTweener();
		LucasLaserManager.Ins.myLaser.transform.DOScale(new Vector3(0.01f, 0.01f, 100f), 0.15f);
		LucasLaserManager.Ins.myLaser.transform.DORotate(laserEndPointROT, 0.35f);
		LucasLaserManager.Ins.myLaser.transform.DOMove(laserEndPointPOS, 0.35f);
		GameManager.TimeSlinger.FireTimer(0.6f, delegate
		{
			if (LucasLaserTrigger.jumpmode == 1)
			{
				LucasLaserManager.ExplodeGameOver();
			}
		});
	}
}
