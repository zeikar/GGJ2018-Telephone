using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;
    public Transform chatPanel;

    public GameObject leftChatBubble;
    public GameObject rightChatBubble;
    
    // Use this for initialization
    void Awake()
    {
        instance = this;

        deleteAllChatting();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void deleteAllChatting()
    {
        foreach (Transform child in chatPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void printChat(string str, bool left = true)
    {
        GameObject chatBubble;

        if (left)
        {
            chatBubble = Instantiate(leftChatBubble, chatPanel);
        }
        else
        {
            chatBubble = Instantiate(rightChatBubble, chatPanel);
        }

        chatBubble.transform.position = new Vector3(chatBubble.transform.position.x, 
            chatBubble.transform.position.y + (chatPanel.childCount - 1) * -(Screen.height / 8), chatBubble.transform.position.z);

        if (left)
        {
            // random color
            Color randomColor = new Color(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f));
            foreach (Transform child in chatBubble.transform)
            {
                Image image = child.GetComponent<Image>();
                image.color = randomColor;
            }
        }

        object [] arguments = new object[] { str, chatBubble.GetComponentInChildren<Text>() };

        StartCoroutine("printCharacter", arguments);
    }

    IEnumerator printCharacter(object[] args)
    {
        string str = (string)args[0];
        Text messageText = (Text)args[1];
        messageText.text = "";

        for (int i = 0; i < str.Length; ++i)
        {
            messageText.text += str[i];

            yield return new WaitForSeconds(0.05f);
        }
    }
}
