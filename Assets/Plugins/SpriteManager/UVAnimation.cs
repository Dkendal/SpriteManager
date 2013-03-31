using UnityEngine;
//-----------------------------------------------------------------
// Describes a UV animation
//-----------------------------------------------------------------
//	NOTE: Currently, you should assign at least two frames to an
//	animation, or else you can expect problems!
//-----------------------------------------------------------------
public class UVAnimation
{
    protected Vector2[] frames;						// Array of UV coordinates (for quads) defining the frames of an animation

    // Animation state vars:
    protected int curFrame = 0;						// The current frame
    protected int stepDir = 1;						// The direction we're currently playing the animation (1=forwards (default), -1=backwards)
    protected int numLoops = 0;						// Number of times we've looped since last animation

    public string name;								// The name of the 
    public int loopCycles = 0;						// How many times to loop the animation (-1 loop infinitely)
    public bool loopReverse = false;				// Reverse the play direction when the end of the animation is reached? (if true, a loop iteration isn't counted until we return to the beginning)
    public float framerate;							// The rate in frames per second at which to play the animation


    // Resets all the animation state vars to ready the object
    // for playing anew:
    public void Reset()
    {
        curFrame = 0;
        stepDir = 1;
        numLoops = 0;
    }

    // Sets the stepDir to -1 and sets the current frame to the end
    // so that the animation plays in reverse
    public void PlayInReverse()
    {
        stepDir = -1;
        curFrame = frames.Length - 1;
    }

    // Stores the UV of the next frame in 'uv', returns false if
    // we've reached the end of the animation (this will never
    // happen if it is set to loop infinitely)
    public bool GetNextFrame(ref Vector2 uv)
    {
        // See if we can advance to the next frame:
        if ((curFrame + stepDir) >= frames.Length || (curFrame + stepDir) < 0)
        {
            // See if we need to loop (if we're reversing, we don't loop until we get back to the beginning):
            if (stepDir > 0 && loopReverse)
            {
                stepDir = -1;	// Reverse playback direction
                curFrame += stepDir;

                uv = frames[curFrame];
            }
            else
            {
                // See if we can loop:
                if (numLoops + 1 > loopCycles && loopCycles != -1)
                    return false;
                else
                {	// Loop the animation:
                    ++numLoops;

                    if (loopReverse)
                    {
                        stepDir *= -1;
                        curFrame += stepDir;
                    }
                    else
                        curFrame = 0;

                    uv = frames[curFrame];
                }
            }
        }
        else
        {
            curFrame += stepDir;
            uv = frames[curFrame];
        }

        return true;
    }

    // Constructs an array of UV coordinates based upon the info
    // supplied.
    //
    // start	-	The UV of the lower-left corner of the first
    //				cell
    // cellSize	-	width and height, in UV space, of each cell
    // cols		-	Number of columns in the grid
    // rows		-	Number of rows in the grid
    // totalCells-	Total number of cells in the grid (left-to-right,
    //				top-to-bottom ordering is assumed, just like reading
    //				English).
    // fps		-	Framerate (frames per second)
    public Vector2[] BuildUVAnim(Vector2 start, Vector2 cellSize, int cols, int rows, int totalCells, float fps)
    {
        int cellCount = 0;

        frames = new Vector2[totalCells];
        framerate = fps;

        frames[0] = start;

        for (int row = 0; row < rows; ++row)
        {
            for (int col = 0; col < cols && cellCount < totalCells; ++col)
            {
                frames[cellCount].x = start.x + cellSize.x * ((float)col);
                frames[cellCount].y = start.y - cellSize.y * ((float)row);

                ++cellCount;
            }
        }

        return frames;
    }

    // Assigns the specified array of UV coordinates to the
    // animation, replacing its current contents
    public void SetAnim(Vector2[] anim)
    {
        frames = anim;
    }

    // Appends the specified array of UV coordinates to the
    // existing animation
    public void AppendAnim(Vector2[] anim)
    {
        frames = frames ?? new Vector2[0];
        Vector2[] tempFrames = frames;
        frames = new Vector2[  + anim.Length];
        tempFrames.CopyTo(frames, 0);
        anim.CopyTo(frames, tempFrames.Length);
    }
}
