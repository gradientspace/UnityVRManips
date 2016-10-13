using System;
using UnityEngine;

namespace f3
{
	public interface ISpaceConversion
	{
		Vector3 WorldToSceneP(Vector3 worldPoint);
		Vector3 SceneToWorldP(Vector3 scenePoint);

		Vector3 WorldToSceneV(Vector3 worldVector);
		Vector3 SceneToWorldV(Vector3 sceneVector);

		Frame3 WorldToSceneF(Frame3 worldFrame);
		Frame3 SceneToWorldF(Frame3 sceneFrame);
	}
}

