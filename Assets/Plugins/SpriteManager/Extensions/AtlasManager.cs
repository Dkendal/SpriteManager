using MiniJSON;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class AtlasManager : MonoBehaviour
{
    public string atlasData;
    private Rect[] frames;

    public Rect this[int i]
    {
        get
        {
            return frames[i];
        }
    }

    // Use this for initialization
    private void Awake()
    {
        var manager = gameObject.GetComponent<SpriteManager>();
        if (atlasData == null)
        {
            // get the file path to the text image, then replace the file extension with txt, assuming that
            // the atlas and its data file have the same name
            atlasData = AssetDatabase.GetAssetPath(manager.material.mainTexture);
            int index = atlasData.LastIndexOf('.');
            atlasData = atlasData.Substring(0, index) + @".txt";
        }
        try
        {
            ReadFile(atlasData);
        }
        catch (Exception)
        {
            throw new Exception("Could not find " + atlasData + ", ");
        }
    }

    private void ReadFile(string uri)
    {
        string jsonString;

        jsonString = File.ReadAllText(uri);
        Dictionary<string, object> frameDict = ((Dictionary<string, object>)Json.Deserialize(jsonString))["frames"] as Dictionary<string, object>;
        frames = new Rect[frameDict.Count];

        int i = 0;
        foreach (Dictionary<string, object> item in frameDict.Values)
        {
            var dim = item["frame"] as Dictionary<string, object>;
            Rect rect = new Rect((long)dim["x"], (long)dim["y"], (long)dim["w"], (long)dim["h"]);
            frames[i++] = rect;
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}