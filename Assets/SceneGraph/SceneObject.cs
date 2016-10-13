using System;
using UnityEngine;

namespace f3
{


	public enum CoordSpace {
		WorldCoords = 0,
		SceneCoords = 1,
		ObjectCoords = 2
	};

	public interface ITransformable
	{
		Frame3 GetLocalFrame(CoordSpace eSpace);
		void SetLocalFrame(Frame3 newFrame, CoordSpace eSpace);
	}




	public interface SceneObject
	{
		void SetScene(Scene s);
		Scene GetScene();

		void SetMaterial(Material m);
		bool FindRayIntersection(Ray ray, out SORayHit hit);
	}


	// should we just make scene object transformable??
	public interface TransformableSceneObject : SceneObject, ITransformable
	{
	}

}

