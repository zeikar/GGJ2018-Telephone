using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;
    public RectTransform chatPanel;

    public GameObject leftChatBubble;
    public GameObject rightChatBubble;
    public GameObject choiceBubble;

    Dictionary<string, Color> colorDict;
    private int chrSoundIndex;

    // Use this for initialization
    void Awake()
    {
        instance = this;

        colorDict = new Dictionary<string, Color>();
    }

    public void printChat(string str, Person person = null)
    {
        if (chatPanel.childCount >= 10)
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

        chatBubble.transform.localPosition = new Vector3(chatBubble.transform.localPosition.x,
            chatPanel.GetChild(chatPanel.childCount - 2).localPosition.y - chatPanel.GetChild(chatPanel.childCount - 2).GetComponent<RectTransform>().rect.height, chatBubble.transform.localPosition.z);

        if (chatBubble.transform.position.y < 120)
        {
            chatPanel.position = new Vector3(chatPanel.position.x, chatPanel.position.y + 60, chatPanel.position.z);
        }

        if (person != null)
        {
            if (!colorDict.ContainsKey(person.getCode()))
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

        object[] arguments = new object[] { str, chatBubble.GetComponentInChildren<Text>() };

        StartCoroutine("printCharacter", arguments);
    }

    public void printChoiceChat(string content, string choice1, string choice2, UnityEngine.Events.UnityAction buttonCallback1, UnityEngine.Events.UnityAction buttonCallback2)
    {
        GameObject chatBubble = Instantiate(choiceBubble, chatPanel);

        chatBubble.transform.localPosition = new Vector3(chatBubble.transform.localPosition.x,
            chatPanel.GetChild(chatPanel.childCount - 2).localPosition.y - chatPanel.GetChild(chatPanel.childCount - 2).GetComponent<RectTransform>().rect.height, chatBubble.transform.localPosition.z);

        if (chatBubble.transform.position.y < 140)
        {
            chatPanel.position = new Vector3(chatPanel.position.x, chatPanel.position.y + 80, chatPanel.position.z);
        }

        Text[] texts = chatBubble.GetComponentsInChildren<Text>();

        texts[0].text = content;
        texts[1].text = choice1;
        texts[2].text = choice2;

        Button[] buttons = chatBubble.GetComponentsInChildren<Button>();

        buttons[0].onClick.AddListener(buttonCallback1);
        buttons[1].onClick.AddListener(buttonCallback2);
    }

	public void RemoveLastChat()
	{
		var o = chatPanel.GetChild (chatPanel.childCount - 1);
		o.parent = null;
		Destroy (o.gameObject);
	}

    void test()
    {
        Debug.Log("asdasd");
    }

    IEnumerator printCharacter(object[] args)
    {
        string str = (string)args[0];
        Text messageText = (Text)args[1];
        messageText.text = "";

        chrSoundIndex = (int)Random.Range(0f, 3.45f);
        
        // Debug.Log("character index" + chrSoundIndex);

        for (int i = 0; i < str.Length; ++i)
        {
            messageText.text += str[i];
            SoundManager.instance.speechSound(chrSoundIndex);

            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }

    }
}
