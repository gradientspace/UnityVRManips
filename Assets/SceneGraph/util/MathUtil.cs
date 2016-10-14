using System;
using UnityEngine;

namespace f3
{
	public class MathUtil
	{
		protected MathUtil ()
		{
		}


		public static Vector3 ClosestPointOnLine(Vector3 p0, Vector3 dir, Vector3 pt) 
		{
			float t = Vector3.Dot (pt - p0, dir);
			return p0 + t * dir;
		}
		public static float ClosestPointOnLineT(Vector3 p0, Vector3 dir, Vector3 pt) 
		{
			float t = Vector3.Dot (pt - p0, dir);
			return t;
		}



		public static Vector3 TransformPointToLocalSpace(Vector3 point, GameObject obj) {
			return obj.transform.InverseTransformPoint(point);
		}
		public static Vector3 TransformVectorToLocalSpace(Vector3 vector, GameObject obj) {
			return obj.transform.InverseTransformDirection(vector);
		}
		public static Ray TransformRayToLocalSpace(Ray r, GameObject obj) {
			return new Ray (
				obj.transform.InverseTransformPoint (r.origin),
				obj.transform.InverseTransformDirection (r.direction));
		}




		// rayhit-test a GameObject, handling collider enable/disable
		public static bool FindGORayIntersection (Ray ray, GameObject go, out GameObjectRayHit hit)
		{
			hit = null;

			bool bIsEnabled = go.GetComponent<MeshCollider> ().enabled;
			go.GetComponent<MeshCollider> ().enabled = true;
			RaycastHit hitInfo;
			if (go.GetComponent<MeshCollider> ().Raycast (ray, out hitInfo, Mathf.Infinity)) {
				hit = new GameObjectRayHit ();
				hit.fHitDist = hitInfo.distance;
				hit.hitPos = hitInfo.point;
				hit.hitGO = go;
			}
			go.GetComponent<MeshCollider> ().enabled = bIsEnabled;

			return (hit != null);
		}


		// basic ray-sphere intersection
		public static bool IntersectRaySphere(Vector3 vOrigin, Vector3 vDirection, Vector3 vCenter, float fRadius, out float fRayT) 
		{
			fRayT = 0.0f;
			Vector3 m = vOrigin - vCenter; 
			float b = Vector3.Dot(m, vDirection); 
			float c = Vector3.Dot(m, m) - fRadius * fRadius; 

			// Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0) 
			if (c > 0.0f && b > 0.0f) 
				return false; 
			float discr = b*b - c; 

			// A negative discriminant corresponds to ray missing sphere 
			if (discr < 0.0f) 
				return false; 

			// Ray now found to intersect sphere, compute smallest t value of intersection
			fRayT = -b - Mathf.Sqrt(discr); 

			// If t is negative, ray started inside sphere so clamp t to zero 
			if (fRayT < 0.0f) 
				fRayT = 0.0f; 

			return true;
		}



		// Returns a distance value that corresponds to a fixed visual angle at the given distance
		public static float GetVRRadiusForVisualAngle(Vector3 vWorldPos, Vector3 vEyePos, float fAngleInDegrees)
		{
			float r = (vWorldPos - vEyePos).magnitude;
			double a = fAngleInDegrees * (Math.PI/180.0);
			float c = 2.0f * r * (float)Math.Sin (a / 2.0);
			return c;
		}


	}
}

