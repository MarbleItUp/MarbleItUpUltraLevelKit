# Level Kit Tutorial Scenes
Included with the Marble It Up! Ultra Level Kit are several tutorial scenes intended to introduce the user to the basics of MIU level editing. Follow this document to learn how to create and export MIU levels!

After importing the level kit package into your Unity project, the tutorial scenes can be found in the project in `Assets/Scenes/Tutorials`. Open the scene "Object Placement" to get started.
<image of this location in the project>

In Unity, you can look around the 3D view by holding right click and moving the mouse. You can navigate around the 3D view by holding right click and using WASD to move horizontally and QE to move vertically. Hold Shift to move faster.

Before continuing, ensure that the MIU Level Kit window is opened by clicking `Marble It Up > Open Level Kit Window` in the Unity toolbar. Also ensure that the ProBuilder window is opened by clicking `Tools > ProBuilder > ProBuilder Window` in the Unity toolbar. If either of these are missing, your project has not been set up correctly!
<image of opening the MIU Level Kit window>

## Object Placement
<image of scene>
This scene features some level geometry, but with no way to complete or even play it.
In order to export a level from the level kit, it must have a Start Pad, an End Pad, a Level Bounds, and a Medal Times. All of these can be accessed from the MIU Level Kit tab. Click on an item to spawn it into the scene.
<image of the MIU Level Kit tab>

Once you have spawned the item, it will appear in the 3D viewport and in the scene hierarchy. Click on it in either place to select it. Once it is selected, you can drag it around the scene using the red, green, and blue movement axes.
<image of movement axes, images of moving it around>

If you do not see the movement axes, ensure that the Move Tool (press W) is currently active, and ensure that your ProBuilder mode is set to Object Selection.
<image of troubleshooting>

Try adding a Start Pad and End Pad into the scene, and place them like so:
<image of placed start pad and end pad>

You will have to rotate the Start Pad to make it face the right way. Select it, then activate the Rotate Tool (press E) and use the green wheel to spin it around. Hold Control to snap to the nearest 15° angle.
<image of rotating the green wheel>

Next, add a Level Bounds. It will automatically try to fit your level geometry, but you can adjust its position and scale by moving it and adjusting the Size field in its BoxCollider component.
<image of bounds with size field highlighted>

Finally, add a Medal Times. This is where you set your level's Silver, Gold, and Diamond times, as well as where you write your Author name and apply physics modifiers.
<image of medal times>

With all of these objects added, you should now be able to export your level. In the MIU Level Kit window, choose a location to save the level to, and then hit the Export button!
<image of level kit window>
The level can be loaded in the Level Tester. Launch it through Steam by hitting Play, then checking "Play Level Tester" instead of "Play Marble It Up! Ultra." You can then find your exported level file and load it via "Load Level."
<image of level loaded in tester>

You might notice that we still can't finish the level - powerups are needed here! Add a Super Jump and a Super Speed to the level, then hit the Export button again. As long as it is still loaded in the tester, it will detect the re-export and update live in-game.
<image of powerups added>

With these powerups added alongside the core level objects, we finally have an exported and completable level!

## Movers
<image of scene>
This scene features some moving platforms, but they are not correctly set up yet. Let's do that!

In order to make some geometry a mover, we need to select it, click on Add Component in the inspector, and add an ElevatorMover component. Let's add one to the platform named `Mover_Auto`.
<image of adding a component>

The ElevatorMover component allows us to define mover behavior on our platform. You can check what each field does by mousing over it.
<image of mover component>

For Mover_Auto, we need it to move 32 units in the Z direction. Set the Z value in the `Delta` field to 32. When the level is exported and loaded in-game, this platform will now pause for 2 seconds, take 4 seconds to move 32 units, then reverse back to the starting position.
<image of mover_auto and component>

Next, add an ElevatorMover component to the platform named `Mover_Touch`. Let's enable `Wait for Touched` and `Move First`, then tell it to move 16 units on the Y axis. When we export the level, this platform will behave like the first one, but will only start moving once the marble touches it. Feel free to edit the `Move Time` field to make this elevator faster.
<image of mover_touch and component>

Next, add an ElevatorMover component to the platform named `Mover_Gem`. We only want this mover to activate if the player has one or more gems. Enable `Move First` and tell it to move 12 units on the Y axis, but set the `Pause Time` to some obscenely high number.

Now, add a GemTrigger component to the platform. Set comparison mode to `Greater Than or Equal` and threshold to 1. Now, when the level is exported, this platform will only move once the gem is picked up!
<image of mover_gem and components>

Finally, add an ElevatorMover component to the platform named `Mover_Spline`. We want this one to follow the spline `Spline01`, so set its Mode to `Spline`. Drag and drop the Spline01 object from the hierarchy into the `Spline Go` field of the ElevatorMover. To adjust speed on a spline mover, use the Spline Speed field instead of the Move Time field. Enable `Wait for Touched`, then export the level - when touched, this mover will follow the designated spline!
<image of mover_spline and component>

## Sandbox
<image of scene>
This scene features a half completed level - it's up to you to use ProBuilder to put the finish it up.

Here are some techniques you can use to finish the stage:

### Geometry Duplication
Select some geometry, then press Control + D to duplicate it. Duplicate the platform underneath the start pad and move the new platform underneath the bumper in front of it.
<image of duplication>

### Material Application
We can apply materials to surfaces by entering Face Selection mode in ProBuilder. Level materials can be found in `Assets/Marble It Up Ultra/Level Materials`.

Navigate to `Level Materials/Physics` to find the material `surface.bounce`. Select the pink tiles of this 2x4 platform, then drag and drop `surface.bounce` onto the platform. This platform will now be bouncy!
<image of material application>

Other materials in the Physics folder will change surfaces to gravity surface, ice surface, and lava surface. Try them out!

### ProBuilder Basics
We can create new ProBuilder geometry by clicking "New Shape" in the ProBuilder window. Doing this will create an untextured cube. Move it between the two ends of this bridge framework.
<image of new cube in place>

We can use Vert, Edge, and Face selection to manipulate this geometry. Drag the top face down to make it 1 unit tall, then adjust one of the side faces so that it bridges the gap between the two platforms.
<image of bridged gap>

We can extrude the sides of the ramp to create trim by selecting those faces and clicking "Extrude Faces" in the ProBuilder window. To make sure this is set up correctly, click the [+] next to it and make sure "Extrude By" is set to "Face Normal."
<image of extruded geometry>

Finally, we can apply materials to the ramp the same way we applied the bounce material earlier. Don't worry too much about cleaning up the texturing - that will be covered later!
<image of textured ramp>

Use these techniques, add platforms, and add powerups to enable the marble to finish the stage. If you need help, unhide the object "ManualExample" for an example of what a completed level might look like!
