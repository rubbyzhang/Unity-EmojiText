using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Text;

/// 已知问题：
/// 1. 下划线支持: 不支持超链接与下划线混排（顶点数据获取方式）, 换行bound计算问题，下划线颜色问题

[ExecuteInEditMode]
public class InlineText : Text, IPointerClickHandler
{
    private List<SpriteAnimInfo> mAnimSpriteInfoList;
    public List<SpriteAnimInfo> AnimSpriteInfoList
    {
        get { return mAnimSpriteInfoList; }
    }

    /// <summary>
    /// 可通过外部设置避免查找
    /// </summary>
    private InlineSpriteManager mSpriteManager;
    public InlineSpriteManager SpriteManager
    {
        get { return mSpriteManager; }
        set { mSpriteManager = value; }
    }

    private HrefClickEvent m_onHrefClick = new HrefClickEvent();
    public HrefClickEvent onHrefClick
    {
        get { return m_onHrefClick; }
        set { m_onHrefClick = value; }
    }

    private InlineSprite mInlineSprite;
    private List<SpriteTagInfo> mAnimSpiteTagList;

    private string mParseOutputText;

    private readonly List<HrefTagInfo> mHrefTagInfos = new List<HrefTagInfo>();
    private readonly List<UnderlineTagInfo> mUnderlineTagInfos = new List<UnderlineTagInfo>();

    private static readonly StringBuilder mTextBuilder = new StringBuilder();

