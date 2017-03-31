using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;

public class EmojiTest : MonoBehaviour
{
    void Awake()
    {
        ListView view = GetComponent<ListView>();

        List<string> strs = new List<string>();
        for (int i = 0; i < 50; ++i)
        {
           string str = i.ToString() + " Test emoji [meat][meat][angry][angry][bleeding][bleeding][bleeding][bleeding][bleeding][bleeding]";
            //string str = "asdfdsafsdafsdafsdfasfdasfasdfasddfdsfas";
          /// string str = i.ToString() + "Test emoji <quad name=meat size=36 width=1 /><quad name=meat size=36 width=1 /><quad name=meat size=36 width=1 /><quad name=meat size=36 width=1 /><quad name=meat size=36 width=1 /><quad name=meat size=36 width=1 /><quad name=meat size=36 width=1 /><quad name=meat size=36 width=1 /><quad name=meat size=36 width=1 /><quad name=meat size=36 width=1 />";

            strs.Add(str);
        }

        view.DataSource = strs.ToObservableList();
    }
}
