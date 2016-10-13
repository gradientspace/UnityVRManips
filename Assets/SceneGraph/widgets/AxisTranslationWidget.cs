using System;
using UnityEngine;

namespace f3
{
	//
	//
	// 
	public class AxisTranslationWidget : Widget
	{
		int nTranslationAxis;
		ISpaceConversion conversion;

		public AxisTranslationWidget(int nFrameAxis, ISpaceConversion conversion)
		{
			nTranslationAxis = nFrameAxis;
			this.conversion = conversion;
		}

		// stored frames from target used during click-drag interaction
		Frame3 translateFrameL;		// local-spaace frame
		Frame3 translateFrameW;		// world-space frame
		Vector3 translateAxisW;		// world translation axis (redundant...)

		// computed values during interaction
		Frame3 raycastFrame;		// camera-facing plane containing translateAxisW
		float fTranslateStartT;		// start T-value along translateAxisW

		public bool BeginCapture(ITransformable target, Ray worldRay, UIRayHit hit)
		{
			// save local and world frames
			translateFrameL = target.GetLocalFrame (CoordSpace.ObjectCoords);
			translateFrameW = target.GetLocalFrame (CoordSpace.WorldCoords);
			translateAxisW = translateFrameW.GetAxis (nTranslationAxis);

			// save t-value of closest point on translation axis, so we can find delta-t
			Vector3 vWorldHitPos = hit.hitPos;
			fTranslateStartT = MathUtil.ClosestPointOnLineT(
				translateFrameW.Origin, translateAxisW, vWorldHitPos);

			// construct plane we will ray-intersect with in UpdateCapture()
			Vector3 vForward = Vector3.Cross( Camera.main.transform.up, translateAxisW );
			raycastFrame = new Frame3 (vWorldHitPos, vForward);

			return true;
		}

		public bool UpdateCapture(ITransformable target, Ray worldRay)
		{
			// ray-hit with plane that contains translation axis
			Vector3 planeHit = raycastFrame.RayPlaneIntersection(worldRay.origin, worldRay.direction, 2);

			// figure out new T-value along axis, then our translation update is delta-t
			float fNewT = MathUtil.ClosestPointOnLineT (translateFrameW.Origin, translateAxisW, planeHit);
			float fDeltaT = (fNewT - fTranslateStartT);

			// construct new frame translated along axis (in local space)
			Frame3 newFrame = translateFrameL;
			newFrame.Origin += fDeltaT * translateFrameL.GetAxis(nTranslationAxis);

			// update target
			target.SetLocalFrame (newFrame, CoordSpace.ObjectCoords);

			return true;
		}
	}
}

