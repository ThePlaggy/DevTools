using UnityEngine;

public class MemCellObject : MonoBehaviour
{
	public delegate void MemCellActions();

	public GameObject MemInactiveLineIMG;

	public GameObject MemInactiveCellIMG;

	public GameObject MemActiveLineIMG;

	public GameObject MemActiveCellIMG;

	public event MemCellActions IWasActivated;
}
