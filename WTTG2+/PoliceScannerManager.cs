using UnityEngine;

public class PoliceScannerManager : MonoBehaviour
{
	[SerializeField]
	private Transform policeScanerParent;

	[SerializeField]
	private GameObject policeScannerObject;

	[SerializeField]
	private Vector3 spawnPOS;

	[SerializeField]
	private Vector3 spawnROT;

	private PoliceScannerBehaviour policeScanerIns;

	private void Awake()
	{
		GameManager.ManagerSlinger.PoliceScanerManager = this;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProductWasActivated.Event += productWasPickedUp;
		policeScanerIns = Object.Instantiate(policeScannerObject, policeScanerParent).GetComponent<PoliceScannerBehaviour>();
		policeScanerIns.SoftBuild();
		Object.Instantiate(CustomObjectLookUp.Router).GetComponent<RouterBehaviour>().SoftBuild();
		Object.Instantiate(CustomObjectLookUp.TarotCards).GetComponent<TarotCardsBehaviour>().SoftBuild();
	}

	private void OnDestroy()
	{
	}

	private void productWasPickedUp(ShadowMarketProductDefinition TheProduct)
	{
		HARDWARE_PRODUCTS productID = TheProduct.productID;
		if (productID == HARDWARE_PRODUCTS.POLICE_SCANNER)
		{
			spawnPoliceScanner();
		}
		if (productID == HARDWARE_PRODUCTS.ROUTER)
		{
			RouterBehaviour.Ins.MoveMe(new Vector3(-5.66f, 39.1f, -1.93f), new Vector3(0f, -86f, 0f), new Vector3(0.45f, 0.45f, 0.45f));
		}
		if (productID == HARDWARE_PRODUCTS.CAMERA)
		{
			CamHookBehaviour.InstallCamera();
		}
		if (productID == HARDWARE_PRODUCTS.KEYPAD)
		{
			KeypadManager.BuyKeypad();
		}
		if (productID == HARDWARE_PRODUCTS.TAROT_CARDS)
		{
			if (!TarotCardsBehaviour.Owned)
			{
				TarotCardsBehaviour.Ins.MoveMe(new Vector3(1.393f, 40.68f, 2.489f), new Vector3(0f, -20f, 180f), new Vector3(0.3f, 0.3f, 0.3f));
				return;
			}
			TarotRefiller.RefillCards();
		}
		if (productID == HARDWARE_PRODUCTS.STRONG_FLASHLIGHT)
		{
			FlashLightBehaviour.Ins.getFlashlight.intensity = 0.6f;
			FlashLightBehaviour.Ins.getFlashlight.range = 50f;
		}
	}

	private void spawnPoliceScanner()
	{
		policeScanerIns.MoveMe(spawnPOS, spawnROT);
	}
}
