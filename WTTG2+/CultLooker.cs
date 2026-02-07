using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CultLooker : MonoBehaviour
{
	public static CultLooker Ins;

	private Camera myCamera;

	public CustomEvent NotVisibleActions = new CustomEvent(2);

	public CustomEvent VisibleActions = new CustomEvent(2);

	public bool CheckForVisible { get; set; }

	public bool CheckForNotVisible { get; set; }

	public Vector3 TargetLocation { get; set; } = Vector3.zero;

	private void Awake()
	{
		Ins = this;
		myCamera = GetComponent<Camera>();
	}

	private void Update()
	{
		if (CheckForVisible)
		{
			if (IsTargetVisible(TargetLocation))
			{
				CheckForVisible = false;
				VisibleActions.Execute();
			}
		}
		else if (CheckForNotVisible && !IsTargetVisible(TargetLocation))
		{
			CheckForNotVisible = false;
			NotVisibleActions.Execute();
		}
	}

	public bool IsTargetVisible(Vector3 TargetPOS)
	{
		Vector3 vector = myCamera.WorldToViewportPoint(TargetPOS);
		int num = 0;
		if (vector.z > 0f)
		{
			num++;
		}
		if (vector.x >= 0f && vector.x <= 1f)
		{
			num++;
		}
		if (vector.y >= 0f && vector.y <= 1f)
		{
			num++;
		}
		return num >= 2 && vector.z >= 0f;
	}
}
