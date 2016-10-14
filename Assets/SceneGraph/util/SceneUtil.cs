using System;
using UnityEngine;

namespace f3
{

	public class RayHit 
	{
		public Vector3 hitPos;
		public float fHitDist;

		public RayHit() {
			fHitDist = Mathf.Infinity;
		}

		public bool IsValid {
			get { return fHitDist < Mathf.Infinity; }
		}
	}

	public class GameObjectRayHit : RayHit
	{
		public GameObject hitGO;
	}



	public class SORayHit : GameObjectRayHit 
	{
		public SceneObject hitSO;

		public SORayHit() {
		}
		public SORayHit(GameObjectRayHit init, SceneObject so) {
			hitPos = init.hitPos;
			fHitDist = init.fHitDist;
			hitGO = init.hitGO;
			hitSO = so;
		}
	}

	public class UIRayHit : GameObjectRayHit 
	{
		public SceneUIElement hitUI;

		public UIRayHit() {
		}
		public UIRayHit(GameObjectRayHit init, SceneUIElement ui) {
			hitPos = init.hitPos;
			fHitDist = init.fHitDist;
			hitGO = init.hitGO;
			hitUI = ui;
		}
	}



	public enum HitType {
		SceneObjectHit,
		SceneUIElementHit
	}

	public class AnyRayHit : GameObjectRayHit 
	{
		public HitType eType;

		public SceneObject hitSO;
		public SceneUIElement hitUI;

		public AnyRayHit() {
		}
		public AnyRayHit(GameObjectRayHit init, SceneObject so) {
			hitPos = init.hitPos;
			fHitDist = init.fHitDist;
			hitGO = init.hitGO;
			eType = HitType.SceneObjectHit;
			hitSO = so;
		}
		public AnyRayHit(SORayHit init) {
			hitPos = init.hitPos;
			fHitDist = init.fHitDist;
			hitGO = init.hitGO;
			eType = HitType.SceneObjectHit;
			hitSO = init.hitSO;
		}
		public AnyRayHit(GameObjectRayHit init, SceneUIElement ui) {
			hitPos = init.hitPos;
			fHitDist = init.fHitDist;
			hitGO = init.hitGO;
			eType = HitType.SceneUIElementHit;
			hitUI = ui;
		}
		public AnyRayHit(UIRayHit init) {
			hitPos = init.hitPos;
			fHitDist = init.fHitDist;
			hitGO = init.hitGO;
			eType = HitType.SceneUIElementHit;
			hitUI = init.hitUI;
		}
	}


}

