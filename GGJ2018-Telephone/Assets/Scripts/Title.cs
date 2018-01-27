using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {
	public static Title Instance;
	public static int firstStage = 1;
	public static string lang = "kr";
	public Text textObj;
	public int SwitchN = -1;
	public int BoardWidth = -1;
	public int BoardHeight = -1;

	void Awake()
	{
		if (Instance == null)
			Instance = this;

		else if (Instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	public void onClick()
    {
		firstStage = 1;
		SwitchN = 2;
		BoardWidth = 2;
		BoardHeight = 2;
        SceneManager.LoadScene(1);
    }
	public void onInfiClick()
	{
		firstStage = 4;
		SceneManager.LoadScene(1);
	}

	public void onHelpClick()
	{
		SceneManager.LoadScene ("scenes/helpscene");
	}

	public void onLangClick()
	{
		if (lang == "kr") {
			textObj.text = "LANG: EN";
			lang = "en";
		} else if (lang == "en") {
			textObj.text = "LANG: KR";
			lang = "kr";
		}

	}
}
