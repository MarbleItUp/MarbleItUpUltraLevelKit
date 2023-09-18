# Level Kit Tutorial Scenes
Included with the Marble It Up! Ultra Level Kit are several tutorial scenes intended to introduce the user to the basics of MIU level editing. Follow this document to learn how to create and export MIU levels!

After importing the level kit package into your Unity project, the tutorial scenes can be found in the project in `Assets/Scenes/Tutorials`. Open the scene "Object Placement" to get started.
<p align="center"> <img width="251" height="131" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/962e9635-1c35-4b22-9bf8-3b199b85e719"> </p>

In Unity, you can look around the 3D view by holding right click and moving the mouse. You can navigate around the 3D view by holding right click and using WASD to move horizontally and QE to move vertically. Hold Shift to move faster.

Before continuing, ensure that the MIU Level Kit window is opened by clicking `Marble It Up > Open Level Kit Window` in the Unity toolbar. Also ensure that the ProBuilder window is opened by clicking `Tools > ProBuilder > ProBuilder Window` in the Unity toolbar. If either of these are missing, your project has not been set up correctly!

<p align="center"> <img width="730" height="254" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/53a9b6a0-7633-44fa-b708-2d0d1a1503c9"> </p>

## Object Placement
![image](https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/a1709067-53d9-4904-ab79-2d2bbae484d6)
This scene features some level geometry, but with no way to complete or even play it.
In order to export a level from the level kit, it must have a Start Pad, an End Pad, a Level Bounds, and a Medal Times. All of these can be accessed from the MIU Level Kit tab. Click on an item to spawn it into the scene.

<p align="center"> <img width="390" height="587" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/82f335d7-12cb-4fbe-a22c-c00d7228a9ad"> </p>

Once you have spawned the item, it will appear in the 3D viewport and in the scene hierarchy. Click on it in either place to select it. Once it is selected, you can drag it around the scene using the red, green, and blue movement axes.

<p align="center"> <img width="427" height="240" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/75b7845e-8e6f-4e39-9fce-5a4346276c08"> </p>

If you do not see the movement axes, ensure that the Move Tool (press W) is currently active, and ensure that your ProBuilder mode is set to Object Selection.

<p align="center"> <img width="549" height="149" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/551991a6-c87b-43f1-8078-aa8b2795aea5"> </p>


Try adding a Start Pad and End Pad into the scene, and place them like so:

<p align="center"> <img width="468" height="217" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/7a9c20d3-bd14-43bd-a5ed-ecf877414cb1"> </p>

You will have to rotate the Start Pad to make it face the right way. Select it, then activate the Rotate Tool (press E) and use the green wheel to spin it around. Hold Control to snap to the nearest 15° angle.

<p align="center"> <img width="427" height="240" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/75341056-22fb-4f55-94a9-16678f2a6e72"> </p>

Next, add a Level Bounds. It will automatically try to fit your level geometry, but you can adjust its position and scale by moving it and adjusting the Size field in its BoxCollider component.

<p align="center"> <img width="610" height="256" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/f6d42a60-37bd-49fc-b125-4def340ebd61"> </p>

Finally, add a Medal Times. This is where you set your level's Silver, Gold, and Diamond times, as well as where you write your Author name and apply physics modifiers.

<p align="center"> <img width="492" height="609" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/46fe58a2-eac4-439a-9929-07fabd3c0acc"> </p>

With all of these objects added, you should now be able to export your level. In the MIU Level Kit window, choose a location to save the level to, and then hit the Export button!

<p align="center"> <img width="390" height="189" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/6cf85c91-5f58-43e5-b309-f2a4e773f3da"> </p>

The level can be loaded in the Level Tester. Launch it through Steam by hitting Play, then checking `Play Level Tester` instead of `Play Marble It Up! Ultra`. You can then find your exported level file and load it via `Load File`.

<p align="center"> <img width="854" height="480" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/cc8b5b56-ab67-4736-8988-94c6ca4a56f5"> </p>

You might notice that we still can't finish the level - powerups are needed here! Add some to allow the marble to jump up the wall and across the gap, then hit the Export button again. As long as it is still loaded in the tester, it will detect the re-export and update live in-game.

With these powerups added alongside the core level objects, we finally have an exported and completable level!

## Movers
![image](https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/278b7c32-bcb8-4588-909f-bd40b314a4db)
This scene features some moving platforms, but they are not correctly set up yet. Let's do that!

In order to make some geometry a mover, we need to select it, click on Add Component in the inspector, and add an ElevatorMover component. Let's add one to the platform named `Mover_Auto`.

<p align="center"> <img width="301" height="153" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/f5522f15-3eb0-45fe-9a3f-e4d88e67f0a3"> </p>

The ElevatorMover component allows us to define mover behavior on our platform. You can check what each field does by mousing over it.

<p align="center"> <img width="479" height="352" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/a4f6144b-1ebc-4a21-92ef-cec4e2e8b10e"> </p>

For Mover_Auto, we need it to move 32 units in the Z direction. Set the Z value in the `Delta` field to 32. When the level is exported and loaded in-game, this platform will now pause for 2 seconds, take 4 seconds to move 32 units, then reverse back to the starting position.

<p align="center"> <img width="590" height="269" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/2d0f3df6-dc92-40c1-90d2-a04b9173d566"> </p>

