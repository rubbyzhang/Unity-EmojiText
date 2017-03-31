using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking.Match;

///已知问题：
/// 1.下划线解析和超链接解析都是基于字符位置对应实际字符顶点位置，同时存在时位置计算会有偏差
/// 2.字符串使用正则表达式，会有少量GC (1)减少不必要的表情顶点数据更新; (2)优化更新流程

/// <summary>
/// 表情动画数据组
/// </summary>
public class SpriteAnimInfo
{
    public string Key;
    public Vector3[] Vertices;
    public Rect[]  Uvs;

    public int Currnt = 0;
    public float RuningTime = 0;
    public List<string> Names ;

    public SpriteAnimInfo()
    {
        Key = "";
        Vertices = new Vector3[4];
        Uvs      = new Rect[8];
        Names = null;
    }

    public void Reset()
    {
        Key = "";
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Key);
    }

    public  void GetUv(ref Vector2[] uv , int startIndex)
    {
        if (uv == null || uv.Length == 0)
        {
            return;
        }

        Rect cur = Uvs[Currnt];

		uv[startIndex].x =  cur.x;
		uv[startIndex].y =  cur.y;

		uv [startIndex + 1].x = cur.x + cur.width;
		uv [startIndex + 1].y = cur.y + cur.height;

		uv [startIndex + 2].x = cur.x + cur.width;
		uv [startIndex + 2].y = cur.y;

		uv[startIndex+3].x = cur.x ;
		uv[startIndex+3].y = cur.y + cur.height;
    }
}


/// <summary>
/// 表情渲染管理器，定时更新表情数据并绘制
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(InlineSprite))]
public class InlineSpriteManager : MonoBehaviour
{
    /// <summary>
    /// 所有动画数据，使用前检查Key是否有效
    /// </summary>
    private Dictionary<string, SpriteAnimInfo> mTotalSpriteAnimDic = new Dictionary<string, SpriteAnimInfo>();

    /// <summary>
    /// Text对应的表情动画Key值
    /// </summary>
    private Dictionary<int, List<string>> mTextSpriteAnimKeysDic = new Dictionary<int, List<string>>();

    /// <summary>
    /// 当前激活中的Text
    /// </summary>
    private Dictionary<int, InlineText> mActicveTextDic = new Dictionary<int, InlineText>();

    private int MaxCount = 8;

    private readonly int mMaxAnimSpriteNum = 8;
    private readonly float mSpriteAnimTimeGap = 0.2f;

    //Mesh Data Cache
    private Vector3[] mTempVertices;
    private Vector2[] mTempUv;
    private int[] mTempTriangles;

    public void Register(InlineText inlineText)
    {
        if (null == inlineText)
        {
            return;
        }

        int id = inlineText.GetInstanceID();
        if (mActicveTextDic.ContainsKey(id))
        {
            return;
        }

        // Debug.Log("___________________________Register Name:" + inlineText.name);
        mActicveTextDic[id] = inlineText;
    }

    public void UnRegister(InlineText inlineText)
    {
        if (null == inlineText)
        {
            return;
        }

        RemoveSpriteAnimInfos(inlineText);

        int id = inlineText.GetInstanceID();
        if (mActicveTextDic.ContainsKey(id))
        {
            mActicveTextDic.Remove(id);
        }
        // Debug.Log("___________________________UnRegister Name:" + inlineText.name);
    }

    void OnEnable()
    {
        mTotalSpriteAnimDic.Clear();
        mTextSpriteAnimKeysDic.Clear();
    }

    void OnDisable()
    {
        mTextSpriteAnimKeysDic.Clear();
        mActicveTextDic.Clear();
        mTotalSpriteAnimDic.Clear();
    }

    void OnDestroy()
    {
        mTempVertices = null;
        mTempUv = null;
        mTempTriangles = null;
    }

    public void RemoveSpriteAnimInfos(InlineText inlineText)
    {
        if (inlineText == null)
        {
            return;
        }

        int id = inlineText.GetInstanceID();
        if (!mTextSpriteAnimKeysDic.ContainsKey(id))
        {
            return;
        }

        int count = mTotalSpriteAnimDic.Count;
        List<string> spriteAnimKeys = mTextSpriteAnimKeysDic[id];
        for (int i = 0; i < spriteAnimKeys.Count; ++i)
        {
            if (mTotalSpriteAnimDic.ContainsKey(spriteAnimKeys[i]))
            {
                mTotalSpriteAnimDic.Remove(spriteAnimKeys[i]);
            }
        }
        mTextSpriteAnimKeysDic.Remove(id);

        if (count != mTotalSpriteAnimDic.Count)
        {
            //Debug.Log("mInlineSpriteAnimInfoDic Count:" + mTotalSpriteAnimDic.Count);
            UpdateMeshCapacity();
        }
    }

