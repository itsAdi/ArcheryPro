using UnityEngine;
using System.Collections;

public class setDifficulty : MonoBehaviour {
	public enum modeEnum {easy, moderate, hard};
	public modeEnum playMode = modeEnum.easy;

	private Rigidbody2D selfRgbd;
	private Vector2 movePos;
	private bool moveRgbd, goingUp;
	private Vector2[] screenDeltas;
	private float newX, speed;

	void Start(){
		selfRgbd = transform.GetComponent<Rigidbody2D> ();
		screenDeltas = new Vector2[2];
		movePos = new Vector2 ();
		goingUp = true;
		speed = 0f;
		switch (playMode) {
		case modeEnum.easy :
			movePos = Camera.main.ScreenToWorldPoint(new Vector3((90f / 100f) * Screen.width,Screen.height / 2f, -1f));
			selfRgbd.position = movePos;
			break;
		case modeEnum.moderate:
			movePos = Camera.main.ScreenToWorldPoint(new Vector3((90f / 100f) * Screen.width,(10f / 100f) * Screen.height, -1f));
			speed = 2.5f;
			moveRgbd = true;
			break;
		case modeEnum.hard :
			movePos = Camera.main.ScreenToWorldPoint(new Vector3((40f / 100f) * Screen.width,(10f / 100f) * Screen.height, -1f));
			speed = 3.5f;
			moveRgbd = true;
			break;
		}
		screenDeltas [0] = Camera.main.ScreenToWorldPoint (new Vector3(selfRgbd.position.x, (90f / 100f) * Screen.height, -1f));
		screenDeltas [1] = Camera.main.ScreenToWorldPoint (new Vector3(selfRgbd.position.x, (10f / 100f) * Screen.height, -1f));
	}

	void Update(){
		if (selfRgbd.position.y > screenDeltas [0].y)
			goingUp = false;
		if (selfRgbd.position.y < screenDeltas [1].y)
			goingUp = true;
	}

	void FixedUpdate(){
		if (moveRgbd) {
			if(goingUp){
				selfRgbd.MovePosition(selfRgbd.position + (Vector2.up * speed) * Time.deltaTime);
			}else{
				selfRgbd.MovePosition(selfRgbd.position + (Vector2.down * speed) * Time.deltaTime);
			}
		}
	}

	public void setNewPos(){
		moveRgbd = false;
		int rndNum = Random.Range (0, 6);
		switch (rndNum) {
		case 0:
			newX = (40f/ 100f) * Screen.width;
			break;
		case 1:
			newX = (50f/ 100f) * Screen.width;
			break;
		case 2:
			newX = (60f/ 100f) * Screen.width;
			break;
		case 3:
			newX = (70f/ 100f) * Screen.width;
			break;
		case 4:
			newX = (80f/ 100f) * Screen.width;
			break;
		case 5:
			newX = (90f/ 100f) * Screen.width;
			break;
		}
		movePos = Camera.main.ScreenToWorldPoint(new Vector3(newX,(10f / 100f) * Screen.height, -1f));
		movePos.y = selfRgbd.position.y;
		selfRgbd.position = movePos;
		moveRgbd = true;
	}
}
