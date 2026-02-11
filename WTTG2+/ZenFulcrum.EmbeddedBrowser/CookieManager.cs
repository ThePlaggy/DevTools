using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser
{

	public class CookieManager
	{
		private static readonly Dictionary<IPromise<List<Cookie>>, BrowserNative.GetCookieFunc> cookieFuncs = new Dictionary<IPromise<List<Cookie>>, BrowserNative.GetCookieFunc>();

		internal readonly Browser browser;

		public CookieManager(Browser browser)
		{
			this.browser = browser;
		}

		public IPromise<List<Cookie>> GetCookies()
		{
			Cookie.Init();
			List<Cookie> ret = new List<Cookie>();
			if (!browser.IsReady || !browser.enabled)
			{
				return Promise<List<Cookie>>.Resolved(ret);
			}
			Promise<List<Cookie>> promise = new Promise<List<Cookie>>();
			BrowserNative.GetCookieFunc getCookieFunc = delegate (BrowserNative.NativeCookie cookie)
			{
				try
				{
					if (cookie == null)
					{
						browser.RunOnMainThread(delegate
						{
							promise.Resolve(ret);
						});
						cookieFuncs.Remove(promise);
					}
					else
					{
						ret.Add(new Cookie(this, cookie));
					}
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			};
			BrowserNative.zfb_getCookies(browser.browserId, getCookieFunc);
			cookieFuncs[promise] = getCookieFunc;
			return promise;
		}

		public void ClearAll()
		{
			if (!browser.DeferUnready(ClearAll))
			{
				BrowserNative.zfb_clearCookies(browser.browserId);
			}
		}
	}
}