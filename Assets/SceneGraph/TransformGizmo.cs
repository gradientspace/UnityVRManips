using System;
using UnityEngine;
using System.Collections.Generic;



namespace f3
{

	public interface ITransformWrapper : ITransformable
	{
		void BeginTransformation();
		void DoneTransformation();
	}


	public class PassThroughWrapper : ITransformWrapper
	{
		TransformableSceneObject target;

		public PassThroughWrapper(TransformableSceneObject target) {
			this.target = target;
		}
		public void BeginTransformation() {
		}
		public void DoneTransformation() {
		}
		public Frame3 GetLocalFrame(CoordSpace eSpace) {
			return target.GetLocalFrame (eSpace);
		}
		public void SetLocalFrame (Frame3 newFrame, CoordSpace eSpace) {
			target.SetLocalFrame (newFrame, eSpace);
		}
	}



	// This wrapper provides a frame aligned with the scene axes, and transforms translations/rotations
	// to the objects local frame. However it is a bit tricky to get the behavior right...currently
	// we are applying rotations to the scene frame within a Begin/DoneTransformation pair, and then
	// snapping back to world-aligned axes on DoneTransformation()
	public class SceneFrameWrapper : ITransformWrapper
	{
		Scene parentScene;
		TransformableSceneObject target;

		public SceneFrameWrapper(Scene scene, TransformableSceneObject target) {
			this.parentScene = scene;
			this.target = target;
		}

		// [RMS] one ugly bit here is that we are calling BeginCapture before BeginTransformation,
		//   which may call GetLocalFrame before BeginTransformation has a change to initialize,
		//   so we have to initialize curRotation explicitly. 

		Frame3 objectFrame;
		Quaternion curRotation = Quaternion.identity;

		public void BeginTransformation() {
			objectFrame = target.GetLocalFrame (CoordSpace.ObjectCoords);
			curRotation = Quaternion.identity;
		}
		public void DoneTransformation() {
			curRotation = Quaternion.identity;
		}

		public Frame3 GetLocalFrame (CoordSpace eSpace)
		{
			Frame3 targetFrame = target.GetLocalFrame (eSpace);
			if (eSpace == CoordSpace.WorldCoords) {
				return new Frame3 (targetFrame.Origin, parentScene.RootGameObject.transform.rotation);
			} else if (eSpace == CoordSpace.SceneCoords) {
				return new Frame3 (targetFrame.Origin, parentScene.RootGameObject.transform.localRotation);
			} else {
				return new Frame3 (targetFrame.Origin, curRotation * Quaternion.identity);					
			}
		}

		public void SetLocalFrame (Frame3 newFrame, CoordSpace eSpace)
		{
			Debug.Assert (eSpace == CoordSpace.ObjectCoords);

			Frame3 updateFrame = objectFrame;
			curRotation = newFrame.Rotation;
			updateFrame.Rotation = curRotation * objectFrame.Rotation;
			updateFrame.Origin = newFrame.Origin;
			target.SetLocalFrame (updateFrame, eSpace);
		}

	}




	public class TransformGizmo : GameObjectSet, SceneUIElement
	{

		GameObject gizmo;
		GameObject x, y, z;
		GameObject rotate_x, rotate_y, rotate_z;
		GameObject translate_xy, translate_xz, translate_yz;

		Scene parentScene;
		TransformableSceneObject target;
		ITransformWrapper targetWrapper;

		Dictionary<GameObject, Widget> Widgets;
		Widget activeWidget;

		Material xMaterial, yMaterial, zMaterial;


		bool EnableDebugLogging;

		public TransformGizmo ()
		{
			Widgets = new Dictionary<GameObject, Widget> ();
			EnableDebugLogging = false;
		}


		public GameObject RootGameObject {
			get { return gizmo; }
		}

		public void Disconnect() {
			this.parentScene.SelectionChangedEvent -= OnSceneSelectionChanged;
		}


