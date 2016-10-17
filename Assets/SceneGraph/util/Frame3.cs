using System;
using UnityEngine;

namespace f3
{
	public struct Frame3
	{
		Quaternion rotation;
		Vector3 origin;

		static readonly public Frame3 Identity = new Frame3(Vector3.zero, Quaternion.identity);

		public Frame3 (Frame3 copy)
		{
			this.rotation = copy.rotation;
			this.origin = copy.origin;
		}

		public Frame3 (Vector3 origin)
		{
			rotation = Quaternion.identity;
			this.origin = origin;
		}

		public Frame3 (Vector3 origin, Vector3 setZ)
		{
			rotation = Quaternion.FromToRotation (Vector3.forward, setZ);
			this.origin = origin;
		}

		public Frame3 (Vector3 origin, Quaternion orientation)
		{
			rotation = orientation;
			this.origin = origin;
		}


		public Frame3(Transform t, bool bLocal)
		{
			if (bLocal) {
				this.rotation = t.localRotation;
				this.origin = t.localPosition;
			} else { 
				this.rotation = t.rotation;
				this.origin = t.position;
			}
		}


		public Quaternion Rotation {
			get { return rotation; }
			set { rotation = value; }
		}

		public Vector3 Origin {
			get { return origin; }
			set { origin = value; }
		}

		public Vector3 X {
			get { return rotation * Vector3.right; } 
		}
		public Vector3 Y {
			get { return rotation * Vector3.up; } 
		}
		public Vector3 Z {
			get { return rotation * Vector3.forward; } 
		}

		public Vector3 GetAxis(int nAxis) {
			if (nAxis == 0)
				return rotation * Vector3.right;
			else if (nAxis == 1)
				return rotation * Vector3.up;
			else if (nAxis == 2)
				return rotation * Vector3.forward;
			else
				throw new ArgumentOutOfRangeException ("nAxis");
		}


		public void Translate(Vector3 v) {
			origin += v;
		}
		public Frame3 Translated(Vector3 v) {
			return new Frame3 (this.origin + v, this.rotation);
		}
		public void Rotate(Quaternion q) {
			rotation *= q;
		}
		public Frame3 Rotated(Quaternion q) {
			return new Frame3 (this.origin, this.rotation * q);
		}


		public Vector3 RayPlaneIntersection(Vector3 ray_origin, Vector3 ray_direction, int nAxisAsNormal) 
		{
			Vector3 N = GetAxis (nAxisAsNormal);
			float d = -Vector3.Dot (Origin, N);
			float t = - ( Vector3.Dot(ray_origin, N) + d ) / ( Vector3.Dot(ray_direction, N) );
			return ray_origin + t * ray_direction;
		}


		public override string ToString ()
		{
			return string.Format ("[Frame3: Origin={0}, X={1}, Y={2}, Z={3}]", Origin.ToString("F5"), X.ToString("F5"), Y.ToString("F5"), Z.ToString("F5"));
		}

	}
}

