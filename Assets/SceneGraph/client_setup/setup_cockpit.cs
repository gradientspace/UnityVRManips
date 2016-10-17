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

		public void Initialize(Cockpit cockpit) 
		{
			var defaultMaterial = MaterialUtil.CreateStandardMaterial( new Color(0.25f, 0.75f, 0.1f) );
			Frame3 cockpitF = cockpit.GetLocalFrame (CoordSpace.WorldCoords);

			HUDButton addCylinderButton = new HUDButton () { Radius = 0.08f };
			addCylinderButton.Create (MaterialUtil.CreateImageMaterial ("icons/cylinder_v1"));
			addCylinderButton.SetObjectFrame (
				addCylinderButton.GetObjectFrame ().Translated (0.5f * Vector3.forward - 0.4f * Vector3.right + 0.1f * Vector3.up));
			addCylinderButton.OnClicked += (s, e) => {
				cockpit.Parent.Scene.AddCylinder ();
			};
			cockpit.AddUIElement (addCylinderButton, true);


			HUDButton addBoxButton = new HUDButton () { Radius = 0.08f };
			addBoxButton.Create (MaterialUtil.CreateImageMaterial ("icons/cylinder_v1"));
			addBoxButton.SetObjectFrame (
				addBoxButton.GetObjectFrame ().Translated (0.5f * Vector3.forward - 0.4f * Vector3.right - 0.1f * Vector3.up));
			addBoxButton.OnClicked += (s, e) => {
				cockpit.Parent.Scene.AddBox ();
			};
			cockpit.AddUIElement (addBoxButton, true);

		}


	}
}