		public void Create(Scene parentScene, TransformableSceneObject target) {
			this.parentScene = parentScene;
			this.target = target;

			this.parentScene.SelectionChangedEvent += OnSceneSelectionChanged;

			gizmo = new GameObject("TransformGizmo");

			float fAlpha = 0.5f;
			xMaterial = MaterialUtil.CreateTransparentMaterial (Color.red, fAlpha);
			yMaterial = MaterialUtil.CreateTransparentMaterial (Color.green, fAlpha);
			zMaterial = MaterialUtil.CreateTransparentMaterial (Color.blue, fAlpha);

			x = AppendMeshGO ("x_translate", 
				(Mesh)Resources.Load ("meshes/transform_gizmo_x", typeof(Mesh)),
				xMaterial, gizmo);
			Widgets [x] = new AxisTranslationWidget (0, parentScene);
			y = AppendMeshGO ("y_translate", 
				(Mesh)Resources.Load ("meshes/transform_gizmo_y", typeof(Mesh)),
				yMaterial, gizmo);
			Widgets [y] = new AxisTranslationWidget (1, parentScene);
			z = AppendMeshGO ("z_translate", 
				(Mesh)Resources.Load ("meshes/transform_gizmo_z", typeof(Mesh)),
				zMaterial, gizmo);	
			Widgets [z] = new AxisTranslationWidget (2, parentScene);

			rotate_x = AppendMeshGO ("x_rotate",
				(Mesh)Resources.Load ("meshes/axisrotate_x", typeof(Mesh)),
				xMaterial, gizmo);
			Widgets [rotate_x] = new AxisRotationWidget (0, parentScene);
			rotate_y = AppendMeshGO ("y_rotate",
				(Mesh)Resources.Load ("meshes/axisrotate_y", typeof(Mesh)),
				yMaterial, gizmo);
			Widgets [rotate_y] = new AxisRotationWidget (1, parentScene);
			rotate_z = AppendMeshGO ("z_rotate",
				(Mesh)Resources.Load ("meshes/axisrotate_z", typeof(Mesh)),
				zMaterial, gizmo);			
			Widgets [rotate_z] = new AxisRotationWidget (2, parentScene);


			// plane translation widgets
			translate_xy = AppendMeshGO ("xy_translate",
				(Mesh)Resources.Load ("meshes/plane_translate_xy", typeof(Mesh)),
				zMaterial, gizmo);
			Widgets [translate_xy] = new PlaneTranslationWidget (2, parentScene);
			translate_xz = AppendMeshGO ("xz_translate",
				(Mesh)Resources.Load ("meshes/plane_translate_xz", typeof(Mesh)),
				yMaterial, gizmo);
			Widgets [translate_xz] = new PlaneTranslationWidget (1, parentScene);
			translate_yz = AppendMeshGO ("yz_translate",
				(Mesh)Resources.Load ("meshes/plane_translate_yz", typeof(Mesh)),
				xMaterial, gizmo);
			Widgets [translate_yz] = new PlaneTranslationWidget (0, parentScene);


			// disable shadows on widget components
			foreach ( var go in GameObjects ) 
				go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			targetWrapper = new PassThroughWrapper (target);
			//targetWrapper = new SceneFrameWrapper(parentScene, target);

			// update gizmo transform to match target frame
			Frame3 widgetFrame = targetWrapper.GetLocalFrame(CoordSpace.ObjectCoords);
			gizmo.transform.localPosition = widgetFrame.Origin;
			gizmo.transform.localRotation = widgetFrame.Rotation;

			int nWidgetLayer = LayerMask.NameToLayer (SceneGraphConfig.WidgetOverlayLayerName);
			foreach ( var go in GameObjects )
				go.layer = nWidgetLayer;
		}


		void OnSceneSelectionChanged(object sender, EventArgs e)
		{
			// TODO this should be done at a higher level...we should not be removing ourself!!
			if (parentScene.IsSelected (this.target) == false) {
				parentScene.RemoveUIElement (this);
			}
		}



		public bool FindRayIntersection (Ray ray, out UIRayHit hit)
		{
			hit = null;
			GameObjectRayHit hitg = null;
			if (FindGORayIntersection (ray, out hitg)) {
				if (hitg.hitGO != null) {
					hit = new UIRayHit (hitg, this);
					return true;
				}
			}
			return false;
		}




		public bool BeginCapture(Ray ray, UIRayHit hit)
		{
			activeWidget = null;

			// if the hit gameobject has a widget attached to it, begin capture & transformation
			// TODO maybe wrapper class should have Begin/Update/End capture functions, then we do not need BeginTransformation/EndTransformation ?
			if (Widgets.ContainsKey (hit.hitGO)) {
				Widget w = Widgets [hit.hitGO];
				if (w.BeginCapture (targetWrapper, ray, hit)) {
					targetWrapper.BeginTransformation ();
					activeWidget = w;
					return true;
				}
			}
			return false;
		}

		public bool UpdateCapture(Ray worldRay)
		{
			// update capture if we have an active widget
			if (activeWidget != null) {
				if (activeWidget.UpdateCapture (targetWrapper, worldRay)) {
					// keep widget synced with object frame of target
					Frame3 widgetFrame = targetWrapper.GetLocalFrame(CoordSpace.ObjectCoords);
					gizmo.transform.localPosition = widgetFrame.Origin;
					gizmo.transform.localRotation = widgetFrame.Rotation;
				}
				return true;
			}
			return false;
		}

		public bool EndCapture(Ray ray)
		{
			if (activeWidget != null) {
				// update widget frame in case we want to do something like stay scene-aligned...
				targetWrapper.DoneTransformation ();
				Frame3 widgetFrame = targetWrapper.GetLocalFrame (CoordSpace.ObjectCoords);
				gizmo.transform.localPosition = widgetFrame.Origin;
				gizmo.transform.localRotation = widgetFrame.Rotation;

				activeWidget = null;
			}
			return true;
		}


	}
}

