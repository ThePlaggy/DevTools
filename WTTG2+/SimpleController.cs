using UnityEngine;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

[RequireComponent(typeof(Browser))]
public class SimpleController : MonoBehaviour
{
	public InputField urlInput;

	private Browser browser;

	public void Start()
	{
		browser = GetComponent<Browser>();
	}

	public void GoToURLInput()
	{
		browser.Url = urlInput.text;
	}
}
