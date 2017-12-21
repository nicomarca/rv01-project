using UnityEngine;
using System.Collections;

public class ActivateChest : MonoBehaviour {

	public Transform lid, lidOpen, lidClose;	// Lid, Lid open rotation, Lid close rotation
	public float openSpeed = 5F;				// Opening speed
	public bool canClose;						// Can the chest be closed
	private float distanceToPlayer = 10.0f;			// Distance between mirror and player
	[HideInInspector]
	public bool _open;							// Is the chest opened
	public GameObject orbe;
	public GameObject player;  						// Player

	void Start(){
		_open = true;
	}

	void Update () {
		distanceToPlayer = Vector3.Distance (transform.position - new Vector3 (0, transform.position.y, 0), player.transform.position - new Vector3 (0, player.transform.position.y, 0));

		if(_open){
			ChestClicked(lidOpen.rotation);
			orbe.SetActive (true);

		}
		else{
			ChestClicked(lidClose.rotation);
		}
	}
	
	// Rotate the lid to the requested rotation
	void ChestClicked(Quaternion toRot){
		if(lid.rotation != toRot){
			lid.rotation = Quaternion.Lerp(lid.rotation, toRot, Time.deltaTime * openSpeed);
		}
	}
	
	void OnMouseDoswn(){
		if(canClose) _open = !_open; 
		else _open = true;
	}

	void OnTriggerEnter(Collider other)
	{

		if (other.transform.name == "key") {
			if (other.transform.lossyScale.x > 0.8 && (distanceToPlayer < 40)) {
				_open = true;
			} else {
			
				//TODO VOICE
				Debug.Log("trop petite");
			}
		}
	}
}
