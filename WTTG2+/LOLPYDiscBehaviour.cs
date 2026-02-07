using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class LOLPYDiscBehaviour : MonoBehaviour
{
	private Tweener insertTween;

	private InteractionHook myInteractionHook;

	private MeshRenderer myMeshRenderer;

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= leftClickAction;
	}

	public void SoftBuild()
	{
		myMeshRenderer = GetComponent<MeshRenderer>();
		myInteractionHook = GetComponent<InteractionHook>();
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
		myMeshRenderer.enabled = false;
		myInteractionHook.LeftClickAction += leftClickAction;
		insertTween = DOTween.To(() => new Vector3(3.818f, 0.6124f, -23.1333f), delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(3.818f, 0.6124f, -23.2183f), 1f).SetEase(Ease.OutSine).OnComplete(delegate
		{
			GameManager.ManagerSlinger.TenantTrackManager.UnLockSystem();
		});
		insertTween.Pause();
		insertTween.SetAutoKill(autoKillOnCompletion: false);
	}

	public void MoveMe(Vector3 SetPOS, Vector3 SetROT)
	{
		myMeshRenderer.enabled = true;
		base.transform.position = SetPOS;
		base.transform.rotation = Quaternion.Euler(SetROT);
	}

	public void InsertMe()
	{
		GameManager.ManagerSlinger.LOLPYDiscManager.LOLPYDiscWasInserted();
		myMeshRenderer.enabled = true;
		if (Random.Range(0, 100) == 0)
		{
			base.transform.localScale = new Vector3(10f, 10f, 10f);
		}
		base.transform.position = new Vector3(3.818f, 0.6124f, -23.1333f);
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, -90f, 90f));
		insertTween.Restart();
	}

	public void HardInsert()
	{
		myMeshRenderer.enabled = false;
		myInteractionHook.ForceLock = true;
		GameManager.ManagerSlinger.TenantTrackManager.UnLockSystem();
	}

	private void leftClickAction()
	{
		myMeshRenderer.enabled = false;
		myInteractionHook.ForceLock = true;
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPickUp2);
		GameManager.ManagerSlinger.LOLPYDiscManager.LOLPYDiscWasPickedUp();
	}
}
