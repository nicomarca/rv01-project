using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class ExitJailScript : MonoBehaviour {

	void Start () {
		

	}
	
	void Update () {
		
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Player") {
			SceneManager.LoadScene ("Assets/Scenes/MainScene.unity", LoadSceneMode.Single);
			/*if (Application.isEditor) {
				EditorSceneManager.OpenScene ("Assets/Scenes/MainScene.unity");
			} else {
				SceneManager.LoadScene ("Assets/Scenes/MainScene.unity", LoadSceneMode.Single);
			}*/
		}
	}
}
