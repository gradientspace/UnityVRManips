# UnityVRManips



Scene Setup:

- Need a top-level GameObject named X for the Scene hierarchy (X=SceneObjectManager in test scene).
  Assign the SceneController script to this object
    - this creates Scene, Cockpit, Cursor, GameObjects on startup
  
- SceneLighting GameObject with script SceneLightingSetup and X assigned as Scene variable
    - SceneLightingSetup will create some lights and parent this object under Scene
	- can't we create this GO in code??

- GroundPlane GameObject with script GroundPlaneSetup and X assigned as Scene variable
    - script will parent this object under Scene, and will include in WorldBounds object list that mouse cursor can slide across
	- will be xformed along with Scene, because of parenting
	- does not have to be a plane, can be any geometry
	- is this necessary?
	
- CameraController requires a second camera named WidgetCamera
    - must be placed at same position as Main Camera in editor view (ie so cameras are coincident). 
	- This camera is used to render objects in the WidgetOverlayLayer, which are in their own
      depth layer. 
	- Cannot create this camera in code because then it lags one frame behind Main Camera (why???)
  
