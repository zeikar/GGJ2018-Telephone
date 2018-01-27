using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;
    public Text chatText;
    
    // Use this for initialization
    void Awake()
    {
        instance = this;

        chatText.text = "";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void printChat(string str)
    {
        StopAllCoroutines();
        StartCoroutine("printCharacter", str);
    }

    IEnumerator printCharacter(string str)
    {
        chatText.text = "";

        for (int i = 0; i < str.Length; ++i)
        {
            chatText.text += str[i];

            yield return new WaitForSeconds(0.05f);
        }
    }
}
