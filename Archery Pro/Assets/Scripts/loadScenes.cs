using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class loadScenes : MonoBehaviour {
	public void sceneIndex(int index){
		SceneManager.LoadScene (index);
	}
}
