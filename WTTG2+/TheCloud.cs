using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

public class TheCloud : MonoBehaviour
{
	private const string NOT_FOUND_URL = "localGame://NotFound/index.html";

	private const string SEIZED_URL = "localGame://Seized/index.html";

	private const string DOC_ROOT = "localGame://";

	[SerializeField]
	private List<WebSiteDefinition> wikis;

	[SerializeField]
	private List<WebSiteDefinition> Websites;

	public string websitesToTap;

	private Dictionary<int, BookmarkData> bookmarks = new Dictionary<int, BookmarkData>();

	private WebPageDefinition curWebPageDef;

	private WebSiteDefinition curWebsiteDef;

	private List<int> fakeDomains = new List<int>();

	private bool itsNewATap;

	public CustomEvent KeyDiscoveredEvent = new CustomEvent(6);

	private List<string> keys = new List<string>();

	private WebPageDefinition lastSourceWebPageDef;

	private WebSiteDefinition lastSourceWebSiteDef;

	private BookMarksData myBookMarksData;

	private WallpaperUtils _myWallpaperUtils;

	private MasterKeyData myMasterKeyData;

	private WebSitesData myWebSitesData;

	private WikiSiteData myWikiSiteData;

	private bool nightmarePossible;

	private bool rollCoolDownActive;

	private float rollTimeStamp;

	private List<string> validDomains = new List<string>();

	private Dictionary<string, int> websiteLookUp = new Dictionary<string, int>();

	private Dictionary<string, int> wikiLookUp = new Dictionary<string, int>();

	private List<List<int>> wikiSites = new List<List<int>>();

	public static bool InitANN;

	public static bool InitNotes;

	public static bool InitSource;

	public static bool InitSkybreak;

	public static string thethreeTXT;

	public static bool thethreeTXTset;

	[NonSerialized]
	public bool speedActive;

	[NonSerialized]
	public float speedFireWindow;

	[NonSerialized]
	public float speedTimeStamp;

	[NonSerialized]
	public bool keyActive;

	[NonSerialized]
	public float keyFireWindow;

	[NonSerialized]
	public float keyTimeStamp;

	public string MasterKey { get; private set; }

	public static string WebsiteHoldingWikiLink { get; set; }

	private void Awake()
	{
		ZeroDayProductObject.isDiscountOn = false;
		ShadowProductObject.isDiscountOn = false;
		WallpaperUtils.FindADOS();
		MarketUtilities.ApplyMarketSettings();
		if (DifficultyManager.Nightmare)
		{
			Debug.Log("[Nightmare] PLAYING ON NIGHTMARE MODE??? You're brave!!");
		}
		GameManager.TheCloud = this;
		GameManager.StageManager.Stage += stageMe;
	}

	private void Start()
	{
		UIManager.InstantiateDevText();
	}

	private void Update()
	{
		if (rollCoolDownActive && Time.time - rollTimeStamp >= 30f)
		{
			rollCoolDownActive = false;
		}
	}

	private void OnDisable()
	{
		StaticMemorySlinger.ResetStaticMemory();
		Debug.Log("TheCloud is disabled.");
	}

	public void InvalidURL(out string ReturnURL)
	{
		ReturnURL = "localGame://NotFound/index.html";
		curWebPageDef = null;
		curWebsiteDef = null;
	}

	public bool SoftValidateURL(out string returnURL, string checkURL = "")
	{
		returnURL = "localGame://NotFound/index.html";
		bool flag = false;
		checkURL = checkURL.Replace("http://", string.Empty);
		checkURL = checkURL.Replace("https://", string.Empty);
		checkURL = checkURL.Replace("www.", string.Empty);
		string[] array = checkURL.Split(new string[1] { "/" }, StringSplitOptions.None);
		return array[0].Equals("game.local") || validDomains.Contains(array[0].ToLower());
	}