    private static readonly Regex mConstSpriteTagRegex = new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?)/>", RegexOptions.Singleline);
    private static readonly Regex mConstSimpleSpriteTagRegex2 = new Regex(@"\[(.+?)\]", RegexOptions.Singleline);
    private static readonly Regex mHrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);
    private static readonly Regex mUnderlineRegex = new Regex(@"<u>(.+?)</u>", RegexOptions.Singleline);

    #region override
    protected override void OnEnable()
    {
        alignByGeometry = true;
        supportRichText = true;

        Register();
        
        base.OnEnable();
    }

    private void Register()
    {
        if (mSpriteManager == null && canvas != null)
        {
            mSpriteManager = GetSpriteManager();

            if (mSpriteManager == null)
            {
                Debug.LogError("InlineSpriteAnimManager is miss");
            }
        }

        if (mSpriteManager != null)
        {
            mInlineSprite = mSpriteManager.GetComponent<InlineSprite>();
            mInlineSprite.SetAllDirty();

            ParseText();
            SetVerticesDirty();
            mSpriteManager.Register(this);
        }
    }

    /// <summary>
    /// 从自身向上查找，表情图片单独渲染，解决层级问题可以通过增加多个管理器解决（不是很好的解决方案）
    /// </summary>
    /// <returns></returns>
    private InlineSpriteManager GetSpriteManager()
    {
		Transform current = transform.parent;
		while (null != current) 
		{
			InlineSpriteManager temp = current.GetComponentInChildren<InlineSpriteManager> ();	
			if (temp != null) 
			{
				return temp;
			}

			current = current.parent;
		}
        return null;
    }

    protected override void OnDisable()
    {
        if (mSpriteManager != null)
        {
            mSpriteManager.UnRegister(this);
        }

        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        if (mSpriteManager != null)
        {
            mSpriteManager.UnRegister(this);
        }
        base.OnDestroy();
    }

    public override string text
    {
        get
        {
            return base.text;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                if (string.IsNullOrEmpty(m_Text))
                {
                    return;
                }

                m_Text = "";
                DebugLog("Text Changed");
                ParseText();
                SetVerticesDirty();
            }
            else if (m_Text != value)
            {
                m_Text = value;
                DebugLog("Text Changed");
                ParseText();
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    public override void SetVerticesDirty()
    {
        if (!IsActive())
        {
            return;
        }

#if UNITY_EDITOR
        //处理编辑下文本修改问题，默认TextEditor会绕开override的text实现
        ParseText();
#endif
        DebugLog("Text SetVerticesDirty");
        base.SetVerticesDirty();
    }
    
    #endregion

    #region ParseText

    private void ParseText()
    {
        Profiler.BeginSample("InlineText ParseText");

        mParseOutputText = text;

        if (mParseOutputText.IndexOf('[') >= 0 && mParseOutputText.IndexOf(']') > 0)
        {
            mParseOutputText = ReplaceSimpleSpriteTags(mParseOutputText);
        }

        mParseOutputText = ParseUnderlineTag(mParseOutputText);

        mParseOutputText = ParseHrefTags(mParseOutputText);

        ParseSpriteTags(mParseOutputText);

        ResetSpriteInfoList();

        Profiler.EndSample();
    }

    //计算在顶点中起始和结束位置，考虑<u></u>的影响，其他标签暂且不考虑
    //归根到底是计算文字在顶点数据中位置方式不太靠谱
    protected string ParseHrefTags(string strText)
    {
        Profiler.BeginSample("InlineText Parse ParseUnderlineTag");

        mTextBuilder.Length = 0;

        if (string.IsNullOrEmpty(strText) || strText.IndexOf("href") == -1)
        {
            for (int i = 0; i < mHrefTagInfos.Count; ++i)
            {
                mHrefTagInfos[i].Reset();
            }
            return strText;
        }

        var indexText = 0;
        int index = 0;
        foreach (Match match in mHrefRegex.Matches(strText))
        {
            mTextBuilder.Append(strText.Substring(indexText, match.Index - indexText));

            if (index + 1 > mHrefTagInfos.Count)
            {
                var temp = new HrefTagInfo();
                mHrefTagInfos.Add(temp);
            }

            HrefTagInfo hrefInfo = mHrefTagInfos[index];

            hrefInfo.StartIndex = mTextBuilder.Length;
            hrefInfo.EndIndex = mTextBuilder.Length + match.Groups[2].Length;
            hrefInfo.Name = match.Groups[1].Value;

            mTextBuilder.Append(match.Groups[2].Value);
            indexText = match.Index + match.Length;
            index ++;
        }
        mTextBuilder.Append(strText.Substring(indexText, strText.Length - indexText));

        if (index < mHrefTagInfos.Count)
        {
            int count = mHrefTagInfos.Count;
            for (int i = index; i < count; ++i)
            {
                mHrefTagInfos[i].Reset();
            }
        }

        Profiler.EndSample();

        return mTextBuilder.ToString();
    }

    protected string ParseUnderlineTag(string strText)
    {
        Profiler.BeginSample("InlineText Parse ParseUnderlineTag");

        mTextBuilder.Length = 0;

        if (string.IsNullOrEmpty(strText) ||  strText.IndexOf("<u>") ==-1 || strText.IndexOf("</u>") == -1)
        {
            for (int i = 0; i < mUnderlineTagInfos.Count; ++i)
            {
                mUnderlineTagInfos[i].Reset();
            }
            return strText;
        }

        var indexText = 0;
        int index = 0;
        foreach (Match match in mUnderlineRegex.Matches(strText))
        {
            mTextBuilder.Append(strText.Substring(indexText, match.Index - indexText));

            if (index + 1 > mUnderlineTagInfos.Count)
            {
                var temp = new UnderlineTagInfo();
                mUnderlineTagInfos.Add(temp);
            }

            //文本起始顶点索引 TODO 位置计算容易出问题，万一中间带有其他解释符号。所以下滑先只能放在次里面（仅次于超链接）
            mUnderlineTagInfos[index].StartIndex = mTextBuilder.Length;
            mUnderlineTagInfos[index].EndIndex = mTextBuilder.Length + match.Groups[1].Length;

            mTextBuilder.Append(match.Groups[1].Value);
            indexText = match.Index + match.Length;

            index ++;
        }

        if (index < mUnderlineTagInfos.Count)
        {
            int count = mUnderlineTagInfos.Count;
            for (int i = index; i < count; ++i)
            {
                mUnderlineTagInfos[i].Reset();
            }
        }
        
        mTextBuilder.Append(strText.Substring(indexText, strText.Length - indexText));

        Profiler.EndSample();

        return mTextBuilder.ToString();
    }

    private string ReplaceSimpleSpriteTags(string strText)
    {
        Profiler.BeginSample("InlineText Parse ReplaceSimpleSpriteTags");

        mTextBuilder.Length = 0;
        var indexText = 0;
        foreach (Match match in mConstSimpleSpriteTagRegex2.Matches(strText))
        {
            mTextBuilder.Append(strText.Substring(indexText, match.Index - indexText));
            string strSprite = "<quad name=" + match.Groups[1].ToString().Trim() + " size=" + fontSize + " width=1.2/>";
            mTextBuilder.Append(strSprite);
            indexText = match.Index + match.Length;
        }
        mTextBuilder.Append(strText.Substring(indexText, strText.Length - indexText));
        Profiler.EndSample();
        return mTextBuilder.ToString();
    }

    private void ParseSpriteTags(string strText)
    {
        if (mInlineSprite == null)
        {
            return;
        }

        if (mAnimSpiteTagList == null)
        {
            mAnimSpiteTagList = new List<SpriteTagInfo>();
        }

        if (string.IsNullOrEmpty(strText) || -1 == strText.IndexOf("quad") )
        {
            for (int i = 0; i < mAnimSpiteTagList.Count; ++i)
            {
                mAnimSpiteTagList[i].Reset();
            }
            return;
        }

        int index = 0;
        foreach (Match match in mConstSpriteTagRegex.Matches(strText))
        {
            List<string> names = mInlineSprite.GetSpriteNamesFromPrefix(match.Groups[1].Value);
            if (names != null && names.Count > 0)
            {
                if (index + 1 > mAnimSpiteTagList.Count)
                {
                    SpriteTagInfo tempNew = new SpriteTagInfo();
                    mAnimSpiteTagList.Add(tempNew);
                }

                SpriteTagInfo tempArrayTag = mAnimSpiteTagList[index];
                tempArrayTag.Key = GenerateKey(match.Groups[1].Value, index);
                tempArrayTag.Names = names;
                tempArrayTag.VertextIndex = match.Index;
                float size = float.Parse(match.Groups[2].Value);

                float width = float.Parse(match.Groups[3].Value);
                float offset = 0.0f;
                if (width > 1.0f)
                {
                    offset = (width - 1.0f) / 2.0f;
                }

                tempArrayTag.Size   = new Vector2(size, size);
                tempArrayTag.Offset = offset;

                index ++;
            }
        }

        if (index < mAnimSpiteTagList.Count)
        {
            int count = mAnimSpiteTagList.Count;
            for (int i = index ; i < count; ++i)
            {
                mAnimSpiteTagList[i].Reset();
            }
        }
    }

    private void ResetSpriteInfoList()
    {
        if (mAnimSpiteTagList == null || mAnimSpiteTagList.Count == 0)
        {
            mAnimSpriteInfoList = null;
            return;
        }

        if (mAnimSpriteInfoList == null)
        {
            mAnimSpriteInfoList = new List<SpriteAnimInfo>(2);
        }

        int validCount = 0;
        for (int i = 0; i < mAnimSpiteTagList.Count; ++ i)
        {
            if (mAnimSpiteTagList[i].IsValid())
            {
                validCount++;
            }
        }

        if (validCount > mAnimSpriteInfoList.Count)
        {
            int needCount = validCount - mAnimSpriteInfoList.Count;
            for (int i = 0; i <= needCount; ++i)
            {
                SpriteAnimInfo infos = new SpriteAnimInfo();
                mAnimSpriteInfoList.Add(infos);
            }
        }
        else
        {
            for (int i = validCount; i < mAnimSpriteInfoList.Count; ++i)
            {
                mAnimSpriteInfoList[i].Reset();
            }
        }
    }

    private string GenerateKey(string name, int pos)
    {
        return name + "_" + gameObject.GetInstanceID() + "_" +pos.ToString();
    }
    
    #endregion

    #region update
    void LateUpdate()
    {
        if (mSpriteManager == null)
        {
            Register();
        }

        if (rectTransform.hasChanged)
        {
            rectTransform.hasChanged = false;
            UpdateSpritePos();
        }
    }
    
    readonly UIVertex[] m_TempVerts = new UIVertex[4];
    private VertexHelper mVertexHelperRef;
    private TextGenerationSettings TextGenerationSettings;
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
		Profiler.BeginSample("inlineText OnPopulateMesh");

        DebugLog("Text OnPopulateMesh");

        Profiler.BeginSample("inlineText OnPopulateMesh Data");
        if (font == null)
        {
            return;
        }

        // We don't care if we the font Texture changes while we are doing our Update.
        // The end result of cachedTextGenerator will be valid for this instance.
        // Otherwise we can get issues like Case 619238.
        m_DisableFontTextureRebuiltCallback = true;

        Vector2 extents = rectTransform.rect.size;

        TextGenerationSettings = GetGenerationSettings(extents);
        cachedTextGenerator.Populate(mParseOutputText, TextGenerationSettings);

        Rect inputRect = rectTransform.rect;

        // get the text alignment anchor point for the text in local space
        Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
        Vector2 refPoint = Vector2.zero;
        refPoint.x = (textAnchorPivot.x == 1 ? inputRect.xMax : inputRect.xMin);
        refPoint.y = (textAnchorPivot.y == 0 ? inputRect.yMin : inputRect.yMax);

        // Determine fraction of pixel to offset text mesh.
        Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;

        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        //Last 4 verts are always a new line...
        int vertCount = verts.Count - 4;

        toFill.Clear();

        ClearQuadUv( verts);

        if (roundingOffset != Vector2.zero)
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                if (tempVertsIndex == 3)
                {
                    toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
        }
        else
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                if (tempVertsIndex == 3)
                {
                    toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
        }
        m_DisableFontTextureRebuiltCallback = false;
        mVertexHelperRef = toFill;
        Profiler.EndSample();

        Profiler.BeginSample("inlineText HerfTagHandler ");
        HerfTagHandler();
        Profiler.EndSample();

        Profiler.BeginSample("inlineText UnderlineTagsHandler ");
        UnderlineTagsHandler();
		Profiler.EndSample ();

		Profiler.BeginSample("inlineText SpriteTagHandler ");
        SpriteTagHandler();
		Profiler.EndSample ();

		Profiler.EndSample ();
    }

    #endregion

    #region sprite
    private void UpdateSpritePos()
    {
        if (mSpriteVertPositionList != null)
        {
            CalcQuadTag(mSpriteVertPositionList , true);
            if (mSpriteManager != null)
            {
                mSpriteManager.UpdatePositon(this, mAnimSpriteInfoList);
            }
        }
    }

    private List<UIVertex> mSpriteVertPositionList; 
    private void SpriteTagHandler()
    {
        if (mAnimSpriteInfoList == null || mAnimSpriteInfoList.Count == 0)
        {
            return;
        }

        if (mAnimSpiteTagList == null || mAnimSpiteTagList.Count == 0)
        {
            return;
        }

        if (mSpriteVertPositionList == null)
        {
            mSpriteVertPositionList = new List<UIVertex>();
        }

        Profiler.BeginSample("inlineText SpriteTagHandler Position");

        for (int i = 0; i < mAnimSpiteTagList.Count; i++)
        {
            SpriteTagInfo tempTagInfo = mAnimSpiteTagList[i];
            if (!tempTagInfo.IsValid())
            {
                continue;
            }

            int vertexIndex = ((tempTagInfo.VertextIndex + 1) * 4) - 1;
            if (vertexIndex >= mVertexHelperRef.currentVertCount || vertexIndex < 0)
            {
                // Debug.LogError("CalcQuadTag Vertex Index is out of range:" + j);
                continue;
            }

            if (i+1 > mSpriteVertPositionList.Count)
            {
                int needCount = i + 1 - mSpriteVertPositionList.Count;
                for (int m = 0; m < needCount; ++m)
                {
                    mSpriteVertPositionList.Add(new UIVertex());
                }
            }
            UIVertex tempVer = new UIVertex();
            mVertexHelperRef.PopulateUIVertex(ref tempVer, vertexIndex);
            mSpriteVertPositionList[i] = tempVer;
        }
        Profiler.EndSample();

        CalcQuadTag(mSpriteVertPositionList);

        if (mSpriteManager != null)
        {
            mSpriteManager.UpdateSpriteAnimInfos(this,mAnimSpriteInfoList);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteVerters">图片位置信息</param>
    /// <param name="onlyUpdatePositon">是否只更新位置</param>
    void CalcQuadTag(List<UIVertex> spriteVerters , bool onlyUpdatePositon = false)
    {
        Profiler.BeginSample("inlineText SpriteTagHandler CalcQuadTag");

        if (mAnimSpriteInfoList == null || mAnimSpriteInfoList.Count == 0)
        {
            return;
        }

        if (mAnimSpiteTagList == null || mAnimSpiteTagList.Count == 0)
        {
            return;
        }

        if (spriteVerters == null || spriteVerters.Count == 0)
        {
            return;
        }

        //canvas scale 
        Vector3 relativePostion = Vector3.zero;
        if (mSpriteManager != null)
        {
            relativePostion = transform.position - mSpriteManager.transform.position;
            if (canvas != null)
            {
                Vector3 scale = canvas.transform.localScale;
                relativePostion = new Vector3(relativePostion.x / scale.x, relativePostion.y / scale.y, relativePostion.z / scale.z);
            }
        }

        Profiler.BeginSample("inlineText CalcQuadTag Cal");

        for (int i = 0; i < mAnimSpiteTagList.Count; i++)
        {
            SpriteTagInfo tempTagInfo = mAnimSpiteTagList[i];
            if (!tempTagInfo.IsValid())
            {
                continue;
            }

            SpriteAnimInfo tempSpriteAnimInfos = mAnimSpriteInfoList[i];

            tempSpriteAnimInfos.Key =  tempTagInfo.Key;
            tempSpriteAnimInfos.Names = tempTagInfo.Names;

            if (i >= spriteVerters.Count)
            {
                //Debug.LogWarning("SpriteAnim Position is less");
                continue;
            }
            Vector3 textpos = relativePostion + spriteVerters[i].position;

            float xOffset = tempTagInfo.Offset * tempTagInfo.Size.x;

            tempSpriteAnimInfos.Vertices[0] = new Vector3(xOffset, 0, 0) + textpos;
            tempSpriteAnimInfos.Vertices[1] = new Vector3(xOffset + tempTagInfo.Size.x , tempTagInfo.Size.y, 0) + textpos;
            tempSpriteAnimInfos.Vertices[2] = new Vector3(xOffset + tempTagInfo.Size.x , 0, 0) + textpos;
            tempSpriteAnimInfos.Vertices[3] = new Vector3(xOffset, tempTagInfo.Size.x, 0) + textpos;

            if (onlyUpdatePositon == true)
            {
                continue;
            }

            for (int j = 0; j < tempTagInfo.Names.Count; j++)
            {
                Rect newSpriteRect ;
                SpriteAssetInfo tempSpriteAsset = mInlineSprite.GetSpriteInfo(tempTagInfo.Names[j]);
                if (tempSpriteAsset != null)
                {
                    newSpriteRect = tempSpriteAsset.rect;
                }
                else
                {
                    newSpriteRect = mInlineSprite.GetSpriteInfo(0).rect;
                    Debug.LogError("CalcQuadTag Can Find Sprite(name=" + tempTagInfo.Key + ")");
                }
                tempSpriteAnimInfos.Uvs[j] = newSpriteRect;
            }
        }

        Profiler.EndSample();

        Profiler.EndSample();
    }

    //UGUIText不支持<quad/>标签，表现为乱码, 将uv全设置为0
    private void ClearQuadUv( IList<UIVertex> verts)
    {
        if (mAnimSpiteTagList == null || mAnimSpiteTagList.Count == 0)
        {
            return;
        }

        Profiler.BeginSample("inlineText Cal ClearQuadUv");

        UIVertex tempVertex;

        for (int i = 0; i < mAnimSpiteTagList.Count; i++)
        {
            SpriteTagInfo temp = mAnimSpiteTagList[i];
            if (!temp.IsValid())
            {
                continue;
            }

            int startIndex = temp.VertextIndex * 4;
            int endIndex = startIndex +  4;

            for (int m = startIndex; m < endIndex; m++)
            {
                if (m >= verts.Count)
                {
                    continue;
                }

                tempVertex = verts[m];
                tempVertex.uv0 = Vector2.zero;
                verts[m] = tempVertex;
            }
        }
        Profiler.EndSample();
    }
    #endregion

    #region underline

    private void UnderlineTagsHandler()
    {
        if (mUnderlineTagInfos == null || mUnderlineTagInfos.Count == 0)
        {
            return;
        }

        for (int i = 0; i < mUnderlineTagInfos.Count; ++i)
        {
            UnderlineTagInfo temp = mUnderlineTagInfos[i];
            if (!temp.IsValid())
            {
                continue;
            }

            int vertexStart = mUnderlineTagInfos[i].StartIndex * 4;
            int vertexEnd = (mUnderlineTagInfos[i].EndIndex - 1) * 4 + 3;
            mUnderlineTagInfos[i].Boxes = GetBounds(vertexStart, vertexEnd);
        }

        TextGenerator textGenerator = new TextGenerator();
        textGenerator.Populate("_", TextGenerationSettings);
        IList<UIVertex> underlineVerts = textGenerator.verts;

        for(int m = 0 ; m < mUnderlineTagInfos.Count ; ++m)
        {
            var underlineInfo = mUnderlineTagInfos[m];
            if (!underlineInfo.IsValid())
            {
                continue;
            }
            if (underlineInfo.StartIndex >= mVertexHelperRef.currentVertCount)
            {
                continue;
            }

            for (int i = 0; i < underlineInfo.Boxes.Count; i++)
            {
                Vector3 startBoxPos = new Vector3(underlineInfo.Boxes[i].x, underlineInfo.Boxes[i].y, 0.0f);
                Vector3 endBoxPos = startBoxPos + new Vector3(underlineInfo.Boxes[i].width, 0.0f, 0.0f);
                AddUnderlineQuad(underlineVerts, startBoxPos, endBoxPos);
            }
        }
    }

    //根据起始位置获得包围盒
    private List<Rect> GetBounds(int vertexStartIndex, int vertexEndIndex)
    {
        List<Rect> boxs = new List<Rect>();
        if (null == mVertexHelperRef)
        {
            return boxs;
        }

        if (vertexStartIndex < 0 || vertexStartIndex >= mVertexHelperRef.currentVertCount)
        {
            return boxs;
        }

        if (vertexEndIndex < 0 || vertexEndIndex >= mVertexHelperRef.currentVertCount)
        {
            return boxs;
        }

        UIVertex vert = new UIVertex();
        mVertexHelperRef.PopulateUIVertex(ref vert, vertexStartIndex);
        var pos = vert.position;
        var bounds = new Bounds(pos, Vector3.zero);
        for (int i = vertexStartIndex, m = vertexEndIndex; i < m; i++)
        {
            if (i >= mVertexHelperRef.currentVertCount)
            {
                break;
            }

            mVertexHelperRef.PopulateUIVertex(ref vert, i);
            pos = vert.position;
            if (pos.x < bounds.min.x)      // 换行重新添加包围框     todo
            {
                boxs.Add(new Rect(bounds.min, bounds.size));
                bounds = new Bounds(pos, Vector3.zero);
            }
            else                          //扩展包围盒
            {
                bounds.Encapsulate(pos);
            }
        }

        boxs.Add(new Rect(bounds.min, bounds.size));
        return boxs;
    }

    //添加下划线
    private void AddUnderlineQuad( IList<UIVertex> underlineVerts, Vector3 startBoxPos, Vector3 endBoxPos)
    {
        Vector3[] underlinePos = new Vector3[4];
        underlinePos[0] = startBoxPos + new Vector3(0, fontSize * -0.1f, 0);
        underlinePos[1] = endBoxPos + new Vector3(0, fontSize * -0.1f, 0); ;
        underlinePos[2] = endBoxPos + new Vector3(0, fontSize * 0f, 0);
        underlinePos[3] = startBoxPos + new Vector3(0, fontSize * 0f, 0);

        for (int i = 0; i < 4; ++i)
        {
            int tempVertsIndex = i & 3;
            m_TempVerts[tempVertsIndex] = underlineVerts[i % 4];
            m_TempVerts[tempVertsIndex].color = Color.blue;
            m_TempVerts[tempVertsIndex].position = underlinePos[i];
            if (tempVertsIndex == 3)
            {
                mVertexHelperRef.AddUIVertexQuad(m_TempVerts);
            }
        }
    }
    #endregion

    #region href
    private void HerfTagHandler()
    {
        if (mHrefTagInfos == null ||  mHrefTagInfos.Count == 0)
        {
            return;
        }

        for(int i = 0 ; i < mHrefTagInfos.Count ; ++i)
        {
            HrefTagInfo temp = mHrefTagInfos[i];
            if (!temp.IsValid())
            {
                continue;
            }

            int vertexStart = temp.StartIndex * 4;
            int vertexEnd = (temp.EndIndex - 1) * 4 + 3;

            mHrefTagInfos[i].Boxes = GetBounds(vertexStart, vertexEnd);
        }
    }

    /// 点击事件检测是否点击到超链接文本
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 lp;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out lp);

        foreach (var hrefInfo in mHrefTagInfos)
        {
            var boxes = hrefInfo.Boxes;
            for (var i = 0; i < boxes.Count; ++i)
            {
                if (boxes[i].Contains(lp))
                {
                    m_onHrefClick.Invoke(hrefInfo.Name);
                    return;
                }
            }
        }
    }
    #endregion

    #region define
    [System.Serializable]
    public class SpriteTagInfo
    {
        public string Key;
        public List<string> Names; 
        public int VertextIndex;
        public Vector2 Size;
        public float Offset;


        public void Reset()
        {
            Key = "";
        }

        public SpriteTagInfo()
        {
            Names = new List<string>();
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Key);
        }
    }

    /// <summary>
    /// 超链接信息类
    /// </summary>
    private class HrefTagInfo
    {
        public int StartIndex;

        public int EndIndex;

        public string Name;

        public void Reset()
        {
            StartIndex = -1;
            EndIndex = -1;
        }

        public bool IsValid()
        {
            return StartIndex != -1 && EndIndex != -1;
        }

        public  List<Rect> Boxes = new List<Rect>();
    }

    private class UnderlineTagInfo
    {
        public int StartIndex;

        public int EndIndex;

        public  List<Rect> Boxes = new List<Rect>();

        public  UnderlineTagInfo()
        {
            Reset();
        }

        public void Reset()
        {
            StartIndex = -1;
            EndIndex = -1;
        }

        public bool IsValid()
        {
            return StartIndex != -1 && EndIndex != -1;
        }
    }

    [System.Serializable]
    public class HrefClickEvent : UnityEvent<string> { }

    #endregion

    void DebugLog(string str)
    {
       // Debug.Log("_______________________" + str);
    }
}

