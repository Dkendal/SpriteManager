// Copyright (C) 2013 Dylan Kendal
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without 
// limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteArtist : MonoBehaviour
{
    public Dictionary<string, int[]> Animations;
    public SpriteManager Manager;

    //Use this for initialization
    public Sprite MySprite;
    public Rect StaticSprite;

    private void Start()
    {
        ResetStaticSprite();
        //UVAnimation anim = new UVAnimation();
        //anim.name = "test";
        //anim.BuildUVAnim(0, 3, 1, manager);
        //sprite.AddAnimation(anim);
    }

    public void ResetStaticSprite()
    {
        if (Manager == null) return;
        if (MySprite != null)
        {
            Manager.RemoveSprite(MySprite);
        }
        MySprite = Manager.AddSprite(gameObject, (int) StaticSprite.width, (int) StaticSprite.height,
                                     (int) StaticSprite.xMin, (int) StaticSprite.yMax, (int) StaticSprite.width,
                                     (int) StaticSprite.height, false);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}