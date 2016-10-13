using System;

namespace f3
{
	public class SceneGraphConfig
	{
		protected SceneGraphConfig ()
		{
		}

		// assumption is that layer with this name has been created in the Editor, because we 
		// cannot create layers in code. We will use this layer to draw overlay 3D objects (eg 3D widgets, etc)
		public static string WidgetOverlayLayerName {
			get { return "3DWidgetOverlay"; }
		}


		public static string DefaultTransparentMaterialResourcePath {
			get { return "StandardMaterials/transparentMaterial"; }
		}

	}
}

