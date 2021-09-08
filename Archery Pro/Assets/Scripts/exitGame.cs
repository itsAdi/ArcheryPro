using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using GoogleMobileAds.Api;

public class exitGame : MonoBehaviour {
	private BannerView bnrAd;
	private bool adLoaded;
	void Start(){
		string adUnitId = "ca-app-pub-8347424776413444/3344338019";
		if (SceneManager.GetActiveScene().buildIndex != 0) {
			bnrAd = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
			bnrAd.OnAdLoaded += onBnrLoaded;
			AdRequest req = new AdRequest.Builder().Build();
			bnrAd.LoadAd(req);
		}
	}

	void Update () {
		if (Input.GetKeyUp (KeyCode.Escape)) {
			if(adLoaded){
				bnrAd.Destroy ();
			}
			Application.Quit ();
		}
	}
	public void onBnrLoaded(object sender, EventArgs args){
		adLoaded = true;
	}

	public void goToHome(){
		if (adLoaded) {
			bnrAd.Destroy ();
		}
		SceneManager.LoadScene (0);
	}
}
