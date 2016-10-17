using System;
using UnityEngine;

namespace f3
{
	//
	// this Widget implements rotation around an axis
	//
	public class AxisRotationWidget : Widget
	{
		int nRotationAxis;
		ISpaceConversion conversion;

		public AxisRotationWidget(int nFrameAxis, ISpaceConversion conversion)
		{
			nRotationAxis = nFrameAxis;
			this.conversion = conversion;
		}

		// stored frames from target used during click-drag interaction
		Frame3 rotateFrameL;		// local-space frame 
		Frame3 rotateFrameW;		// world-space frame
		Vector3 rotateAxisW;		// world-space axis we are rotating around (redundant...)

		// computed values during interaction
		Frame3 raycastFrame;		// camera-facing plane containing translateAxisW
		float fRotateStartAngle;

		public bool BeginCapture(ITransformable target, Ray worldRay, UIRayHit hit)
		{
			// save local and world frames
			rotateFrameL = target.GetLocalFrame (CoordSpace.ObjectCoords);
			rotateFrameW = target.GetLocalFrame (CoordSpace.WorldCoords);
			rotateAxisW = rotateFrameW.GetAxis (nRotationAxis);

			// save angle of hitpos in 2D plane perp to rotateAxis, so we can find delta-angle later
			Vector3 vWorldHitPos = hit.hitPos;
			Vector3 dv = vWorldHitPos - rotateFrameW.Origin;
			int iX = (nRotationAxis + 1) % 3;
			int iY = (nRotationAxis + 2) % 3;
			float fX = Vector3.Dot( dv, rotateFrameW.GetAxis(iX) );
			float fY = Vector3.Dot( dv, rotateFrameW.GetAxis(iY) );
			fRotateStartAngle = (float)Math.Atan2 (fY, fX);

			// construct plane we will ray-intersect with in UpdateCapture()
			raycastFrame = new Frame3( vWorldHitPos, rotateAxisW );

			return true;
		}

		public bool UpdateCapture(ITransformable target, Ray worldRay)
		{
			// ray-hit with plane perpendicular to rotateAxisW
			Vector3 planeHit = raycastFrame.RayPlaneIntersection (worldRay.origin, worldRay.direction, 2);

			// find angle of hitpos in 2D plane perp to rotateAxis, and compute delta-angle
			Vector3 dv = planeHit - rotateFrameW.Origin;
			int iX = (nRotationAxis + 1) % 3;
			int iY = (nRotationAxis + 2) % 3;
			float fX = Vector3.Dot( dv, rotateFrameW.GetAxis(iX) );
			float fY = Vector3.Dot( dv, rotateFrameW.GetAxis(iY) );
			float fNewAngle = (float)Math.Atan2 (fY, fX);
			float fDeltaAngle = (fNewAngle - fRotateStartAngle);

			// construct new frame for target that is rotated around axis
			Vector3 rotateAxisL = rotateFrameL.GetAxis(nRotationAxis);
			Quaternion q = Quaternion.AngleAxis(fDeltaAngle * Mathf.Rad2Deg, rotateAxisL );
			Frame3 newFrame = rotateFrameL;
			newFrame.Rotation = q * newFrame.Rotation;		// order matters here!

			// update target
			target.SetLocalFrame (newFrame, CoordSpace.ObjectCoords);

			return true;
		}
	}
}

