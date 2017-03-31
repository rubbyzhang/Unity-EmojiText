using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmojiSelectionListItem : MonoBehaviour
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

	[HideInInspector]
	public InlineText mText;
    private Button mButton;

    void Awake()
    {
		mText = GetComponentInChildren<InlineText>();
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