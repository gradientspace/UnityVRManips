using System;
using System.Collections.Generic;
using UnityEngine;

namespace f3
{
	public class GameObjectSet
	{
		public List<GameObject> vObjects;

		public GameObjectSet ()
		{
			vObjects = new List<GameObject> ();
		}

		public List<GameObject> GameObjects { 
			get { return vObjects; } 
		}

		public bool HasGO(GameObject go) {
			GameObject found = vObjects.Find (x => x == go);
			return found != null;
		}

		public virtual GameObject AppendMeshGO(string name, Mesh mesh, Material setMaterial, GameObject parent) {
			var gameObj = new GameObject (name);
			var gameObjMesh = (MeshFilter)gameObj.AddComponent(typeof(MeshFilter));
			gameObjMesh.mesh = mesh;
			gameObj.AddComponent (typeof(MeshCollider));
			gameObj.GetComponent<MeshCollider> ().enabled = false;
			(gameObj.AddComponent (typeof(MeshRenderer)) as MeshRenderer).material = setMaterial;

			vObjects.Add (gameObj);

			gameObj.transform.parent = parent.transform;

			return gameObj;
		}


		public virtual GameObject AppendUnityPrimitiveGO(string name, PrimitiveType eType, Material setMaterial, GameObject parent) {
			var gameObj = GameObject.CreatePrimitive (eType);
			gameObj.AddComponent (typeof(MeshCollider));
			gameObj.GetComponent<MeshCollider> ().enabled = false;
			gameObj.GetComponent<MeshRenderer> ().material = setMaterial;

			vObjects.Add (gameObj);

			gameObj.transform.parent = parent.transform;

			return gameObj;
		}


		public virtual void SetGOMaterials(Material m) {
			foreach (var go in vObjects) 
				go.GetComponent<MeshRenderer> ().material = m;
		}


		public virtual bool FindGORayIntersection (Ray ray, out GameObjectRayHit hit)
		{
			hit = new GameObjectRayHit();
			RaycastHit hitInfo;

			foreach (var go in vObjects) {
				go.GetComponent<MeshCollider> ().enabled = true;
				if (go.GetComponent<MeshCollider> ().Raycast (ray, out hitInfo, Mathf.Infinity)) {
					if (hitInfo.distance < hit.fHitDist) {
						hit.fHitDist = hitInfo.distance;
						hit.hitPos = hitInfo.point;
						hit.hitGO = go;
					}
				}
				go.GetComponent<MeshCollider> ().enabled = false;
			}

			return (hit.hitGO != null);
		}


		public virtual bool IsGOHit(Ray ray, GameObject go) {
			bool bHit = false;
			RaycastHit hitInfo;
			go.GetComponent<MeshCollider> ().enabled = true;
			if (go.GetComponent<MeshCollider> ().Raycast (ray, out hitInfo, Mathf.Infinity))
				bHit = true;
			go.GetComponent<MeshCollider> ().enabled = false;
			return bHit;
		}

	}
}

