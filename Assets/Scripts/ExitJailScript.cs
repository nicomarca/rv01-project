using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class ExitJailScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider collider){
		Debug.Log ("1");
		if (collider.gameObject.tag == "Player") {
			Debug.Log ("2");
			SceneManager.LoadScene ("Assets/Scenes/MainScene.unity", LoadSceneMode.Single);
			/*if (Application.isEditor) {
				Debug.Log ("3");
				EditorSceneManager.OpenScene ("Assets/Scenes/MainScene.unity");
			} else {
				SceneManager.LoadScene ("Assets/Scenes/MainScene.unity", LoadSceneMode.Single);
			}*/
		}
	}
}
