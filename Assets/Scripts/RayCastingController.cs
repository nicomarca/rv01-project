using UnityEngine;
using System.Collections;


public class RayCastingController : MonoBehaviour 
{
	const float BASEDISTANCE = 1;

	private float distanceToObj;	// Distance entre le personnage et l'objet saisi
	private Rigidbody attachedObject;	// Objet saisi, null si aucun objet saisi
	private Vector3 objectSizeInitial;
	private Vector3 secondObjectSize;

	private float distanceInitial;
	private float newSizeY;

	public const int RAYCASTLENGTH = 100;	// Longueur du rayon issu de la caméra
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = new Vector2(16, 16);	// Offset du centre du curseur
	public Texture2D cursorOff, cursorDragged, cursorDraggable;	// Textures à appliquer aux curseurs


	void Start () 
	{
		distanceToObj = -1;
		Cursor.SetCursor (cursorOff, hotSpot, cursorMode);
		Cursor.visible = true;
		Cursor.SetCursor (cursorDragged, hotSpot, cursorMode);

	}


	void Update () 
	{
		// Le raycast attache un objet cliqué
		RaycastHit[] hitInfo;
		RaycastHit firstHit;
		Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay (ray.origin, ray.direction * RAYCASTLENGTH, Color.blue);
		bool rayCasted = Physics.Raycast (ray, out firstHit, RAYCASTLENGTH);
		RaycastHit objectFirstPlane;

		if (rayCasted) 
		{
			rayCasted = firstHit.transform.CompareTag ("draggable");

		}

		if (Input.GetMouseButtonDown (0) && attachedObject == null)	// L'utilisateur vient de cliquer
		{

			if (rayCasted) 
			{
				objectFirstPlane = firstHit;
				Debug.Log ("Object attached");
				attachedObject = objectFirstPlane.rigidbody;
				attachedObject.isKinematic = true;
				distanceToObj = objectFirstPlane.distance;

				/** On place l'objet dans la main de l'utilisateur **/
				//Déterminer la taille et la distance
				objectSizeInitial = attachedObject.GetComponent<Renderer>().bounds.size;
				float sizeY = objectSizeInitial.y;
				distanceInitial = objectFirstPlane.distance;
				//Calculer nouvelle taille
				newSizeY = BASEDISTANCE/distanceInitial*sizeY;
				Debug.Log (sizeY);
				Debug.Log (newSizeY);
				float ratio = newSizeY / sizeY;
				//attachedObject.GetComponent<Renderer>().bounds.size.Set(objectSizeInitial.x * ratio, newSizeY, objectSizeInitial.z * ratio);
				attachedObject.transform.localScale = new Vector3(objectSizeInitial.x * ratio, newSizeY, objectSizeInitial.z * ratio);
				attachedObject.MovePosition (ray.origin + (ray.direction * BASEDISTANCE));
			}
		} 

		else if (Input.GetMouseButtonDown (0) && attachedObject != null) 	// L'utilisateur relache un objet saisi
		{
			hitInfo = Physics.RaycastAll (ray, (float)RAYCASTLENGTH);
			Debug.Log (hitInfo.Length);
			if (hitInfo.Length >= 2) {
				RaycastHit objectSecondPlane;

				objectFirstPlane = hitInfo [0];
				objectSecondPlane = hitInfo [1];
				Debug.Log ("info premier objet");
				Debug.Log (hitInfo [0].transform.name);
				Debug.Log (hitInfo [0].transform.position);

				Debug.Log ("info deuxieme objet");
				Debug.Log (hitInfo [1].transform.name);
				Debug.Log (hitInfo [1].transform.position);



				if (objectFirstPlane.transform.CompareTag ("draggable") && objectSecondPlane.transform.GetComponent<Renderer> ()) {
					Debug.Log ("distance actuelle");
					Debug.Log (hitInfo [0].distance);
					Debug.Log ("nouvelle distance");
					Debug.Log (hitInfo [1].distance);




					distanceToObj = objectSecondPlane.distance;
					objectSizeInitial = hitInfo [0].rigidbody.GetComponent<Renderer> ().bounds.size;
					secondObjectSize = objectSecondPlane.transform.GetComponent<Renderer> ().bounds.size;
					float sizeY = objectSizeInitial.y;
					distanceInitial = objectFirstPlane.distance;
					//Calculer nouvelle taille
					newSizeY = (distanceToObj - secondObjectSize.x) / BASEDISTANCE * sizeY;
					Debug.Log ("tailleactuelle");
					Debug.Log (sizeY);
					Debug.Log ("nouvelle taille");
					Debug.Log (newSizeY);
					float ratio = newSizeY / sizeY;
					//Translater
					Debug.Log ("ray direction");
					Debug.Log (ray.direction);
					Debug.Log ("nouvelle position");
					Debug.Log (ray.origin + (ray.direction * (distanceToObj)));

					hitInfo [0].transform.position = (ray.origin + (ray.direction * (distanceToObj - (objectSizeInitial.z * ratio) / 2)));
					//attachedObject.GetComponent<Renderer>().bounds.size.Set(objectSizeInitial.x * ratio, newSizeY, objectSizeInitial.z * ratio);
					attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x * ratio, newSizeY, objectSizeInitial.z * ratio);

			
					attachedObject.isKinematic = false;
					attachedObject = null;
					Debug.Log ("Object detached");
					if (rayCasted) {
						Cursor.SetCursor (cursorDraggable, hotSpot, cursorMode);
					} else {
						Cursor.SetCursor (cursorOff, hotSpot, cursorMode);
					}
			
				}
			}
		}

		if ( attachedObject != null) // L'utilisateur continue la saisie d'un objet
		{
			attachedObject.MovePosition (ray.origin + (ray.direction * BASEDISTANCE));
			Cursor.SetCursor (cursorDragged, hotSpot, cursorMode);

		} 
		else  // L'utilisateur bouge la sourie sans cliquer 
		{
			if (rayCasted) 
			{
				Cursor.SetCursor (cursorDraggable, hotSpot, cursorMode);
			} 
			else 
			{
				Cursor.SetCursor (cursorOff, hotSpot, cursorMode);
			}
		}
	}
}


