using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;

public class ChatPanel : MonoBehaviour
{
    private EmojiSelectionPanel mEmojiSelectionPanel;

    private Button mEmojiButton;
    private Button mSendButton; 

    private InputField mInputField;

    private StringBuilder mInputStringBuilder;

    void Awake()
    {
        //mEmojiSelectionPanel = transform.FindChild("EmojiSelectionPanel").GetComponent<EmojiSelectionPanel>();
        if (mEmojiSelectionPanel == null)
        {
            Debug.LogError("____________________EmojiSelectionPanel is miss");
        }

       // mEmojiSelectionPanel.CallBack = OnEmojiSelected;


        mInputField = transform.FindChild("Input/InputField").GetComponent<InputField>();

        mEmojiButton = transform.FindChild("Input/Emoji").GetComponent<Button>();
        if (null == mEmojiButton)
        {
            Debug.LogError("____________________ mEmojiButton is miss");
        }
        
        mEmojiButton.onClick.AddListener(OnEmojiSelected);

        mSendButton = transform.FindChild("Input/Send").GetComponent<Button>();
        mSendButton.onClick.AddListener(OnSendMessage);

        mInputStringBuilder = new StringBuilder();
    }

    private void OnEmojiSelected(string name)
    {
        mInputStringBuilder.Append(name);
        UpdateInput();
    }

    private void OnSendMessage()
    {
        mInputStringBuilder.Length = 0;
        UpdateInput();
    }

    private void UpdateInput()
    {
        mInputField.text = mInputStringBuilder.ToString();
    }

    private void OnEmojiSelected()
    {
        if (null != mEmojiSelectionPanel)
        {
            mEmojiSelectionPanel.gameObject.SetActive(true);
        }
    }
}
