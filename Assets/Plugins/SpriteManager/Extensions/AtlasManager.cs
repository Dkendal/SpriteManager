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
using MiniJSON;
using UnityEngine;

[ExecuteInEditMode]
public class AtlasManager : MonoBehaviour
{
    public TextAsset AtlasConfigFile;
    public Rect[] Frames { get; private set; }
    
    // Use this for initialization
    private void Awake()
    {
        BuildAtlasFromFile(AtlasConfigFile);
    }

    private void BuildAtlasFromFile(TextAsset asset)
    {
        if (asset == null) return;

        var jsonString = asset.text;
        var frameDict =
            ((Dictionary<string, object>) Json.Deserialize(jsonString))["frames"] as Dictionary<string, object>;
        Frames = new Rect[frameDict.Count];

        var i = 0;
        foreach (Dictionary<string, object> item in frameDict.Values)
        {
            var dim = item["frame"] as Dictionary<string, object>;
            var rect = new Rect((long) dim["x"], (long) dim["y"], (long) dim["w"], (long) dim["h"]);
            Frames[i++] = rect;
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}