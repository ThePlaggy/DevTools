using DG.Tweening;
using UnityEngine;

public class BombMaster : MonoBehaviour
{
	public GameObject head;

	public Vector3 targetPosition = new Vector3(0f, 5f, 0f);

	private Vector3 prevPos;

	public float time = 5f;

	private void Start()
	{
		prevPos = head.transform.position;
		seq1();
	}

	private void seq1()
	{
		head.transform.DOMove(targetPosition, time).OnComplete(delegate
		{
			seq2();
		});
	}

	private void seq2()
	{
		head.transform.DOMove(prevPos, time).OnComplete(delegate
		{
			seq1();
		});
	}

	private void Update()
	{
	}
}
