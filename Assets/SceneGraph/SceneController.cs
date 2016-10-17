using UnityEngine;
using System.Collections;

namespace f3 {

	public class SceneController : MonoBehaviour {

		Scene scene;							// set of objects in our universe
		Cockpit cockpit;						// HUD that kind of sticks to view
		MouseCursorController mouseCursor;		// handles mouse cursor interaction in VR

		bool bInCameraControl;

		SceneUIElement pCapturing;


		// [RMS] why do it this way? can't we just create in Start?
		public Scene Scene {
			get { return GetScene (); }
		}
		public Scene GetScene() {
			if (scene == null)
				scene = new Scene ();
			return scene;
		}
		public Cockpit GetCockpit() {
			if (cockpit == null)
				cockpit = new Cockpit(this);
			return cockpit;
		}
		public MouseCursorController GetMouseCursor() {
			if (mouseCursor == null) 
				mouseCursor = new MouseCursorController (Camera.main, this);
			return mouseCursor;
		}


		// Use this for initialization
		void Start () {
			GetScene ();
			GetCockpit ().Start ();
			GetMouseCursor ().Start ();

			pCapturing = null;
			bInCameraControl = false;

			// [RMS] this locks cursor to game unless user presses escape or exits
			Cursor.lockState = CursorLockMode.Locked;

			// intialize camera stuff
			CameraController.Singleton.Initialize();
		}

		// Update is called once per frame
		public void Update () {

			GetCockpit ().Update ();
			GetMouseCursor ().Update ();

			if (Input.GetKeyDown ("escape")) {
				Cursor.lockState = CursorLockMode.None;
				Application.Quit ();
			}


			DoShortcutkeys ();


			if (Input.GetKeyDown (KeyCode.LeftAlt)) {
				bInCameraControl = true;
			} else if (Input.GetKeyUp (KeyCode.LeftAlt)) {
				bInCameraControl = false;
			} else if (bInCameraControl) {
				DoCameraControl ();


			} else { 

				// if we have a capturing UIElement, let it update capture, and if it consumes, we are done
				if (pCapturing != null) {
					bool bConsumed = TryDoCapturingUI ();
					if (bConsumed)
						return;
				}

				// check for UI hit on mouse down, and if found and wants to capture, begin capture
				if (Input.GetMouseButtonDown (0)) {
					Ray eyeRay = GetWorldRayAtWorkplaneCursor ();
					UIRayHit uiHit;
					if ( FindUIHit(eyeRay, out uiHit) ) {
						if (uiHit.hitUI.BeginCapture (eyeRay, uiHit))
							pCapturing = uiHit.hitUI;
					}


				} else if (Input.GetMouseButtonUp(0)) {
					Ray eyeRay = GetWorldRayAtWorkplaneCursor ();

					SORayHit rayHit;
					if (scene.FindSORayIntersection (eyeRay, out rayHit)) {
						if (scene.Selected.Count == 0) {
							scene.Select (rayHit.hitSO);

						} else if (scene.IsSelected (rayHit.hitSO) == false) {
							scene.ClearSelection ();
							scene.Select (rayHit.hitSO);

						} else {
							// interact with selected object
						}

					} else { 
						if (scene.Selected.Count > 0)
							scene.ClearSelection ();
					}

				}

			}


		}  // end Update





		public bool FindUIHit(Ray eyeRay, out UIRayHit bestHit) {
			bestHit = new UIRayHit();
			UIRayHit sceneHit = null, cockpitHit = null;
			if ( scene.FindUIRayIntersection(eyeRay, out sceneHit) ) {
				bestHit = sceneHit;
			}
			if ( cockpit.FindUIRayIntersection(eyeRay, out cockpitHit) ) {
				if ( cockpitHit.fHitDist < bestHit.fHitDist )
					bestHit = cockpitHit;
			}
			return bestHit.IsValid;
		}


		public bool FindAnyRayIntersection(Ray eyeRay, out AnyRayHit anyHit) {
			anyHit = new AnyRayHit ();
			AnyRayHit sceneHit = null;
			UIRayHit cockpitHit = null;
			if (scene.FindAnyRayIntersection (eyeRay, out sceneHit)) {
				anyHit = sceneHit;
			}
			if (cockpit.FindUIRayIntersection (eyeRay, out cockpitHit)) {
				if (cockpitHit.fHitDist < anyHit.fHitDist)
					anyHit = new AnyRayHit (cockpitHit);
			}
			return anyHit.IsValid;
		}



		public bool InCapture { 
			get { return (pCapturing != null); }
		}


		bool TryDoCapturingUI() {
			if (pCapturing == null)
				return false;

			Ray eyeRay = GetWorldRayAtWorkplaneCursor ();

			if (Input.GetMouseButtonUp (0)) {
				pCapturing.EndCapture (eyeRay);
				pCapturing = null;
				return true;

			} else if (Input.GetMouseButton (0)) {
				pCapturing.UpdateCapture (eyeRay);
				return true;

			} else {
				Debug.Log ("[SceneController::TryDoCapturingUI] mouse is not pressed or up, why are we here?");
				return false;
			}

		}







		void DoShortcutkeys() {
			
			if (Input.GetKeyUp (KeyCode.C)) {
				scene.AddCylinder ();

			} else if (Input.GetKeyUp (KeyCode.T)) {
				foreach (var ui in scene.UIElements) {
					TransformGizmo gizmo = ui as TransformGizmo;
					if (gizmo != null) {
						if (gizmo.CurrentFrameMode == TransformGizmo.FrameType.LocalFrame)
							gizmo.CurrentFrameMode = TransformGizmo.FrameType.WorldFrame;
						else
							gizmo.CurrentFrameMode = TransformGizmo.FrameType.LocalFrame;
					}
				}
			}
				

		}





		void DoCameraControl() {
			float dx = Input.GetAxis ("Mouse X");
			float dy = Input.GetAxis ("Mouse Y");

			if (Input.GetMouseButton (0)) {
				scene.WorldOrbit (10.0f * dx, 10.0f * dy);

			} else if (Input.GetMouseButton (1)) {
				scene.WorldZoom (1.0f * dy);
			} else if (Input.GetMouseButton (2)) {
				scene.WorldPan (0.5f * dx, 0.5f * dy);
			}
		}



		Ray GetWorldRayAtViewCenter() {
			Ray eyeRay = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
			return eyeRay;
		}

		Ray GetWorldRayAtWorkplaneCursor() {
			Vector3 camPos = GetMouseCursor().CurrentCursorRaySourceWorld;
			Vector3 cursorPos = GetMouseCursor().CurrentCursorPosWorld;
			Ray ray = new Ray (camPos, (cursorPos - camPos).normalized);
			return ray;
		}



	} // end SceneController

} // end namespace
