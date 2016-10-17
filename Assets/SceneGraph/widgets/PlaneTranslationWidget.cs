using System;
using UnityEngine;

namespace f3
{
	//
	// this Widget implements translation constrained to a plane
	// 
	public class PlaneTranslationWidget : Widget
	{
		int nTranslationPlaneNormal;
		ISpaceConversion conversion;

		public PlaneTranslationWidget(int nNormalAxis, ISpaceConversion conversion)
		{
			nTranslationPlaneNormal = nNormalAxis;
			this.conversion = conversion;
		}

		// stored frames from target used during click-drag interaction
		Frame3 translateFrameL;		// local-space frame
		Frame3 translateFrameW;		// world-space frame

		// computed values during interaction
		Vector3 vInitialHitPos;		// initial hit position in frame

		public bool BeginCapture(ITransformable target, Ray worldRay, UIRayHit hit)
		{
			// save local and world frames
			translateFrameL = target.GetLocalFrame (CoordSpace.ObjectCoords);
			translateFrameW = target.GetLocalFrame (CoordSpace.WorldCoords);

			// save initial hitpos in translation plane
			vInitialHitPos = translateFrameW.RayPlaneIntersection(worldRay.origin, worldRay.direction, nTranslationPlaneNormal);

			return true;
		}

		public bool UpdateCapture(ITransformable target, Ray worldRay)
		{
			// ray-hit with world-space translation plane
			Vector3 planeHit = translateFrameW.RayPlaneIntersection(worldRay.origin, worldRay.direction, nTranslationPlaneNormal);
			int e0 = (nTranslationPlaneNormal + 1) % 3;
			int e1 = (nTranslationPlaneNormal + 2) % 3;

			// construct delta in world space and project into frame coordinates
			Vector3 delta = (planeHit - vInitialHitPos);
			float dx = Vector3.Dot (delta, translateFrameW.GetAxis (e0));
			float dy = Vector3.Dot (delta, translateFrameW.GetAxis (e1));

			// construct new local frame translated along plane axes
			Frame3 newFrame = translateFrameL;
			newFrame.Origin += dx*translateFrameL.GetAxis(e0) + dy*translateFrameL.GetAxis(e1);

			// update target
			target.SetLocalFrame (newFrame, CoordSpace.ObjectCoords);

			return true;
		}
	}
}

