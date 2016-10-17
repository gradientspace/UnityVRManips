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


		public static Material CreateFlatMaterial(Color c, float alpha = 1.0f ) {
			Material m = new Material (Shader.Find ("Unlit/Texture"));
			m.color = MakeColor (c, c.a * alpha);
			return m;
		}

		public static Material CreateImageMaterial(string sResourcePath) {
			Material m = new Material (Shader.Find ("Unlit/Texture"));
			m.color = Color.white;
			Texture2D tex = (Texture2D)Resources.Load (sResourcePath);
			m.mainTexture = tex;
			return m;
		}


	}
}

