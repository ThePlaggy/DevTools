using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace SWS
{

	public class MoveAnimator : MonoBehaviour
	{
		private Animator animator;

		private float lastRotY;

		private NavMeshAgent nAgent;

		private splineMove sMove;

		private void Start()
		{
			animator = GetComponentInChildren<Animator>();
			sMove = GetComponent<splineMove>();
			if (!sMove)
			{
				nAgent = GetComponent<NavMeshAgent>();
			}
		}

		private void OnAnimatorMove()
		{
			float value;
			float value2;
			if ((bool)sMove)
			{
				value = ((sMove.tween != null && sMove.tween.IsPlaying()) ? sMove.speed : 0f);
				value2 = (base.transform.eulerAngles.y - lastRotY) * 10f;
				lastRotY = base.transform.eulerAngles.y;
			}
			else
			{
				value = nAgent.velocity.magnitude;
				Vector3 vector = Quaternion.Inverse(base.transform.rotation) * nAgent.desiredVelocity;
				value2 = Mathf.Atan2(vector.x, vector.z) * 180f / 3.14159f;
			}
			animator.SetFloat("Speed", value);
			animator.SetFloat("Direction", value2, 0.15f, Time.deltaTime);
		}
	}
}