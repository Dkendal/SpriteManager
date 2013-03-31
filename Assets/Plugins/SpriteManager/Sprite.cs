//-----------------------------------------------------------------
//  Sprite (part of SpriteManager) v0.64 (21-10-2012)
//  Copyright 2012 Brady Wright and Above and Beyond Software
//  All rights reserved
//-----------------------------------------------------------------
// A class to allow the drawing of multiple "quads" as part of a
// single aggregated mesh so as to achieve multiple, independently
// moving objects using a single draw call.
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------
// Describes a sprite
//-----------------------------------------------------------------
public class Sprite
{
    protected float m_width;					// Width and Height of the sprite in worldspace units
    protected float m_height;
    protected Vector2 m_lowerLeftUV;			// UV coordinate for the upper-left corner of the sprite
    protected Vector2 m_UVDimensions;			// Distance from the upper-left UV to place the other UVs
    protected GameObject m_client;				// Reference to the client GameObject
    protected SpriteManager m_manager;			// Reference to the sprite manager in which this sprite resides
    protected bool m_billboarded = false;		// Is the sprite to be billboarded?
    public bool m_hidden___DoNotAccessExternally = false;	// Indicates whether this sprite is currently hidden (has to be public because C# has no "friend" feature, just don't access directly from outside)

    protected Vector3[] meshVerts;				// Pointer to the array of vertices in the mesh
    protected Vector2[] UVs;					// Pointer to the array of UVs in the mesh
    protected Vector3[] normals;				// Pointer to the array of normals in the mesh

    public Transform clientTransform;			// Transform of the client GameObject
    public Vector3 offset = new Vector3();		// Offset of sprite from center of client GameObject
    public Color color;							// The color to be used by all four vertices

    public int index;							// Index of this sprite in its SpriteManager's list
    public int drawLayer;						// The draw layer indicating the order in which this sprite should be rendered relative to other sprites

    public Vector3 v1 = new Vector3();			// The sprite's vertices in local space
    public Vector3 v2 = new Vector3();
    public Vector3 v3 = new Vector3();
    public Vector3 v4 = new Vector3();

    public int mv1;							// Indices of the associated vertices in the actual mesh (this just provides a quicker way for the SpriteManager to get straight to the right vertices in the vertex array)
    public int mv2;
    public int mv3;
    public int mv4;

    public int uv1;							// Indices of the associated UVs in the mesh
    public int uv2;
    public int uv3;
    public int uv4;

    public int cv1;							// Indices of the associated color values in the mesh
    public int cv2;
    public int cv3;
    public int cv4;

    public int nv1;							// Indices of the associated normal values in the mesh
    public int nv2;
    public int nv3;
    public int nv4;

    // Animation-related vars and types:
    public delegate void AnimCompleteDelegate();		// Definition of delegate to be called upon animation completion

    protected ArrayList animations = new ArrayList();	// Array of available animations
    protected UVAnimation curAnim = null;				// The current animation
    protected AnimCompleteDelegate animCompleteDelegate = null;	// Delegate to be called upon animation completion
    protected float timeSinceLastFrame = 0;				// The total time since our last animation frame change
    protected float timeBetweenAnimFrames;				// The amount of time we want to pass before moving to the next frame of animation
    protected int framesToAdvance;						// (working) The number of animation frames to advance given the time elapsed

    ~Sprite()
    {
    }


    public Sprite()
    {
        m_width = 0;
        m_height = 0;
        m_client = null;
        m_manager = null;
        clientTransform = null;
        index = 0;
        drawLayer = 0;
        color = Color.white;

        offset = Vector3.zero;
    }

    public SpriteManager manager
    {
        get { return m_manager; }
        set { m_manager = value; }
    }

    public GameObject client
    {
        get { return m_client; }
        set
        {
            m_client = value;
            if (m_client != null)
                clientTransform = m_client.transform;
            else
                clientTransform = null;
        }
    }

    public Vector2 lowerLeftUV
    {
        get { return m_lowerLeftUV; }
        set
        {
            m_lowerLeftUV = value;
            m_manager.UpdateUV(this);
        }
    }

