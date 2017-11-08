using UnityEngine;
using System.Collections;


public class RayCastingController : MonoBehaviour 
{
	const float BASEDISTANCE = 1;

	private float distanceToObj;	// Distance entre le personnage et l'objet saisi
	private Rigidbody attachedObject;	// Objet saisi, null si aucun objet saisi
	private Vector3 objectSizeInitial;
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
		RaycastHit hitInfo;
		Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay (ray.origin, ray.direction * RAYCASTLENGTH, Color.blue);
		bool rayCasted = Physics.Raycast (ray, out hitInfo, RAYCASTLENGTH);

		if (rayCasted) 
		{
			rayCasted = hitInfo.transform.CompareTag ("draggable");
		}

		if (Input.GetMouseButtonDown (0))	// L'utilisateur vient de cliquer
		{
			if (rayCasted) 
			{
				Debug.Log ("Object attached");
				attachedObject = hitInfo.rigidbody;
				attachedObject.isKinematic = true;
				distanceToObj = hitInfo.distance;

				/** On place l'objet dans la main de l'utilisateur **/
				//Déterminer la taille et la distance
				objectSizeInitial = attachedObject.GetComponent<Renderer>().bounds.size;
				float sizeY = objectSizeInitial.y;
				distanceInitial = hitInfo.distance;
				//Calculer nouvelle taille
				newSizeY = BASEDISTANCE/distanceInitial*sizeY;
				Debug.Log (sizeY);
				Debug.Log (newSizeY);
				float ratio = newSizeY / sizeY;
				//attachedObject.GetComponent<Renderer>().bounds.size.Set(objectSizeInitial.x * ratio, newSizeY, objectSizeInitial.z * ratio);
				attachedObject.transform.localScale = new Vector3(objectSizeInitial.x * ratio, newSizeY, objectSizeInitial.z * ratio);
				//Translater
				attachedObject.MovePosition (ray.origin + (ray.direction * BASEDISTANCE));
			}
		} 

		else if (Input.GetMouseButtonUp (0) && attachedObject != null) 	// L'utilisateur relache un objet saisi
		{
			attachedObject.isKinematic = false;
			attachedObject = null;
			Debug.Log ("Object detached");
			if (rayCasted) 
			{
				Cursor.SetCursor (cursorDraggable, hotSpot, cursorMode);
			} else 
			{
				Cursor.SetCursor (cursorOff, hotSpot, cursorMode);
			}
		} 

		if (Input.GetMouseButton (0) && attachedObject != null) // L'utilisateur continue la saisie d'un objet
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


