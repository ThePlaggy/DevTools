using System;
using System.Collections.Generic;

[Serializable]
public class WebSitesData : DataObject
{
	public List<WebSiteData> Sites { get; set; }

	public WebSitesData(int SetID)
		: base(SetID)
	{
	}
}