	public bool ValidateURL(out string returnURL, string checkURL = "")
	{
		returnURL = "localGame://NotFound/index.html";
		checkURL = checkURL.Trim();
		bool flag = false;
		if (checkURL != string.Empty)
		{
			checkURL = checkURL.Replace("http://", string.Empty);
			checkURL = checkURL.Replace("www.", string.Empty);
			string[] array = checkURL.Split(new string[1] { "/" }, StringSplitOptions.None);
			string key = array[0].Replace(".ann", string.Empty);
			curWebPageDef = null;
			curWebsiteDef = null;
			if (validDomains.Contains(array[0].ToLower()))
			{
				returnURL = "http://www." + checkURL;
				SteamSlinger.Ins.CheckStalkerURL(returnURL);
			}
			else if (array[0].Contains(".ann"))
			{
				if (wikiLookUp.ContainsKey(key))
				{
					int index = wikiLookUp[key];
					curWebsiteDef = wikis[index];
					returnURL = "localGame://" + wikis[index].GetDocumentRoot() + "/" + wikis[index].HomePage.FileName;
					curWebPageDef = wikis[index].HomePage;
				}
				else if (websiteLookUp.ContainsKey(key))
				{
					int index2 = websiteLookUp[key];
					curWebsiteDef = Websites[index2];
					if (array.Length > 1)
					{
						if (array[1] != string.Empty)
						{
							if (array[1].ToLower() == Websites[index2].HomePage.FileName)
							{
								returnURL = "localGame://" + Websites[index2].GetDocumentRoot() + "/" + Websites[index2].HomePage.FileName;
								curWebPageDef = Websites[index2].HomePage;
							}
							else
							{
								for (int i = 0; i < Websites[index2].SubPages.Count; i++)
								{
									if (array[1].ToLower() == Websites[index2].SubPages[i].FileName)
									{
										returnURL = "localGame://" + Websites[index2].GetDocumentRoot() + "/" + Websites[index2].SubPages[i].FileName;
										curWebPageDef = Websites[index2].SubPages[i];
										i = Websites[index2].SubPages.Count;
									}
								}
							}
						}
						else
						{
							returnURL = "localGame://" + Websites[index2].GetDocumentRoot() + "/" + Websites[index2].HomePage.FileName;
							curWebPageDef = Websites[index2].HomePage;
						}
					}
					else if (Websites[index2].isFake)
					{
						if (Websites[index2].PageTitle == "Cotton Road" || Websites[index2].PageTitle == "The Bunker" || Websites[index2].PageTitle == "PlaneCrashInfo" || Websites[index2].PageTitle == "Rent A Hacker" || Websites[index2].PageTitle == "Evidence Locker" || Websites[index2].PageTitle == "Pay To Rape" || Websites[index2].PageTitle == "Unsolved Area 51" || Websites[index2].PageTitle == "Roses Destruction" || Websites[index2].PageTitle == "Better Call Saul")
						{
							returnURL = "localGame://Seized/index.html";
						}
						else
						{
							returnURL = "localGame://NotFound/index.html";
							if (!rollCoolDownActive)
							{
								GameManager.HackerManager.RollHack();
								rollTimeStamp = Time.time;
								rollCoolDownActive = true;
							}
						}
						curWebPageDef = null;
						curWebsiteDef = null;
					}
					else
					{
						returnURL = "localGame://" + Websites[index2].GetDocumentRoot() + "/" + Websites[index2].HomePage.FileName;
						curWebPageDef = Websites[index2].HomePage;
					}
					if (Websites[index2].HasWindow)
					{
						bool flag2 = true;
						if (DifficultyManager.CasualMode || TarotManager.HermitActive)
						{
							flag2 = false;
						}
						else
						{
							switch (Websites[index2].WindowTime)
							{
							case WEBSITE_WINDOW_TIME.FIRST_QUARTER:
								if (GameManager.TimeKeeper.GetCurrentGameMin() >= 0 && GameManager.TimeKeeper.GetCurrentGameMin() <= 15)
								{
									flag2 = false;
								}
								break;
							case WEBSITE_WINDOW_TIME.SECOND_QUARTER:
								if (GameManager.TimeKeeper.GetCurrentGameMin() >= 15 && GameManager.TimeKeeper.GetCurrentGameMin() <= 30)
								{
									flag2 = false;
								}
								break;
							case WEBSITE_WINDOW_TIME.THRID_QUARTER:
								if (GameManager.TimeKeeper.GetCurrentGameMin() >= 30 && GameManager.TimeKeeper.GetCurrentGameMin() <= 45)
								{
									flag2 = false;
								}
								break;
							case WEBSITE_WINDOW_TIME.FOURTH_QUARTER:
								if (GameManager.TimeKeeper.GetCurrentGameMin() >= 45 && GameManager.TimeKeeper.GetCurrentGameMin() <= 60)
								{
									flag2 = false;
								}
								break;
							case WEBSITE_WINDOW_TIME.FIRST_HALF:
								if (GameManager.TimeKeeper.GetCurrentGameMin() >= 0 && GameManager.TimeKeeper.GetCurrentGameMin() <= 30)
								{
									flag2 = false;
								}
								break;
							case WEBSITE_WINDOW_TIME.SECNOND_HALF:
								if (GameManager.TimeKeeper.GetCurrentGameMin() >= 30 && GameManager.TimeKeeper.GetCurrentGameMin() <= 60)
								{
									flag2 = false;
								}
								break;
							}
						}
						if (flag2)
						{
							returnURL = "localGame://NotFound/index.html";
							curWebPageDef = null;
							curWebsiteDef = null;
						}
					}
				}
			}
		}
		if (returnURL == "localGame://NotFound/index.html")
		{
			return false;
		}
		if (curWebsiteDef != null && websiteLookUp.ContainsKey(curWebsiteDef.PageURL))
		{
			if (!curWebsiteDef.WasVisted && curWebsiteDef.IsTapped && !DifficultyManager.LeetMode && !DifficultyManager.Nightmare)
			{
				KeyDiscoveredEvent.Execute();
			}
			curWebsiteDef.WasVisted = true;
			int index3 = websiteLookUp[curWebsiteDef.PageURL];
			myWebSitesData.Sites[index3].Visted = true;
			DataManager.Save(myWebSitesData);
		}
		if (curWebPageDef != null)
		{
			curWebPageDef.InvokePageEvent();
			string text = curWebPageDef.PageName.ToLower().Replace(" ", "");
			string text2 = text;
			if (!(text2 == "thebombmaker"))
			{
				if (text2 == "cleanup")
				{
					DelfalcoBehaviour.Ins.ReleaseMe();
				}
			}
			else
			{
				EnemyManager.BombMakerManager.ReleaseTheBombMaker();
			}
		}
		return true;
	}

	public void ClearCurrentWebDeff()
	{
		curWebsiteDef = null;
		curWebPageDef = null;
	}

	public WebPageDefinition GetCurrentWebPageDef()
	{
		return curWebPageDef;
	}

	public void GetCurrentPageSourceCode()
	{
		if (!(curWebsiteDef != null) || !(curWebPageDef != null))
		{
			return;
		}
		string text = curWebPageDef.PageHTML;
		bool doSetHTML = false;
		if (curWebPageDef != lastSourceWebPageDef)
		{
			lastSourceWebPageDef = curWebPageDef;
			doSetHTML = true;
			if (curWebsiteDef.HoldsSecondWikiLink && curWebsiteDef.HomePage == curWebPageDef)
			{
				int num = UnityEngine.Random.Range(Mathf.RoundToInt((float)text.Length * 0.1f), text.Length);
				string text2 = text.Substring(0, num);
				string text3 = text.Substring(num);
				text = text2 + "<!-- " + GetWikiURL(1) + " -->" + text3;
			}
			if (curWebPageDef.IsTapped && curWebPageDef.KeyDiscoverMode == KEY_DISCOVERY_MODES.SOURCE_CODE)
			{
				int num2 = UnityEngine.Random.Range(Mathf.RoundToInt((float)text.Length * 0.3f), text.Length);
				string text4 = text.Substring(0, num2);
				string text5 = text.Substring(num2);
				text = text4 + "<!-- " + (curWebPageDef.HashIndex + 1) + " - " + curWebPageDef.HashValue + " -->" + text5;
			}
		}
		GameManager.BehaviourManager.SourceViewerBehaviour.ViewSourceCode(text, doSetHTML);
	}

	public bool TriggerBookMark()
	{
		if (!(curWebsiteDef != null) || !(curWebPageDef != null))
		{
			return false;
		}
		if (!bookmarks.ContainsKey(curWebPageDef.GetHashCode()))
		{
			string setURL = "http://" + curWebsiteDef.PageURL + ".ann/" + curWebPageDef.FileName;
			BookmarkData bookmarkData = new BookmarkData(curWebsiteDef.PageTitle, setURL);
			bookmarks.Add(curWebPageDef.GetHashCode(), bookmarkData);
			myBookMarksData.BookMarks.Add(curWebPageDef.GetHashCode(), bookmarkData);
			GameManager.BehaviourManager.AnnBehaviour.AddBookmarkTab(curWebPageDef.GetHashCode(), bookmarkData);
			DataManager.Save(myBookMarksData);
			return true;
		}
		bookmarks.Remove(curWebPageDef.GetHashCode());
		myBookMarksData.BookMarks.Remove(curWebPageDef.GetHashCode());
		GameManager.BehaviourManager.AnnBehaviour.RemoveBookmarkTab(curWebPageDef.GetHashCode());
		DataManager.Save(myBookMarksData);
		return false;
	}

	public bool CheckToSeeIfPageIsBookMarked()
	{
		return curWebsiteDef != null && curWebPageDef != null && bookmarks.ContainsKey(curWebPageDef.GetHashCode());
	}

	public bool CheckIfSiteWasTapped()
	{
		return curWebPageDef != null && curWebPageDef.IsTapped;
	}

