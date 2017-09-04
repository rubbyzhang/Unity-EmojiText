using System.Collections;
using System.Collections.Generic;
using System.IO;
using Celf;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites ;

public class AtlasTest : MonoBehaviour
{
    public Sprite image1;
    public Sprite image2;
    public Image image3;


    void Awake()
    {
        //        PrintImageInfo(image1);
        //        PrintImageInfo(image2);

        image3.sprite = AtlasManager.Instance.GetSprite("ui/atlas/common/scope");

        PrintImageInfo(image3.sprite);

        Vector4 outer = DataUtility.GetOuterUV(image3.overrideSprite);
        Vector4 inter = DataUtility.GetInnerUV(image3.overrideSprite);
        Debug.Log("_____________________outer:" + outer);
        Debug.Log("_____________________inter:" + inter);

        Debug.Log("____________________" + image3.mainTexture.name);
    }

    void PrintImageInfo(Sprite image)
    {
        Debug.Log("__________-------------------------------__image name" + image.name);
        Debug.Log("____________image rect" + image.rect);
        Debug.Log("____________image texture width" + image.texture.width);
        Debug.Log("____________image texture height" + image.texture.height);
        Debug.Log("____________image packed" + image.packed);
        Debug.Log("____________image bounds" + image.bounds);
        Debug.Log("____________image bounds" + image.bounds);
        Debug.Log("____________image textureRect" + image.textureRect);
        Debug.Log("____________image textureRectOffset" + image.textureRectOffset);

        ushort[] triangles = image.triangles;
        for (int i = 0; i < triangles.Length; i++)
        {
            Debug.Log("triangle:" + triangles[i]);
        }

        Vector2[] vus = image.uv;
        for (int i = 0; i < vus.Length; i++)
        {
            Debug.Log("uv:" + vus[i]);
        }

        Vector2[] vertices = image.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            Debug.Log("vertices:" + vertices[i]);
        }
        Debug.Log("atlasname:" + image.texture.name);
    }
	// Use this for initialization
	void Start ()
    {
	    	
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}


    public void Save(Texture2D savedTexture)
    {
        byte[] bytes = savedTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/atlas.png", bytes);
    }

    public Texture2D FillInClear(Texture2D tex2D, Color whatToFillWith)
    {

        for (int i = 0; i < tex2D.width; i++)
        {
            for (int j = 0; j < tex2D.height; j++)
            {
                if (tex2D.GetPixel(i, j) == Color.clear)
                    tex2D.SetPixel(i, j, whatToFillWith);
            }
        }
        return tex2D;
    }
}
