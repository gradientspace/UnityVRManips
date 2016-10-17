using UnityEngine;
using System.Collections;

namespace f3 { 

	public class MeshGenerators {


		// create a triangle fan
		public static Mesh CreateTrivialDisc(float radius, int nSteps) {

			Vector3[] vertices = new Vector3[nSteps + 1];
			Vector2[] uv = new Vector2[nSteps + 1];
			Vector3[] normals = new Vector3[nSteps + 1];
			int[] triangles = new int[nSteps * 3];

			int vi = 0;
			vertices [vi] = Vector3.zero;
			uv [vi] = new Vector2 (0.5f, 0.5f);
			normals [vi] = Vector3.up;
			vi++;

			float fDelta = (2 * Mathf.PI) / nSteps;
			for (int k = 0; k < nSteps; ++k) {
				float a = (float)k * fDelta;
				float cosa = Mathf.Cos (a), sina = Mathf.Sin (a);
				vertices [vi] = new Vector3 (radius * cosa, 0, radius * sina);
				uv [vi] = new Vector2 (0.5f * (1.0f + cosa), 0.5f * (1 + sina));
				normals [vi] = Vector3.up;
				vi++;
			}

			int ti = 0;
			for (int k = 1; k < nSteps; ++k) {
				triangles [ti++] = k;
				triangles [ti++] = 0;
				triangles [ti++] = (k + 1);
			}
			triangles [ti++] = nSteps;
			triangles [ti++] = 0;
			triangles [ti++] = 1;

			Mesh m = new Mesh ();
			m.vertices = vertices;
			m.uv = uv;
			m.normals = normals;
			m.triangles = triangles;

			return m;
		}





		// from http://answers.unity3d.com/questions/855827/problems-with-creating-a-disk-shaped-mesh-c.html
		// ugh this code is shit! 
		public static Mesh CreateDisc(float radius, int radiusTiles,int tilesAround)
		{
			Vector3[] vertices = new Vector3    [radiusTiles*tilesAround*6];
			Vector3[] normals = new Vector3[vertices.Length];
			int[] triangles     = new int        [radiusTiles*tilesAround*6];
			Vector2[] UV = new Vector2[vertices.Length];
			int currentVertex = 0;

			float tileLength = radius / (float)radiusTiles;    //the length of a tile parallel to the radius
			float radPerTile = 2 * Mathf.PI / tilesAround; //the radians the tile takes

			for(int angleNum = 0; angleNum < tilesAround; angleNum++)//loop around
			{
				float angle = (float)radPerTile*(float)angleNum;    //the current angle in radians

				for(int offset = 0; offset < radiusTiles; offset++)//loop from the center outwards
				{
					vertices[currentVertex]        =    new Vector3(Mathf.Cos(angle)*offset*tileLength                 ,0,  Mathf.Sin(angle)*offset*tileLength);
					vertices[currentVertex + 1]    =    new Vector3(Mathf.Cos(angle + radPerTile)*offset*tileLength ,0,  Mathf.Sin(angle + radPerTile)*offset*tileLength);
					vertices[currentVertex + 2]    =    new Vector3(Mathf.Cos(angle)*(offset + 1)*tileLength         ,0,  Mathf.Sin(angle)*(offset + 1)*tileLength);					

					vertices[currentVertex + 3]    =    new Vector3(Mathf.Cos(angle + radPerTile)*offset*tileLength         ,0,  Mathf.Sin(angle + radPerTile)*offset*tileLength);
					vertices[currentVertex + 4]    =    new Vector3(Mathf.Cos(angle + radPerTile)*(offset + 1)*tileLength     ,0,  Mathf.Sin(angle + radPerTile)*(offset + 1)*tileLength);
					vertices[currentVertex + 5]    =    new Vector3(Mathf.Cos(angle)*(offset + 1)*tileLength                 ,0,  Mathf.Sin(angle)*(offset + 1)*tileLength);

					currentVertex += 6;
				}
			}

			for(int j = 0; j < triangles.Length; j++)    //set the triangles
			{
				triangles[j] = j;
			}

			for (int k = 0; k < vertices.Length; ++k)
				normals [k] = Vector3.up;

			//create the mesh and apply vertices/triangles/UV
			Mesh disk = new Mesh();
			disk.vertices = vertices;
			disk.triangles = triangles;
			disk.normals = normals;
			disk.uv = UV;    //the UV doesnt need to be set

			return disk;
		}


		// from http://wiki.unity3d.com/index.php/ProceduralPrimitives#C.23_-_Tube
		public static Mesh CreateCylider(float radius, float height = 1, int nbSides = 24)
		{
			// Outter shell is at radius1 + radius2 / 2, inner shell at radius1 - radius2 / 2
			float bottomRadius = radius;
			float topRadius = radius;

			// vertices & uvs

			Vector3[] vertices = new Vector3[nbSides * 2 + 2];
			Vector3[] normals = new Vector3[vertices.Length];
			Vector2[] uvs = new Vector2[vertices.Length];
			float _2pi = Mathf.PI * 2f;

			// Sides (out)
			int vert = 0;
			for ( int vi = 0; vi <= nbSides; vi++ ) {
				int k = (vi == nbSides) ? 0 : vi;
				float t = (float)(vi) / nbSides;

				float r1 = (float)(k) / nbSides * _2pi;
				float cos = Mathf.Cos(r1);
				float sin = Mathf.Sin(r1);

				vertices[vert] = new Vector3(cos * topRadius, height, sin * topRadius);
				vertices[vert + 1] = new Vector3(cos * bottomRadius, 0, sin * bottomRadius);
				uvs [vert] = new Vector2 (t, 0f);
				uvs [vert + 1] = new Vector2 (t, 1f);
				vert+=2;
			}

			for (int vi = 0; vi < vertices.Length; ++vi)
				normals [vi] = new Vector3 (vertices [vi].x, 0, vertices [vi].z).normalized;

			//Triangles
			int nbFace = nbSides;
			int nbTriangles = nbFace * 2;
			int nbIndexes = nbTriangles * 3;
			int[] triangles = new int[nbIndexes];

			// Sides (out)
			int ti = 0;
			for ( int vi = 0; vi < nbSides; vi++ ) {
				int current = 2*vi;
				int next = 2*vi+2;

				triangles[ ti++ ] = current;
				triangles[ ti++ ] = next;
				triangles[ ti++ ] = next + 1;

				triangles[ ti++ ] = current;
				triangles[ ti++ ] = next + 1;
				triangles[ ti++ ] = current + 1;
			}



			Mesh cylinder = new Mesh();
			cylinder.vertices = vertices;
			cylinder.normals = normals;
			cylinder.uv = uvs;
			cylinder.triangles = triangles;

			cylinder.RecalculateBounds();
			cylinder.Optimize();

			return cylinder;
		}


	}

} // end namespace 