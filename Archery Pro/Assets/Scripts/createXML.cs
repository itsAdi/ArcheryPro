using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.IO;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class createXML : MonoBehaviour {
	public Image musicButton;
	public Sprite[] imageSprites;
	public AudioSource bgAudio;

	private bool returnToCaller;
	private string xmlLoc, musicStatus;

	void Start(){
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder ().Build ();
		PlayGamesPlatform.InitializeInstance (config);
		PlayGamesPlatform.Activate ();
		xmlLoc = Application.persistentDataPath + @"\kapData.xml";
		bgAudio = GameObject.FindGameObjectWithTag ("Scenery").GetComponent<AudioSource> ();
		if (!File.Exists (xmlLoc)) {
			try {
				using (StreamWriter sW = new StreamWriter(xmlLoc)) {
					sW.WriteLine ("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
					sW.WriteLine ("<root>");
					sW.WriteLine ("</root>");
				}
			} catch (IOException ex) {
				showDialog ("XML Creation Failed\n" + ex.TargetSite);
			}
			XmlDocument xDoc = new XmlDocument ();
			try {
				xDoc.Load (xmlLoc);
				try {
					XmlElement xRoot = xDoc.DocumentElement;
					try {
						XmlElement xMusic = xDoc.CreateElement ("music");
						xMusic.InnerText = "1";
						XmlElement xBest = xDoc.CreateElement ("easyBest");
						xBest.InnerText = "0";
						XmlElement xMbest = xDoc.CreateElement ("moderateBest");
						xMbest.InnerText = "0";
						XmlElement xHbest = xDoc.CreateElement ("hardBest");
						xHbest.InnerText = "0";
						xRoot.AppendChild (xMusic);
						xRoot.AppendChild (xBest);
						xRoot.AppendChild (xMbest);
						xRoot.AppendChild (xHbest);
						xDoc.Save (xmlLoc);
					} catch (XmlException xmlEx) {
						showDialog ("XML Writing Failed\n" + xmlEx.TargetSite);
					} catch (IOException ioEx) {
						showDialog ("File Writing Failed\n" + ioEx.TargetSite);
					}
				} catch (XmlException ex) {
					showDialog ("Corrupt XML Found\n" + ex.TargetSite);
				}
			} catch (XmlException ex) {
				showDialog ("XML Load Failed\n" + ex.TargetSite);
			}
		} else {
			readXML();
		}
	}

	void Update(){
		if (returnToCaller)
			Application.Quit ();
	}

	public void setMusic(){
		XmlDocument xDoc = new XmlDocument();
		try{
			xDoc.Load(xmlLoc);
			try{
				XmlNodeList xMusic = xDoc.GetElementsByTagName("music");
				if(musicStatus == "1"){
					xMusic[0].InnerText = "0";
					musicStatus = "0";
					musicButton.sprite = imageSprites[1];
					bgAudio.volume = 0f;
				}else{
					xMusic[0].InnerText = "1";
					musicStatus = "1";
					musicButton.sprite = imageSprites[0];
					bgAudio.volume = 1f;
				}
				xDoc.Save(xmlLoc);
			}catch(XmlException xmlEx){
				showDialog("XML Writing Failed\n" + xmlEx.TargetSite);
			}
			catch(IOException ioEx){
				showDialog("File Writing Failed\n" + ioEx.TargetSite);
			}
		}catch(XmlException ex){
			showDialog("XML Load Failed\n" + ex.TargetSite);
		}
	}

	void readXML(){
		XmlDocument xDoc = new XmlDocument();
		try{
			xDoc.Load(xmlLoc);
			try{
				XmlNodeList xMusic = xDoc.GetElementsByTagName("music");
				if(xMusic[0].InnerText == "1"){
					musicButton.sprite = imageSprites[0];
				}else{
					musicButton.sprite = imageSprites[1];
					bgAudio.volume = 0f;
				}
				musicStatus = xMusic[0].InnerText;
			}catch(XmlException ex){
				showDialog("Reading XML Failed\n" + ex.TargetSite);
			}
		}catch(XmlException ex){
			showDialog("XML Load Failed\n" + ex.TargetSite);
		}
	}

	public void loadFB(){
		Application.OpenURL ("https://m.facebook.com/KGDevs");
	}

	public void loadMoreGames(){
		Application.OpenURL ("https://play.google.com/store/apps/developer?id=Kemoth+Games");
	}

	public void showLB(){
		if (PlayGamesPlatform.Instance.localUser.authenticated) {
			PlayGamesPlatform.Instance.ShowLeaderboardUI ();
		} else {
			PlayGamesPlatform.Instance.localUser.Authenticate ((result) => {
				if(result){
					PlayGamesPlatform.Instance.ShowLeaderboardUI ();
				}
			});
		}
	}

	private class positiveButtonListener : AndroidJavaProxy{
		createXML hgO;
		public positiveButtonListener(createXML HGO):base("android.content.DialogInterface$OnClickListener"){
			hgO = HGO;
		}
		public void OnClick(AndroidJavaObject ajO, int value){
			hgO.returnToCaller = true;
		}
	}
	
	private void showDialog(string msg){
		AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = unityPlayer.GetStatic< AndroidJavaObject>  ("currentActivity");
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>  {
			AndroidJavaObject alertDialogBuilder = new AndroidJavaObject("android/app/AlertDialog$Builder", activity);
			alertDialogBuilder.Call< AndroidJavaObject> ("setMessage", msg);
			alertDialogBuilder.Call< AndroidJavaObject> ("setCancelable", true);
			alertDialogBuilder.Call< AndroidJavaObject> ("setPositiveButton", "Yes", new positiveButtonListener(this));
			AndroidJavaObject dialog = alertDialogBuilder.Call< AndroidJavaObject> ("create");
			dialog.Call("show");
		}));
	}
}
