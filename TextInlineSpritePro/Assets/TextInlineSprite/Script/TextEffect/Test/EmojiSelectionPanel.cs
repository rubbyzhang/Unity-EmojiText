using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

public class EmojiSelectionPanel : MonoBehaviour
{
    [HideInInspector]
    public Action<string> CallBack; 

    public GameObject ListItem; 

    private ScrollRect  scrollview;

    private List<EmojiSelectionListItem> mListItem;

    private bool mInitSucess = false;

    private Button BgBtn; 

	private InlineSpriteManager mAnimManager;

    void Awake()
    {
        scrollview = transform.Find("ScrollView").GetComponent<ScrollRect>();
        if (scrollview == null)
        {
            Debug.Log("_____________________________scrollview is miss");
            return;
        }

        BgBtn = transform.Find("BackGround").GetComponent<Button>();
        BgBtn.onClick.AddListener(onBgClick);

    }

    private void onBgClick()
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (mInitSucess)
        {
            return;
        }

        InitListItems();
        List<string> names = InlineTextManager.Instance.GetAllEmojiNames();
        SetData(names);
        mInitSucess = true;
    }

    void InitListItems()
    {
        if (scrollview == null)
        {
            return;
        }

        Transform content = scrollview.content;
        if (content == null)
        {
            return;
        }

        if (content.childCount == 0)
        {
            Debug.LogError("_________________________必须有一个");
            return;
        }

        mListItem = new List<EmojiSelectionListItem>();
        for (int i = 0; i < content.childCount; ++ i)
        {
            Transform child = content.GetChild(i);
			EmojiSelectionListItem listItem = child.gameObject.GetComponent<EmojiSelectionListItem> ();
			//listItem.mText.mSriteAnimManager = mAnimManager;
			mListItem.Add(listItem);
        }
    }

    void SetData(List<string> items )
    {
        if (items == null)
        {
            return;
        }

        if (mListItem.Count > items.Count )
        {
            for (int i = 0; i < items.Count; ++ i)
            {
                mListItem[i].StrText = "[" + items[i] + "]";
                mListItem[i].gameObject.SetActive(true);

                mListItem[i].OnClickCallBack = OnItemClick;
            }

            for (int i = items.Count ; i < mListItem.Count; ++ i)
            {
                mListItem[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < mListItem.Count; ++i)
            {
                mListItem[i].StrText = "[" + items[i] + "]";
                mListItem[i].gameObject.SetActive(true);
                mListItem[i].OnClickCallBack = OnItemClick;
            }

            int from = mListItem.Count;
            for (int i = from; i < items.Count; ++i)
            {
                GameObject itemObj = Instantiate(mListItem[0].gameObject);
                itemObj.transform.parent = scrollview.content;
                itemObj.name = "item" + i.ToString();
                itemObj.transform.localScale = Vector3.one;
                EmojiSelectionListItem listItem = itemObj.GetComponent<EmojiSelectionListItem>();
                listItem.StrText = "[" + items[i] + "]";

				//listItem.mText.mSriteAnimManager = mAnimManager;

                //TODO
                itemObj.SetActive(false);
                itemObj.SetActive(true);

                mListItem.Add(listItem);
                mListItem[i].OnClickCallBack = OnItemClick;
            }
        }
    }

    void OnItemClick(string name)
    {
        Debug.Log("_____________________________EmojiPanel:" + name);

        if (CallBack != null)
        {
            CallBack(name);
        }
    }
}
