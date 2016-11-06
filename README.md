# UnityVRManips

3D manipulation is hard. Traditional mouse & keyboard CAD interfaces have converged on semi-standardized 3D widgets to provide with constrained 3D translate/rotate/scale. But VR is different. What does 2D mouse input mean in VR? And how will we do constrained 3D manipulation with true 3D input devices? 

The goal of this project is to (1) figure this out, and (2) provide an open reference implementation. 

Developed with Oculus Rift. Apache 2.0 License. Implemented in Unity ( *so even though this is code, you can run it without knowing anything about programming/compilers/etc. Just install the free personal version from https://store.unity.com/download?ref=personal* )

Questions? Contact Ryan Schmidt [@rms80](http://www.twitter.com/rms80) / [gradientspace](http://www.gradientspace.com)


## Usage

**NB: Currently only mouse input is supported. Still waiting for Oculus Touch...**

Open **TestScene1**, put on your headset, click Play (or ctrl+p). You will see a blue ground plane and some floating shapes to your left. 

When you move the mouse you will see a moving blue dot. Move the dot onto the cylinder and left-click. A cylinder will appear in the scene. 

The cylinder exists in a 3D scene that you can move as a whole. Alt+left-mouse rotates, alt+middle pans, and alt+right zooms (ie Maya hotkeys).

Click on the cylinder. A transform gizmo will appear. Mouse over the widget elements and left-click-drag to move the cylinder.

![3D manipulator](https://github.com/gradientspace/UnityVRManips/raw/master/doc/screenshot1.png "3D Manip")



## Manipulator Geometry

Each element in the 3D manipulator/gizmo/thingy is a separate mesh file, so they can be modified at will. You will find the .obj files in **/Assets/Resources/meshes**. I export these meshes from a .mix file which you can open in Autodesk Meshmixer. 

The colors in the meshes are ignored. The material setup is done in code, in TransformGizmo.cs. You can easily disable elements of the gizmo by commenting out the relevent lines in the TransformGizmo.Create() function.



## Scene Setup

The included TestScene1 is configured so that you can just press Play. 
However if you want to try to use the gizmo in your own code, you need to configure your scene as follows:
( *note: this list may be incomplete - if it doesn't work, get in touch* )

- A top-level GameObject named **X** for the Scene hierarchy (**X**=*SceneObjectManager* in test scene). 
    - Assign the **SceneController** script to this object
    - this creates *Scene*, *Cockpit*, *Cursor*, GameObjects on startup
 
- A second camera named **WidgetCamera**
    - is used by **CameraController** class
    - must be placed at same position as **Main Camera** in editor view (ie so cameras are coincident). 
    - This camera is used to render objects in the *WidgetOverlayLayer*, which are in their own depth layer. 
    - Cannot create this camera in code because then it lags one frame behind Main Camera [TODO] figure out why

- A User Rendering Layer named **3DWidgetOverlay**
    - access via Edit Menu -> Project Settings -> Tags and Layers, add 
    - used by *WidgetOverlayLayer* class to draw widgets in a separate depth layer

- [Optional] A *SceneLighting* GameObject with script **SceneLightingSetup** and **X** assigned as *Scene* variable
    - SceneLightingSetup will create some lights and parent this object under *Scene*
    - [TODO] create this GO in code

- [Optional] *GroundPlane* GameObject with script **GroundPlaneSetup** and **X** assigned as *Scene* variable
    - script will parent this object under *Scene*, and will include in WorldBounds object list that mouse cursor can slide across
    - will be xformed along with *Scene*, because of parenting
    - does not have to be a plane, can be any geometry



## Miscellany

Why "Manips"? That's what they're called in Maya.
