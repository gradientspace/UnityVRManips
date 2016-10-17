using System;
using UnityEngine;

namespace f3
{
	//
	// Cockpit object will create an instance of this and call Initialize once.
	// You can use this to configure your HUD, if you want. 
	//
	public class setup_cockpit
	{
		public setup_cockpit ()
		{
		}

	
		Frame3 make_hud_sphere_frame(float fHUDRadius, float fHorzAngleDeg, float fVertAngleDeg) {
			Ray r = MathUtil.MakeRayFromSphereCenter (fHorzAngleDeg, fVertAngleDeg);
			float fRayT = 0.0f;
			MathUtil.IntersectRaySphere (r.origin, r.direction, Vector3.zero, fHUDRadius, out fRayT);
			// fRayT is negative inside sphere (?)
			Vector3 v = r.origin + Math.Abs(fRayT) * r.direction;
			return new Frame3 (v, v.normalized);
		}


		public void Initialize(Cockpit cockpit) 
		{
			var defaultMaterial = MaterialUtil.CreateStandardMaterial( new Color(0.25f, 0.75f, 0.1f) );
			Frame3 cockpitF = cockpit.GetLocalFrame (CoordSpace.WorldCoords);

			float fHUDRadius = 0.7f;

			HUDButton addCylinderButton = new HUDButton () { Radius = 0.08f };
			addCylinderButton.Create (MaterialUtil.CreateImageMaterial ("icons/cylinder_v1"));
			Frame3 cylFrame = addCylinderButton.GetObjectFrame();
			Frame3 cylHUDFrame = make_hud_sphere_frame (fHUDRadius, -35.0f, 10.0f);
			addCylinderButton.SetObjectFrame ( 
				cylFrame.Translated(cylHUDFrame.Origin)
				.Rotated(Quaternion.FromToRotation (cylFrame.Z, cylHUDFrame.Z)) );
			addCylinderButton.OnClicked += (s, e) => {
				cockpit.Parent.Scene.AddCylinder ();
			};
			cockpit.AddUIElement (addCylinderButton, true);


			HUDButton addBoxButton = new HUDButton () { Radius = 0.08f };
			addBoxButton.Create (MaterialUtil.CreateImageMaterial ("icons/cylinder_v1"));
			Frame3 boxFrame = addBoxButton.GetObjectFrame();
			Frame3 boxHUDFrame = make_hud_sphere_frame (fHUDRadius, -35.0f, -10.0f);
			addBoxButton.SetObjectFrame ( 
				boxFrame.Translated(boxHUDFrame.Origin)
				.Rotated(Quaternion.FromToRotation (boxFrame.Z, boxHUDFrame.Z)) );
			addBoxButton.OnClicked += (s, e) => {
				cockpit.Parent.Scene.AddBox ();
			};
			cockpit.AddUIElement (addBoxButton, true);

		}


	}
}

