using System;
using System.Text.RegularExpressions;

namespace ZenFulcrum.EmbeddedBrowser;

public class Cookie
{
	private static readonly Regex dateRegex = new Regex("(\\d{4})-(\\d{2})-(\\d{2}) (\\d{2}):(\\d{2}):(\\d{2}).(\\d{3})");

	private CookieManager cookies;

	public DateTime creation;

	public string domain = string.Empty;

	public DateTime? expires;

	public bool httpOnly;

	public DateTime lastAccess;

	public string name = string.Empty;

	private BrowserNative.NativeCookie original;

	public string path = string.Empty;

	public bool secure;

	public string value = string.Empty;

	public Cookie(CookieManager cookies)
	{
		this.cookies = cookies;
	}

	internal Cookie(CookieManager cookies, BrowserNative.NativeCookie cookie)
	{
		this.cookies = cookies;
		original = cookie;
		Copy(original, this);
	}

	public static void Init()
	{
	}

	public void Delete()
	{
		if (original != null)
		{
			BrowserNative.zfb_editCookie(cookies.browser.browserId, original, BrowserNative.CookieAction.Delete);
			original = null;
		}
	}

	public void Update()
	{
		if (original != null)
		{
			Delete();
		}
		original = new BrowserNative.NativeCookie();
		Copy(this, original);
		BrowserNative.zfb_editCookie(cookies.browser.browserId, original, BrowserNative.CookieAction.Create);
	}

	public static void Copy(BrowserNative.NativeCookie src, Cookie dest)
	{
		dest.name = src.name;
		dest.value = src.value;
		dest.domain = src.domain;
		dest.path = src.path;
		Func<string, DateTime> func = delegate(string s)
		{
			Match match = dateRegex.Match(s);
			return new DateTime(int.Parse(match.Groups[1].ToString()), int.Parse(match.Groups[2].ToString()), int.Parse(match.Groups[3].ToString()), int.Parse(match.Groups[4].ToString()), int.Parse(match.Groups[5].ToString()), int.Parse(match.Groups[6].ToString()), int.Parse(match.Groups[7].ToString()));
		};
		dest.creation = func(src.creation);
		dest.expires = ((src.expires != null) ? new DateTime?(func(src.expires)) : ((DateTime?)null));
		dest.lastAccess = func(src.lastAccess);
		dest.secure = src.secure != 0;
		dest.httpOnly = src.httpOnly != 0;
	}

	public static void Copy(Cookie src, BrowserNative.NativeCookie dest)
	{
		dest.name = src.name;
		dest.value = src.value;
		dest.domain = src.domain;
		dest.path = src.path;
		Func<DateTime, string> func = (DateTime s) => s.ToString("yyyy-MM-dd hh:mm:ss.fff");
		dest.creation = func(src.creation);
		DateTime? dateTime = src.expires;
		dest.expires = (dateTime.HasValue ? func(src.expires.Value) : null);
		dest.lastAccess = func(src.lastAccess);
		dest.secure = (byte)(src.secure ? 1u : 0u);
		dest.httpOnly = (byte)(src.httpOnly ? 1u : 0u);
	}
}
