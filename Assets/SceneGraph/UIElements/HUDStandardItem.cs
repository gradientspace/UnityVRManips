using System;
using UnityEngine;

namespace f3
{
	public abstract class HUDStandardItem : GameObjectSet, SceneUIElement
	{
		public HUDStandardItem ()
		{
		}

		// utility functions

		public Frame3 GetObjectFrame() {
			return MathUtil.GetGameObjectFrame (RootGameObject, CoordSpace.ObjectCoords);
		}
		public void SetObjectFrame(Frame3 value) {
			MathUtil.SetGameObjectFrame (RootGameObject, value, CoordSpace.ObjectCoords);
		}



		// abstract impl of SceneUIElement
		public abstract GameObject RootGameObject { get; }

		public virtual void Disconnect() {
			// standard is to do nothing
		}

		public virtual bool FindRayIntersection (UnityEngine.Ray ray, out UIRayHit hit)
		{
			hit = null;
			GameObjectRayHit hitg = null;
			if (FindGORayIntersection (ray, out hitg)) {
				if (hitg.hitGO != null) {
					hit = new UIRayHit (hitg, this);
					return true;
				}
			}
			return false;
		}

		virtual public bool BeginCapture (UnityEngine.Ray ray, UIRayHit hit)
		{
			return false;
		}

		virtual public bool UpdateCapture (UnityEngine.Ray ray)
		{
			return true;
		}

		virtual public bool EndCapture (UnityEngine.Ray ray)
		{
			return false;
		}

	}
}

