using UnityEngine;
using System.Collections;

namespace f3 {

	public class SceneLightingSetup : MonoBehaviour {

		public SceneController Scene;

		// Use this for initialization
		void Start () {
			Vector3 vCornerPos = 20.0f * new Vector3 (1, 3, 1).normalized;

			for (int k = 0; k < 4; k++) {
				GameObject lightObj = new GameObject (string.Format ("spotlight{0}", k));
				Light lightComp = lightObj.AddComponent<Light> ();
				lightComp.type = LightType.Directional;
				lightComp.transform.position = vCornerPos;
				lightComp.transform.LookAt (Vector3.zero);
				lightComp.transform.RotateAround(Vector3.zero, Vector3.up, (float)k * 90.0f);

				lightComp.intensity = 0.1f;
				lightComp.color = Color.white;

				if ( k == 0 || k == 1 )
					lightComp.shadows = LightShadows.Hard;

				lightComp.transform.SetParent ( this.gameObject.transform );
			}


			this.gameObject.transform.SetParent (Scene.GetScene ().RootGameObject.transform);
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}


}
