using System;
using UnityEngine;

namespace f3
{
	public class HUDButton : HUDStandardItem
	{
		GameObject button, buttonDisc;


		public HUDButton ()
		{
			Radius = 0.1f;
		}


		public float Radius { get; set; }

		static int button_counter = 1;
		public void Create( Material defaultMaterial) {

			button = new GameObject( string.Format("HUDButton{0}", button_counter++) );
			buttonDisc = AppendMeshGO ("disc", 
				MeshGenerators.CreateTrivialDisc (Radius, 32), 
				defaultMaterial, button);

			button.transform.Rotate (Vector3.right, -90.0f); // ??
		}



		// event handler for clicked event
		//public delegate void HUDButtonClickedEventHandler(object sender, EventArgs e);
		public event EventHandler OnClicked;

		protected virtual void SendOnClicked(EventArgs e) {
			var tmp = OnClicked;
			if ( tmp != null )
				tmp(this, e);
		}





		#region SceneUIElement implementation

		override public UnityEngine.GameObject RootGameObject {
			get { return button; }
		}

		override public bool BeginCapture (UnityEngine.Ray ray, UIRayHit hit)
		{
			return HasGO (hit.hitGO);
		}

		override public bool UpdateCapture (UnityEngine.Ray ray)
		{
			return true;
		}

		override public bool EndCapture (UnityEngine.Ray ray)
		{
			if (IsGOHit (ray, buttonDisc)) {
				OnClicked(this, new EventArgs() );
			}
			return true;
		}

		#endregion
	}
}

