using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser
{

	public abstract class WebResources
	{
		public struct Response
		{
			public int responseCode;

			public string mimeType;

			public byte[] data;
		}

		public static readonly Dictionary<string, string> extensionMimeTypes = new Dictionary<string, string>
	{
		{ "css", "text/css" },
		{ "gif", "image/gif" },
		{ "html", "text/html" },
		{ "jpeg", "image/jpeg" },
		{ "jpg", "image/jpeg" },
		{ "js", "application/javascript" },
		{ "mp3", "audio/mpeg" },
		{ "mpeg", "video/mpeg" },
		{ "ogg", "application/ogg" },
		{ "ogv", "video/ogg" },
		{ "webm", "video/webm" },
		{ "png", "image/png" },
		{ "svg", "image/svg+xml" },
		{ "txt", "text/plain" },
		{ "xml", "application/xml" },
		{ "*", "application/octet-stream" }
	};

		private readonly Regex matchDots = new Regex("\\.[2,]");

		public virtual Response this[string url]
		{
			get
			{
				if (string.IsNullOrEmpty(url) || url[0] != '/')
				{
					return GetError("Invalid path");
				}
				if (url.IndexOf('?') >= 0)
				{
					url = url.Substring(0, url.IndexOf('?'));
				}
				if (url.IndexOf('#') >= 0)
				{
					url = url.Substring(0, url.IndexOf('#'));
				}
				string input = WWW.UnEscapeURL(url);
				input = matchDots.Replace(input, ".");
				Response result;
				try
				{
					byte[] data = GetData(input);
					if (data == null)
					{
						result = GetError("Not found", 404);
					}
					else
					{
						string text = Path.GetExtension(input);
						if (text.Length > 0)
						{
							text = text.Substring(1);
						}
						if (!extensionMimeTypes.TryGetValue(text, out var value))
						{
							value = extensionMimeTypes["*"];
						}
						result = new Response
						{
							mimeType = value,
							data = data,
							responseCode = 200
						};
					}
				}
				catch (Exception exception)
				{
					Debug.LogError("WebResources: Failed to fetch URL " + input);
					Debug.LogException(exception);
					result = GetError("Internal error");
				}
				return result;
			}
		}

		public abstract byte[] GetData(string path);

		public Response GetError(string text, int status = 500)
		{
			return new Response
			{
				data = Encoding.UTF8.GetBytes(text),
				mimeType = "text/plain",
				responseCode = status
			};
		}
	}
}