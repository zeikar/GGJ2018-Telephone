using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UnityEngine.EventSystems; // Required when using Event data.

public class TutorialScript : MonoBehaviour {
	int index = 0;
	public GameObject image;
	public Text desc;
	public Texture tut0, tut1, tut2, tut3, tut4, tut5, tut6, tut7, tut8;

	public AudioSource buttonSound;

	string[] descs = new string[]{
		"당신은 전화교환수입니다.\n전화가 오면 케이블과 버튼을 조작하여\n 전화를 연결해야 합니다.",
		"전화가 오면 녹색불이 들어옵니다.\n해당 구멍에 왼쪽 케이블을 연결합니다.",
		"왼쪽 버튼을 누르면, 고객과 대화할 수 있습니다.\n먼저 인사를 하고, 어떤 분과 연결하길 원하는지 물어봅시다.",
		"전화번호부를 열어 원하는 상대를 찾아봅시다.\n각 구멍에는 가로로 A, B, C, 세로로 1, 2, 3의 이름이 붙어 있습니다.",
		"해당하는 케이블 구멍에 오른쪽 케이블을 연결합니다.", 
		"그리고 오른쪽 버튼을 누르면 받을 상대에게 전화 벨이 울립니다. 상대가 전화를 받으면 불이 켜집니다.",
		"불이 켜지면 스위치를 둘 다 끕니다. 두 명간의 통화가 진행됩니다.",
		"통화가 끝나면 LED불이 꺼집니다.",
		"양 쪽 케이블을 뽑아서 정리하고, 다음 전화를 이어서 처리합니다.",

	};
	Texture[] imgs;
	Rect imageRect;
	Rect buttonRect;
	Rect descRect;
	// Use this for initialization
	void Start () {
		//image.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite> ("Imgs/tut"+index);
		//desc.text = descs [index];
		index = 0;
		imgs = new Texture[9] {
			tut0,
			tut1,
			tut2,
			tut3,
			tut4,
			tut5,
			tut6,
			tut7,
			tut8,
		};
		imageRect = new Rect(0, 0, Screen.width, Screen.height);
		descRect = new Rect (
			Screen.width - Screen.height*3.25f / 10,
			Screen.height * 1 / 10,
			Screen.height * 3 / 10,
			Screen.height * 4);
		buttonRect = new Rect(Screen.width-Screen.height/10, Screen.height - Screen.height / 10, Screen.height/10, Screen.height / 10);
		buttonSound = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI() {
		if (index < 9) {
			var style = GUI.skin.GetStyle ("label");
			style.fontSize = 15;
			style.alignment = TextAnchor.UpperLeft;

			GUI.Label (imageRect, imgs [index]);
			GUI.Label (descRect, descs [index]);
		}
		if (GUI.Button (buttonRect, "Next")) {
			index++;
			buttonSound.Play();
			if (index == 9)
				SceneManager.LoadScene ("scenes/title");
		}
	}
}
