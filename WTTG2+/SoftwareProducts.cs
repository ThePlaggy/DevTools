using System.Collections.Generic;
using UnityEngine;

public class SoftwareProducts : MonoBehaviour
{
	public List<SoftwareProductDefinition> Products;

	private Dictionary<SOFTWARE_PRODUCTS, int> productLookUp = new Dictionary<SOFTWARE_PRODUCTS, int>();

	private void Awake()
	{
		LookUp.SoftwareProducts = this;
		for (int i = 0; i < Products.Count; i++)
		{
			SoftwareProductDefinition softwareProductDefinition = Products[i];
			if (!productLookUp.ContainsKey(softwareProductDefinition.Product))
			{
				productLookUp.Add(softwareProductDefinition.Product, i);
			}
		}
	}

	public SoftwareProductDefinition Get(SOFTWARE_PRODUCTS ProductToGet)
	{
		if (productLookUp.ContainsKey(ProductToGet))
		{
			return Products[productLookUp[ProductToGet]];
		}
		return null;
	}
}
