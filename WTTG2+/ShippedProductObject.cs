using UnityEngine;

public class ShippedProductObject : MonoBehaviour
{
	[SerializeField]
	private InteractionHook myInteractionHook;

	[SerializeField]
	private AudioFileDefinition pickUpPackageSFX;

	private AudioHubObject myAudioHub;

	private MeshCollider myMeshCollider;

	private MeshRenderer myMeshRender;

	private ShadowMarketProductDefinition myProduct;

	private Rigidbody myRigidBody;

	public CustomEvent<ShippedProductObject> ProductPickUp = new CustomEvent<ShippedProductObject>(2);

	private void Awake()
	{
		myMeshRender = GetComponent<MeshRenderer>();
		myMeshCollider = GetComponent<MeshCollider>();
		myRigidBody = GetComponent<Rigidbody>();
		myAudioHub = GetComponent<AudioHubObject>();
		myInteractionHook.LeftClickAction += pickUpPackage;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= pickUpPackage;
	}

	public void SoftBuild()
	{
		myMeshRender.enabled = false;
		myMeshCollider.enabled = false;
		myRigidBody.isKinematic = true;
		myInteractionHook.ForceLock = true;
	}

	public void ShipMe(ShadowMarketProductDefinition TheProduct, Vector3 TargetLocation, Vector3 TargetRotation)
	{
		myProduct = TheProduct;
		myMeshRender.enabled = true;
		myMeshCollider.enabled = true;
		myInteractionHook.ForceLock = false;
		base.transform.position = TargetLocation;
		base.transform.rotation = Quaternion.Euler(TargetRotation);
		myRigidBody.isKinematic = false;
	}

	public void DroneDeliver(ShadowMarketProductDefinition TheProduct)
	{
		myProduct = TheProduct;
		myMeshRender.enabled = true;
		myMeshCollider.enabled = true;
		myInteractionHook.ForceLock = false;
		GameManager.BehaviourManager.DroneBehaviour.DeliverPackage(this);
	}

	public void DroneDrop()
	{
		base.transform.SetParent(GameManager.ManagerSlinger.ProductsManager.ShippedProductParent);
		myRigidBody.isKinematic = false;
	}

	private void pickUpPackage()
	{
		if ((!(base.transform.position.x < 18f) || !(base.transform.position.z > 100f)) && !GameManager.PauseManager.Paused)
		{
			myAudioHub.PlaySound(pickUpPackageSFX);
			myMeshRender.enabled = false;
			myMeshCollider.enabled = false;
			myRigidBody.isKinematic = true;
			myInteractionHook.ForceLock = true;
			GameManager.TimeSlinger.FireTimer(1f, delegate
			{
				base.transform.position = Vector3.zero;
				base.transform.rotation = Quaternion.Euler(Vector3.zero);
			});
			GameManager.ManagerSlinger.ProductsManager.ActivateShadowMarketProduct(myProduct);
			ProductPickUp.Execute(this);
		}
	}
}
