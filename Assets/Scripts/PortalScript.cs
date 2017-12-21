using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Player") {

			if (SceneManager.GetActiveScene ().name == "MainScene") {
				SceneManager.LoadScene ("Assets/Scenes/Sewer.unity", LoadSceneMode.Single);
			} else if (SceneManager.GetActiveScene ().name == "Sewer") {
			
				SceneManager.LoadScene ("Assets/Scenes/MainScene back.unity", LoadSceneMode.Single);

			}
			/*if (Application.isEditor) {
				EditorSceneManager.OpenScene ("Assets/Scenes/MainScene.unity");
			} else {
				SceneManager.LoadScene ("Assets/Scenes/MainScene.unity", LoadSceneMode.Single);
			}*/
		}
	}
}
