using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(InlineSpriteAsset))]
public class InlineSpriteAssetEditor : Editor
{
    InlineSpriteAsset inlineSpriteAsset;

    public void OnEnable()
    {
        inlineSpriteAsset = (InlineSpriteAsset)target;
    }
    private Vector2 ve2ScorllView;
    public override void OnInspectorGUI()
    {
       // EditorGUILayout.TextField("Texture:", inlineSpriteAsset.TextureSource.name);

        ve2ScorllView = GUILayout.BeginScrollView(ve2ScorllView);
        GUILayout.Label("UGUI Sprite Asset");
        if (inlineSpriteAsset.listSpriteInfor == null)
        {
            return;
        }

        for (int i = 0; i < inlineSpriteAsset.listSpriteInfor.Count; i++)
        {
            GUILayout.Label("\n");
            //     EditorGUILayout.ObjectField("", InlineSpriteAsset.listSpriteInfor[i].sprite, typeof(Sprite),false);
            //EditorGUILayout.IntField("ID:", inlineSpriteAsset.listSpriteInfor[i].ID);
            EditorGUILayout.TextField("name:", inlineSpriteAsset.listSpriteInfor[i].name);
            //   EditorGUILayout.Vector2Field("povit:", InlineSpriteAsset.listSpriteInfor[i].pivot);
             //     EditorGUILayout.RectField("rect:", inlineSpriteAsset.listSpriteInfor[i].rect);
            //    GUILayout.Label("\n");
        }
        GUILayout.EndScrollView();
    }
}