	public bool CheckIfWiki()
	{
		return curWebsiteDef != null && wikiLookUp.ContainsKey(curWebsiteDef.PageURL);
	}

	public JSONNode BuildCurrentWiki()
	{
		List<JSONNode> list = new List<JSONNode>(20);
		if (curWebsiteDef != null && wikiLookUp.ContainsKey(curWebsiteDef.PageURL))
		{
			int index = wikiLookUp[curWebsiteDef.PageURL];
			for (int i = 0; i < wikiSites[index].Count; i++)
			{
				int index2 = wikiSites[index][i];
				string text = Websites[index2].PageURL + "|" + Websites[index2].PageTitle + "|" + Websites[index2].PageDesc + "|";
				text = ((!Websites[index2].WasVisted) ? (text + "0") : (text + "1"));
				list.Add(text);
			}
		}
		return new JSONNode(list);
	}

	public string GetWikiURL(int WikiIndex)
	{
		string result = string.Empty;
		if (wikis[WikiIndex] != null)
		{
			result = "http://" + wikis[WikiIndex].PageURL + ".ann";
		}
		return result;
	}

	public void ForceKeyDiscover()
	{
		KeyDiscoveredEvent?.Execute();
	}

	private void prepTheMasterKey()
	{
		myMasterKeyData = new MasterKeyData(1010)
		{
			Keys = new List<string>(8)
		};
		for (int i = 0; i < 8; i++)
		{
			string item = MagicSlinger.MD5It(Time.time.ToString() + UnityEngine.Random.Range(0, 99999).ToString() + ":REFLECTSTUDIOS:" + Time.deltaTime + ":" + i).Substring(0, 12);
			myMasterKeyData.Keys.Add(item);
		}
		keys = new List<string>(myMasterKeyData.Keys);
		for (int j = 0; j < keys.Count; j++)
		{
			MasterKey += keys[j];
		}
	}

