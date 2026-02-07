using DG.Tweening;
using UnityEngine;

public static class SecCamsMover
{
	public static void Revert()
	{
		AppCreator.SecCamera.transform.position = new Vector3(-4.3527f, 41.7599f, -7.5946f);
		AppCreator.SecCamera.transform.rotation = Quaternion.Euler(20.5818f, 42.0364f, 0f);
	}

	public static void MoveMe(float x, float y, float z, float dx, float dy, float dz)
	{
		AppCreator.SecCamera.transform.position = new Vector3(x, y, z);
		AppCreator.SecCamera.transform.rotation = Quaternion.Euler(dx, dy, dz);
	}

	public static void DOMoveMe(float x, float y, float z, float dx, float dy, float dz, float t, float dt)
	{
		AppCreator.SecCamera.transform.DOMove(new Vector3(x, y, z), t).SetEase(Ease.Linear);
		AppCreator.SecCamera.transform.DORotate(new Vector3(dx, dy, dz), dt).SetEase(Ease.Linear);
	}
}
