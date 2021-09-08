using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Xml;
using GooglePlayGames;

public class handleGameOver : MonoBehaviour {
	[HideInInspector]
	public bool gameIsOver;
	public GameObject windowObject;
	public Text gameplayScoreText, windowScoreText, bestScoreText;
	public Button homeButton, retryButton;

	private bool returnToCaller;
	private string xmlLocation;
	private int currentScore, bestScore;
	private XmlDocument xDoc;
	private XmlNodeList xBestScore;
	private string[] lbIDs;

	void Start(){
		lbIDs = new string[3]{"CgkIyYCZ38UJEAIQBQ","CgkIyYCZ38UJEAIQBg","CgkIyYCZ38UJEAIQBw"};
		xmlLocation = Application.persistentDataPath + @"\kapData.xml";
		xDoc = new XmlDocument ();
		try{
			if (!PlayGamesPlatform.Instance.localUser.authenticated) {
				PlayGamesPlatform.Instance.localUser.Authenticate ((result) =>{
					if(result){
						showDialog("Done with login");
					}else{
						showDialog("Something went wrong");
					}
				});
			}
			xDoc.Load(xmlLocation);
			try{
				xBestScore = xDoc.ChildNodes;
				switch(SceneManager.GetActiveScene().buildIndex){
				case 1:
					xBestScore = xDoc.GetElementsByTagName("easyBest");
					break;
				case 2:
					xBestScore = xDoc.GetElementsByTagName("moderateBest");
					break;
				case 3:
					xBestScore = xDoc.GetElementsByTagName("hardBest");
					break;
				}
				int.TryParse(xBestScore[0].InnerText, out bestScore);
				bestScoreText.text = bestScore.ToString();
			}catch(XmlException ex){
				showDialog("XML Read Failed\n" + ex.TargetSite);
			}
		}catch(XmlException ex){
			showDialog("XML Load Failed\n" + ex.TargetSite);
		}
	}

	void Update(){
		if (returnToCaller)
			Application.Quit ();
	}

	public void gameOverFunc(){
		int.TryParse (gameplayScoreText.text, out currentScore);
		if (currentScore > bestScore) {
			try{
				xBestScore[0].InnerText = currentScore.ToString();
				xDoc.Save(xmlLocation);
				bestScoreText.text = currentScore.ToString();
				if(PlayGamesPlatform.Instance.localUser.authenticated){
					postScore((long) currentScore, lbIDs[SceneManager.GetActiveScene().buildIndex - 1]);
				}else{
					PlayGamesPlatform.Instance.localUser.Authenticate((result) => {
						if(result){
							postScore((long) currentScore, lbIDs[SceneManager.GetActiveScene().buildIndex - 1]);
						}
					});
				}
			}catch(XmlException ex){
				showDialog("XML Writing Failed\n" + ex.TargetSite);
			}
		}
		windowScoreText.text = currentScore.ToString ();
		windowObject.SetActive (true);
		windowScoreText.gameObject.SetActive (true);
		bestScoreText.gameObject.SetActive (true);
		homeButton.onClick.RemoveAllListeners ();
		homeButton.onClick.AddListener (loadHome);
		retryButton.onClick.RemoveAllListeners ();
		retryButton.onClick.AddListener (reloadScene);
		homeButton.gameObject.SetActive (true);
		retryButton.gameObject.SetActive (true);
	}

	public void loadHome(){
		SceneManager.LoadScene (0);
	}

	public void reloadScene(){
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}

	void postScore(long score, string boardID){
		PlayGamesPlatform.Instance.ReportScore (score, boardID, (result) => {
//			showDialog(boardID + " ; " + (SceneManager.GetActiveScene().buildIndex - 1).ToString());
		});
	}

	private class positiveButtonListener : AndroidJavaProxy{
		handleGameOver hgO;
		public positiveButtonListener(handleGameOver HGO):base("android.content.DialogInterface$OnClickListener"){
			hgO = HGO;
		}
		public void OnClick(AndroidJavaObject ajO, int value){
			hgO.returnToCaller = false;
		}
	}

	private void showDialog(string msg){
		AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = unityPlayer.GetStatic< AndroidJavaObject>  ("currentActivity");
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>  {
			AndroidJavaObject alertDialogBuilder = new AndroidJavaObject("android/app/AlertDialog$Builder", activity);
			alertDialogBuilder.Call< AndroidJavaObject> ("setMessage", msg);
			alertDialogBuilder.Call< AndroidJavaObject> ("setCancelable", false);
			alertDialogBuilder.Call< AndroidJavaObject> ("setPositiveButton", "Yes", new positiveButtonListener(this));
			AndroidJavaObject dialog = alertDialogBuilder.Call< AndroidJavaObject> ("create");
			dialog.Call("show");
		}));
	}
}
