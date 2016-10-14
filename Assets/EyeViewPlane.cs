using UnityEngine;
using System.Collections;


namespace f3 {

	public class EyeViewPlane : MonoBehaviour {

		public Camera mycam;
		public SceneController Scene;

		GameObject cursor;
		Material cursorDefaultMaterial;
		Material cursorHitMaterial;
		Material capturingMaterial;

		float fCursorVisualAngleInDegrees;

		Vector3 vCursorPlaneOrigin;
		Vector3 vCursorPlaneRight;
		Vector3 vCursorPlaneForward;
		Vector3 vRaySourcePosition;


		float dx;
		float dy;
		Vector3 vPlaneCursorPos;
		Vector3 vSceneCursorPos;

		// Use this for initialization
		void Start () {
			dx = 0;
			dy = 0;
			vPlaneCursorPos = Vector3.zero;
			vSceneCursorPos = vPlaneCursorPos;

			cursorDefaultMaterial = MaterialUtil.CreateTransparentMaterial (Color.grey, 0.2f);
			cursorHitMaterial = MaterialUtil.CreateTransparentMaterial (Color.yellow, 0.8f);
			capturingMaterial = MaterialUtil.CreateTransparentMaterial (Color.cyan, 0.3f);

			fCursorVisualAngleInDegrees = 1.5f;

			cursor = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			cursor.name = "cursor";
			cursor.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			MeshRenderer ren = cursor.GetComponent<MeshRenderer> ();
			ren.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			ren.receiveShadows = false;
			ren.material = cursorDefaultMaterial;
		}
		
		// FixedUpdate is called before any Update
		void Update () {

			// if we are in capture we freeze the cursor plane
			if (Scene.InCapture == false) {
				Vector3 camPos = mycam.gameObject.transform.position;
				Vector3 forward = mycam.gameObject.transform.forward;

				this.transform.position = camPos + 10 * forward;	
				this.transform.LookAt (mycam.gameObject.transform);
				this.transform.RotateAround (this.transform.position, this.transform.right, 90);

				this.vCursorPlaneOrigin = this.transform.position;
				this.vCursorPlaneRight = this.transform.right;
				this.vCursorPlaneForward = this.transform.forward;
				this.vRaySourcePosition = mycam.transform.position;
			}

			Vector3 curPos = new Vector3 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"), 0);
			dx -= 0.3f * curPos.x;
			dy -= 0.3f * curPos.y;

			vPlaneCursorPos = 
				vCursorPlaneOrigin + dx * vCursorPlaneRight + dy * vCursorPlaneForward;
			vSceneCursorPos = vPlaneCursorPos;


			bool bHit = false;
			if (Scene != null) {
				Ray r = new Ray (mycam.transform.position, (vPlaneCursorPos - mycam.transform.position).normalized);
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

			WorkPlaneController.Singleton.CurrentCursorPosWorld = vPlaneCursorPos;
			WorkPlaneController.Singleton.CurrentCursorRaySourceWorld = this.vRaySourcePosition;

			cursor.transform.position = vSceneCursorPos;
			if (Scene.InCapture)
				cursor.GetComponent<MeshRenderer> ().material = capturingMaterial;
			else if (bHit)
				cursor.GetComponent<MeshRenderer> ().material = cursorHitMaterial;
			else
				cursor.GetComponent<MeshRenderer> ().material = cursorDefaultMaterial;
			cursor.layer = (bHit || Scene.InCapture) ? LayerMask.NameToLayer (SceneGraphConfig.WidgetOverlayLayerName) : 0 ;

			// maintain a consistent visual size for 3D cursor sphere
			float fScaling = MathUtil.GetVRRadiusForVisualAngle(vSceneCursorPos, mycam.transform.position, fCursorVisualAngleInDegrees);
			cursor.transform.localScale = new Vector3 (fScaling, fScaling, fScaling);
		}
	}

}