using System;
using System.Collections.Generic;
using UnityEngine;

namespace f3
{
	public class Cockpit
	{
		Material defaultMaterial;
		GameObject gameobject;

		List<SceneUIElement> vUIElements;

		public Cockpit()
		{
			vUIElements = new List<SceneUIElement> ();
			gameobject = new GameObject("cockpit");

			defaultMaterial = MaterialUtil.CreateStandardMaterial( new Color(0.25f, 0.75f, 0.1f) );
		}


		public GameObject RootGameObject {
			get { return gameobject; }
		}


		// these should be called by parent Unity functions
		public void Start()
		{
			RootGameObject.transform.position = Camera.main.transform.position;
			RootGameObject.transform.rotation = Camera.main.transform.rotation;


			HUDButton button = new HUDButton () { Radius = 0.1f };
			button.Create (defaultMaterial);
			button.RootGameObject.transform.Rotate (Vector3.up, 10.0f, Space.World);
			button.RootGameObject.transform.position += 0.6f * Vector3.forward - 0.5f * Vector3.right;
			AddUIElement (button);
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



		public void AddUIElement(SceneUIElement e) {
			vUIElements.Add (e);
			if (e.RootGameObject != null) {
				// assume gizmo transform is set to a local transform, so we want to apply current scene transform
				e.RootGameObject.transform.SetParent (RootGameObject.transform, false);
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

