using System;
using System.Collections.Generic;
using UnityEngine;

namespace f3
{

	public delegate void SceneSelectionChangedHandler(object sender, EventArgs e);


	public class Scene : ISpaceConversion
	{
		Material defaultMaterial;
		Material selectedMaterial;

		GameObject sceneo;

		List<SceneObject> vObjects;
		List<SceneObject> vSelected;
		List<SceneUIElement> vUIElements;
		List<GameObject> vBoundsObjects;

		// camera stuff
		public float[] turntable_angles = { 0.0f, 0.0f };

		public Scene()
		{
			vObjects = new List<SceneObject> ();
			vSelected = new List<SceneObject> ();
			vUIElements = new List<SceneUIElement> ();
			vBoundsObjects = new List<GameObject> ();

			sceneo = new GameObject ("Scene");

			// initialize materials
			defaultMaterial = MaterialUtil.CreateStandardMaterial( new Color(0.75f, 0.75f, 0.5f) );
			selectedMaterial = MaterialUtil.CreateStandardMaterial( new Color(1.0f, 0.6f, 0.05f) );
		}


		public GameObject RootGameObject {
			get { return sceneo; }
		}



		public event SceneSelectionChangedHandler SelectionChangedEvent;
		protected virtual void OnSelectionChanged(EventArgs e) {
			if (SelectionChangedEvent != null)
				SelectionChangedEvent(this, e);
		}




		public void AddWorldBoundsObject(GameObject obj)
		{
			vBoundsObjects.Add (obj);
			obj.transform.SetParent (RootGameObject.transform);
		}



		public List<SceneObject> SceneObjects { 
			get { return vObjects; }
		}

		public Cylinder AddCylinder() {
			Cylinder c = new Cylinder();
			c.Create (defaultMaterial);
			AddSceneObject (c);
			return c;
		}
		public Box AddBox() {
			Box b = new Box();
			b.Create (defaultMaterial);
			AddSceneObject (b);
			return b;
		}
		public void AddSceneObject(SceneObject o)
		{
			vObjects.Add (o);
			o.SetScene(this);
			o.RootGameObject.transform.SetParent (sceneo.transform, false);
		}


		public List<SceneObject> Selected {
			get { return vSelected; }
		}

		public bool IsSelected(SceneObject s) {
			var found = vSelected.Find (x => x == s);
			return (found != null);
		}

		public bool Select(SceneObject s) {
			if (!IsSelected (s)) {
				vSelected.Add (s);
				OnSelectionChanged (EventArgs.Empty);

				s.SetMaterial (selectedMaterial);

				// TODO this should be in a controller
				if ((s as TransformableSceneObject) != null) {
					TransformGizmo gizmo = new TransformGizmo ();
					gizmo.Create ( this, s as TransformableSceneObject );
					AddUIElement (gizmo);
				}

				return true;
			}
			return false;
		}

		public void Deselect(SceneObject s) {
			s.SetMaterial (defaultMaterial);
			vSelected.Remove (s);
			OnSelectionChanged (EventArgs.Empty);
		}

		public void ClearSelection() { 
			foreach (var v in vSelected)
				v.SetMaterial (defaultMaterial);
			vSelected = new List<SceneObject> ();
			OnSelectionChanged (EventArgs.Empty);
		}



		public List<SceneUIElement> UIElements { 
			get { return vUIElements; }
		}

		public void AddUIElement(SceneUIElement e) {
			vUIElements.Add (e);
			if (e.RootGameObject != null) {
				// assume gizmo transform is set to a local transform, so we want to apply current scene transform
				e.RootGameObject.transform.SetParent (sceneo.transform, false);
			}
		}

		public void RemoveUIElement(SceneUIElement e) {
			vUIElements.Remove (e);
			if ( e.RootGameObject != null ) {
				e.RootGameObject.transform.parent = null;
				UnityEngine.Object.Destroy (e.RootGameObject);
			}
		}




