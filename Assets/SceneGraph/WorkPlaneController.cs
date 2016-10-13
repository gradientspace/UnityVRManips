using System;
using UnityEngine;

namespace f3
{
	public class WorkPlaneController
	{
		static WorkPlaneController s_singleton;
		public static WorkPlaneController Singleton { 
			get { 
				if ( s_singleton == null )
					s_singleton = new WorkPlaneController();
				return s_singleton;
			}
		}


		protected WorkPlaneController ()
		{
		}

		public Vector3 CurrentCursorPosWorld {
			get; set; 
		}

		public Vector3 CurrentCursorRaySourceWorld {
			get; set; 
		}


	}
}

