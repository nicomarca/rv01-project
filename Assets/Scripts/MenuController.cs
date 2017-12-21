using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void ButtonLaunch(){
		SceneManager.LoadScene ("Assets/Scenes/JailScene.unity");
	}

	public void ButtonSkipTuto(){
		SceneManager.LoadScene ("Assets/Scenes/MainScene.unity");
	}

	public void ButtonExit(){
		Application.Quit ();
	}
		
}
