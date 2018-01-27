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

    Dictionary<string, Color> colorDict;

    private int chrSoundIndex;
    
    // Use this for initialization
    void Awake()
    {
        instance = this;

        colorDict = new Dictionary<string, Color>();

        deleteAllChatting();
    }

    void deleteAllChatting()
    {
        foreach (Transform child in chatPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void printChat(string str, Person person = null)
    {
        if(chatPanel.childCount >= 10)
        {
            Destroy(chatPanel.GetChild(0).gameObject);
        }

        GameObject chatBubble;

        if (person != null)
        {
            chatBubble = Instantiate(leftChatBubble, chatPanel);
        }
        else
        {
            chatBubble = Instantiate(rightChatBubble, chatPanel);
        }

        if (person != null)
        {
            if(!colorDict.ContainsKey(person.getCode()))
            {
                colorDict.Add(person.getCode(), new Color(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f)));
            }

            // random color
            Color randomColor;
            colorDict.TryGetValue(person.getCode(), out randomColor);
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

        chrSoundIndex = (int)Random.Range(0f, 3.45f);
        Debug.Log("character index" + chrSoundIndex);

        for (int i = 0; i < str.Length; ++i)
        {
            messageText.text += str[i];
            SoundManager.instance.speechSound(chrSoundIndex);

            yield return new WaitForSeconds(0.05f);
        }
    }
}
