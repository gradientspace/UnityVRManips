using UnityEngine;
using System;

namespace f3
{
	public class Box : GameObjectSet, TransformableSceneObject
	{
		float width;  // x
		float height; // y
		float depth;  // z

		Scene parentScene;

		GameObject box;
		GameObject boxMesh;

		public Box ()
		{
			width = height = depth = 3.0f;
		}

		public GameObject RootGameObject {
			get { return box; }
		}


		public void Create( Material defaultMaterial) {
			box = new GameObject("f3Box");
			boxMesh = AppendUnityPrimitiveGO ("boxMesh", PrimitiveType.Cube, defaultMaterial, box);
			boxMesh.transform.localScale = new Vector3 (width, height, depth);
			box.transform.position += 0.5f * height * Vector3.up;
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
				return parentScene.WorldToSceneF (new Frame3 (RootGameObject.transform, false));
			else
				return MathUtil.GetGameObjectFrame (RootGameObject, eSpace);
		}
		public void SetLocalFrame(Frame3 newFrame, CoordSpace eSpace)
		{
			// note: SceneCoords not supported!
			MathUtil.SetGameObjectFrame (RootGameObject, newFrame, eSpace);
		}

	}
}

