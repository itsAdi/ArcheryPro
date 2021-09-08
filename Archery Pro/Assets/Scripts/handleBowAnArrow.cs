using UnityEngine;
using System.Collections;

public class handleBowAnArrow : MonoBehaviour {
	public Transform attackObjectsContainerTransform, bowTransform, arrowTransform;
	public Material lineMat;
	public Rigidbody2D arrowRgbd;
	public SpringJoint2D springObject;
	[HideInInspector]
	public bool invokeReset;
	[HideInInspector]
	public bool enableShoot,arrowFired;
	[HideInInspector]
	public int arrowCount;

	private AudioSource shootAudio;
	private Vector3 arrowPos, lookToMouse;
	private bool isDragging, audioPlayed;
	private float drag;
	private handleGameOver gameOverScript;
	private setDifficulty sD;

	void Start(){
		arrowPos = new Vector3 ();
		enableShoot = true;
		arrowCount = 0;
		gameOverScript = GetComponent<handleGameOver> ();
		shootAudio = GetComponent<AudioSource> ();
		sD = GameObject.FindGameObjectWithTag ("Enemy").GetComponent<setDifficulty>();
	}
	void Update () {
		if (enableShoot) {
			if (Input.GetMouseButton (0)) {
				isDragging = true;
				Vector3 currPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				Vector3 diff = bowTransform.position - currPos;
				bowTransform.rotation = Quaternion.Euler (0f, 0f, getRotation (diff));
				Dragging ();
			} else {
				if (isDragging) {
					arrowRgbd.isKinematic = false;
					isDragging = false;
					arrowFired = true;
				}
			}
		}
		if (arrowFired) {
			if(!audioPlayed){
				audioPlayed = true;
				shootAudio.Play();
			}
			if(arrowTransform.localPosition.x >= -0.91f){
				if(springObject.enabled){
					springObject.enabled = false;
					enableShoot = false;
					arrowCount++;
					Invoke("resetGame", 2f);
				}
			}
			if(arrowRgbd.velocity.sqrMagnitude >= 0.2f){
				arrowTransform.rotation = Quaternion.Euler(0f, 0f, getRotation(arrowRgbd.position - arrowRgbd.velocity) - 180f);
			}
		}
		if (invokeReset) {
			invokeReset = false;
			if(IsInvoking("resetGame")){
				CancelInvoke("resetGame");
			}
			resetGame();
		}
	}

	void Dragging(){
		lookToMouse = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		lookToMouse.z = 0f;
		arrowPos = bowTransform.InverseTransformPoint (lookToMouse);
		arrowPos.x = Mathf.Clamp (arrowPos.x, -1.5f, -0.92f);
		arrowTransform.localPosition = arrowPos;
	}

	float getRotation(Vector3 difference){
		difference.Normalize ();
		float rotZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg;
		return rotZ;
	}

	void resetGame(){
		arrowFired = false;
		bowTransform.rotation = Quaternion.identity;
		arrowTransform.SetParent (bowTransform, true);
		arrowTransform.rotation = Quaternion.identity;
		arrowTransform.localPosition = new Vector3(-0.92f, 0f, 0f);
		springObject.enabled = true;
		arrowRgbd.isKinematic = true;
		if (arrowCount != 10) {
			enableShoot = true;
			audioPlayed = false;
			if(sD.playMode == setDifficulty.modeEnum.hard || sD.playMode == setDifficulty.modeEnum.easy){
				sD.setNewPos();
			}
		} else {
			gameOverScript.gameOverFunc();
		}
	}

	public void OnRenderObject(){
		lineMat.SetPass (0);
		GL.PushMatrix ();
		GL.MultMatrix (bowTransform.localToWorldMatrix);
		GL.Begin (GL.LINES);
		GL.Color (new Color(1f,1f,1f,1f));
		if (isDragging) {
			GL.Vertex3 (-0.92f,1.2f,1f);
			GL.Vertex3 (arrowTransform.localPosition.x,0f,1f);
			GL.Vertex3 (arrowTransform.localPosition.x,0f,1f);
			GL.Vertex3 (-0.92f,-1.2f,1f);
		} else {
			GL.Vertex3 (-0.92f,1.2f,1f);
			GL.Vertex3 (-0.92f,0f,1f);
			GL.Vertex3 (-0.92f,0f,1f);
			GL.Vertex3 (-0.92f,-1.2f,1f);
		}
		GL.End ();
		GL.PopMatrix ();
	}
}
