using UnityEngine;
using UnityEngine.Events;

public class CultSpawner : MonoBehaviour
{
	[SerializeField]
	private SkinnedMeshRenderer myRenderer;

	[SerializeField]
	private UnityEvent DeSpawnEvents;

	[SerializeField]
	private UnityEvent SpawnEvents;

	private bool inMesh;

	private bool inMeshCheckActive;

	private float inMeshCheckTimeStamp;

	public CustomEvent InMeshEvents = new CustomEvent(2);

	public CustomEvent NotInMeshEvents = new CustomEvent(2);

	protected bool IAmSpawned;

	protected bool windowNoir;

	protected bool windowNoirDespawnTime;

	protected virtual void Awake()
	{
		myRenderer.enabled = false;
	}

	private void Update()
	{
		if (inMeshCheckActive && Time.time - inMeshCheckTimeStamp >= 0.05f)
		{
			inMeshCheckActive = false;
			if (inMesh)
			{
				InMeshEvents.Execute();
			}
			else
			{
				myRenderer.enabled = true;
				NotInMeshEvents.Execute();
			}
			inMesh = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		inMesh = true;
	}

	private void OnTriggerStay(Collider other)
	{
		inMesh = true;
	}

	public void SpawnBehindPlayer(Transform TargetTransform, float YOffSet = 0f)
	{
		Vector3 position = TargetTransform.position - TargetTransform.forward * 0.85f;
		position.y -= YOffSet;
		base.transform.position = position;
		base.transform.rotation = TargetTransform.rotation;
		inMeshCheckTimeStamp = Time.time;
		inMeshCheckActive = true;
	}

	public void Spawn(Vector3 TargetPOS, Vector3 TargetROT)
	{
		myRenderer.enabled = true;
		base.transform.position = TargetPOS;
		base.transform.rotation = Quaternion.Euler(TargetROT);
		SpawnEvents.Invoke();
	}

	public void Place(Vector3 TargetPOS, Vector3 TargetROT)
	{
		myRenderer.enabled = true;
		base.transform.position = TargetPOS;
		base.transform.rotation = Quaternion.Euler(TargetROT);
	}

	protected void DeSpawn()
	{
		myRenderer.enabled = false;
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
		DeSpawnEvents.Invoke();
		IAmSpawned = false;
		windowNoir = false;
		windowNoirDespawnTime = false;
	}

	protected void SetupWindowNoir(CultSpawnDefinition _currentSpawnData)
	{
		if (!(_currentSpawnData.Position == new Vector3(1.8f, 39.25f, 3.96f)))
		{
			return;
		}
		Debug.Log("Window noir spawned");
		windowNoirDespawnTime = false;
		windowNoir = true;
		GameManager.TimeSlinger.FireTimer(20f, delegate
		{
			if (windowNoir)
			{
				windowNoirDespawnTime = true;
			}
		});
	}
}
