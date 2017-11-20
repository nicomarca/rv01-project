using UnityEngine;
using System.Collections;
using UnityEngine.VR;


public class RayCastingController : MonoBehaviour 
{
	private const int RAYCASTLENGTH = 200;		// Length of the ray

	private float distanceToObj;				// Distance entre le personnage et l'objet saisi
	private float ratio;						// Ratio between the distances
	private float newSizeY;						// New vertical size of the moved object
	private Rigidbody attachedObject;			// Objet saisi, null si aucun objet saisi
	private Vector3 objectSizeInitial;  		// Initial size of the object

	public Material lazerOff, lazerOK, lazerOn; // Lazer colors
	public GameObject lazer;					// Lazer of the wand 
	public GameObject wand;						// wand in the right hand of the user

	void Start () 
	{
		distanceToObj = -1;
		lazer.GetComponent<Renderer> ().material = lazerOff;
	}


	void Update () 
	{
		RaycastHit[] hitInfo;
		RaycastHit firstHit;
		RaycastHit objectFirstPlane;

		Ray ray = new Ray(wand.transform.position, wand.transform.up);
		Debug.DrawRay (ray.origin, ray.direction * RAYCASTLENGTH, Color.blue);
		bool rayCasted = Physics.Raycast (ray, out firstHit, RAYCASTLENGTH);

		if (rayCasted) 
		{
			rayCasted = firstHit.transform.CompareTag ("draggable");
		}

		/**** L'UTILISATEUR CLIQUE ***/
		if (Input.GetMouseButtonDown (0) && attachedObject == null)
		{
			if (rayCasted) 
			{
				objectFirstPlane = firstHit;
				attachedObject = objectFirstPlane.rigidbody;
				attachedObject.isKinematic = true;
				distanceToObj = objectFirstPlane.distance;
				// Debug.Log ("Object attached");
			}
		}
		/*** L'UTILISATEUR RECLIQUE (LACHE L'OBJET) ***/
		else if (Input.GetMouseButtonDown (0) && attachedObject != null)
		{
			Vector3 vect = new Vector3 (0, newSizeY / 4, 0);
			attachedObject.transform.position += vect;
			attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x, objectSizeInitial.y, objectSizeInitial.z) * ratio;
			attachedObject.isKinematic = false;
			attachedObject = null;
		}

		/*** L'UTILISATEUR A L'OBJET DANS LA MAIN ***/
		if (attachedObject != null)
		{
			hitInfo = Physics.RaycastAll (ray, (float)RAYCASTLENGTH);
			if (hitInfo.Length > 0)
			{
				hitInfo = orderHitInfo(hitInfo); // order hitInfo by distance

				objectFirstPlane = hitInfo [0]; // normalement == attachedObject
				objectSizeInitial = attachedObject.transform.lossyScale;

				if (hitInfo.Length >= 2)
				{
					RaycastHit objectSecondPlane;
					objectSecondPlane = hitInfo [1];

					if (objectSecondPlane.transform.name == "Terrain")
					{
						
						changePositionAndSizeOnGround (objectSecondPlane, objectSizeInitial.y);
					}
					else
					{
						if (objectFirstPlane.transform.name == "Terrain")
						{
							objectSizeInitial = attachedObject.GetComponent<Renderer> ().bounds.size;
							changePositionAndSizeOnGround (objectFirstPlane, objectSizeInitial.y);
						}
					}
				}
				lazer.GetComponent<Renderer> ().material = lazerOn;
			}
		} 
		/*** L'UTILISATEUR BOUGE LA SOURIS SANS CLIQUER ***/
		else
		{
			if (rayCasted) 
			{
				lazer.GetComponent<Renderer> ().material = lazerOK;
			} 
			else 
			{
				lazer.GetComponent<Renderer> ().material = lazerOff;
			}
		}
	}

	// If referenced object is the ground
	private void changePositionAndSizeOnGround(RaycastHit referenceObject, float sizeY)
	{
		// Calculate new size
		Vector3 attachedObjectGroundPosition = attachedObject.position;
		attachedObjectGroundPosition.y = 0;
		float GroundDistanceFirstPlane = Vector3.Distance (wand.transform.position - new Vector3(0, wand.transform.position.y, 0), attachedObjectGroundPosition);
		float GroundDistanceSecondPlane = Vector3.Distance (wand.transform.position - new Vector3(0, wand.transform.position.y, 0), referenceObject.point);

		newSizeY = sizeY * (GroundDistanceSecondPlane / GroundDistanceFirstPlane);
		ratio = newSizeY / sizeY;

		// Translater
		Vector3 verticalReplacement = new Vector3 (0, newSizeY / 2, 0);
		attachedObject.transform.position = referenceObject.point + verticalReplacement;
		attachedObject.transform.localScale = new Vector3 (objectSizeInitial.x, objectSizeInitial.y, objectSizeInitial.z) * ratio;
	}

	// Order hitInfo by distance
	private RaycastHit[] orderHitInfo(RaycastHit[] hitInfo)
	{
		for (int K = 0; K < hitInfo.Length; K++)
		{
			for (int I = hitInfo.Length - 2; I >= 0; I--)
			{
				for (int J = 0; J <= I; J++)
				{
					if (hitInfo [J + 1].distance < hitInfo [J].distance)
					{
						RaycastHit t = hitInfo [J + 1];
						hitInfo [J + 1] = hitInfo [J];
						hitInfo [J] = t;
					}
				}
			}
		}
		return hitInfo;
	}
}


