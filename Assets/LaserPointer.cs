using UnityEngine;
using System.Collections;

public class LaserPointer : MonoBehaviour {

	public Camera mycam;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 camPos = mycam.gameObject.transform.position;
		Vector3 forward = mycam.gameObject.transform.forward;

		this.transform.position = camPos + 10 * forward;
	}
}
