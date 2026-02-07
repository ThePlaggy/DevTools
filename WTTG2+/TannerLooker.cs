using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TannerLooker : MonoBehaviour
{
	public CustomEvent NotVisibleActions = new CustomEvent(2);

	public CustomEvent VisibleActions = new CustomEvent(2);

	private Camera myCamera;

	public bool CheckForVisible { get; set; }

	public bool CheckForNotVisible { get; set; }

	public Vector3 TargetLocation { get; set; } = Vector3.zero;

	public bool IsTargetVisible(Vector3 TargetPOS)
	{
		Vector3 vector = myCamera.WorldToViewportPoint(TargetPOS);
		Vector3 vector2 = myCamera.WorldToScreenPoint(TargetPOS);
		int num = Screen.height / 2;
		int num2 = Screen.width / 2;
		float num3 = Vector3.Distance(new Vector3(num2, 0f, 0f), new Vector3(vector2.x, 0f, 0f));
		float num4 = Vector3.Distance(new Vector3(0f, num, 0f), new Vector3(0f, vector2.y, 0f));
		return !(num3 > (float)num2) && !(num4 > (float)num) && vector.z > 4f;
	}

	private void Awake()
	{
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
}
