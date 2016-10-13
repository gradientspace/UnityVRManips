using System;
using UnityEngine;

namespace f3
{
	public class MaterialUtil
	{
		protected MaterialUtil ()
		{
		}


		public static Color MakeColor(Color c, float alpha) {
			return new Color (c.r, c.g, c.b, alpha);
		}

		public static Material CreateStandardMaterial(Color c) {
			Material m = new Material (Shader.Find ("Standard"));
			m.color = c;
			return m;
		}

		public static Material CreateTransparentMaterial(Color c, float alpha) {
			Material m = new Material (Resources.Load (SceneGraphConfig.DefaultTransparentMaterialResourcePath) as Material);
			m.color = MakeColor (c, alpha);
			return m;
		}

	}
}

