using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SpriteArtist : MonoBehaviour {
    public int startIndex;
    public SpriteManager manager;
    public string[] AnimNames;
    public int[][] AnimFrames;

	//Use this for initialization
    Sprite sprite;

	void Start () {
        var r = manager.GetAtlasManager()[startIndex];
        Debug.Log(r);
        sprite = manager.AddSprite(gameObject, (int)r.width, (int)r.height, (int)r.xMin, (int)r.yMax, (int)r.width, (int)r.height, false);
        //UVAnimation anim = new UVAnimation();
        //anim.name = "test";
        //anim.BuildUVAnim(0, 3, 1, manager);
        //sprite.AddAnimation(anim);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
