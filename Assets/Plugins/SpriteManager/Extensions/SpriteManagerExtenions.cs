using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class SpriteManagerExtensions
{
    public static AtlasManager GetAtlasManager(this SpriteManager _this)
    {
        return _this.gameObject.GetComponent<AtlasManager>() ?? _this.gameObject.AddComponent<AtlasManager>();
    }
}
