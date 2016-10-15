using UnityEngine;
using System.Collections;


namespace f3 {

	public class MouseCursorController {

		Camera camera;
		SceneController Scene;

		public GameObject Cursor { get; set; }
		public float CursorVisualAngleInDegrees { get; set; }
		public Material CursorDefaultMaterial { get; set; }
		public Material CursorHitMaterial { get; set; }
		public Material CursorCapturingMaterial { get; set; }

		public Vector3 CurrentCursorPosWorld;
		public Vector3 CurrentCursorRaySourceWorld;

		GameObject xformObject;			// [RMS] this is an internal GO we use basically just for a transform
										//  Actually a plane that stays in front of eye.

		Vector3 vCursorPlaneOrigin;
		Vector3 vCursorPlaneRight;
		Vector3 vCursorPlaneForward;
		Vector3 vRaySourcePosition;

		float dx;
		float dy;
		Vector3 vPlaneCursorPos;
		Vector3 vSceneCursorPos;


		public MouseCursorController(Camera viewCam, SceneController scene) {
			camera = viewCam;
			Scene = scene;
		}

		// Use this for initialization
		public void Start () {
			dx = 0;
			dy = 0;
			vPlaneCursorPos = Vector3.zero;
			vSceneCursorPos = vPlaneCursorPos;

			CursorDefaultMaterial = MaterialUtil.CreateTransparentMaterial (Color.grey, 0.2f);
			CursorHitMaterial = MaterialUtil.CreateTransparentMaterial (Color.yellow, 0.8f);
			CursorCapturingMaterial = MaterialUtil.CreateTransparentMaterial (Color.cyan, 0.3f);

			CursorVisualAngleInDegrees = 1.5f;

			Cursor = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			Cursor.name = "cursor";
			Cursor.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			MeshRenderer ren = Cursor.GetComponent<MeshRenderer> ();
			ren.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			ren.receiveShadows = false;
			ren.material = CursorDefaultMaterial;

			xformObject = GameObject.CreatePrimitive (PrimitiveType.Plane);
			xformObject.name = "cursor_plane";
			MeshRenderer ren2 = xformObject.GetComponent<MeshRenderer> ();
			ren2.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			ren2.receiveShadows = false;
			ren2.material = MaterialUtil.CreateTransparentMaterial (Color.cyan, 0.2f);
			ren2.enabled = false;
		}
		
		// FixedUpdate is called before any Update
		public void Update () {

			// if we are in capture we freeze the cursor plane
			if (Scene.InCapture == false) {
				Vector3 camPos = camera.gameObject.transform.position;
				Vector3 forward = camera.gameObject.transform.forward;

				// orient Y-up plane so that it is in front of eye, perp to camera direction
				xformObject.transform.position = camPos + 10 * forward;	
				xformObject.transform.LookAt (camera.gameObject.transform);
				xformObject.transform.RotateAround (xformObject.transform.position, xformObject.transform.right, 90);

				// that plane is the plane the mouse cursor moves on
				this.vCursorPlaneOrigin = xformObject.transform.position;
				this.vCursorPlaneRight = xformObject.transform.right;
				this.vCursorPlaneForward = xformObject.transform.forward;
				this.vRaySourcePosition = camera.transform.position;
			}

			Vector3 curPos = new Vector3 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"), 0);
			dx -= 0.3f * curPos.x;
			dy -= 0.3f * curPos.y;

			vPlaneCursorPos = 
				vCursorPlaneOrigin + dx * vCursorPlaneRight + dy * vCursorPlaneForward;
			vSceneCursorPos = vPlaneCursorPos;


			bool bHit = false;
			if (Scene != null) {
				Ray r = new Ray (camera.transform.position, (vPlaneCursorPos - camera.transform.position).normalized);
				AnyRayHit hit = null;
				if (Scene.FindAnyRayIntersection(r, out hit)) {
					vSceneCursorPos = hit.hitPos;
					bHit = true;
				} else { 
					GameObjectRayHit ghit = null;
					if (Scene.GetScene ().FindWorldBoundsHit (r, out ghit))
						vSceneCursorPos = ghit.hitPos;
				}
			}

			this.CurrentCursorPosWorld = vPlaneCursorPos;
			this.CurrentCursorRaySourceWorld = this.vRaySourcePosition;

			Cursor.transform.position = vSceneCursorPos;
			if (Scene.InCapture)
				Cursor.GetComponent<MeshRenderer> ().material = CursorCapturingMaterial;
			else if (bHit)
				Cursor.GetComponent<MeshRenderer> ().material = CursorHitMaterial;
			else
				Cursor.GetComponent<MeshRenderer> ().material = CursorDefaultMaterial;
			Cursor.layer = (bHit || Scene.InCapture) ? LayerMask.NameToLayer (SceneGraphConfig.WidgetOverlayLayerName) : 0 ;

			// maintain a consistent visual size for 3D cursor sphere
			float fScaling = MathUtil.GetVRRadiusForVisualAngle(vSceneCursorPos, camera.transform.position, CursorVisualAngleInDegrees);
			Cursor.transform.localScale = new Vector3 (fScaling, fScaling, fScaling);
		}
	}

}