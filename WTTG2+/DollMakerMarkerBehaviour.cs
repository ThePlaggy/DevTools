using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class DollMakerMarkerBehaviour : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer myMesh;

	public CustomEvent MarkerWasPickedUp = new CustomEvent(2);

	private DollMakerMarkerData myData;

	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		myMesh.enabled = false;
		myInteractionHook.LeftClickAction += wasPickedUp;
		GameManager.StageManager.Stage += stageMe;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= wasPickedUp;
	}

	public void SpawnMeTo(Vector3 Position, Vector3 Rotation)
	{
		myMesh.enabled = true;
		UIInventoryManager.HideDollMakerMarker();
		GameManager.TimeSlinger.FireTimer(0.75f, delegate
		{
			myInteractionHook.ForceLock = false;
		});
		base.transform.position = Position;
		base.transform.rotation = Quaternion.Euler(Rotation);
		myData.IsPlaced = true;
		myData.PlacedLocation = Position.ToVect3();
		myData.PlaceRotation = Rotation.ToVect3();
		DataManager.Save(myData);
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		myData = DataManager.Load<DollMakerMarkerData>(2247);
		if (myData == null)
		{
			myData = new DollMakerMarkerData(2247);
			myData.IsPlaced = false;
			myData.PlacedLocation = Vect3.zero;
			myData.PlaceRotation = Vect3.zero;
		}
		if (myData.IsPlaced)
		{
			myMesh.enabled = true;
			myInteractionHook.ForceLock = false;
			base.transform.position = myData.PlacedLocation.ToVector3;
			base.transform.rotation = Quaternion.Euler(myData.PlaceRotation.ToVector3);
		}
	}

	private void wasPickedUp()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPickUp2);
		UIInventoryManager.ShowDollMakerMarker();
		myMesh.enabled = false;
		myInteractionHook.ForceLock = true;
		base.transform.position = Vector3.zero;
		myData.IsPlaced = false;
		DataManager.Save(myData);
		MarkerWasPickedUp.Execute();
	}

	public void pcPickUp()
	{
		if (myData.IsPlaced)
		{
			UIInventoryManager.ShowDollMakerMarker();
			myMesh.enabled = false;
			myInteractionHook.ForceLock = true;
			base.transform.position = Vector3.zero;
			myData.IsPlaced = false;
			DataManager.Save(myData);
			MarkerWasPickedUp.Execute();
		}
	}

	public void ShowMesh()
	{
		myMesh.enabled = true;
	}

	public void HideMesh()
	{
		myMesh.enabled = false;
	}
}
