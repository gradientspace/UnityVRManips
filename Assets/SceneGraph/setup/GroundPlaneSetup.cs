using UnityEngine;
using System.Collections;

namespace f3 { 

	public class GroundPlaneSetup : MonoBehaviour {

		public SceneController Scene;

		// Use this for initialization
		void Start () {
			Scene.GetScene ().AddWorldBoundsObject (this.gameObject);
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}


}