    public void UpdateSpriteAnimInfos(InlineText inlineText, List<SpriteAnimInfo> inputSpriteAnimInfos)
    {
		Profiler.BeginSample("inlineSpriteManager UpdateSpriteAnimInfos ");

        if ( inlineText == null)
        {
            return;
        }

        bool isUpdateMeshData = false;

        int id = inlineText.GetInstanceID();
        List<string> oldSpriteKeys= null;

        if (mTextSpriteAnimKeysDic.ContainsKey(id))
        {
            oldSpriteKeys = mTextSpriteAnimKeysDic[id];
        }

        //input is null
        if (inputSpriteAnimInfos == null)
        {
            if (oldSpriteKeys != null)
            {
                for (int i = 0; i < oldSpriteKeys.Count; ++i)
                {
                    mTotalSpriteAnimDic.Remove(oldSpriteKeys[i]);
                }
                mTextSpriteAnimKeysDic.Remove(id);
                isUpdateMeshData = true;
            }
        }
        else
        {
            int oldCount = mTotalSpriteAnimDic.Count;
            if (oldSpriteKeys != null)
            {
                for (int i = 0; i < oldSpriteKeys.Count; ++i)
                {
                    mTotalSpriteAnimDic.Remove(oldSpriteKeys[i]);
                }
            }

            List<string> keys = new List<string>();
            for (int i = 0; i < inputSpriteAnimInfos.Count; ++i)
            {
                SpriteAnimInfo temp = inputSpriteAnimInfos[i];
                if (temp != null && temp.IsValid())
                {
                    mTotalSpriteAnimDic[temp.Key] = temp;
                    keys.Add(temp.Key);
                }
            }

            if (keys.Count > 0)
            {
                mTextSpriteAnimKeysDic[id] = keys;
            }
            else
            {
                if (oldSpriteKeys != null)
                {
                    mTextSpriteAnimKeysDic.Remove(id);
                }
            }

            if (oldCount != mTotalSpriteAnimDic.Count)
            {
                isUpdateMeshData = true;
            }
        }

        if (isUpdateMeshData)
        {
            //Debug.LogWarning("mInlineSpriteAnimInfoDic Count:" + mSpriteAnimInfoDic.Count);
            UpdateMeshCapacity();
        }


		Profiler.EndSample ();
    }

    public void UpdatePositon(InlineText inlineText, List<SpriteAnimInfo> inputSpriteAnimInfos)
    {
        UpdateSpriteAnimInfos(inlineText, inputSpriteAnimInfos);
        DrawSprite();
    }

    void LateUpdate()
    {
        if (mTotalSpriteAnimDic == null)
        {
            return;
        }

        List<string> keys = mTotalSpriteAnimDic.Keys.ToList();
        for (int i = 0 ; i < keys.Count ; ++i)
        {
            SpriteAnimInfo temp = mTotalSpriteAnimDic[keys[i]];
            if (!temp.IsValid())
            {
                continue;
            }

            temp.RuningTime += Time.deltaTime;
            if (temp.RuningTime >= mSpriteAnimTimeGap)
            {
                temp.RuningTime = 0;
                temp.Currnt++;

                if (temp.Currnt >= temp.Names.Count)
                {
                    temp.Currnt = 0;
                }
            }
        }
        DrawSprite();
    }
    
    //TODO 分配策略 在文本修改比较多的情况下 分配国语频繁
    void UpdateMeshCapacity()
    {
		Profiler.BeginSample("inlineSpriteManager UpdateMeshCapacity ");

        if (mTotalSpriteAnimDic == null || mTotalSpriteAnimDic.Count == 0)
        {
            mTempVertices = null;
            mTempUv = null;
            mTempTriangles = null;
            return;
        }

        int count = mTotalSpriteAnimDic.Count;
        int needUvCount = count * 4 ;
        int needVertexCount = count * 4 ;
        int needTrianglesCount = count * 6 ;

        if (mTempUv == null || mTempUv.Length != needUvCount)
        {
            mTempUv = new Vector2[needUvCount];
        }

        if (mTempVertices == null || mTempVertices.Length != needVertexCount)
        {
            mTempVertices = new Vector3[needVertexCount];
        }

        if (mTempTriangles  == null || mTempTriangles.Length != needTrianglesCount)
        {
            mTempTriangles = new int[needTrianglesCount];
        }

        Profiler.EndSample ();
    }
    
    public void DrawSprite()
    {
        Profiler.BeginSample("inline SpriteManager DrawSprite");

        if (mTotalSpriteAnimDic.Count == 0)
        {
            UpdateMesh();
            return;
        }

        List<string> keys = mTotalSpriteAnimDic.Keys.ToList();

        int index = 0;
        for (int i = 0; i < keys.Count; ++i)
        {
            SpriteAnimInfo temp = mTotalSpriteAnimDic[keys[i]];
            if (temp == null || !temp.IsValid())
            {
                continue;
            }

            if (temp.Vertices == null)
            {
                continue;
            }

            Array.Copy(temp.Vertices, 0, mTempVertices, index * 4, temp.Vertices.Length);
            temp.GetUv(ref mTempUv , index * 4);

            int startIndex = index * 6;
            mTempTriangles[startIndex + 0] = 0 + 4 * index;
            mTempTriangles[startIndex + 1] = 1 + 4 * index;
            mTempTriangles[startIndex + 2] = 2 + 4 * index;

            mTempTriangles[startIndex + 3] = 1 + 4 * index;
            mTempTriangles[startIndex + 4] = 0 + 4 * index;
            mTempTriangles[startIndex + 5] = 3 + 4 * index;

            index++;
        }
        
		Profiler.EndSample ();

        UpdateMesh();
    }

    private void UpdateMesh()
    {
        Profiler.BeginSample("inline SpriteManager DrawSprite UpdateMesh");

        Mesh newSpriteMesh = new Mesh();
        newSpriteMesh.vertices = mTempVertices;
        newSpriteMesh.uv = mTempUv;
        newSpriteMesh.triangles = mTempTriangles;
        GetComponent<CanvasRenderer>().SetMesh(newSpriteMesh);
        GetComponent<InlineSprite>().UpdateMaterial();

        Profiler.EndSample();
    }
}
