using UnityEngine;
using System.Xml;

public class handleAudioScript : MonoBehaviour {
	public AudioSource shootAudio, hitAudio;
	[HideInInspector]
	public bool returnToCaller;

	private string xmlLoc;

	void Start(){
		xmlLoc = Application.persistentDataPath + @"\kapData.xml";
		XmlDocument xDoc = new XmlDocument();
		try{
			xDoc.Load(xmlLoc);
			try{
				XmlNodeList xMusic = xDoc.GetElementsByTagName("music");
				if(xMusic[0].InnerText == "0")
					shootAudio.volume = hitAudio.volume = 0f;
			}catch(XmlException ex){
				showDialog("Corrupt XML Found\n" + ex.TargetSite);
			}
		}catch(XmlException ex){
			showDialog("XML Load Failed\n" + ex.TargetSite);
		}
	}

	void Update(){
		if (returnToCaller) {
			returnToCaller = false;
			Application.Quit ();
		}
	}
	
	private class positiveButtonListener : AndroidJavaProxy{
		handleAudioScript hgO;
		public positiveButtonListener(handleAudioScript HGO):base("android.content.DialogInterface$OnClickListener"){
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
