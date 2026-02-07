using UnityEngine;
using UnityEngine.AI;

public class BreatherPatrolBehaviour : MonoBehaviour
{
	public static BreatherPatrolBehaviour Ins;

	[SerializeField]
	private Vector3 patrolSpawnPOS = Vector3.zero;

	[SerializeField]
	private Vector3 patrolSpawnROT = Vector3.zero;

	[SerializeField]
	private PatrolPointDefinition[] patrolPoints = new PatrolPointDefinition[0];

	private PatrolPointDefinition currentPatrolPoint;

	private bool destInProgress;

	private bool hadPathPreviousFrame;

	private Animator myAC;

	private NavMeshAgent myNavMeshAgent;

	private void Awake()
	{
		Ins = this;
		myAC = GetComponent<Animator>();
		myNavMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		if (destInProgress && myNavMeshAgent.enabled)
		{
			if (myNavMeshAgent.hasPath)
			{
				myAC.SetFloat("walking", myNavMeshAgent.velocity.magnitude);
				hadPathPreviousFrame = true;
			}
			else if (hadPathPreviousFrame)
			{
				hadPathPreviousFrame = false;
				reachedEndPoint();
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		for (int i = 0; i < patrolPoints.Length; i++)
		{
			Gizmos.DrawWireCube(patrolPoints[i].Position, new Vector3(0.2f, 0.2f, 0.2f));
		}
	}

	public void PatrolSpawn()
	{
		BreatherBehaviour.Ins.SoftSpawn();
		base.transform.position = patrolSpawnPOS;
		base.transform.rotation = Quaternion.Euler(patrolSpawnROT);
		PatrolTo(patrolPoints[0]);
	}

	public void KillPatrol()
	{
		myNavMeshAgent.enabled = false;
		destInProgress = false;
		hadPathPreviousFrame = false;
		myAC.SetFloat("walking", 0f);
	}

	public void LeftDoorPatrol()
	{
		PatrolTo(patrolPoints[1]);
	}

	public void PatrolTo(PatrolPointDefinition Point)
	{
		currentPatrolPoint = Point;
		destInProgress = true;
		myNavMeshAgent.enabled = true;
		myNavMeshAgent.speed = 1f;
		myNavMeshAgent.angularSpeed = 240f;
		myNavMeshAgent.acceleration = 2f;
		myNavMeshAgent.SetDestination(Point.Position);
	}

	private void reachedEndPoint()
	{
		destInProgress = false;
		myNavMeshAgent.enabled = false;
		if (currentPatrolPoint != null)
		{
			currentPatrolPoint.InvokeEvents();
		}
		GameManager.TweenSlinger.FireDOSTweenLiner(myNavMeshAgent.velocity.magnitude, 0f, 1f, delegate(float value)
		{
			myAC.SetFloat("walking", value);
		});
	}
}
