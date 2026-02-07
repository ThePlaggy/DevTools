using DG.Tweening;
using UnityEngine;

public class BombMakerApartmentJump : MonoBehaviour
{
	[SerializeField]
	private GameObject shoulderBM;

	public GameObject Bullet;

	public void ShoulderRotate()
	{
		shoulderBM.transform.DOLocalRotate(new Vector3(0f, -90f, 0f), 0.3f);
	}

	public void gunRecoil()
	{
		Bullet.SetActive(value: true);
		shoulderBM.transform.DOLocalRotate(new Vector3(0f, -90f, -2f), 0.1f).OnComplete(delegate
		{
			gunRecoilBack();
		});
	}

	private void gunRecoilBack()
	{
		shoulderBM.transform.DOLocalRotate(new Vector3(0f, -90f, 1f), 0.1f).OnComplete(delegate
		{
			Bullet.SetActive(value: false);
		});
	}
}