    public Vector2 uvDimensions
    {
        get { return m_UVDimensions; }
        set
        {
            m_UVDimensions = value;
            m_manager.UpdateUV(this);
        }
    }

    public float width
    {
        get { return m_width; }
    }

    public float height
    {
        get { return m_height; }
    }

    public bool billboarded
    {
        get { return m_billboarded; }
        set
        {
            m_billboarded = value;
        }
    }

    public bool hidden
    {
        get { return m_hidden___DoNotAccessExternally; }
        set
        {
            // No need to do anything if we're
            // already in this state:
            if (value == m_hidden___DoNotAccessExternally)
                return;

            if (value)
                m_manager.HideSprite(this);
            else
                m_manager.ShowSprite(this);
        }
    }

    // Resets all sprite values to defaults for reuse:
    public void Clear()
    {
        client = null;
        billboarded = false;
        hidden = false;
        SetColor(Color.white);
        offset = Vector3.zero;

        PauseAnim();
        animations.Clear();
        curAnim = null;
        animCompleteDelegate = null;
    }

    // Does the same as assigning the drawLayer value, except that
    // SortDrawingOrder() is called automatically.
    // The draw layer indicates the order in which this sprite should be 
    // rendered relative to other sprites. Higher values result in a later
    // drawing order relative to sprites with lower values:
    public void SetDrawLayer(int v)
    {
        drawLayer = v;
        m_manager.SortDrawingOrder();
    }

    // Sets the physical dimensions of the sprite in the XY plane:
    public void SetSizeXY(float width, float height)
    {
        m_width = width;
        m_height = height;
        v1 = offset + new Vector3(-m_width / 2, m_height / 2, 0);	// Upper-left
        v2 = offset + new Vector3(-m_width / 2, -m_height / 2, 0);	// Lower-left
        v3 = offset + new Vector3(m_width / 2, -m_height / 2, 0);	// Lower-right
        v4 = offset + new Vector3(m_width / 2, m_height / 2, 0);	// Upper-right

        Transform();
    }

    // Sets the physical dimensions of the sprite in the XZ plane:
    public void SetSizeXZ(float width, float height)
    {
        m_width = width;
        m_height = height;
        v1 = offset + new Vector3(-m_width / 2, 0, m_height / 2);	// Upper-left
        v2 = offset + new Vector3(-m_width / 2, 0, -m_height / 2);	// Lower-left
        v3 = offset + new Vector3(m_width / 2, 0, -m_height / 2);	// Lower-right
        v4 = offset + new Vector3(m_width / 2, 0, m_height / 2);	// Upper-right

        Transform();
    }

    // Sets the physical dimensions of the sprite in the YZ plane:
    public void SetSizeYZ(float width, float height)
    {
        m_width = width;
        m_height = height;
        v1 = offset + new Vector3(0, m_height / 2, -m_width / 2);	// Upper-left
        v2 = offset + new Vector3(0, -m_height / 2, -m_width / 2);	// Lower-left
        v3 = offset + new Vector3(0, -m_height / 2, m_width / 2);	// Lower-right
        v4 = offset + new Vector3(0, m_height / 2, m_width / 2);	// Upper-right

        Transform();
    }

    // Sets the vertex and UV buffers
    public void SetBuffers(Vector3[] v, Vector2[] uv)
    {
        meshVerts = v;
        UVs = uv;
    }

    // Applies the transform of the client GameObject and stores
    // the results in the associated vertices of the overall mesh:
    public void Transform()
    {
        meshVerts[mv1] = clientTransform.TransformPoint(v1);
        meshVerts[mv2] = clientTransform.TransformPoint(v2);
        meshVerts[mv3] = clientTransform.TransformPoint(v3);
        meshVerts[mv4] = clientTransform.TransformPoint(v4);

        m_manager.UpdatePositions();
    }

