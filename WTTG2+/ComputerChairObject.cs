using UnityEngine;

public class ComputerChairObject : MonoBehaviour
{
	public static ComputerChairObject Ins;

	[SerializeField]
	private AudioHubObject chairAudioHub;

	[SerializeField]
	private Vector3 InUsePosition = Vector3.zero;

	[SerializeField]
	private Vector3 InUseRotation = Vector3.zero;

	[SerializeField]
	private Vector3 NotInUsePosition = Vector3.zero;

	[SerializeField]
	private Vector3 NotInUseRotation = Vector3.zero;

	public AudioFileDefinition getOnSFX;

	public AudioFileDefinition getOffSFX;

	private MeshRenderer myMesh;

	private void Awake()
	{
		Ins = this;
		myMesh = GetComponent<MeshRenderer>();
	}

	public void SetToInUsePosition()
	{
		chairAudioHub.PlaySound(getOnSFX);
		base.transform.localPosition = InUsePosition;
		base.transform.localRotation = Quaternion.Euler(InUseRotation);
		if (EnemyStateManager.HasEnemyState(ENEMY_STATE.EXECUTIONER) && base.gameObject.activeSelf)
		{
			EXESoundPopper.PopSound(999);
		}
	}

	public void SetToNotInUsePosition()
	{
		chairAudioHub.PlaySound(getOffSFX);
		base.transform.localPosition = NotInUsePosition;
		base.transform.localRotation = Quaternion.Euler(NotInUseRotation);
		if (EnemyStateManager.HasEnemyState(ENEMY_STATE.EXECUTIONER) && base.gameObject.activeSelf)
		{
			EXESoundPopper.PopSound(999);
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public void ShowBack()
	{
		base.gameObject.SetActive(value: true);
	}
}
