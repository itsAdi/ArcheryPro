using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class detectArrow : MonoBehaviour {
	public Transform arrowTipObject;
	public Text scoreTextObject, scoreIndicatorObject;
	public AudioClip[] hitResultAudio;

	private AudioSource hitAudio;
	private Rigidbody2D arrRgbd;
	private float hitScore;
	private bool stopTrigger;
	private handleBowAnArrow hBA;
	private Color indicatorColor;

	void Start(){
		hitScore = 0f;
		scoreTextObject.text = hitScore.ToString ();
		Invoke ("setIndicatorAlphaZero", 1f);
		hBA = GameObject.FindGameObjectWithTag ("GameController").GetComponent<handleBowAnArrow> ();
		hitAudio = GetComponent<AudioSource> ();
	}

	void Update(){
		if (hBA.enableShoot)
			stopTrigger = false;
	}

	void OnTriggerEnter2D(Collider2D coll){
		if (!stopTrigger) {
			stopTrigger = true;
			if (coll.gameObject.tag == "Player") {
				hitAudio.Play();
				hBA.arrowFired = false;
				arrRgbd = coll.gameObject.GetComponent<Rigidbody2D>();
				arrRgbd.isKinematic = true;
				float tempFloat = 0f;
				tempFloat = Mathf.Clamp(Mathf.Ceil(100f - (((arrowTipObject.position - transform.position).sqrMagnitude / 2f) * 100f)), 0f, 100f);
				if(tempFloat >= 95f){
					if(hitAudio.isPlaying){
						hitAudio.Stop();
					}
					hitAudio.clip = hitResultAudio[1];
					hitAudio.Play();
					Invoke("resetClip", 1f);
					hBA.arrowCount = hBA.arrowCount != 0 ? hBA.arrowCount - 1 : 0;
				}
				scoreIndicatorObject.text = tempFloat.ToString();
				setIndicatorAlphaOne();
				hitScore += tempFloat;
				scoreTextObject.text = hitScore.ToString ();
				coll.gameObject.transform.SetParent(transform.parent, true);
			}
		}
	}

	void setIndicatorAlphaOne(){
		scoreIndicatorObject.CrossFadeAlpha (1f, 0.2f, true);
		Invoke ("setIndicatorAlphaZero", 2f);
	}

	void setIndicatorAlphaZero(){
		scoreIndicatorObject.CrossFadeAlpha (0f, 1.5f, true);
	}

	void resetClip(){
		hitAudio.clip = hitResultAudio [0];
	}
}
