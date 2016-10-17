using System;
using UnityEngine;

namespace f3
{
	public class DebugUtil
	{
		protected DebugUtil ()
		{
		}


		static public GameObject EmitDebugSphere(string name, Vector3 position, float diameter, Color color, GameObject inCoords = null) {
			if (inCoords != null) {
				Transform curt = inCoords.transform;
				while (curt != null) {
					position = curt.TransformPoint (position);
					curt = curt.parent;
				}
			}

			GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			sphere.name = name;
			sphere.transform.position = position;
			sphere.transform.localScale = new Vector3(diameter,diameter,diameter);
			sphere.GetComponent<MeshRenderer> ().material.color = color;
			return sphere;
		}



		static public GameObject EmitDebugLine(string name, Vector3 start, Vector3 end, float diameter, Color color) {
			GameObject line = new GameObject ();
			line.name = name;
			line.transform.position = start;
			line.AddComponent<LineRenderer> ();
			LineRenderer lr = line.GetComponent<LineRenderer> ();
			//lr.material = new Material (Shader.Find ("Particles/Additive"));
			lr.material = new Material(Shader.Find("Unlit/Texture"));
			lr.SetColors (color, color);
			lr.SetWidth (diameter, diameter);
			lr.SetPosition (0, start);
			lr.SetPosition (1, end);
			return line;
		}



		static public void EmitDebugFrame(string name, Frame3 f, float fAxisLength, float diameter = 0.05f) {
			GameObject frame = new GameObject (name);
			GameObject x = EmitDebugLine (name+"_x", f.Origin, f.Origin + fAxisLength * f.X, diameter, Color.red);
			x.transform.parent = frame.transform;
			GameObject y = EmitDebugLine (name+"_y", f.Origin, f.Origin + fAxisLength * f.Y, diameter, Color.green);
			y.transform.parent = frame.transform;
			GameObject z = EmitDebugLine (name+"_z", f.Origin, f.Origin + fAxisLength * f.Z, diameter, Color.blue);
			z.transform.parent = frame.transform;
		}
	}
}

