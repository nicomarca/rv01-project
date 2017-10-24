#pragma strict

var house : int;
var houseOffset : float = 10.0;
var targetHeight : float = 2.0;
var cameraSpeed : float = 10.0;
var angle : float;
var pitch : float;
var currentZoom : float = 10.0;
var targetZoom : float = 10.0;

var minHouse : int;
var maxHouse : int;
var maxHeight : float;
var minHeight : float;
var maxZoom : float;
var minZoom : float;
var maxPitch : float;
var minPitch : float;

var collisionRayDist : float = 5.0;

var colliders : Collider[];
var disableCollidersDuration : float = 1.0;
var disableCollidersUntil : float;

function Start () {
	currentZoom = targetZoom;
}

function Update () {
	house = Mathf.Clamp(house, minHouse, maxHouse);
	targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
	targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
	pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

	transform.position = Vector3.Lerp(transform.position, Vector3(0, targetHeight, house * houseOffset), Time.deltaTime * cameraSpeed);
	transform.rotation = Quaternion.Euler(0, angle, 0);
	transform.RotateAround(transform.position, transform.right, pitch);
	
	
	var useTargetZoom : float = targetZoom;
	var hit : RaycastHit;
	if(Physics.Raycast(transform.position, transform.GetChild(0).transform.position - transform.position, hit, collisionRayDist)){
		//transform.GetChild(0).position = hit.point;
		useTargetZoom = Vector3.Distance(transform.position, hit.point);
		if(useTargetZoom > targetZoom) useTargetZoom = targetZoom;
		else{
			currentZoom = Mathf.Lerp(currentZoom, useTargetZoom, .3);
		}
	}
	
	currentZoom = Mathf.Lerp(currentZoom, useTargetZoom, cameraSpeed * Time.deltaTime);
	transform.GetChild(0).transform.localPosition = Vector3(0,0,currentZoom);
	
	if(Input.GetMouseButton(0)){
		angle += Input.GetAxis("Mouse X") * 2.0;
		pitch += Input.GetAxis("Mouse Y") * 2.0;
	}

	if(Input.GetMouseButton(1)){
		targetHeight += Input.GetAxis("Mouse Y") *.1;
	}
	
	targetZoom -= Input.GetAxis("Mouse ScrollWheel") * 10.0;
	
	if(transform.GetChild(0).transform.position.x > 0){
		if(Input.GetKeyDown(KeyCode.LeftArrow)) house --;
		if(Input.GetKeyDown(KeyCode.RightArrow)) house ++;
	}
	else{
		if(Input.GetKeyDown(KeyCode.LeftArrow)) house ++;
		if(Input.GetKeyDown(KeyCode.RightArrow)) house --;		
	}
	
	if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)){
		disableCollidersUntil = Time.time + disableCollidersDuration;
	}
	
	for(var n = 0; n < colliders.Length; n++){
		if(Time.time < disableCollidersUntil){
			colliders[n].enabled = false;
		}
		else{
			if(n == house) colliders[n].enabled = false;
			else colliders[n].enabled = true;
		}
	}
	
	if(Input.GetKeyDown(KeyCode.Escape)){
		Application.Quit();
	}
	
}