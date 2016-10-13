using System;
using UnityEngine;

namespace f3
{
	public interface SceneUIElement
	{
		GameObject RootGameObject{ get; }

		void Disconnect();

		bool FindRayIntersection(Ray ray, out UIRayHit hit);

		bool BeginCapture(Ray ray, UIRayHit hit);
		bool UpdateCapture(Ray ray);
		bool EndCapture(Ray ray);
	}
}

