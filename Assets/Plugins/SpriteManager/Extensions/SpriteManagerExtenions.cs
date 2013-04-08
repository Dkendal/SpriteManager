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

using UnityEngine;

public static class SpriteManagerExtensions
{
    public static AtlasManager GetAtlasManager(this SpriteManager _this)
    {
        return _this.gameObject.GetComponent<AtlasManager>() ?? _this.gameObject.AddComponent<AtlasManager>();
    }

    public static Rect PixelCoordToUVCoord(this SpriteManager _this, Rect r)
    {
        Vector2 pos = _this.PixelCoordToUVCoord((int) r.xMin, (int) r.yMax);
        Vector2 size = _this.PixelSpaceToUVSpace((int) r.width, (int) r.height);
        return new Rect(pos.x, pos.y, size.x, size.y);
    }
}