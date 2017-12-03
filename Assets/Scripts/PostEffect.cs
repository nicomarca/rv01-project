using UnityEngine;
using System.Collections;

public class PostEffect : MonoBehaviour 
{
	Camera AttachedCamera;
	public Shader Post_Outline;

	void Start () 
	{
		AttachedCamera = GetComponent<Camera>();
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{

	}

}