    // Applies the transform of the client GameObject and stores
    // the results in the associated vertices of the overall mesh:
    public void TransformBillboarded(Transform t)
    {
        Vector3 pos = clientTransform.position;

        meshVerts[mv1] = pos + t.InverseTransformDirection(v1);
        meshVerts[mv2] = pos + t.InverseTransformDirection(v2);
        meshVerts[mv3] = pos + t.InverseTransformDirection(v3);
        meshVerts[mv4] = pos + t.InverseTransformDirection(v4);

        m_manager.UpdatePositions();
    }

    // Sets the specified color and automatically notifies the
    // SpriteManager to update the colors:
    public void SetColor(Color c)
    {
        color = c;
        m_manager.UpdateColors(this);
    }

    //-----------------------------------------------------------------
    // Animation-related routines:
    //-----------------------------------------------------------------

    // Sets the delegate to be called upon animation completion:
    public void SetAnimCompleteDelegate(AnimCompleteDelegate del)
    {
        animCompleteDelegate = del;
    }

    // Adds an animation to the sprite
    public void AddAnimation(UVAnimation anim)
    {
        animations.Add(anim);
    }

    // Steps to the next frame of sprite animation
    public bool StepAnim(float time)
    {
        if (curAnim == null)
            return false;

        timeSinceLastFrame += time;

        framesToAdvance = (int)(timeSinceLastFrame / timeBetweenAnimFrames);

        // If there's nothing to do, return:
        if (framesToAdvance < 1)
            return true;

        while (framesToAdvance > 0)
        {
            if (curAnim.GetNextFrame(ref m_lowerLeftUV))
                --framesToAdvance;
            else
            {
                // We reached the end of our animation
                if (animCompleteDelegate != null)
                    animCompleteDelegate();

                m_manager.UpdateUV(this);

                return false;
            }
        }

        m_manager.UpdateUV(this);
        timeSinceLastFrame = 0;

        return true;
    }

    // Starts playing the specified animation
    // (Note: this doesn't resume from a pause,
    // it completely restarts the animation. To
    // unpause, use UnpauseAnim):
    public void PlayAnim(UVAnimation anim)
    {
        // First stop any currently playing animation:
        m_manager.StopAnimation(this);

        curAnim = anim;
        curAnim.Reset();
        timeBetweenAnimFrames = 1f / anim.framerate;
        timeSinceLastFrame = timeBetweenAnimFrames;
        StepAnim(0);

        m_manager.AnimateSprite(this);
    }

    // Starts playing the specified animation:
    public void PlayAnim(string name)
    {
        for (int i = 0; i < animations.Count; ++i)
        {
            if (((UVAnimation)animations[i]).name == name)
                PlayAnim((UVAnimation)animations[i]);
        }
    }

    // Like PlayAnim but plays in reverse:
    public void PlayAnimInReverse(UVAnimation anim)
    {
        // First stop any currently playing animation:
        m_manager.StopAnimation(this);

        curAnim = anim;
        curAnim.Reset();
        curAnim.PlayInReverse();
        timeBetweenAnimFrames = 1f / anim.framerate;
        timeSinceLastFrame = timeBetweenAnimFrames;
        StepAnim(0);

        m_manager.AnimateSprite(this);
    }

    // Starts playing the specified animation in reverse:
    public void PlayAnimInReverse(string name)
    {
        for (int i = 0; i < animations.Count; ++i)
        {
            if (((UVAnimation)animations[i]).name == name)
            {
                ((UVAnimation)animations[i]).PlayInReverse();
                PlayAnimInReverse((UVAnimation)animations[i]);
            }
        }
    }

    // Pauses the currently-playing animation:
    public void PauseAnim()
    {
        m_manager.StopAnimation(this);
    }

    // Unpauses the currently-playing animation:
    public void UnpauseAnim()
    {
        if (curAnim == null) return;

        m_manager.AnimateSprite(this);
    }
}


// Compares drawing layers of sprites
public class SpriteDrawLayerComparer : IComparer
{
    static Sprite s1;
    static Sprite s2;

    int IComparer.Compare(object a, object b)
    {
        s1 = (Sprite)a;
        s2 = (Sprite)b;

        if (s1.drawLayer > s2.drawLayer)
            return 1;
        else if (s1.drawLayer < s2.drawLayer)
            return -1;
        else
            return 0;
    }
}