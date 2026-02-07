using UnityEngine;

public class RotateWardrobeDoorScrub : MonoBehaviour
{
	[SerializeField]
	private float rotateAmount;

	public void TriggerRotate(float WeightAmount)
	{
		base.transform.localRotation = Quaternion.Euler(new Vector3(base.transform.localRotation.x, Mathf.Lerp(0f, rotateAmount, WeightAmount), base.transform.localRotation.z));
	}
}
