using DG.Tweening;
using UnityEngine;

public class BombMakerBehindJump : MonoBehaviour
{
	public GameObject elbowBM;

	public GameObject Bullet;

	public void ElbowRot()
	{
		elbowBM.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.3f);
	}

	public void gunRecoil()
	{
		Bullet.SetActive(value: true);
		elbowBM.transform.DOLocalRotate(new Vector3(5f, 0f, -5f), 0.2f).OnComplete(delegate
		{
			gunRecoilBack();
		});
	}

	private void gunRecoilBack()
	{
		elbowBM.transform.DOLocalRotate(new Vector3(-3f, 0f, 3f), 0.2f).OnComplete(delegate
		{
			Bullet.SetActive(value: false);
		});
	}
}
