#pragma strict


class SubMeshTriangles{
	var triangles : Array;
	var trianglesBuiltIn : int[];
	function SubMeshTriangles(){
		triangles = new Array();
	}
}

function Start () {
	var combinedMesh : MeshFilter;
	var combinedMeshRenderer : Renderer;

	var meshes : MeshFilter[];
	var renderers : Renderer[];
	var materials : Material[];
	var subMeshes : SubMeshTriangles[];
	
	meshes = gameObject.GetComponentsInChildren.<MeshFilter>() as MeshFilter[];
	renderers = gameObject.GetComponentsInChildren.<Renderer>() as Renderer[];	
	
	var materialArray = new Array();
	for(var b = 0; b < renderers.Length; b++){
		var newMaterial : boolean = true;
		
		for(var y = 0; y < materialArray.length; y++){
			var thisMaterial : Material = materialArray[y] as Material;
			if(renderers[b].material.name == thisMaterial.name){
				newMaterial = false;
			}
		}
		
		if(newMaterial){
			materialArray.Push(renderers[b].material);
		}
	}
	materials = materialArray.ToBuiltin(Material) as Material[];
	subMeshes = new SubMeshTriangles[materials.Length];	
	for(var u = 0 ; u < subMeshes.Length; u++){
		subMeshes[u] = new SubMeshTriangles();
	}
			
	for(var e = 0; e < materials.Length; e++){
		
	}
	
	combinedMesh = gameObject.AddComponent.<MeshFilter>();
	combinedMeshRenderer = gameObject.AddComponent.<MeshRenderer>();
	
	var verticesLength : int;
	var uvLength : int;
	var trianglesLength : int;
	var normalsLength : int;
	
	for(var i = 0; i < meshes.Length; i++){
		verticesLength += meshes[i].mesh.vertices.Length;
		uvLength += meshes[i].mesh.uv.Length;
		trianglesLength += meshes[i].mesh.triangles.Length;
	}
	
	combinedMesh.mesh.vertices = new Vector3[verticesLength];
	combinedMesh.mesh.uv = new Vector2[uvLength];
	combinedMesh.mesh.triangles = new int[trianglesLength];
	combinedMesh.mesh.normals = new Vector3[normalsLength];
	
	var currentVertex : int;
	var currentTriangle : int;
	var lastVertex : int;
	
	var vertices : Vector3[] = new Vector3[verticesLength];
	var uvs : Vector2[] = new Vector2[verticesLength];
	var triangles : int[] = new int[trianglesLength];
	var normals : Vector3[] = new Vector3[verticesLength];
	
	for(i = 0; i < meshes.Length; i++){
		var thisMeshMaterial : int;
		for(var h = 0; h < materials.Length; h++){
			if(meshes[i].GetComponent.<Renderer>().material.name == materials[h].name){
				thisMeshMaterial = h;
				break;
			}
		}
		
		for(var n = 0; n < meshes[i].mesh.vertices.Length; n++){
			vertices[currentVertex] = meshes[i].transform.localToWorldMatrix.MultiplyPoint3x4(meshes[i].mesh.vertices[n]);
			vertices[currentVertex] = transform.worldToLocalMatrix.MultiplyPoint3x4(vertices[currentVertex]);
			
			uvs[currentVertex] = meshes[i].mesh.uv[n];
			
			normals[currentVertex] = meshes[i].mesh.normals[n];

			currentVertex++;
		}
		
		var invertNormals : boolean;
		if(meshes[i].transform.localScale.x < 0 || meshes[i].transform.localScale.y < 0 || meshes[i].transform.localScale.z < 0){
			invertNormals = true;
		}
		
		
		if(invertNormals){
			for(n = meshes[i].mesh.triangles.Length - 1; n >= 0; n--){
				triangles[currentTriangle] = meshes[i].mesh.triangles[n] + lastVertex;
				subMeshes[thisMeshMaterial].triangles.Push(triangles[currentTriangle]);
				currentTriangle++;
			}			
		}
		else{
			for(n = 0; n < meshes[i].mesh.triangles.Length; n++){
				triangles[currentTriangle] = meshes[i].mesh.triangles[n] + lastVertex;
				subMeshes[thisMeshMaterial].triangles.Push(triangles[currentTriangle]);
				currentTriangle++;
			}
		}

		lastVertex = currentVertex;
	}
	
	combinedMesh.mesh.vertices = vertices;
	combinedMesh.mesh.uv = uvs;
	combinedMesh.mesh.triangles = triangles;
	combinedMesh.mesh.normals = normals;
	
	combinedMesh.mesh.subMeshCount = subMeshes.Length;
	for(var a = 0; a < subMeshes.Length; a++){
		subMeshes[a].trianglesBuiltIn = subMeshes[a].triangles.ToBuiltin(int) as int[];
		combinedMesh.mesh.SetTriangles(subMeshes[a].trianglesBuiltIn, a);
	}
	

	combinedMesh.mesh.RecalculateNormals();
	combinedMesh.mesh.RecalculateBounds();
	
	combinedMeshRenderer.materials = materials;
	
	combinedMesh.gameObject.isStatic = true;
	
	for(var j = 0; j < meshes.Length; j++){
		Destroy(meshes[j].gameObject);
	}
}
