using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AgentLinkMover : MonoBehaviour
{
	public float FirstRotationSpeed = 1f;

	[Range(0f, 1f)]
	public float CrossSpeed = 1f;

	private IEnumerator Start()
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agent.autoTraverseOffMeshLink = false;
		while (true)
		{
			if (agent.isOnOffMeshLink)
			{
				yield return StartCoroutine(NormalSpeed(agent));
				if (agent != null && agent.isOnOffMeshLink)
				{
					agent.CompleteOffMeshLink();
				}
			}
			yield return null;
		}
	}

	private IEnumerator NormalSpeed(NavMeshAgent agent)
	{
		agent.updateRotation = false;
		OffMeshLinkData data = agent.currentOffMeshLinkData;
		Vector3 startPOS = data.startPos + Vector3.up * agent.baseOffset;
		Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
		Vector3 dir = data.endPos - data.startPos;
		dir.y = 0f;
		Quaternion endROT = Quaternion.LookRotation(dir);
		while (agent.transform.position != startPOS)
		{
			agent.transform.position = Vector3.MoveTowards(agent.transform.position, startPOS, agent.speed * Time.deltaTime);
			agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, endROT, agent.angularSpeed * FirstRotationSpeed * Time.deltaTime);
			yield return null;
		}
		while (Quaternion.Angle(agent.transform.rotation, endROT) >= 5f)
		{
			agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, endROT, agent.angularSpeed * FirstRotationSpeed * Time.deltaTime);
			yield return null;
		}
		while (agent.transform.position != endPos)
		{
			agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * CrossSpeed * Time.deltaTime);
			yield return null;
		}
		agent.updateRotation = true;
	}
}
