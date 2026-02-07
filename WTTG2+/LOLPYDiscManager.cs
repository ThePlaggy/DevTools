using UnityEngine;

public class LOLPYDiscManager : MonoBehaviour
{
	public delegate void LOLPYDiscActions();

	[SerializeField]
	private Transform lolpyDiscParent;

	[SerializeField]
	private GameObject lolpyDiscObject;

	[SerializeField]
	private Vector3 spawnPOS;

	[SerializeField]
	private Vector3 spawnROT;

	private LOLPYDiscData myData;

	private int myID;

	public LOLPYDiscBehaviour LOLPYDiscBeh { get; private set; }

	public event LOLPYDiscActions DiscWasPickedUp;

	private void Awake()
	{
		myID = base.transform.position.GetHashCode();
		GameManager.ManagerSlinger.LOLPYDiscManager = this;
		GameManager.StageManager.Stage += stageMe;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProductWasActivated.Event += productWasPickedUp;
		LOLPYDiscBeh = Object.Instantiate(lolpyDiscObject, lolpyDiscParent).GetComponent<LOLPYDiscBehaviour>();
		LOLPYDiscBeh.SoftBuild();
		new GameObject("SulphurPackageObject").AddComponent<SulphurPackageObject>();
	}

	private void OnDestroy()
	{
	}

	public void LOLPYDiscWasPickedUp()
	{
		if (this.DiscWasPickedUp != null)
		{
			this.DiscWasPickedUp();
		}
	}

	public void LOLPYDiscWasInserted()
	{
		myData.WasInserted = true;
		DataManager.Save(myData);
	}

	private void productWasPickedUp(ShadowMarketProductDefinition TheProduct)
	{
		switch (TheProduct.productID)
		{
		case HARDWARE_PRODUCTS.FLOPPY_DISK:
			spawnLOLPYDisc();
			break;
		case HARDWARE_PRODUCTS.SULPHUR:
			SulphurInventory.AddSulphur(1);
			break;
		}
	}

	public void spawnLOLPYDisc()
	{
		LOLPYDiscBeh.MoveMe(spawnPOS, spawnROT);
	}

	private void stageMe()
	{
		myData = DataManager.Load<LOLPYDiscData>(myID);
		if (myData == null)
		{
			myData = new LOLPYDiscData(myID);
			myData.WasInserted = false;
		}
		if (myData.WasInserted)
		{
			LOLPYDiscBeh.HardInsert();
		}
		GameManager.StageManager.Stage -= stageMe;
	}
}
