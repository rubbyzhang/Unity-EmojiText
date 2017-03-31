using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatListItem : MonoBehaviour
{
    public string StrText
    {
        set
        {
            if (mText != null)
            {
                mText.text = value;
            }
        }

        get
        {
            if (mText != null)
            {
                return mText.text;
            }

            return "";
        }
    }

    public Action<string> OnClickCallBack;

    private Text mText;
    private Button mButton;

    void Awake()
    {
        mText = GetComponentInChildren<Text>();
        mButton = gameObject.GetComponent<Button>();
        mButton.onClick.AddListener(OnPointerClick);
    }

    void OnPointerClick()
    {
        if (OnClickCallBack != null)
        {
            OnClickCallBack(mText.text);
        }
    }
}
