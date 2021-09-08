using UnityEngine;

public class singletonClass : MonoBehaviour {
	private static singletonClass instance = null;

	void Awake(){
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}
}
