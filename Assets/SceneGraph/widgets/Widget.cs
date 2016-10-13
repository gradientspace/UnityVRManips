using System;
using UnityEngine;

namespace f3
{
	public interface Widget
	{
		bool BeginCapture(ITransformable target, Ray worldRay, UIRayHit hit);
		bool UpdateCapture(ITransformable target, Ray worldRay);
	}
}

