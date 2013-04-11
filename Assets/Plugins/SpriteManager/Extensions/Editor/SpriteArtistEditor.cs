using System;
using System.Linq;
using SpriteEditor;
using UnityEditor;
using UnityEngine;

namespace SpriteEditor
{
    [Serializable]
    public class Animation
    {
        public Animation(string name, params int[] sequence)
        {
            Name = name;

            Frames = sequence;
        }

        public int[] Frames { get; private set; }

        public string Name { get; private set; }
    }

    [Serializable]
    public class SequenceAnimation : Animation
    {
        public readonly int EndIndex;
        public readonly int StartIndex;

        public SequenceAnimation(int startIndex, int endIndex, string name)
            : base(name, Enumerable.Range(startIndex, endIndex).ToArray())
        {
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
        }
    }
}

[CustomEditor(typeof (SpriteArtist))]
public class SpriteArtistEditor : Editor
{
    public SequenceAnimation[] SeqAnims;
    private AtlasManager _atlasManager;
    private int _currSeqIndex = 0;
    private SpriteManager _manager;
    private SpriteArtist _myTarget;
    private Vector2 _scrollPos;
    private SerializedProperty _spriteManagerProp;
    private SerializedProperty _staticSpriteProp;
    private Texture2D _tex;

    public override void OnInspectorGUI()
    {
        bool isDirty;
        serializedObject.Update();
        
        InitProperties(out _myTarget, out _spriteManagerProp, out _staticSpriteProp);

        EditorGUILayout.PropertyField(_spriteManagerProp, new GUIContent("Sprite Manager"));

        if (_myTarget.Manager != null)
        {
            _atlasManager = _manager.GetAtlasManager();
            _tex = _myTarget.Manager.material.mainTexture as Texture2D;
        }

        //Sequence groups
        EditorGUILayout.LabelField("Sequence Animations");
        name = EditorGUILayout.TextField("Name", name);

        isDirty = StaticSpriteGUI();

        //SequenceEditGUI();

        serializedObject.ApplyModifiedProperties();
        if (isDirty)
        {
            _myTarget.ResetStaticSprite();
            
        }
    }

    /// <summary>
    /// Draws all frames in frameIndices at pos
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="frameIndices"> Atlas entry indexs</param>
    private void DrawSpriteFrames(Rect pos, params int[] frameIndices)
    {
        var view = new Rect(pos);
        const int padding = 2;

        float cellWidth = pos.width + padding;

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        {
            // hack to make the scroll view work
            // gui texture don't count as content
            EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(frameIndices.Length*cellWidth));
            {
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndHorizontal();

            foreach (var frame in frameIndices)
            {

                if (_atlasManager.Frames.Length > frame)
                {
                    var r = _atlasManager.Frames[frame];

                    var spriteRect = _manager.PixelCoordToUVCoord(r);

                    GUI.DrawTextureWithTexCoords(pos, _tex, spriteRect);
                }

                pos.x += cellWidth;
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void InitProperties(out SpriteArtist artist, out SerializedProperty manager, out SerializedProperty staticSprite)
    {
        // init properties
        artist = (SpriteArtist)target;
        manager = serializedObject.FindProperty("Manager");
        staticSprite = serializedObject.FindProperty("StaticSprite");

        
        if (SeqAnims == null)
        {
            SeqAnims = new SequenceAnimation[1];
        }
    }

    private void SequenceEditGUI()
    {
        int startI = 0, endI = 0;

        int totalFrames = _myTarget.Manager.GetAtlasManager().Frames.Length;

        if (SeqAnims[_currSeqIndex] != null)
        {
            startI = SeqAnims[_currSeqIndex].StartIndex;
            endI = SeqAnims[_currSeqIndex].EndIndex;
        }

        EditorGUILayout.BeginHorizontal();
        {
            // prevent the start index from exceeding the end
            EditorGUILayout.LabelField("Start Index", GUILayout.Width(80));
            startI = Mathf.Min(EditorGUILayout.IntField(startI, GUILayout.Width(30)), endI);

            GUILayout.Space(30);
            EditorGUILayout.LabelField("End Index", GUILayout.Width(80));
            endI = Mathf.Min(EditorGUILayout.IntField(endI, GUILayout.Width(30)), totalFrames);
        }
        EditorGUILayout.EndHorizontal();

        SeqAnims[_currSeqIndex] = new SequenceAnimation(startI, endI, name);

        EditorGUILayout.BeginHorizontal();
        {
            DrawSpriteFrames(new Rect(0, 10, 30, 30), SeqAnims[_currSeqIndex].Frames);
        }
        EditorGUILayout.EndHorizontal();
    }

    private bool StaticSpriteGUI()
    {
        var size = new Rect(0, 0, 50, 50);
        var r = _staticSpriteProp.rectValue;

        if (_atlasManager == null ) return false;

        var index = _atlasManager.Frames.ToList().IndexOf(r);
        var maxIndex = _atlasManager.Frames.Length;
        var oldIndex = index;

        EditorGUILayout.BeginHorizontal();
        {
            if (maxIndex <= 0)
            {
                EditorGUILayout.LabelField("Problem with atlas.");
            }
            else
            {
                index = EditorGUILayout.IntField("Static Sprite", index);
                index = Mathf.Clamp(index, 0, _atlasManager.Frames.Length - 1);

                DrawSpriteFrames(size, index);
                _staticSpriteProp.rectValue = _atlasManager.Frames[index];

            }
        

        }
        EditorGUILayout.EndHorizontal();
        // sprite changed


        return index != oldIndex;
    }
}