using UnityEngine;
using System.Collections;

namespace f3 {


	public class CameraController {

		protected static CameraController s_singleton;
		protected CameraController() {
		}
		public static CameraController Singleton { 
			get { 
				if (s_singleton == null)
					s_singleton = new CameraController ();
				return s_singleton;
			}
		}


		Camera mainCamera;
		Camera widgetCamera;

		// Use this for initialization
		public void Initialize () {

			// find main camera
			var mainCameraObj = GameObject.FindWithTag ("MainCamera");
			mainCamera = mainCameraObj.GetComponent<Camera> () as Camera;

			// argh duplicate camera always lags main one, so we have to use hardcoded...
			var widgetCameraObj = GameObject.Find("WidgetCamera");
			widgetCamera = widgetCameraObj.GetComponent<Camera> () as Camera;
			// duplicate to create new camera for 3D widgets
//			widgetCamera = Camera.Instantiate (mainCamera);
//			widgetCamera.name = "WidgetCamera";
			(widgetCamera.GetComponent<AudioListener> () as AudioListener).enabled = false;

			// only do depth clear for this camera
			widgetCamera.clearFlags = CameraClearFlags.Depth;

			// this camera only renders 3DWidgetOverlay layer, and mainCam does not!
			int nWidgetLayer = LayerMask.NameToLayer ( SceneGraphConfig.WidgetOverlayLayerName );
			widgetCamera.cullingMask = (1 << nWidgetLayer);
			mainCamera.cullingMask &= ~(1 << nWidgetLayer);
		}
		


		// [RMS] this doesn't work...
		public void FixTracking() {
			//yield return new WaitForEndOfFrame ();

			// track main camera position
			widgetCamera.transform.position = mainCamera.transform.position;
			widgetCamera.transform.rotation = mainCamera.transform.rotation;
		}

	}

}