Next, add an ElevatorMover component to the platform named `Mover_Touch`. Let's enable `Wait for Touched` and `Move First`, then tell it to move 16 units on the Y axis. When we export the level, this platform will behave like the first one, but will only start moving once the marble touches it. Feel free to edit the `Move Time` field to make this elevator faster.

<p align="center"> <img width="590" height="269" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/bdda6d76-71be-4697-9aa2-504b6749626d"> </p>

Next, add an ElevatorMover component to the platform named `Mover_Gem`. We only want this mover to activate if the player has one or more gems. Enable `Move First` and tell it to move 12 units on the Y axis, but set the `Pause Time` to some obscenely high number.

Now, add a GemTrigger component to the platform. Set comparison mode to `Greater Than or Equal` and threshold to 1. Now, when the level is exported, this platform will only move once the gem is picked up!

<p align="center"> <img width="590" height="269" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/16e0cf82-dbb9-4df4-a480-4e25fee92e42"> </p>

Finally, add an ElevatorMover component to the platform named `Mover_Spline`. We want this one to follow the spline `Spline01`, so set its Mode to `Spline`. Drag and drop the Spline01 object from the hierarchy into the `Spline Go` field of the ElevatorMover. To adjust speed on a spline mover, use the `Spline Speed` field instead of the `Move Time` field. Enable `Wait for Touched`, then export the level - when touched, this mover will follow the designated spline!

<p align="center"> <img width="590" height="269" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/637c4f17-b741-4439-810a-c23a8951be7b"> </p>

## Sandbox
![image](https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/a9b61ab9-4745-4d0c-8c1e-419c4c9c18a2)
This scene features a half completed level - it's up to you to use ProBuilder to finish it up.

Here are some techniques you can use to finish the stage:

### Duplication
Select some geometry, then press Control + D to duplicate it. Duplicate the platform underneath the start pad and move the new platform underneath the bumper in front of it.

<p align="center"> <img width="427" height="240" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/2a46338f-ac64-41e2-9cea-b11a65aaf745"> </p>

You can also duplicate other objects, such as powerups - this can save you time when placing them in your level.

### Material Application
We can apply materials to surfaces by entering Face Selection mode in ProBuilder. Level materials can be found in `Assets/Marble It Up Ultra/Level Materials`.

Navigate to `Level Materials/Physics` to find the material `surface.bounce`. Select the pink tiles of this 2x4 platform, then drag and drop `surface.bounce` onto the platform. This platform will now be bouncy!

<p align="center"> <img width="427" height="240" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/1d892f1a-5f4e-4755-91ff-7ca55d58b2ac"> </p>

Other materials in the Physics folder will change surfaces to gravity surface, ice surface, and lava surface. Try them out!

### ProBuilder Basics
We can create new ProBuilder geometry by clicking `New Shape` in the ProBuilder window. Doing this will create an untextured cube. Move it between the two ends of this bridge framework.

<p align="center"> <img width="401" height="238" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/0783879b-1ff4-4c21-a476-7d4b47d5e3ad"> </p>

We can use Vert, Edge, and Face Selection to manipulate this geometry. Drag the top face down to make it 1 unit tall, then adjust one of the side faces so that it bridges the gap between the two platforms.

<p align="center"> <img width="427" height="240" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/8fe04acb-2144-4fed-8c4f-3805994f12f5"> </p>

We can extrude the sides of the ramp to create trim by selecting those faces and clicking `Extrude Faces` in the ProBuilder window. To make sure this is set up correctly, click the [+] next to it and make sure `Extrude By` is set to `Face Normal`.

You can also extrude geometry by holding Shift and dragging a face along an axis. This will allow you to quickly build out complex platforms.

<p align="center"> <img width="427" height="240" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/6f31560f-c52d-4bab-8610-6ddfeda86619"> </p>

Finally, we can apply materials to the ramp the same way we applied the bounce material earlier. Don't worry too much about cleaning up the texturing - that will be covered later!

<p align="center"> <img width="400" height="239" src="https://github.com/MarbleItUp/MarbleItUpUltraLevelKit/assets/64559457/82cd6154-4c31-4446-b577-b6482817c88f"> </p>

### Best Practices

All level objects, except for the Main Camera, are split across four "folders" in the scene hierarchy. Each of these folders is marked by an empty GameObject, placed at position `0, 0, 0`. To place a level object in the relevant "folder," simply drag and drop it ontop of the "folder" in the scene hierarchy.

| "Folder" | Purpose |
| :--- | :--- |
| Static | Level geometry and lighting that will not move during gameplay. For each object in here, ensure that the `Static` field is checked in the inspector, or lighting and reflections may not render correctly. |
| Gameplay | Powerups and core level elements, such as gems and the end pad. |
| Dynamic | Movers and other dynamic objects, such as signs and MTAs. |
| Skybox | Decorations, such as clouds, stars, and particle effects. |


When your level is ready for export, make sure the Main Camera is in a good spot! This is the view of your level that will be displayed on the level select, and (by default) in the Steam Workshop.

For an easy way to adjust this, simply move around in the viewport to get a good view of your level, then select the Main Camera and press Control + Shift + F to move the Main Camera to the viewport position!

Use these techniques, add platforms, and add powerups to enable the marble to finish the stage. If you need help, unhide the object "ManualExample" for an example of what a completed level might look like!
