using System;
using System.Collections.Generic;
using UnityEngine;

namespace f3
{
	public class Cockpit
	{
		SceneController parent;
		GameObject gameobject;

		List<SceneUIElement> vUIElements;

		public Cockpit(SceneController parent)
		{
			this.parent = parent;
			vUIElements = new List<SceneUIElement> ();
		}

		public SceneController Parent {
			get { return parent; }
		}
		public GameObject RootGameObject {
			get { return gameobject; }
		}


		// cockpit frame is oriented such that
		//    +X is right
		//    +Y is up
		//    +Z is into scene
		// note that for most unity mesh objects are created on the XZ plane, and so you 
		// need to rotate world_y to point to -cockpit_z, ie Quaternion.FromToRotation (Vector3.up, -cockpitF.Z)
		public virtual Frame3 GetLocalFrame(CoordSpace eSpace) 
		{
			return MathUtil.GetGameObjectFrame (gameobject, eSpace);
		}
		public virtual void SetLocalFrame(Frame3 newFrame, CoordSpace eSpace)
		{
			MathUtil.SetGameObjectFrame (gameobject, newFrame, eSpace);
		}


		// these should be called by parent Unity functions
		public void Start()
		{
			// create invisible plane for cockpit
			gameobject = GameObject.CreatePrimitive (PrimitiveType.Plane);
			gameobject.name = "cockpit";
			MeshRenderer ren = gameobject.GetComponent<MeshRenderer> ();
			ren.enabled = false;
			gameobject.GetComponent<MeshCollider> ().enabled = false;

			RootGameObject.transform.position = Camera.main.transform.position;
			RootGameObject.transform.rotation = Camera.main.transform.rotation;

			setup_cockpit setup = new setup_cockpit ();
			setup.Initialize (this);
		}
		public void Update()
		{
			//Vector3 camPos = Camera.main.transform.position;
			//Vector3 forward = Camera.main.transform.forward;
			//RootGameObject.transform.position = camPos;	
			//RootGameObject.transform.LookAt (camPos + 10 * forward);
			//RootGameObject.transform.RotateAround (RootGameObject.transform.position, RootGameObject.transform.right, 90);

			// cockpit tracks camera
			RootGameObject.transform.position = Camera.main.transform.position;
			//RootGameObject.transform.rotation = Camera.main.transform.rotation;
		}



		public void AddUIElement(SceneUIElement e, bool bIsInLocalFrame = true) {
			vUIElements.Add (e);
			if (e.RootGameObject != null) {
				// assume element transform is set to a local transform, so we want to apply current scene transform?
				e.RootGameObject.transform.SetParent (RootGameObject.transform, (bIsInLocalFrame == false) );
			}
		}

		public void RemoveUIElement(SceneUIElement e) {
			vUIElements.Remove (e);
			if ( e.RootGameObject != null ) {
				e.RootGameObject.transform.parent = null;
				UnityEngine.Object.Destroy (e.RootGameObject);
			}
		}


		public bool FindUIRayIntersection(Ray ray, out UIRayHit hit) {
			hit = null;

			foreach (var ui in vUIElements) {
				UIRayHit uiHit;
				if (ui.FindRayIntersection (ray, out uiHit)) {
					if (hit == null || uiHit.fHitDist < hit.fHitDist)
						hit = uiHit;
				}
			}
			return (hit != null);
		}



		public bool FindAnyRayIntersection(Ray ray, out AnyRayHit hit) {
			hit = null;
			UIRayHit bestUIHit = null;
			if (FindUIRayIntersection (ray, out bestUIHit)) {
				hit = new AnyRayHit (bestUIHit);
			}
			return (hit != null);
		}



	}
}

