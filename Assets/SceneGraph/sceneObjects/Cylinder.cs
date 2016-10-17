using UnityEngine;
using System;

namespace f3
{
	public class Cylinder : GameObjectSet, TransformableSceneObject
	{
		float radius;
		float height;

		Scene parentScene;

		GameObject cylinder;
		GameObject topCap, bottomCap, body;

		public Cylinder ()
		{
			radius = 1.0f;
			height = 5.0f;
		}

		public GameObject RootGameObject {
			get { return cylinder; }
		}


		public void Create( Material defaultMaterial) {

			cylinder = new GameObject("f3Cylinder");

			topCap = AppendMeshGO ("topCap", 
				MeshGenerators.CreateDisc (radius, 2, 16), 
				defaultMaterial, cylinder);
			topCap.transform.position += 0.5f * height * Vector3.up;

			bottomCap = AppendMeshGO ("bottomCap", 
				MeshGenerators.CreateDisc (radius, 2, 16), 
				defaultMaterial, cylinder);
			bottomCap.transform.RotateAround (Vector3.zero, Vector3.right, 180.0f);
			bottomCap.transform.position -= 0.5f * height * Vector3.up;

			body = AppendMeshGO ("body", 
				MeshGenerators.CreateCylider (radius, height, 16),
				defaultMaterial, cylinder);
			body.transform.position -= 0.5f * height * Vector3.up;

			cylinder.transform.position += 0.5f * height * Vector3.up;
		}



		//
		// SceneObject impl
		//

		public void SetScene(Scene s) {
			parentScene = s;
		}
		public Scene GetScene() {
			return parentScene;
		}

		public void SetMaterial(Material m) {
			SetGOMaterials (m);
		}

		public bool FindRayIntersection (Ray ray, out SORayHit hit)
		{
			hit = null;
			GameObjectRayHit hitg = null;
			if (FindGORayIntersection (ray, out hitg)) {
				if (hitg.hitGO != null) {
					hit = new SORayHit (hitg, this);
					return true;
				}
			}
			return false;
		}


		//
		// TransformableSceneObject impl
		//
		public Frame3 GetLocalFrame(CoordSpace eSpace)
		{
			if (eSpace == CoordSpace.SceneCoords)
				return parentScene.WorldToSceneF (new Frame3 (cylinder.transform, false));
			else
				return MathUtil.GetGameObjectFrame (cylinder, eSpace);
		}
		public void SetLocalFrame(Frame3 newFrame, CoordSpace eSpace)
		{
			// note: SceneCoords not supported!
			MathUtil.SetGameObjectFrame (cylinder, newFrame, eSpace);
		}

	}
}