	private void prepWikis()
	{
		myWikiSiteData = DataManager.Load<WikiSiteData>(1919);
		if (myWikiSiteData == null)
		{
			myWikiSiteData = new WikiSiteData(1919)
			{
				Wikis = new List<WebSiteData>(wikis.Count),
				WikiSites = new List<List<int>>(3)
			};
			List<WebSiteDefinition> list = new List<WebSiteDefinition>(Websites);
			List<int> list2 = new List<int>(fakeDomains);
			list2.Remove(17);
			list2.Remove(18);
			list2.Remove(20);
			list2.Remove(21);
			list2.Remove(22);
			list2.Remove(23);
			list2.Remove(25);
			list2.Remove(26);
			list2.Remove(27);
			list2.Remove(28);
			list2.Remove(32);
			list2.Remove(34);
			list2.Remove(37);
			for (int i = 0; i < wikis.Count; i++)
			{
				WebSiteData item = new WebSiteData
				{
					PageURL = MagicSlinger.MD5It($"Deep Web Wiki {i + 1}" + Time.time + UnityEngine.Random.Range(0, 9999))
				};
				myWikiSiteData.Wikis.Add(item);
			}
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (list[num].isFake || list[num].DoNotList || num == 53)
				{
					list.RemoveAt(num);
				}
			}
			for (int j = 0; j < 3; j++)
			{
				Dictionary<int, string> dictionary = new Dictionary<int, string>(20);
				List<int> list3 = new List<int>(20);
				int num2 = 20;
				switch (j)
				{
				case 2:
				{
					for (int num4 = list.Count - 1; num4 >= 0; num4--)
					{
						if (list[num4].WikiSpecific && list[num4].WikiIndex == 2)
						{
							dictionary.Add(websiteLookUp[list[num4].PageURL], list[num4].PageTitle);
							list.RemoveAt(num4);
							num2--;
						}
					}
					break;
				}
				case 1:
				{
					for (int num5 = list.Count - 1; num5 >= 0; num5--)
					{
						if (list[num5].WikiSpecific && list[num5].WikiIndex == 1)
						{
							dictionary.Add(websiteLookUp[list[num5].PageURL], list[num5].PageTitle);
							list.RemoveAt(num5);
							num2--;
						}
					}
					break;
				}
				case 0:
				{
					for (int num3 = list.Count - 1; num3 >= 0; num3--)
					{
						if (list[num3].WikiSpecific && list[num3].WikiIndex == 0)
						{
							dictionary.Add(websiteLookUp[list[num3].PageURL], list[num3].PageTitle);
							list.RemoveAt(num3);
							num2--;
						}
					}
					bool flag = false;
					while (!flag)
					{
						int index = UnityEngine.Random.Range(0, list.Count);
						if (!list[index].WikiSpecific && (!list[index].HasWindow || DifficultyManager.CasualMode))
						{
							list[index].HoldsSecondWikiLink = true;
							WebsiteHoldingWikiLink = list[index].PageTitle;
							myWikiSiteData.PickedSiteToHoldSecondWiki = list[index].PageTitle.GetHashCode();
							dictionary.Add(websiteLookUp[list[index].PageURL], list[index].PageTitle);
							list.RemoveAt(index);
							num2--;
							flag = true;
						}
					}
					break;
				}
				}
				int num6 = 0;
				while (num6 < num2)
				{
					int index2 = UnityEngine.Random.Range(0, list.Count);
					if (!list[index2].WikiSpecific)
					{
						dictionary.Add(websiteLookUp[list[index2].PageURL], list[index2].PageTitle);
						list.RemoveAt(index2);
						num6++;
					}
				}
				for (int k = 0; k < 10; k++)
				{
					int index3 = UnityEngine.Random.Range(0, list2.Count);
					dictionary.Add(list2[index3], Websites[list2[index3]].PageTitle);
					list2.RemoveAt(index3);
				}
				List<KeyValuePair<int, string>> list4 = dictionary.ToList();
				list4.Sort((KeyValuePair<int, string> pair1, KeyValuePair<int, string> pair2) => pair1.Value.CompareTo(pair2.Value));
				for (int num7 = 0; num7 < list4.Count; num7++)
				{
					list3.Add(list4[num7].Key);
				}
				myWikiSiteData.WikiSites.Add(list3);
			}
		}
		wikiLookUp = new Dictionary<string, int>(myWikiSiteData.Wikis.Count);
		for (int num8 = 0; num8 < myWikiSiteData.Wikis.Count; num8++)
		{
			wikis[num8].PageURL = myWikiSiteData.Wikis[num8].PageURL;
			wikiLookUp.Add(myWikiSiteData.Wikis[num8].PageURL, num8);
		}
		for (int num9 = 0; num9 < myWikiSiteData.WikiSites.Count; num9++)
		{
			wikiSites.Add(myWikiSiteData.WikiSites[num9]);
		}
		for (int num10 = 0; num10 < Websites.Count; num10++)
		{
			if (Websites[num10].PageTitle.GetHashCode() == myWikiSiteData.PickedSiteToHoldSecondWiki)
			{
				Websites[num10].HoldsSecondWikiLink = true;
			}
			else
			{
				Websites[num10].HoldsSecondWikiLink = false;
			}
		}
		DataManager.Save(myWikiSiteData);
	}

	private void prepWebsites()
	{
		for (int i = 0; i < Websites.Count; i++)
		{
			if (Websites[i].PageTitle == "The Rule Of Three" || Websites[i].PageTitle == "Claffis" || Websites[i].PageTitle == "ChosenAwake")
			{
				if (!thethreeTXTset)
				{
					thethreeTXTset = true;
					thethreeTXT = Websites[i].HomePage.PageHTML;
				}
				switch (UnityEngine.Random.Range(0, 3))
				{
				case 0:
					AnnBehaviour.ChosenAwake = false;
					AnnBehaviour.Claffis = true;
					Websites[i].DocumentRoot = "Claffis";
					Websites[i].PageTitle = "Claffis";
					Websites[i].HomePage.PageName = "Claffis";
					Websites[i].PageDesc = "Convoluted music-based puzzle webpage.";
					Websites[i].HomePage.PageHTML = "Per has notas, earumque quantitaes ac fedes, docet citatus Autor fecretum Germanicis verbis conceptum, uc quis clavum magnum ac firmum frangere poffic manibus, fine malleo, forcipe, aut alio manuali inftrumento, prout fequirut, In fehemate Auctoris norae non occupant loca debrita ; quare reftituendae fuere.";
					Debug.Log("[Wiki2Puzzle] Puzzle chosen Claffis");
					break;
				case 1:
					AnnBehaviour.ChosenAwake = true;
					AnnBehaviour.Claffis = false;
					Websites[i].DocumentRoot = "ChosenAwake";
					Websites[i].PageTitle = "ChosenAwake";
					Websites[i].HomePage.PageName = "Chosen Awake";
					Websites[i].PageDesc = "How Will You Tell The World?";
					Websites[i].HomePage.PageHTML = "010101000100100001000101001000000100110101000101010100110101001101000001010001110100010100100000010010010101001100100000010000110100111101001101010010010100111001000111001011100000101001010100010010000100010100100000010001100100111101010010010011010010000001001001010100110010000001000101010101000100010101010010010011100100000101001100001011100010111000101110000010100101010001001000010001010010000001000101010011100100010000100000010011110100001101000011010101010101001001010010010001010100010000101110";
					Debug.Log("[Wiki2Puzzle] Puzzle chosen ChosenAwake");
					break;
				default:
					AnnBehaviour.ChosenAwake = false;
					AnnBehaviour.Claffis = false;
					Websites[i].DocumentRoot = "thethree";
					Websites[i].PageTitle = "The Rule Of Three";
					Websites[i].HomePage.PageName = "The Rule Of Three";
					Websites[i].PageDesc = "Thought process behind the rule of three.";
					Websites[i].HomePage.PageHTML = thethreeTXT;
					Debug.Log("[Wiki2Puzzle] Puzzle chosen The Rule Of Three");
					break;
				}
			}
			if (Websites[i].PageTitle == "Chosen Awake" && Websites[i].isFake)
			{
				Websites[i].PageTitle = "Chopper";
				Websites[i].PageDesc = "Best tutorials for human meat cooking on the Deep Web.";
			}
			if (Websites[i].PageTitle == "Illumanti")
			{
				Websites[i].PageTitle = "Illumination";
				Websites[i].PageDesc = "Website dedicated to showing people's \"bright\" future.";
			}
			if (Websites[i].PageTitle == "Roses Destruction")
			{
				Websites[i].PageTitle = "Rotten Meal";
				Websites[i].PageDesc = "Weird ass people selling meal for canniballs.";
			}
			if (Websites[i].PageTitle == "The Doll Maker" || Websites[i].PageTitle == "The Bomb Maker")
			{
				Websites[i].WikiSpecific = true;
				Websites[i].WikiIndex = UnityEngine.Random.Range(0, 3);
			}
			if (Websites[i].PageTitle == "Cleaning Services")
			{
				Websites[i].WikiSpecific = true;
				Websites[i].WikiIndex = UnityEngine.Random.Range(0, 3);
				foreach (WebPageDefinition item3 in WebsiteLookUp.webpageDefinitions.Where((WebPageDefinition t) => t.PageName == "Cleaning Services Mr. Delfalco"))
				{
					Websites[i].SubPages.Add(item3);
				}
			}
			if (Websites[i].PageTitle == "Forgive Me")
			{
				Websites[i].HasWindow = true;
				Websites[i].DoNotList = true;
				Websites[i].DoNotTap = true;
			}
			if (Websites[i].PageTitle == "Evidence Locker")
			{
				Websites[i].HasWindow = true;
				Websites[i].isFake = true;
				Websites[i].DocumentRoot = string.Empty;
				Websites[i].SubPages.Clear();
				bool flag = false;
			}
			if (Websites[i].PageTitle == "Pay To Rape")
			{
				Websites[i].HasWindow = true;
				Websites[i].isFake = true;
				Websites[i].DocumentRoot = string.Empty;
				Websites[i].SubPages.Clear();
				bool flag2 = false;
			}
		}
		bool flag3 = PlayerPrefs.GetInt("[GAME]Nudity") == 0;
		bool dMCA = ModSettings.DMCA;
		List<WebSiteDefinition> websiteDefinitions = WebsiteLookUp.websiteDefinitions;
		List<WebPageDefinition> webpageDefinitions = WebsiteLookUp.webpageDefinitions;
		for (int num = 0; num < websiteDefinitions.Count; num++)
		{
			if (dMCA)
			{
				switch (websiteDefinitions[num].PageTitle)
				{
				case "FUNNY MONKE":
				case "The Art":
				case "Takedownman":
				case "CannabisWorld":
					websiteDefinitions[num].HomePage.HasMusic = true;
					break;
				case "Lethal Dosage":
					websiteDefinitions[num].HomePage.HasMusic = true;
					websiteDefinitions[num].SubPages[0].HasMusic = true;
					break;
				case "Deep Web Radio":
					websiteDefinitions[num].noNutNovember = false;
					break;
				}
			}
			else
			{
				switch (websiteDefinitions[num].PageTitle)
				{
				case "FUNNY MONKE":
				case "The Art":
				case "Takedownman":
				case "CannabisWorld":
					websiteDefinitions[num].HomePage.HasMusic = false;
					break;
				case "Lethal Dosage":
					websiteDefinitions[num].HomePage.HasMusic = false;
					websiteDefinitions[num].SubPages[0].HasMusic = false;
					break;
				case "Deep Web Radio":
					websiteDefinitions[num].noNutNovember = true;
					break;
				}
			}
		}
		for (int num2 = 0; num2 < websiteDefinitions.Count; num2++)
		{
			if (flag3)
			{
				switch (websiteDefinitions[num2].PageTitle)
				{
				case "Pou Sex":
				case "Bathroom Cams":
				case "Steroid Queen":
				case "Euthanasia Legion":
					websiteDefinitions[num2].noNutNovember = false;
					break;
				}
			}
			else
			{
				switch (websiteDefinitions[num2].PageTitle)
				{
				case "Pou Sex":
				case "Bathroom Cams":
				case "Steroid Queen":
				case "Euthanasia Legion":
					websiteDefinitions[num2].noNutNovember = true;
					break;
				}
			}
		}
		foreach (WebSiteDefinition item4 in websiteDefinitions.Where((WebSiteDefinition t) => t.PageTitle == "Deep Web Radio" || t.PageTitle == "You Are An Idiot" || t.PageTitle == "Beer Opening Tips"))
		{
			item4.WikiSpecific = true;
			item4.WikiIndex = UnityEngine.Random.Range(0, 3);
		}
		foreach (WebPageDefinition item5 in webpageDefinitions.Where((WebPageDefinition t) => t.HasMusic))
		{
			item5.AudioFile.MyAudioSourceDefinition = LookUp.SoundLookUp.PuzzleSolved.MyAudioSourceDefinition;
		}
		Websites.AddRange(websiteDefinitions);
		bool flag4 = false;
		myWebSitesData = DataManager.Load<WebSitesData>(2020);
		if (myWebSitesData == null)
		{
			myWebSitesData = new WebSitesData(2020)
			{
				Sites = new List<WebSiteData>(Websites.Count)
			};
			for (int num3 = 0; num3 < Websites.Count; num3++)
			{
				WebSiteData webSiteData = new WebSiteData
				{
					Pages = new List<WebPageData>()
				};
				if (!Websites[num3].isStatic)
				{
					webSiteData.PageURL = MagicSlinger.MD5It(Websites[num3].PageTitle + Time.time + UnityEngine.Random.Range(0, 9999));
				}
				else
				{
					webSiteData.PageURL = Websites[num3].PageURL;
				}
				webSiteData.Fake = Websites[num3].isFake;
				webSiteData.Visted = false;
				webSiteData.IsTapped = false;
				WebPageData item = new WebPageData
				{
					KeyDiscoveryMode = UnityEngine.Random.Range(0, 4),
					IsTapped = false,
					HashIndex = 0,
					HashValue = string.Empty
				};
				webSiteData.Pages.Add(item);
				if (Websites[num3].SubPages != null)
				{
					for (int num4 = 0; num4 < Websites[num3].SubPages.Count; num4++)
					{
						WebPageData item2 = new WebPageData
						{
							KeyDiscoveryMode = UnityEngine.Random.Range(0, 4),
							IsTapped = false,
							HashIndex = 0,
							HashValue = string.Empty
						};
						webSiteData.Pages.Add(item2);
					}
				}
				myWebSitesData.Sites.Add(webSiteData);
			}
			itsNewATap = true;
		}
		websiteLookUp = new Dictionary<string, int>(myWebSitesData.Sites.Count);
		for (int num5 = 0; num5 < myWebSitesData.Sites.Count; num5++)
		{
			Websites[num5].PageURL = myWebSitesData.Sites[num5].PageURL;
			Websites[num5].WasVisted = myWebSitesData.Sites[num5].Visted;
			Websites[num5].IsTapped = myWebSitesData.Sites[num5].IsTapped;
			if (myWebSitesData.Sites[num5].Fake)
			{
				fakeDomains.Add(num5);
			}
			else if (myWebSitesData.Sites[num5].Pages != null)
			{
				Websites[num5].HomePage.KeyDiscoverMode = (KEY_DISCOVERY_MODES)myWebSitesData.Sites[num5].Pages[0].KeyDiscoveryMode;
				Websites[num5].HomePage.IsTapped = myWebSitesData.Sites[num5].Pages[0].IsTapped;
				Websites[num5].HomePage.HashIndex = myWebSitesData.Sites[num5].Pages[0].HashIndex;
				Websites[num5].HomePage.HashValue = myWebSitesData.Sites[num5].Pages[0].HashValue;
				for (int num6 = 0; num6 < Websites[num5].SubPages.Count; num6++)
				{
					if (myWebSitesData.Sites[num5].Pages[num6 + 1] != null)
					{
						Websites[num5].SubPages[num6].KeyDiscoverMode = (KEY_DISCOVERY_MODES)myWebSitesData.Sites[num5].Pages[num6 + 1].KeyDiscoveryMode;
						Websites[num5].SubPages[num6].IsTapped = myWebSitesData.Sites[num5].Pages[num6 + 1].IsTapped;
						Websites[num5].SubPages[num6].HashIndex = myWebSitesData.Sites[num5].Pages[num6 + 1].HashIndex;
						Websites[num5].SubPages[num6].HashValue = myWebSitesData.Sites[num5].Pages[num6 + 1].HashValue;
					}
				}
			}
			websiteLookUp.Add(myWebSitesData.Sites[num5].PageURL, num5);
		}
		DataManager.Save(myWebSitesData);
	}

	private void prepBookmarks()
	{
		myBookMarksData = DataManager.Load<BookMarksData>(2021) ?? new BookMarksData(2021)
		{
			BookMarks = new Dictionary<int, BookmarkData>()
		};
		bookmarks = new Dictionary<int, BookmarkData>(myBookMarksData.BookMarks.Count);
		foreach (KeyValuePair<int, BookmarkData> bookMark in myBookMarksData.BookMarks)
		{
			bookmarks.Add(bookMark.Key, bookMark.Value);
			GameManager.BehaviourManager.AnnBehaviour.AddBookmarkTab(bookMark.Key, bookMark.Value);
		}
	}

	private void tapSites()
	{
		if (!itsNewATap)
		{
			return;
		}
		List<string> list = new List<string>(keys);
		Dictionary<string, int> dictionary = new Dictionary<string, int>(8);
		List<KEY_DISCOVERY_MODES> list2 = new List<KEY_DISCOVERY_MODES>(8)
		{
			KEY_DISCOVERY_MODES.CLICK_TO_FILE,
			KEY_DISCOVERY_MODES.CLICK_TO_FILE,
			KEY_DISCOVERY_MODES.CLICK_TO_PLAIN_SIGHT,
			KEY_DISCOVERY_MODES.CLICK_TO_PLAIN_SIGHT,
			KEY_DISCOVERY_MODES.PLAIN_SIGHT,
			KEY_DISCOVERY_MODES.PLAIN_SIGHT,
			KEY_DISCOVERY_MODES.SOURCE_CODE,
			KEY_DISCOVERY_MODES.SOURCE_CODE
		};
		ShuffleList(list2);
		for (int i = 0; i < keys.Count; i++)
		{
			dictionary.Add(keys[i], i);
		}
		for (int j = 0; j < wikiSites.Count; j++)
		{
			List<int> list3 = new List<int>(wikiSites[j]);
			int num = 0;
			while (num < 2)
			{
				int index = UnityEngine.Random.Range(0, list3.Count);
				WebSiteDefinition webSiteDefinition = Websites[list3[index]];
				if (!webSiteDefinition.isFake && !webSiteDefinition.DoNotTap && !webSiteDefinition.IsTapped)
				{
					webSiteDefinition.IsTapped = true;
					websitesToTap = websitesToTap + webSiteDefinition.PageTitle + ":";
					myWebSitesData.Sites[list3[index]].IsTapped = true;
					int index2 = 0;
					string text = list[0];
					int hashIndex = dictionary[text];
					if (webSiteDefinition.SubPages.Count > 0 && UnityEngine.Random.Range(0, 100) > 37)
					{
						int num2 = UnityEngine.Random.Range(0, webSiteDefinition.SubPages.Count);
						webSiteDefinition.SubPages[num2].IsTapped = true;
						webSiteDefinition.SubPages[num2].HashIndex = hashIndex;
						webSiteDefinition.SubPages[num2].HashValue = text;
						webSiteDefinition.SubPages[num2].KeyDiscoverMode = list2[0];
						if (myWebSitesData.Sites[list3[index]].Pages[num2 + 1] != null)
						{
							myWebSitesData.Sites[list3[index]].Pages[num2 + 1].IsTapped = true;
							myWebSitesData.Sites[list3[index]].Pages[num2 + 1].HashIndex = hashIndex;
							myWebSitesData.Sites[list3[index]].Pages[num2 + 1].HashValue = text;
							myWebSitesData.Sites[list3[index]].Pages[num2 + 1].KeyDiscoveryMode = (int)list2[0];
						}
					}
					else
					{
						webSiteDefinition.HomePage.IsTapped = true;
						webSiteDefinition.HomePage.HashIndex = hashIndex;
						webSiteDefinition.HomePage.HashValue = text;
						webSiteDefinition.HomePage.KeyDiscoverMode = list2[0];
						myWebSitesData.Sites[list3[index]].Pages[0].IsTapped = true;
						myWebSitesData.Sites[list3[index]].Pages[0].HashIndex = hashIndex;
						myWebSitesData.Sites[list3[index]].Pages[0].HashValue = text;
						myWebSitesData.Sites[list3[index]].Pages[0].KeyDiscoveryMode = (int)list2[0];
					}
					list.RemoveAt(index2);
					dictionary.Remove(text);
					list2.RemoveAt(0);
					num++;
				}
				list3.RemoveAt(index);
			}
		}
		for (int k = 1; k < wikiSites.Count; k++)
		{
			List<int> list4 = new List<int>(wikiSites[k]);
			int num3 = 0;
			while (num3 < 1)
			{
				int index3 = UnityEngine.Random.Range(0, list4.Count);
				WebSiteDefinition webSiteDefinition2 = Websites[list4[index3]];
				if (webSiteDefinition2.isFake || webSiteDefinition2.DoNotTap || webSiteDefinition2.IsTapped)
				{
					continue;
				}
				webSiteDefinition2.IsTapped = true;
				websitesToTap = websitesToTap + webSiteDefinition2.PageTitle + ":";
				myWebSitesData.Sites[index3].IsTapped = true;
				int index4 = 0;
				string text2 = list[index4];
				int hashIndex2 = dictionary[text2];
				if (webSiteDefinition2.SubPages.Count > 0 && UnityEngine.Random.Range(0, 100) > 37)
				{
					int num4 = UnityEngine.Random.Range(0, webSiteDefinition2.SubPages.Count);
					webSiteDefinition2.SubPages[num4].IsTapped = true;
					webSiteDefinition2.SubPages[num4].HashIndex = hashIndex2;
					webSiteDefinition2.SubPages[num4].HashValue = text2;
					webSiteDefinition2.SubPages[num4].KeyDiscoverMode = list2[0];
					if (myWebSitesData.Sites[list4[index3]].Pages[num4 + 1] != null)
					{
						myWebSitesData.Sites[list4[index3]].Pages[num4 + 1].IsTapped = true;
						myWebSitesData.Sites[list4[index3]].Pages[num4 + 1].HashIndex = hashIndex2;
						myWebSitesData.Sites[list4[index3]].Pages[num4 + 1].HashValue = text2;
						myWebSitesData.Sites[list4[index3]].Pages[num4 + 1].KeyDiscoveryMode = (int)list2[0];
					}
				}
				else
				{
					webSiteDefinition2.HomePage.IsTapped = true;
					webSiteDefinition2.HomePage.HashIndex = hashIndex2;
					webSiteDefinition2.HomePage.HashValue = text2;
					webSiteDefinition2.HomePage.KeyDiscoverMode = list2[0];
					myWebSitesData.Sites[list4[index3]].Pages[0].IsTapped = true;
					myWebSitesData.Sites[list4[index3]].Pages[0].HashIndex = hashIndex2;
					myWebSitesData.Sites[list4[index3]].Pages[0].HashValue = text2;
					myWebSitesData.Sites[list4[index3]].Pages[0].KeyDiscoveryMode = (int)list2[0];
				}
				list.RemoveAt(index4);
				dictionary.Remove(text2);
				list2.RemoveAt(0);
				num3++;
			}
		}
		DataManager.Save(myWebSitesData);
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		if (!DifficultyManager.HackerMode)
		{
			prepWebsites();
			prepWikis();
			prepBookmarks();
			prepTheMasterKey();
			tapSites();
			new GameObject("WallpaperUtils").AddComponent<WallpaperUtils>();
			if (ModSettings.DOSTwitchActive)
			{
				new GameObject("DOSTwitch").AddComponent<DOSTwitch>();
			}
		}
		LoadMods();
	}

	public void attemptNightmare()
	{
		for (int i = 0; i < 8; i++)
		{
			ForceKeyDiscover();
		}
		TannerDOSPopper.PopDOS(TannerDOSPopper.TannerMAXDOSPop);
		GameManager.TimeSlinger.FireTimer(8f, EnemyManager.DollMakerManager.ForceMarker);
		GameManager.TimeSlinger.FireTimer(14f, EnemyManager.BombMakerManager.ReleaseTheBombMakerInstantly);
		GameManager.TimeSlinger.FireTimer(18f, GameManager.HackerManager.theSwan.ActivateTheSwan);
		GameManager.TimeSlinger.FireTimer(20f, GameManager.HackerManager.ForceTwitchHack);
		GameManager.TimeSlinger.FireTimer(22f, EnemyManager.CultManager.attemptSpawn);
	}

	public void ForceInsanityEnding()
	{
		TimeKeeper.NightmareEndingTriggered = true;
		for (int i = 0; i < 25; i++)
		{
			DancingLoader.Ins.InstantinateAll(new Vector3(UnityEngine.Random.Range(-5f, 5f), 39.582f, UnityEngine.Random.Range(-5f, 5f)), new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));
		}
		for (int j = 0; j < 25; j++)
		{
			DancingLoader.Ins.InstantinateAll(new Vector3(UnityEngine.Random.Range(-25f, 30f), 39.582f, UnityEngine.Random.Range(-7.5f, -5.5f)), new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));
		}
		EnemyStateManager.AddEnemyState(ENEMY_STATE.CULT);
		PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.INSANITY);
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(CustomSoundLookUp.party, 0.4f);
		EnvironmentManager.PowerBehaviour.ForcePowerOff();
		StatisticsManager.Ins.BeatRun(Difficulty.NIGHTMARE);
		TrophyManager.Ins.nightmareTrophy.SetActive(value: true);
	}

	private void LoadMods()
	{
		UnityEngine.Object.Instantiate(CustomObjectLookUp.vapeAttack);
		UnityEngine.Object.Instantiate(CustomObjectLookUp.DOSAttack);
		if (DifficultyManager.HackerMode)
		{
			new GameObject("HackerModeManager").AddComponent<HackerModeManager>();
			return;
		}
		UnityEngine.Object.Instantiate(CustomObjectLookUp.TrophyCase);
		UnityEngine.Object.Instantiate(CustomObjectLookUp.Delfalco);
		UnityEngine.Object.Instantiate(CustomObjectLookUp.NewFemaleNoir);
		UnityEngine.Object.Instantiate(CustomObjectLookUp.NewMaleNoir);
		UnityEngine.Object.Instantiate(CustomObjectLookUp.ELM);
		setTaskbarColor();
		setWallpaper();
		InitANN = false;
		InitNotes = false;
		InitSource = false;
		InitSkybreak = false;
		GameObject.Find("VPNButton").SetActive(value: false);
		if (DifficultyManager.Nightmare)
		{
			GameManager.TimeSlinger.FireTimer(5f, GameManager.TheCloud.attemptNightmare);
		}
		GameManager.TimeSlinger.FireTimer(5f, delayLoadMods);
		GameManager.TimeSlinger.FireTimer(UnityEngine.Random.Range(1860f, 3480f), VoIPManager.ShowVoIP);
		TrackerManager.Build();
		new GameObject("BombMakerManager").AddComponent<BombMakerManager>();
		new GameObject("KidnapperManager").AddComponent<KidnapperManager>();
		new GameObject("TannerManager").AddComponent<TannerManager>();
		new GameObject("TarotManager").AddComponent<TarotManager>();
		new GameObject("DeepWebRadioManager").AddComponent<DeepWebRadioManager>();
		new GameObject("DancingLoader").AddComponent<DancingLoader>();
		new GameObject("ManipulatorHook").AddComponent<ManipulatorHook>();
		new GameObject("LucasPlusManager").AddComponent<LucasPlusManager>();
		new GameObject("EXEManager").AddComponent<EXEManager>();
		new GameObject("VWipeApp").AddComponent<VWipeApp>();
		new GameObject("PhoneManager").AddComponent<PhoneManager>();
		new GameObject("CamBatteryController").AddComponent<CamBatteryController>();
		new GameObject("KeypadManager").AddComponent<KeypadManager>();
		new GameObject("EventsManager").AddComponent<EventManager>();
		new GameObject("doorlogBanter").AddComponent<doorlogRandomBanter>();
		PhoneBehaviour.PlacePhoneObject();
		TarotManager.tappedSites = websitesToTap.Split(':');
		bool flag = false;
	}

	public string getWebsiteInfoForDev(int wiki)
	{
		int index = wikiLookUp[wikis[wiki - 1].PageURL];
		string text = string.Empty;
		for (int i = 0; i < wikiSites[index].Count; i++)
		{
			int index2 = wikiSites[index][i];
			string text2 = Websites[index2].PageTitle + " - " + getTimeStringForWebsite(Websites[index2]);
			text += text2;
			text += Environment.NewLine;
		}
		return text;
	}

	private string getTimeStringForWebsite(WebSiteDefinition webSite)
	{
		if (webSite.isFake)
		{
			return "FAKE";
		}
		if (!webSite.HasWindow)
		{
			return "ALWAYS";
		}
		return webSite.WindowTime switch
		{
			WEBSITE_WINDOW_TIME.FIRST_QUARTER => "00-15", 
			WEBSITE_WINDOW_TIME.SECOND_QUARTER => "15-30", 
			WEBSITE_WINDOW_TIME.THRID_QUARTER => "30-45", 
			WEBSITE_WINDOW_TIME.FOURTH_QUARTER => "45-60", 
			WEBSITE_WINDOW_TIME.FIRST_HALF => "00-30", 
			WEBSITE_WINDOW_TIME.SECNOND_HALF => "30-60", 
			_ => "ALWAYS", 
		};
	}

	public void SpawnManipulatorIcon(float timeFor, ManipulatorHook.THE_MANIPULATOR mANIPULATOR, ManipulatorHook.MANIPULATOR_TYPE tYPE)
	{
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.manipulator);
		switch (mANIPULATOR)
		{
		case ManipulatorHook.THE_MANIPULATOR.KEY:
			ManipulatorHook.Ins.UpdateKeyManipulator(tYPE);
			GameManager.TimeSlinger.FireTimer(timeFor, ManipulatorHook.Ins.RemoveKeyManipulator);
			keyActive = true;
			keyFireWindow = timeFor;
			keyTimeStamp = Time.time;
			break;
		case ManipulatorHook.THE_MANIPULATOR.SPEED:
			ManipulatorHook.Ins.UpdateBoostManipulator(tYPE);
			GameManager.TimeSlinger.FireTimer(timeFor, ManipulatorHook.Ins.RemoveBoostManipulator);
			speedActive = true;
			speedFireWindow = timeFor;
			speedTimeStamp = Time.time;
			break;
		}
	}

	private void delayLoadMods()
	{
		DonutTrigger.BuildTrigger();
		CoffeeTime.BuildTrigger();
		UnityEngine.Object.Instantiate(CustomObjectLookUp.BeerFor504);
		currencyTextHook.TextIns = GameObject.Find("CurrentCurrency").GetComponent<Text>();
		if (Themes.selected == THEME.WTTG2BETA && !DifficultyManager.Nightmare && !DifficultyManager.LeetMode && !DifficultyManager.HackerMode && !DifficultyManager.CasualMode)
		{
			currencyTextHook.TextIns.text = "10.1337";
		}
		bool flag = true;
	}

	public string PlayerLoc()
	{
		Vector3 position = roamController.Ins.transform.position;
		float x = position.x;
		float y = position.y;
		float z = position.z;
		return $"X: {x}, Y: {y}, Z: {z}";
	}

	public int getTenantNumberDev(int tenantIndex)
	{
		if (tenantIndex < 6)
		{
			return 1001 + tenantIndex;
		}
		if (tenantIndex < 12)
		{
			return 795 + tenantIndex;
		}
		if (tenantIndex < 18)
		{
			return 589 + tenantIndex;
		}
		if (tenantIndex < 24)
		{
			return 483 + tenantIndex;
		}
		if (tenantIndex < 30)
		{
			return 277 + tenantIndex;
		}
		if (tenantIndex < 32)
		{
			return 71 + tenantIndex;
		}
		return 72 + tenantIndex;
	}

	private void setTaskbarColor()
	{
		if (DifficultyManager.Nightmare)
		{
			GameObject.Find("TopBarBG").GetComponent<Image>().color = Color.red;
			GameObject.Find("BotBarBG").GetComponent<Image>().color = Color.red;
			if (Themes.selected == THEME.TWR)
			{
				GameObject.Find("TopBarBG").GetComponent<Image>().sprite = ThemesLookUp.WTTG1TWR.topBar;
				GameObject.Find("BotBarBG").GetComponent<Image>().sprite = ThemesLookUp.WTTG1TWR.botBar;
				GameObject.Find("TopBarBG").GetComponent<Image>().color = new Color(0.8f, 0.1f, 0f);
				GameObject.Find("BotBarBG").GetComponent<Image>().color = new Color(0.8f, 0.1f, 0f);
			}
			if (Themes.selected <= THEME.TWR)
			{
				GameObject.Find("MotionSensorButton/hoverIMG").SetActive(value: false);
				GameObject.Find("WifiButton/hoverIMG").SetActive(value: false);
				GameObject.Find("SoundButton/hoverIMG").SetActive(value: false);
				GameObject.Find("PowerButton/hoverIMG").SetActive(value: false);
				GameObject.Find("DOSCoinBTN/hoverIMG").SetActive(value: false);
			}
			return;
		}
		switch (Themes.selected)
		{
		case THEME.WTTG1:
			GameObject.Find("TopBarBG").GetComponent<Image>().color = new Color(0f, 1f, 1f);
			GameObject.Find("MotionSensorButton/hoverIMG").SetActive(value: false);
			GameObject.Find("WifiButton/hoverIMG").SetActive(value: false);
			GameObject.Find("SoundButton/hoverIMG").SetActive(value: false);
			GameObject.Find("PowerButton/hoverIMG").SetActive(value: false);
			GameObject.Find("DOSCoinBTN/hoverIMG").SetActive(value: false);
			break;
		case THEME.TWR:
			GameObject.Find("TopBarBG").GetComponent<Image>().sprite = ThemesLookUp.WTTG1TWR.topBar;
			GameObject.Find("BotBarBG").GetComponent<Image>().sprite = ThemesLookUp.WTTG1TWR.botBar;
			GameObject.Find("MotionSensorButton/hoverIMG").SetActive(value: false);
			GameObject.Find("WifiButton/hoverIMG").SetActive(value: false);
			GameObject.Find("SoundButton/hoverIMG").SetActive(value: false);
			GameObject.Find("PowerButton/hoverIMG").SetActive(value: false);
			GameObject.Find("DOSCoinBTN/hoverIMG").SetActive(value: false);
			break;
		case THEME.DEEPHUNTERS:
			GameObject.Find("TopBarBG").GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f);
			break;
		case THEME.WTTG2BETA:
		case THEME.WTTG2:
			break;
		}
	}

	private void setWallpaper()
	{
		switch (Themes.selected)
		{
		case THEME.WTTG1:
			WallpaperUtils.desktopWallpaper.sprite = (DifficultyManager.Nightmare ? ThemesLookUp.WTTG1TWR.wttg1nightmare : ThemesLookUp.WTTG1TWR.wttg1wallpaper);
			WallpaperUtils.desktopWallpaper.color = Color.white;
			GameObject.Find("ADOS").SetActive(value: false);
			break;
		case THEME.TWR:
			WallpaperUtils.desktopWallpaper.sprite = (DifficultyManager.Nightmare ? ThemesLookUp.WTTG1TWR.twr_nightmare : ThemesLookUp.WTTG1TWR.twr_wallpaper);
			WallpaperUtils.desktopWallpaper.color = Color.white;
			GameObject.Find("ADOS").SetActive(value: false);
			break;
		case THEME.WTTG2BETA:
			WallpaperUtils.desktopWallpaper.sprite = (DifficultyManager.Nightmare ? ThemesLookUp.WTTG2Beta.nightmareWallpaper : ThemesLookUp.WTTG2Beta.wallpaper);
			WallpaperUtils.desktopWallpaper.color = Color.white;
			GameObject.Find("ADOS").SetActive(value: false);
			break;
		case THEME.DEEPHUNTERS:
			WallpaperUtils.desktopWallpaper.sprite = null;
			WallpaperUtils.desktopWallpaper.color = ((!DifficultyManager.Nightmare) ? new Color(0.08627451f, 0.1490196f, 0.1960784f, 1f) : new Color(0.245283f, 0.09119143f, 0.07289071f, 1f));
			GameObject.Find("ADOS").SetActive(value: false);
			break;
		default:
			if (DifficultyManager.Nightmare)
			{
				WallpaperUtils.desktopWallpaper.sprite = CustomSpriteLookUp.adios;
				WallpaperUtils.desktopWallpaper.color = Color.white;
				GameObject.Find("ADOS").SetActive(value: false);
			}
			break;
		}
		TheSwanBehaviour.MySwanDefaultColor = WallpaperUtils.desktopWallpaper.color;
	}

	public WebSiteDefinition GetCurrentWebSiteDef()
	{
		return curWebsiteDef;
	}

	public void devOOOOoooSpookyJumpscare(Texture2D texture2D, AudioClip audio, float time = 0.85f)
	{
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<Image>().sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f);
		gameObject.GetComponent<RectTransform>().SetParent(LookUp.PlayerUI.HandTransform.transform);
		gameObject.GetComponent<RectTransform>().transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
		Debug.Log(gameObject.GetComponent<RectTransform>().transform.position.x);
		Debug.Log(gameObject.GetComponent<RectTransform>().transform.position.y);
		Debug.Log(gameObject.GetComponent<RectTransform>().transform.position.z);
		if (Screen.currentResolution.height == 720)
		{
			gameObject.transform.localScale = new Vector3(13f, 7f, 1f);
		}
		else if (Screen.currentResolution.height == 768)
		{
			gameObject.transform.localScale = new Vector3(14f, 8f, 1f);
		}
		else if (Screen.currentResolution.height == 900)
		{
			gameObject.transform.localScale = new Vector3(16f, 9f, 1f);
		}
		else if (Screen.currentResolution.height >= 1080)
		{
			gameObject.transform.localScale = new Vector3(20f, 11f, 1f);
		}
		gameObject.SetActive(value: true);
		GameManager.TimeSlinger.FireTimer(time, delegate
		{
			UnityEngine.Object.Destroy(gameObject);
		});
	}

	private void ShuffleList<T>(IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = UnityEngine.Random.Range(0, num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
