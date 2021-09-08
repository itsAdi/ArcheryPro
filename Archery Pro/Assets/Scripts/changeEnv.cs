using UnityEngine;
using System.Collections;

public class changeEnv : MonoBehaviour {
	public SpriteRenderer[] envRenderers;
	public Sprite[] daySprites;
	public Sprite[] nightSprites;

	private bool moveCloud;
	private float speed;
	private Transform cloudTransform;
	private Vector3 newPos, flagPos;

	void Start () {
		int rndNum = Random.Range (1, 11);
		if (rndNum % 2 == 0) {
			for (int i = 0; i < envRenderers.Length; i++) {
				envRenderers [i].sprite = daySprites [i];
			}
		} else {
			for (int i = 0; i < envRenderers.Length; i++) {
				envRenderers [i].sprite = nightSprites [i];
			}
		}
		speed = 0.2f;
		cloudTransform = envRenderers [1].gameObject.transform;
		newPos = flagPos = new Vector3 ();
		newPos.y = -2.5f;
		flagPos = Camera.main.ScreenToWorldPoint (new Vector3(-600f,0f,Camera.main.farClipPlane));
		moveCloud = true;
	}

	void Update(){
		if (moveCloud) {
			newPos.x -= speed * Time.deltaTime;
			cloudTransform.position = newPos;
			if(newPos.x < flagPos.x){
				newPos.x = Camera.main.ScreenToWorldPoint(new Vector3(((float)Screen.width + 600f), 0f, Camera.main.farClipPlane)).x;
				newPos.y = -2.5f;
			}
		}
	}
}
