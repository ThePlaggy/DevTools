using DG.Tweening;
using UnityEngine;

public class BombMakerPresenceGunJump : MonoBehaviour
{
	public GameObject ArmBM;

	public AudioHubObject hub;

	public void ArmAppear()
	{
		ArmBM.transform.DOLocalMove(new Vector3(-0.262026f, -0.01137987f, 0.03041466f), 0.3f);
		ArmBM.transform.DOLocalRotate(new Vector3(-2.861f, -6.695f, 6.725f), 0.3f);
	}

	public void RandGunShake()
	{
		ArmBM.transform.DOLocalRotate(new Vector3(55.796f, 3.62f, 12.009f), 0.5f).OnComplete(delegate
		{
			GunShakeBack();
		});
	}

	private void GunShakeBack()
	{
		ArmBM.transform.DOLocalRotate(new Vector3(-2.861f, -6.695f, 6.725f), 0.6f);
	}
}
