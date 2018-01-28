using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour {
	public Text text;
	// Use this for initialization
	void Start () {
		text.text = "Game Over\nDay: " + Title.Instance.gameDay + "\n\nGame made by ipkn, elore, zulkur";
		StartCoroutine ("toTitle");
	}

	IEnumerator toTitle()
	{
		yield return new WaitForSeconds(5.0f);
		yield return SceneManager.LoadSceneAsync ("scenes/title");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