		public bool FindSORayIntersection(Ray ray, out SORayHit hit) {
			hit = null;

			foreach (SceneObject so in vObjects) {
				SORayHit objHit;
				if (so.FindRayIntersection (ray, out objHit)) {
					if (hit == null || objHit.fHitDist < hit.fHitDist)
						hit = objHit;
				}
			}
			return (hit != null);
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
			SORayHit bestSOHit = null;

			foreach (var ui in vUIElements) {
				UIRayHit uiHit;
				if (ui.FindRayIntersection (ray, out uiHit)) {
					if (bestUIHit == null || uiHit.fHitDist < bestUIHit.fHitDist)
						bestUIHit = uiHit;
				}
			}
			foreach (var so in vObjects) {
				SORayHit objHit;
				if (so.FindRayIntersection (ray, out objHit)) {
					if (bestSOHit == null || objHit.fHitDist < bestSOHit.fHitDist)
						bestSOHit = objHit;
				}
			}
			if (bestUIHit != null) {
				if (bestSOHit == null || bestSOHit.fHitDist > bestUIHit.fHitDist)
					hit = new AnyRayHit (bestUIHit);
				else
					hit = new AnyRayHit (bestSOHit);
			} else if (bestSOHit != null)
				hit = new AnyRayHit (bestSOHit);

			return (hit != null);
		}




		public bool FindWorldBoundsHit(Ray ray, out GameObjectRayHit hit) {
			hit = null;
			foreach (var go in this.vBoundsObjects) {
				GameObjectRayHit myHit = null;
				if (MathUtil.FindGORayIntersection (ray, go, out myHit)) {
					if (hit == null || myHit.fHitDist < hit.fHitDist)
						hit = myHit;
				}
			}
			return (hit != null);		
		}



		// TODO should move somewhere else??

		public void WorldTumble(float dx, float dy) {
			Camera curCam = Camera.main;
			Vector3 up = curCam.gameObject.transform.up;
			Vector3 right = curCam.gameObject.transform.right;

			Vector3 curOrigin = sceneo.transform.position;

			sceneo.transform.RotateAround (curOrigin, up, dx);
			sceneo.transform.RotateAround (curOrigin, right, dy);
		}


		public void WorldOrbit(float dx, float dy) {
			Vector3 curOrigin = sceneo.transform.position;

			turntable_angles[0] -= dx;
			turntable_angles [1] += dy;
			turntable_angles [1] = Mathf.Clamp (turntable_angles [1], -89.9f, 89.9f);

			sceneo.transform.rotation = Quaternion.identity;
			sceneo.transform.RotateAround (curOrigin, Vector3.up, turntable_angles [0]);
			sceneo.transform.RotateAround (curOrigin, Vector3.right, turntable_angles [1]);
		}


		public void WorldPan(float dx, float dy) {
			Camera curCam = Camera.main;
			Vector3 right = curCam.gameObject.transform.right;
			Vector3 up = curCam.gameObject.transform.up;

			Vector3 newPos = sceneo.transform.position + dx * right + dy * up;
			sceneo.transform.position = newPos;
		}

		public void WorldZoom(float dz) {
			Camera curCam = Camera.main;
			Vector3 fw = curCam.gameObject.transform.forward;

			Vector3 newPos = sceneo.transform.position + dz * fw;
			sceneo.transform.position = newPos;
		}



		//
		// ISpaceConversion implementation
		//

		public Vector3 WorldToSceneP(Vector3 worldPoint) {
			return sceneo.transform.InverseTransformPoint(worldPoint);
		}
		public Vector3 SceneToWorldP(Vector3 scenePoint) {
			return sceneo.transform.TransformPoint (scenePoint);
		}
		public Vector3 WorldToSceneV(Vector3 worldVector)
		{
			return sceneo.transform.InverseTransformVector(worldVector);
		}
		public Vector3 SceneToWorldV(Vector3 sceneVector)
		{
			return sceneo.transform.TransformVector(sceneVector);
		}

		public Frame3 WorldToSceneF(Frame3 worldFrame) 
		{
			return new Frame3 (
				worldFrame.Origin - sceneo.transform.localPosition,
				Quaternion.Inverse (sceneo.transform.localRotation) * worldFrame.Rotation);
		}
		public Frame3 SceneToWorldF(Frame3 sceneFrame)
		{
			return new Frame3 (
				sceneo.transform.TransformPoint(sceneFrame.Origin),
				sceneo.transform.rotation * sceneFrame.Rotation );			
		}



	}
}

