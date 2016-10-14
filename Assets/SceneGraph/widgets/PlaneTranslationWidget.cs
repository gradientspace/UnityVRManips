using System;
using UnityEngine;

namespace f3
{
	//
	//
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
		Frame3 translateFrameL;		// local-spaace frame
		Frame3 translateFrameW;		// world-space frame
		Vector3 translateNormalW;	// world translation plane normal (redundant...)

		// computed values during interaction
		Frame3 raycastFrame;		// plane perpendicular to translateNormalW (redundant!)
		Vector3 vInitialHitPos;		// initial hit position in frame

		public bool BeginCapture(ITransformable target, Ray worldRay, UIRayHit hit)
		{
			// save local and world frames
			translateFrameL = target.GetLocalFrame (CoordSpace.ObjectCoords);
			translateFrameW = target.GetLocalFrame (CoordSpace.WorldCoords);
			translateNormalW = translateFrameW.GetAxis (nTranslationPlaneNormal);

			// construct plane we will ray-intersect with in UpdateCapture()
			raycastFrame = new Frame3( translateFrameW.Origin, translateNormalW );

			// save initial hitpos in this frame
			vInitialHitPos = raycastFrame.RayPlaneIntersection(worldRay.origin, worldRay.direction, 2);

			return true;
		}

		public bool UpdateCapture(ITransformable target, Ray worldRay)
		{
			// ray-hit with plane that contains translation axis
			Vector3 planeHit = raycastFrame.RayPlaneIntersection(worldRay.origin, worldRay.direction, 2);

			// construct delta
			Vector3 delta = (planeHit - vInitialHitPos);

			// construct new frame translated along axis
			//  [RMS] doing this in world-space here, but we will need to do in local space to do snapping...
			Frame3 newFrame = translateFrameW;
			newFrame.Origin += delta;

			// update target
			target.SetLocalFrame (newFrame, CoordSpace.WorldCoords);

			return true;
		}
	}
}

