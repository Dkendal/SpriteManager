== SpriteManager!
=== Disclaimer
I, dkendal, take no credit for the creation of SpriteManager; I'm simply creating the GitHub repo with the permission of Brady


=== Terms of Use
This code is provided for all to use on one condition: that the notice at the top of each script is kept intact and unmodified, and that if you make any improvements to the code, that you share them with the Unity community so everyone can benefit from them (please post to this thread). This revision has not yet been thoroughly tested for stability, but the code is pretty simple and it should be pretty stable.


== Overview
Drawing lots of simple, independently-moving sprites for a 2D game can be performance prohibitive in Unity iPhone because the engine was designed with 3D in mind. For each object that has its own transform, another draw call is normally required. The significant overhead of a draw call quickly adds up and will cause framerate problems with only a modest number of objects on-screen. To address this, my SpriteManager class builds a single mesh containing the sprite "quads" to be displayed, and then "manually" transforms the vertices of these quads at runtime to create the appearance of multiple, independently moving objects - all in a single draw call! This dramatically increases the number of independently moving objects allowed on-screen at a time while maintaining a decent framerate.
While these classes were designed as a solution to performance limitations on the iPhone, they should work perfectly well in reducing draw calls using regular Unity as well.


== Usage

1. Create an empty GameObject (or you may use any other GameObject so long as it is located at the origin (0,0,0) with no rotations or scaling) and attach the SpriteManager or LinkedSpriteManager script to it. (NOTE: It is vital that the object containing the SpriteManager script be at the origin and have no rotations or scaling or else the sprites will be drawn out of alignment with the positions of the GameObjects they are intended to represent! This gets forced in the Awake() method of SpriteManager so that you don't have to worry about it in the editor. But do not relocate the object containing SpriteManager at run-time unless you have a very good reason for doing so!) Fill in the allocBlockSize and material values in the Unity editor. The SpriteManager is now ready to use.
2. To use it, create GameObjects which you want to represent using sprites at run-time. Add a script to each of these objects that contains a reference to the instance of the SpriteManager script you created in step 1.
3. In Start() of each such GameObject, place code calling the appropriate initialization routines of the SpriteManager object to add the sprite you want to represent this GameObject to the SpriteManager. Depending on the animation techniques used, you may also need to add code to Update() to manually inform the SpriteManager of changes you have made to the sprite at run-time. (In a later revision, all the necessary update calls could be made automatically to the SpriteManager through the Sprite class's own property accessors.)
