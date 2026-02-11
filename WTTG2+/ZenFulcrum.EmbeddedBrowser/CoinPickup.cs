using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser
{

	public class CoinPickup : MonoBehaviour
	{
		public float spinSpeed = 20f;

		public bool isMassive;

		private Transform coinVis;

		public void Start()
		{
			coinVis = base.transform.Find("Vis");
		}

		public void Update()
		{
			coinVis.transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * spinSpeed, Vector3.up);
		}

		public void OnTriggerEnter(Collider other)
		{
			PlayerInventory component = other.GetComponent<PlayerInventory>();
			if ((bool)component)
			{
				if (isMassive)
				{
					HUDManager.Instance.LoadBrowseLevel();
				}
				else
				{
					component.AddCoin();
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}