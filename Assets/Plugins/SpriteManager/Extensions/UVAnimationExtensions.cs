using UnityEngine;

public static class UVAnimationExtensions
{
    /// <summary>
    /// Adds support for atlas manager indexs
    /// 
    /// Assumes that SpriteManager manager has an attached AtlasManager
    /// </summary>
    public static void BuildUVAnim(this UVAnimation _this, int startIndex, int totalCells, float fps, SpriteManager manager)
    {
        AtlasManager aManager = manager.GetAtlasManager();

        Vector2[] temp = new Vector2[totalCells];

        for (int i = startIndex; i < totalCells; i++)
        {
            temp[i] = new Vector2(aManager[i].xMin, aManager[i].yMax);
            temp[i] = manager.PixelCoordToUVCoord(temp[i]);
        }

        _this.AppendAnim(temp);
        _this.framerate = fps;
    }
}

