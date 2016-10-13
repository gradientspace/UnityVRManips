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



		// try to estimate relative scaling factor in world space that will maintain constant size relative to pixel space
		//  (harder in VR because pixel space is curved...)
		public static float EstimateWorldEyeScaling(Vector3 vWorldPos, Vector3 vWorldDeltaDir, float fDeltaDist, Vector3 vEyePos, float fEyeRadius) 
		{
			float fHitT0, fHitT1;
			Vector3 vDir0 = (vEyePos - vWorldPos).normalized;
			bool bHit0 = IntersectRaySphere (vWorldPos, vDir0, vEyePos, fEyeRadius, out fHitT0);
			Vector3 vHit0 = vWorldPos + fHitT0 * vDir0;

			Vector3 vDeltaPos = vWorldPos + fDeltaDist * vWorldDeltaDir;
			Vector3 vDir1 = (vEyePos - vDeltaPos).normalized;
			bool bHit1 = IntersectRaySphere (vDeltaPos, vDir1, vEyePos, fEyeRadius, out fHitT1);
			Vector3 vHit1 = vDeltaPos + fHitT1 * vDir1;

			Debug.Assert (bHit0 && bHit1);

			float dp = (vHit1 - vHit0).magnitude / fEyeRadius;
			return dp;
		}


	}
}

