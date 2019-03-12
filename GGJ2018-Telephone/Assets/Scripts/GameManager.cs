using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
		//if (Instance == null) {
			Instance = this;
			//SceneManager.activeSceneChanged += OnSceneChanged;
			//DontDestroyOnLoad(gameObject);

		//}
		/*else if (Instance != this) {
			Destroy (gameObject);
		}*/
    }


	void OnSceneChanged (Scene previousScene, Scene changedScene)
	{
		Debug.LogError ("OnSceneChanged "+previousScene.name+" -> " + changedScene.name);
		Debug.Log (""+Title.Instance.gameStage+" "+Title.Instance.gameDay);
	}
    
    void Start()
    {
		InitGame();
    }

	void OnGUI()
	{
		if (Time.time - dayBeginTime < 3) {
			Rect r = new Rect (Screen.width/10, Screen.height/10, Screen.width*8/10, Screen.height*8/10);
			//GUI.backgroundColor = new Color (0, 0, 0, 0.7f);
			var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
			texture.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f, 0.8f));
			texture.Apply();

			GUIStyle style = GUI.skin.GetStyle ("label");
			style.fontSize = 50;
			style.alignment = TextAnchor.MiddleCenter;
			GUI.Box (r, texture);
			GUI.Label (r, "DAY " + Title.Instance.gameDay, style);
		}
	}

	int callsIndex = 0;
	List<object[]> calls = new List<object[]>(); 
	Dictionary<int, bool> isCalling = new Dictionary<int, bool>();

	bool IsCalling(int slot)
	{
		bool ret = false;
		isCalling.TryGetValue(slot, out ret);
		return ret;
	}

    void Update()
    {
		var now = Time.time - dayBeginTime;
		if (Title.Instance.life < 0) {
			// TODO Game over
		}
		if (now > 0 && Title.Instance.life < 1000)
			Title.Instance.life += Time.deltaTime;
		if (Title.Instance.life > 1000)
			Title.Instance.life = 1000;
		if (Title.Instance.life < 0) {
			SceneManager.LoadScene ("scenes/gameover");
			return;
		}
		{
			// life gauge
			var s = GameObject.Find("LifeGauge").transform.localScale;
			s.y = 2000-Title.Instance.life * 2 + Mathf.Sin (now / 3) * 20 + UnityEngine.Random.Range (-2.5f, 2.5f);
			if (s.y < 0)
				s.y = 0;
            GameObject.Find("LifeGauge").transform.localScale = s;
		}
		for (; callsIndex < calls.Count; callsIndex++) {
			var row = calls [callsIndex];
			if ((float)row [1] > now) {
				break;
			}
			Person sender = (Person)row [2];
			Person recver = (Person)row [3];
			if (IsCalling (sender.getSlot ()) || IsCalling (recver.getSlot ())) {
				break;
			}
			StartCoroutine ((string)row[0], row);
		}
		if (goalCount == 0) {
			// day completed!
			Title.Instance.gameDay ++;
			if (Title.Instance.gameStage == 1)
				Title.Instance.gameStage = 4;
			SetPreStageVariable ();
            goalCount = -1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

	void SetPreStageVariable()
	{
		if (Title.Instance.gameStage == 1) {
			Title.Instance.BoardHeight = 2;
			Title.Instance.BoardWidth = 2;
			Title.Instance.SwitchN = 2;
		} else if (Title.Instance.gameStage == 2) {
		} else if (Title.Instance.gameStage == 3) {
		} else if (Title.Instance.gameStage >= 4) {
			Title.Instance.BoardHeight = 4;
			Title.Instance.BoardWidth = 6;
			Title.Instance.SwitchN = 6;
		}
	}

    Dictionary<int, int> cableEndConnections = new Dictionary<int, int>();
    bool IsCableConnected(int cableEnd, int cableSlot)
    {
        int value;
        if (cableEndConnections.TryGetValue(cableEnd, out value))
        {
            if (value == cableSlot)
                return true;
        }
        return false;
    }

    public void OnConnect(int cableEnd, int cableSlot)
    {
        // if cableSlot == -1, cableEnd disconnected;
        // TODO 인디케이터 연결해주기
        if (cableSlot == -1)
        {
            cableEndConnections.Remove(cableEnd);
            SoundManager.instance.ConnectorSound(false);
        }
        else
        {
            cableEndConnections[cableEnd] = cableSlot;
            SoundManager.instance.ConnectorSound(true);
        }
    }

    void SetLineIndicator(int lineIndex, bool onoff)
    {
        if (onoff)
        {
            LineSetScript.instance.indicators[lineIndex].color = new Color(1, 237f / 255, 0);
        }
        else
        {
            LineSetScript.instance.indicators[lineIndex].color = new Color(103f / 255, 97f / 255, 14f / 255);
        }
    }

    void SetSlotIndicator(int slot, bool onoff)
    {
        if (onoff)
        {
            SwitchScript.Instance.indicators[slot].color = new Color(51f / 255, 1, 0);
        }
        else
        {
            SwitchScript.Instance.indicators[slot].color = new Color(15f / 255, 64f / 255, 0);
        }
    }

    void callback1()
    {
        ChatManager.instance.printChat("음...");
    }

    void callback2()
    {
        ChatManager.instance.printChat("안녕하세요 교환원입니다.");
    }

	IEnumerator jokeCall(object[] args)
	{
		float secondsToBegin = (float)args[1];
		//int slot = (int)args[1];
		Person sender = (Person)args[2];
		int slot = sender.getSlot();

		yield return new WaitForSeconds(secondsToBegin-Time.time+dayBeginTime);
	}

	IEnumerator tutorialCall(object[] args)
	{
		float secondsToBegin = (float)args[1];
		//int slot = (int)args[1];
		Person sender = (Person)args[2];
		int slot = sender.getSlot();
		Person recver = (Person)args[3];
		string msg = (string)args[4];
		int opponentWaiting = (int)args[5];
		int callDuration = (int)args[6];

		isCalling [sender.getSlot ()] = true;
		isCalling [recver.getSlot ()] = true;
	retry:
		yield return new WaitForSeconds(secondsToBegin-Time.time+dayBeginTime);

		SetSlotIndicator(slot, true);

		int lineUsing = -1;
		var lineSet = GameObject.Find("LineSetArray").GetComponent<LineSetScript>();
		while(true)
		{
			bool found = false;
			for(int i = 0; i < lineSet.N; i ++)
			{
				//Debug.Log(i + " " + LineSetScript.instance.IsSwitchOperatorOn(i) + " " + IsCableConnected(i*2, slot));
				if (LineSetScript.instance.IsSwitchOperatorOn(i) && IsCableConnected(i*2, slot))
				{
					found = true;
					lineUsing = i;
			//		Debug.Log("lineUsing " + i);
					SetLineIndicator(2*i, true);
					break;
				}
			}
			if (found)
				break;
			yield return new WaitForSeconds(0.1f);
		}

		bool choiced = false;

		ChatManager.instance.printChoiceChat("", "음...", "안녕하세요 교환원입니다.", ()=> {
			ChatManager.instance.RemoveLastChat();
			ChatManager.instance.printChat("음...");
			choiced = true;
					
		}, ()=>{
			ChatManager.instance.RemoveLastChat();
			ChatManager.instance.printChat("안녕하세요 교환원입니다. 누구를 찾으시나요?");
			choiced = true;
		});
		for (int i = 0; i < 5; i++) {
			yield return new WaitForSeconds (1.0f);
			if (choiced) {
				yield return new WaitForSeconds (1.0f);
				break;
			}
		}
		if (!choiced) {
			ChatManager.instance.RemoveLastChat ();
		}
		ChatManager.instance.printChat(msg, sender);

		Debug.Log("print chat");

		while(true)
		{
			//Debug.Log("line valid " + LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) + " " + IsCableConnected(lineUsing * 2 + 1, recver.getSlot()));

			if (!(LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()))) {
				SetSlotIndicator(sender.getSlot(), false);
				SetLineIndicator(lineUsing * 2, false);
				SetSlotIndicator(recver.getSlot(), false);
				SetLineIndicator(lineUsing * 2 + 1, false);
				yield return new WaitForSeconds (1.0f);
				goto retry;
			}

			if (LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) && IsCableConnected(lineUsing * 2+1, recver.getSlot()))
				break;
			yield return new WaitForSeconds(0.1f);
		}

		Debug.Log("connect correct line");

		{
			var currentTime = Time.time;
			while(Time.time - currentTime < opponentWaiting*dayScale)
			{
				if (!(
					LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
					LineSetScript.instance.IsSwitchTelephoneOn (lineUsing) && IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {
					SetSlotIndicator(sender.getSlot(), false);
					SetLineIndicator(lineUsing * 2, false);
					SetSlotIndicator(recver.getSlot(), false);
					SetLineIndicator(lineUsing * 2 + 1, false);
					yield return new WaitForSeconds (1.0f);
					goto retry;
				}
				Debug.Log("wait opponent " + (Time.time - currentTime));
				yield return new WaitForSeconds(0.1f);
			}
		}

		Debug.Log("wait done");

		SetSlotIndicator(recver.getSlot(), true);
		SetLineIndicator(2 * lineUsing + 1, true);

		while (true)
		{
			if (!(
				IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
				IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {
				SetSlotIndicator(sender.getSlot(), false);
				SetLineIndicator(lineUsing * 2, false);
				SetSlotIndicator(recver.getSlot(), false);
				SetLineIndicator(lineUsing * 2 + 1, false);
				yield return new WaitForSeconds (1.0f);
				goto retry;
			}
			if (!LineSetScript.instance.IsSwitchTelephoneOn(lineUsing))
				break;
			yield return new WaitForSeconds(0.1f);
		}

		{
            SoundManager.instance.transmissionSound();

			var currentTime = Time.time;
			bool blinking = true;
			while (Time.time - currentTime < callDuration*dayScale)
			{
				if (!(
					IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
					IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {
					SetSlotIndicator(sender.getSlot(), false);
					SetLineIndicator(lineUsing * 2, false);
					SetSlotIndicator(recver.getSlot(), false);
					SetLineIndicator(lineUsing * 2 + 1, false);
					yield return new WaitForSeconds (1.0f);
					goto retry;
				}

				blinking = !blinking;
				SetSlotIndicator(sender.getSlot(), blinking);
				SetLineIndicator(lineUsing * 2, blinking);
				SetSlotIndicator(recver.getSlot(), !blinking);
				SetLineIndicator(lineUsing * 2 + 1, !blinking);
				yield return new WaitForSeconds(0.1f);
			}
		}
		SetSlotIndicator(sender.getSlot(), true);
		SetLineIndicator(lineUsing * 2, true);
		SetSlotIndicator(recver.getSlot(), true);
		SetLineIndicator(lineUsing * 2 + 1, true);

		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			SetSlotIndicator(sender.getSlot(), false);
			SetLineIndicator(lineUsing * 2, false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
			SetSlotIndicator(recver.getSlot(), false);
			SetLineIndicator(lineUsing * 2+1, false);
		}
		else
		{
			SetSlotIndicator(recver.getSlot(), false);
			SetLineIndicator(lineUsing * 2 + 1, false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
			SetSlotIndicator(sender.getSlot(), false);
			SetLineIndicator(lineUsing * 2, false);
		}
		yield return new WaitForSeconds (1);
		goalCount -= 1;

		isCalling.Remove (recver.getSlot ());
		isCalling.Remove (sender.getSlot ());
	}

	void ClearLED(Person sender, Person recver, int lineUsing)
	{
		SetSlotIndicator(sender.getSlot(), false);
		SetLineIndicator(lineUsing * 2, false);
		SetSlotIndicator(recver.getSlot(), false);
		SetLineIndicator(lineUsing * 2 + 1, false);
	}

	IEnumerator normalCall(object[] args)
	{
		float secondsToBegin = (float)args[1];
		//int slot = (int)args[1];
		Person sender = (Person)args[2];
		int slot = sender.getSlot();
		Person recver = (Person)args[3];
		Debug.Log ("what is " + args [4]);
		int opponentWaiting = (int)args[4];
		int callDuration = (int)args[5];

		isCalling [sender.getSlot ()] = true;
		isCalling [recver.getSlot ()] = true;

		yield return new WaitForSeconds(secondsToBegin-Time.time+dayBeginTime);

		SetSlotIndicator(slot, true);

		int lineUsing = -1;
		var lineSet = GameObject.Find("LineSetArray").GetComponent<LineSetScript>();

		var waitStart = Time.time;

		while(true)
		{
			bool found = false;
			for(int i = 0; i < lineSet.N; i ++)
			{
				//Debug.Log(i + " " + LineSetScript.instance.IsSwitchOperatorOn(i) + " " + IsCableConnected(i*2, slot));
				if (LineSetScript.instance.IsSwitchOperatorOn(i) && IsCableConnected(i*2, slot))
				{
					found = true;
					lineUsing = i;
					//		Debug.Log("lineUsing " + i);
					SetLineIndicator(2*i, true);
					break;
				}
			}
			if (found)
				break;
			yield return new WaitForSeconds(0.1f);
			if (Time.time - waitStart > 30*dayScale) {
				// out of time
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				SetSlotIndicator (slot, false);
				goalCount -= 1;

				yield break;;
			}
		}

		bool choiced = false;

		ChatManager.instance.printChoiceChat("", "음...", "안녕하세요 교환원입니다.", ()=> {
			ChatManager.instance.RemoveLastChat();
			ChatManager.instance.printChat("음...");
			Title.Instance.life -= 50;
			choiced = true;

		}, ()=>{
			ChatManager.instance.RemoveLastChat();
			ChatManager.instance.printChat("안녕하세요 교환원입니다. 누구를 찾으시나요?");
			choiced = true;
		});
		for (int i = 0; i < 5; i++) {
			yield return new WaitForSeconds (1.0f);
			if (!(LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()))) {
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				if (!choiced) {
					ChatManager.instance.RemoveLastChat ();
				}
				goalCount -= 1;

				yield break;
			}
			if (choiced) {
				yield return new WaitForSeconds (1.0f);
				break;
			}
		}
		if (!choiced) {
			ChatManager.instance.RemoveLastChat ();
			Title.Instance.life -= 50;
		}
		ChatManager.instance.printChat(recver.getName() + " 바꿔주세요.", sender);

		Debug.Log("print chat");

		waitStart = Time.time;
		while(true)
		{
			//Debug.Log("line valid " + LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) + " " + IsCableConnected(lineUsing * 2 + 1, recver.getSlot()));

			if (!(LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()))) {
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				goalCount -= 1;

				yield break;
			}
			if (Time.time - waitStart > 30*dayScale) {
				// out of time
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				goalCount -= 1;

				yield break;;
			}

			if (LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) && IsCableConnected(lineUsing * 2+1, recver.getSlot()))
				break;
			yield return new WaitForSeconds(0.1f);
		}

		Debug.Log("connect correct line");

		{
			var currentTime = Time.time;
			while(Time.time - currentTime < opponentWaiting*dayScale)
			{
				if (!(
					LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
					LineSetScript.instance.IsSwitchTelephoneOn (lineUsing) && IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {
					yield return new WaitForSeconds (1.0f);
					ClearLED (sender, recver, lineUsing);
					Title.Instance.life -= 230;
					isCalling [sender.getSlot ()] = false;
					isCalling [recver.getSlot ()] = false;
					goalCount -= 1;

					yield break;
				}
				Debug.Log("wait opponent " + (Time.time - currentTime));
				yield return new WaitForSeconds(0.1f);
			}
		}

		Debug.Log("wait done");

		SetSlotIndicator(recver.getSlot(), true);
		SetLineIndicator(2 * lineUsing + 1, true);

		while (true)
		{
			if (!(
				IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
				IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {

				yield return new WaitForSeconds (1.0f);
				ClearLED (sender, recver, lineUsing);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				goalCount -= 1;

				yield break;
			}
			if (!LineSetScript.instance.IsSwitchTelephoneOn(lineUsing))
				break;
			yield return new WaitForSeconds(0.1f);
		}

		{
            SoundManager.instance.transmissionSound();


            var currentTime = Time.time;
			bool blinking = true;
			while (Time.time - currentTime < callDuration*dayScale)
			{
				if (!(
					IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
					IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {

					yield return new WaitForSeconds (1.0f);
					ClearLED (sender, recver, lineUsing);
					Title.Instance.life -= 230;
					isCalling [sender.getSlot ()] = false;
					isCalling [recver.getSlot ()] = false;
					goalCount -= 1;

					yield break;
				}

				blinking = !blinking;
				SetSlotIndicator(sender.getSlot(), blinking);
				SetLineIndicator(lineUsing * 2, blinking);
				SetSlotIndicator(recver.getSlot(), !blinking);
				SetLineIndicator(lineUsing * 2 + 1, !blinking);
				yield return new WaitForSeconds(0.1f);
			}
		}
		SetSlotIndicator(sender.getSlot(), true);
		SetLineIndicator(lineUsing * 2, true);
		SetSlotIndicator(recver.getSlot(), true);
		SetLineIndicator(lineUsing * 2 + 1, true);

		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			SetSlotIndicator(sender.getSlot(), false);
			SetLineIndicator(lineUsing * 2, false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
			SetSlotIndicator(recver.getSlot(), false);
			SetLineIndicator(lineUsing * 2+1, false);
		}
		else
		{
			SetSlotIndicator(recver.getSlot(), false);
			SetLineIndicator(lineUsing * 2 + 1, false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
			SetSlotIndicator(sender.getSlot(), false);
			SetLineIndicator(lineUsing * 2, false);
		}
		yield return new WaitForSeconds (1);
		goalCount -= 1;

		isCalling.Remove (recver.getSlot ());
		isCalling.Remove (sender.getSlot ());
	}

	IEnumerator mustacheCall(object[] args)
	{
		float secondsToBegin = (float)args[1];
		//int slot = (int)args[1];
		Person sender = (Person)args[2];
		int slot = sender.getSlot();
		Person recver = (Person)args[3];
		Debug.Log ("what is " + args [4]);
		int opponentWaiting = (int)args[4];
		int callDuration = (int)args[5];

		isCalling [sender.getSlot ()] = true;
		isCalling [recver.getSlot ()] = true;

		yield return new WaitForSeconds(secondsToBegin-Time.time+dayBeginTime);

		SetSlotIndicator(slot, true);

		int lineUsing = -1;
		var lineSet = GameObject.Find("LineSetArray").GetComponent<LineSetScript>();

		var waitStart = Time.time;

		while(true)
		{
			bool found = false;
			for(int i = 0; i < lineSet.N; i ++)
			{
				//Debug.Log(i + " " + LineSetScript.instance.IsSwitchOperatorOn(i) + " " + IsCableConnected(i*2, slot));
				if (LineSetScript.instance.IsSwitchOperatorOn(i) && IsCableConnected(i*2, slot))
				{
					found = true;
					lineUsing = i;
					//		Debug.Log("lineUsing " + i);
					SetLineIndicator(2*i, true);
					break;
				}
			}
			if (found)
				break;
			yield return new WaitForSeconds(0.1f);
			if (Time.time - waitStart > 30*dayScale) {
				// out of time
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				SetSlotIndicator (slot, false);
				goalCount -= 1;

				yield break;;
			}
		}

		bool choiced = false;

		ChatManager.instance.printChoiceChat("", "음...", "안녕하세요 교환원입니다.", ()=> {
			ChatManager.instance.RemoveLastChat();
			ChatManager.instance.printChat("음...");
			Title.Instance.life -= 50;
			choiced = true;

		}, ()=>{
			ChatManager.instance.RemoveLastChat();
			ChatManager.instance.printChat("안녕하세요 교환원입니다. 누구를 찾으시나요?");
			choiced = true;
		});
		for (int i = 0; i < 5; i++) {
			yield return new WaitForSeconds (1.0f);
			if (!(LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()))) {
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				if (!choiced) {
					ChatManager.instance.RemoveLastChat ();
				}
				goalCount -= 1;

				yield break;
			}
			if (choiced) {
				yield return new WaitForSeconds (1.0f);
				break;
			}
		}
		if (!choiced) {
			ChatManager.instance.RemoveLastChat ();
			Title.Instance.life -= 50;
		}

		ChatManager.instance.printChat ("수염쟁이 김씨 바꿔줘.", sender);

		choiced = false;
		bool needSendName = false;

		ChatManager.instance.printChoiceChat("", "혹시 그 분 성함을 알려줄 수 있으신가요?", "네, 조금만 기다려주세요.", ()=> {
			ChatManager.instance.RemoveLastChat();
			needSendName = true;
			choiced = true;

		}, ()=>{
			ChatManager.instance.RemoveLastChat();
			choiced = true;
		});
		for (int i = 0; i < 5; i++) {
			yield return new WaitForSeconds (1.0f);
			if (!(LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()))) {
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				if (!choiced) {
					ChatManager.instance.RemoveLastChat ();
				}
				goalCount -= 1;

				yield break;
			}
			if (choiced) {
				yield return new WaitForSeconds (1.0f);
				break;
			}
		}
		if (!choiced) {
			ChatManager.instance.RemoveLastChat ();
		}
		if (needSendName) {
			yield return new WaitForSeconds (2.0f);
			ChatManager.instance.printChat("그 사람 이름은 "+recver.getName() + " 이야.", sender);
		}

		Debug.Log("print chat");

		waitStart = Time.time;
		while(true)
		{
			//Debug.Log("line valid " + LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) + " " + IsCableConnected(lineUsing * 2 + 1, recver.getSlot()));

			if (!(LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()))) {
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				goalCount -= 1;

				yield break;
			}
			if (Time.time - waitStart > 30*dayScale) {
				// out of time
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				goalCount -= 1;

				yield break;;
			}
			if (LineSetScript.instance.IsSwitchTelephoneOn (lineUsing) && !IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()) &&
			    cableEndConnections.ContainsKey (lineUsing * 2 + 1)) {

				// 잘못 연결
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				goalCount -= 1;

				yield break;
			}
			if (LineSetScript.instance.IsSwitchTelephoneOn (lineUsing) && !IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()) &&
				cableEndConnections.ContainsKey (lineUsing * 2 + 1)) {

				// 잘못 연결
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				goalCount -= 1;

				yield break;
			}

			if (LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) && IsCableConnected(lineUsing * 2+1, recver.getSlot()))
				break;
			yield return new WaitForSeconds(0.1f);
		}

		Debug.Log("connect correct line");

		{
			var currentTime = Time.time;
			while(Time.time - currentTime < opponentWaiting*dayScale)
			{
				if (!(
					LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
					LineSetScript.instance.IsSwitchTelephoneOn (lineUsing) && IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {
					yield return new WaitForSeconds (1.0f);
					ClearLED (sender, recver, lineUsing);
					Title.Instance.life -= 230;
					isCalling [sender.getSlot ()] = false;
					isCalling [recver.getSlot ()] = false;
					goalCount -= 1;

					yield break;
				}
				Debug.Log("wait opponent " + (Time.time - currentTime));
				yield return new WaitForSeconds(0.1f);
			}
		}

		Debug.Log("wait done");

		SetSlotIndicator(recver.getSlot(), true);
		SetLineIndicator(2 * lineUsing + 1, true);

		while (true)
		{
			if (!(
				IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
				IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {

				yield return new WaitForSeconds (1.0f);
				ClearLED (sender, recver, lineUsing);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				goalCount -= 1;

				yield break;
			}
			if (!LineSetScript.instance.IsSwitchTelephoneOn(lineUsing))
				break;
			yield return new WaitForSeconds(0.1f);
		}

		{

            SoundManager.instance.transmissionSound();

            var currentTime = Time.time;
			bool blinking = true;
			while (Time.time - currentTime < callDuration*dayScale)
			{
				if (!(
					IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
					IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {

					yield return new WaitForSeconds (1.0f);
					ClearLED (sender, recver, lineUsing);
					Title.Instance.life -= 230;
					isCalling [sender.getSlot ()] = false;
					isCalling [recver.getSlot ()] = false;
					goalCount -= 1;

					yield break;
				}

				blinking = !blinking;
				SetSlotIndicator(sender.getSlot(), blinking);
				SetLineIndicator(lineUsing * 2, blinking);
				SetSlotIndicator(recver.getSlot(), !blinking);
				SetLineIndicator(lineUsing * 2 + 1, !blinking);
				yield return new WaitForSeconds(0.1f);
			}
		}
		SetSlotIndicator(sender.getSlot(), true);
		SetLineIndicator(lineUsing * 2, true);
		SetSlotIndicator(recver.getSlot(), true);
		SetLineIndicator(lineUsing * 2 + 1, true);

		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			SetSlotIndicator(sender.getSlot(), false);
			SetLineIndicator(lineUsing * 2, false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
			SetSlotIndicator(recver.getSlot(), false);
			SetLineIndicator(lineUsing * 2+1, false);
		}
		else
		{
			SetSlotIndicator(recver.getSlot(), false);
			SetLineIndicator(lineUsing * 2 + 1, false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
			SetSlotIndicator(sender.getSlot(), false);
			SetLineIndicator(lineUsing * 2, false);
		}
		yield return new WaitForSeconds (1);
		goalCount -= 1;

		isCalling.Remove (recver.getSlot ());
		isCalling.Remove (sender.getSlot ());
	}

	IEnumerator importantCall(object[] args)
	{
		float secondsToBegin = (float)args[1];
		//int slot = (int)args[1];
		Person sender = (Person)args[2];
		int slot = sender.getSlot();
		Person recver = (Person)args[3];
		Debug.Log ("what is " + args [4]);
		int opponentWaiting = (int)args[4];
		int callDuration = (int)args[5];

		isCalling [sender.getSlot ()] = true;
		isCalling [recver.getSlot ()] = true;

		yield return new WaitForSeconds(secondsToBegin-Time.time+dayBeginTime);

		SetSlotIndicator(slot, true);

		int lineUsing = -1;
		var lineSet = GameObject.Find("LineSetArray").GetComponent<LineSetScript>();

		var waitStart = Time.time;

		while(true)
		{
			bool found = false;
			for(int i = 0; i < lineSet.N; i ++)
			{
				//Debug.Log(i + " " + LineSetScript.instance.IsSwitchOperatorOn(i) + " " + IsCableConnected(i*2, slot));
				if (LineSetScript.instance.IsSwitchOperatorOn(i) && IsCableConnected(i*2, slot))
				{
					found = true;
					lineUsing = i;
					//		Debug.Log("lineUsing " + i);
					SetLineIndicator(2*i, true);
					break;
				}
			}
			if (found)
				break;
			yield return new WaitForSeconds(0.1f);
			if (Time.time - waitStart > 30*dayScale) {
				// out of time
				Title.Instance.life -= 460;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				SetSlotIndicator (slot, false);
				goalCount -= 1;

				yield break;;
			}
		}

		bool choiced = false;

		ChatManager.instance.printChoiceChat("", "음...", "안녕하세요 교환원입니다.", ()=> {
			ChatManager.instance.RemoveLastChat();
			ChatManager.instance.printChat("음...");
			Title.Instance.life -= 100;
			choiced = true;

		}, ()=>{
			ChatManager.instance.RemoveLastChat();
			ChatManager.instance.printChat("안녕하세요 교환원입니다. 누구를 찾으시나요?");
			choiced = true;
		});
		for (int i = 0; i < 5; i++) {
			yield return new WaitForSeconds (1.0f);
			if (!(LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()))) {
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 460;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				if (!choiced) {
					ChatManager.instance.RemoveLastChat ();
				}
				goalCount -= 1;

				yield break;
			}
			if (choiced) {
				yield return new WaitForSeconds (1.0f);
				break;
			}
		}
		if (!choiced) {
			ChatManager.instance.RemoveLastChat ();
			Title.Instance.life -= 100;
		}
		ChatManager.instance.printChat(recver.getName() + " 바꿔주게나.", sender);

		Debug.Log("print chat");

		waitStart = Time.time;
		while(true)
		{
			//Debug.Log("line valid " + LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) + " " + IsCableConnected(lineUsing * 2 + 1, recver.getSlot()));

			if (!(LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()))) {
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 460;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				goalCount -= 1;

				yield break;
			}
			if (Time.time - waitStart > 30*dayScale) {
				// out of time
				Title.Instance.life -= 460;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				goalCount -= 1;

				yield break;;
			}
			if (LineSetScript.instance.IsSwitchTelephoneOn (lineUsing) && !IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()) &&
				cableEndConnections.ContainsKey (lineUsing * 2 + 1)) {

				// 잘못 연결
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				goalCount -= 1;

				yield break;
			}

			if (LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) && IsCableConnected(lineUsing * 2+1, recver.getSlot()))
				break;
			yield return new WaitForSeconds(0.1f);
		}

		Debug.Log("connect correct line");

		{
			var currentTime = Time.time;
			while(Time.time - currentTime < opponentWaiting*dayScale)
			{
				if (!(
					LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
					LineSetScript.instance.IsSwitchTelephoneOn (lineUsing) && IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {
					yield return new WaitForSeconds (1.0f);
					ClearLED (sender, recver, lineUsing);
					Title.Instance.life -= 460;
					isCalling [sender.getSlot ()] = false;
					isCalling [recver.getSlot ()] = false;
					goalCount -= 1;

					yield break;
				}
				Debug.Log("wait opponent " + (Time.time - currentTime));
				yield return new WaitForSeconds(0.1f);
			}
		}

		Debug.Log("wait done");

		SetSlotIndicator(recver.getSlot(), true);
		SetLineIndicator(2 * lineUsing + 1, true);

		while (true)
		{
			if (!(
				IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
				IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {

				yield return new WaitForSeconds (1.0f);
				ClearLED (sender, recver, lineUsing);
				Title.Instance.life -= 460;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				goalCount -= 1;

				yield break;
			}
			if (!LineSetScript.instance.IsSwitchTelephoneOn(lineUsing))
				break;
			yield return new WaitForSeconds(0.1f);
		}

		{
            SoundManager.instance.transmissionSound();


            var currentTime = Time.time;
			bool blinking = true;
			while (Time.time - currentTime < callDuration*dayScale)
			{
				if (!(
					IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
					IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {

					yield return new WaitForSeconds (1.0f);
					ClearLED (sender, recver, lineUsing);
					Title.Instance.life -= 460;
					isCalling [sender.getSlot ()] = false;
					isCalling [recver.getSlot ()] = false;
					goalCount -= 1;

					yield break;
				}

				blinking = !blinking;
				SetSlotIndicator(sender.getSlot(), blinking);
				SetLineIndicator(lineUsing * 2, blinking);
				SetSlotIndicator(recver.getSlot(), !blinking);
				SetLineIndicator(lineUsing * 2 + 1, !blinking);
				yield return new WaitForSeconds(0.1f);
			}
		}
		SetSlotIndicator(sender.getSlot(), true);
		SetLineIndicator(lineUsing * 2, true);
		SetSlotIndicator(recver.getSlot(), true);
		SetLineIndicator(lineUsing * 2 + 1, true);

		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			SetSlotIndicator(sender.getSlot(), false);
			SetLineIndicator(lineUsing * 2, false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
			SetSlotIndicator(recver.getSlot(), false);
			SetLineIndicator(lineUsing * 2+1, false);
		}
		else
		{
			SetSlotIndicator(recver.getSlot(), false);
			SetLineIndicator(lineUsing * 2 + 1, false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
			SetSlotIndicator(sender.getSlot(), false);
			SetLineIndicator(lineUsing * 2, false);
		}
		yield return new WaitForSeconds (1);
		goalCount -= 1;

		isCalling.Remove (recver.getSlot ());
		isCalling.Remove (sender.getSlot ());
	}

	IEnumerator angryCall(object[] args)
	{
		float secondsToBegin = (float)args[1];
		//int slot = (int)args[1];
		Person sender = (Person)args[2];
		int slot = sender.getSlot();
		Person recver = (Person)args[3];
		int opponentWaiting = (int)args[4];
		int callDuration = (int)args[5];

		isCalling [sender.getSlot ()] = true;
		isCalling [recver.getSlot ()] = true;

		yield return new WaitForSeconds(secondsToBegin-Time.time+dayBeginTime);

		SetSlotIndicator(slot, true);

		int lineUsing = -1;
		var lineSet = GameObject.Find("LineSetArray").GetComponent<LineSetScript>();

		var waitStart = Time.time;

		while(true)
		{
			bool found = false;
			for(int i = 0; i < lineSet.N; i ++)
			{
				//Debug.Log(i + " " + LineSetScript.instance.IsSwitchOperatorOn(i) + " " + IsCableConnected(i*2, slot));
				if (LineSetScript.instance.IsSwitchOperatorOn(i) && IsCableConnected(i*2, slot))
				{
					found = true;
					lineUsing = i;
					//		Debug.Log("lineUsing " + i);
					SetLineIndicator(2*i, true);
					break;
				}
			}
			if (found)
				break;
			yield return new WaitForSeconds(0.1f);
			if (Time.time - waitStart > 10*dayScale) {
				// out of time
				Title.Instance.life -= 180;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				SetSlotIndicator (slot, false);

				goalCount -= 1;

				yield break;;
			}
		}

		ChatManager.instance.printChat("빨리 " + recver.getName() + " 바꿔줘!", sender);

		Debug.Log("print chat");

		waitStart = Time.time;
		while(true)
		{
			//Debug.Log("line valid " + LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) + " " + IsCableConnected(lineUsing * 2 + 1, recver.getSlot()));

			if (!(LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()))) {
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 200;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				goalCount -= 1;

				yield break;
			}
			if (Time.time - waitStart > 20*dayScale+opponentWaiting*dayScale) {
				// out of time
				Title.Instance.life -= 180;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				goalCount -= 1;

				yield break;;
			}

			if (LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) && IsCableConnected(lineUsing * 2+1, recver.getSlot()))
				break;
			yield return new WaitForSeconds(0.1f);
		}

		Debug.Log("connect correct line");

		{
			var currentTime = Time.time;
			while(Time.time - currentTime < opponentWaiting*dayScale)
			{
				if (!(
					LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
					LineSetScript.instance.IsSwitchTelephoneOn (lineUsing) && IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {
					yield return new WaitForSeconds (1.0f);
					ClearLED (sender, recver, lineUsing);
					Title.Instance.life -= 200;
					isCalling [sender.getSlot ()] = false;
					isCalling [recver.getSlot ()] = false;
					goalCount -= 1;

					yield break;
				}
				Debug.Log("wait opponent " + (Time.time - currentTime));
				yield return new WaitForSeconds(0.1f);
			}
		}

		Debug.Log("wait done");

		SetSlotIndicator(recver.getSlot(), true);
		SetLineIndicator(2 * lineUsing + 1, true);

		while (true)
		{
			if (!(
				IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
				IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {

				yield return new WaitForSeconds (1.0f);
				ClearLED (sender, recver, lineUsing);
				Title.Instance.life -= 200;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				goalCount -= 1;

				yield break;
			}
			if (!LineSetScript.instance.IsSwitchTelephoneOn(lineUsing))
				break;
			yield return new WaitForSeconds(0.1f);
		}

		{
            SoundManager.instance.transmissionSound();


            var currentTime = Time.time;
			bool blinking = true;
			while (Time.time - currentTime < callDuration*dayScale)
			{
				if (!(
					IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
					IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {

					yield return new WaitForSeconds (1.0f);
					ClearLED (sender, recver, lineUsing);
					Title.Instance.life -= 200;
					isCalling [sender.getSlot ()] = false;
					isCalling [recver.getSlot ()] = false;
					goalCount -= 1;

					yield break;
				}

				blinking = !blinking;
				SetSlotIndicator(sender.getSlot(), blinking);
				SetLineIndicator(lineUsing * 2, blinking);
				SetSlotIndicator(recver.getSlot(), !blinking);
				SetLineIndicator(lineUsing * 2 + 1, !blinking);
				yield return new WaitForSeconds(0.1f);
			}
		}
		SetSlotIndicator(sender.getSlot(), true);
		SetLineIndicator(lineUsing * 2, true);
		SetSlotIndicator(recver.getSlot(), true);
		SetLineIndicator(lineUsing * 2 + 1, true);

		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			SetSlotIndicator(sender.getSlot(), false);
			SetLineIndicator(lineUsing * 2, false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
			SetSlotIndicator(recver.getSlot(), false);
			SetLineIndicator(lineUsing * 2+1, false);
		}
		else
		{
			SetSlotIndicator(recver.getSlot(), false);
			SetLineIndicator(lineUsing * 2 + 1, false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
			SetSlotIndicator(sender.getSlot(), false);
			SetLineIndicator(lineUsing * 2, false);
		}
		yield return new WaitForSeconds (1);
		goalCount -= 1;

		isCalling.Remove (recver.getSlot ());
		isCalling.Remove (sender.getSlot ());
	}

	IEnumerator numberCall(object[] args)
	{
		float secondsToBegin = (float)args[1];
		//int slot = (int)args[1];
		Person sender = (Person)args[2];
		int slot = sender.getSlot();
		Person recver = (Person)args[3];
		int opponentWaiting = (int)args[4];
		int callDuration = (int)args[5];

		isCalling [sender.getSlot ()] = true;
		isCalling [recver.getSlot ()] = true;

		yield return new WaitForSeconds(secondsToBegin-Time.time+dayBeginTime);

		SetSlotIndicator(slot, true);

		int lineUsing = -1;
		var lineSet = GameObject.Find("LineSetArray").GetComponent<LineSetScript>();

		var waitStart = Time.time;

		while(true)
		{
			bool found = false;
			for(int i = 0; i < lineSet.N; i ++)
			{
				//Debug.Log(i + " " + LineSetScript.instance.IsSwitchOperatorOn(i) + " " + IsCableConnected(i*2, slot));
				if (LineSetScript.instance.IsSwitchOperatorOn(i) && IsCableConnected(i*2, slot))
				{
					found = true;
					lineUsing = i;
					//		Debug.Log("lineUsing " + i);
					SetLineIndicator(2*i, true);
					break;
				}
			}
			if (found)
				break;
			yield return new WaitForSeconds(0.1f);
			if (Time.time - waitStart > 30*dayScale) {
				// out of time
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				SetSlotIndicator (slot, false);

				goalCount -= 1;

				yield break;
			}
		}

		bool choiced = false;

		ChatManager.instance.printChoiceChat("", "음...", "안녕하세요 교환원입니다.", ()=> {
			ChatManager.instance.RemoveLastChat();
			ChatManager.instance.printChat("음...");
			Title.Instance.life -= 50;
			choiced = true;

		}, ()=>{
			ChatManager.instance.RemoveLastChat();
			ChatManager.instance.printChat("안녕하세요 교환원입니다. 누구를 찾으시나요?");
			choiced = true;
		});
		for (int i = 0; i < 5; i++) {
			yield return new WaitForSeconds (1.0f);
			if (!(LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()))) {
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				if (!choiced) {
					ChatManager.instance.RemoveLastChat ();
				}
				goalCount -= 1;

				yield break;
			}
			if (choiced) {
				yield return new WaitForSeconds (1.0f);
				break;
			}
		}
		if (!choiced) {
			ChatManager.instance.RemoveLastChat ();
			Title.Instance.life -= 50;
		}
		ChatManager.instance.printChat("전화번호 " + recver.getNumber().Substring(recver.getNumber().Length-4) + " 바꿔줘요.", sender);

		Debug.Log("print chat");

		while(true)
		{
			//Debug.Log("line valid " + LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) + " " + IsCableConnected(lineUsing * 2 + 1, recver.getSlot()));

			if (!(LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()))) {
				yield return new WaitForSeconds (1.0f);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				ClearLED (sender, recver, lineUsing);
				goalCount -= 1;

				yield break;
			}

			if (LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) && IsCableConnected(lineUsing * 2+1, recver.getSlot()))
				break;
			yield return new WaitForSeconds(0.1f);
		}

		Debug.Log("connect correct line");

		{
			var currentTime = Time.time;
			while(Time.time - currentTime < opponentWaiting*dayScale)
			{
				if (!(
					LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
					LineSetScript.instance.IsSwitchTelephoneOn (lineUsing) && IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {
					yield return new WaitForSeconds (1.0f);
					ClearLED (sender, recver, lineUsing);
					Title.Instance.life -= 230;
					isCalling [sender.getSlot ()] = false;
					isCalling [recver.getSlot ()] = false;
					goalCount -= 1;

					yield break;
				}
				Debug.Log("wait opponent " + (Time.time - currentTime));
				yield return new WaitForSeconds(0.1f);
			}
		}

		Debug.Log("wait done");

		SetSlotIndicator(recver.getSlot(), true);
		SetLineIndicator(2 * lineUsing + 1, true);

		while (true)
		{
			if (!(
				IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
				IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {

				yield return new WaitForSeconds (1.0f);
				ClearLED (sender, recver, lineUsing);
				Title.Instance.life -= 230;
				isCalling [sender.getSlot ()] = false;
				isCalling [recver.getSlot ()] = false;
				goalCount -= 1;

				yield break;
			}
			if (!LineSetScript.instance.IsSwitchTelephoneOn(lineUsing))
				break;
			yield return new WaitForSeconds(0.1f);
		}

		{
            SoundManager.instance.transmissionSound();


            var currentTime = Time.time;
			bool blinking = true;
			while (Time.time - currentTime < callDuration*dayScale)
			{
				if (!(
					IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
					IsCableConnected (lineUsing * 2 + 1, recver.getSlot ()))) {

					yield return new WaitForSeconds (1.0f);
					ClearLED (sender, recver, lineUsing);
					Title.Instance.life -= 230;
					isCalling [sender.getSlot ()] = false;
					isCalling [recver.getSlot ()] = false;
					goalCount -= 1;

					yield break;
				}

				blinking = !blinking;
				SetSlotIndicator(sender.getSlot(), blinking);
				SetLineIndicator(lineUsing * 2, blinking);
				SetSlotIndicator(recver.getSlot(), !blinking);
				SetLineIndicator(lineUsing * 2 + 1, !blinking);
				yield return new WaitForSeconds(0.1f);
			}
		}
		SetSlotIndicator(sender.getSlot(), true);
		SetLineIndicator(lineUsing * 2, true);
		SetSlotIndicator(recver.getSlot(), true);
		SetLineIndicator(lineUsing * 2 + 1, true);

		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			SetSlotIndicator(sender.getSlot(), false);
			SetLineIndicator(lineUsing * 2, false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
			SetSlotIndicator(recver.getSlot(), false);
			SetLineIndicator(lineUsing * 2+1, false);
		}
		else
		{
			SetSlotIndicator(recver.getSlot(), false);
			SetLineIndicator(lineUsing * 2 + 1, false);
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
			SetSlotIndicator(sender.getSlot(), false);
			SetLineIndicator(lineUsing * 2, false);
		}
		yield return new WaitForSeconds (1);
		goalCount -= 1;

		isCalling.Remove (recver.getSlot ());
		isCalling.Remove (sender.getSlot ());
	}

    IEnumerator normalCall2(object[] args)
    {
        float secondsToBegin = (float)args[0];
        //int slot = (int)args[1];
        Person sender = (Person)args[1];
        int slot = sender.getSlot();
        Person recver = (Person)args[2];
        string msg = (string)args[3];
        int opponentWaiting = (int)args[4];
        int callDuration = (int)args[5];
        yield return new WaitForSeconds(secondsToBegin);
        SetSlotIndicator(slot, true);

        int lineUsing = -1;
        var lineSet = GameObject.Find("LineSetArray").GetComponent<LineSetScript>();
        while (true)
        {
            bool found = false;
            for (int i = 0; i < lineSet.N; i++)
            {
                //Debug.Log(i + " " + LineSetScript.instance.IsSwitchOperatorOn(i) + " " + IsCableConnected(i*2, slot));
                if (LineSetScript.instance.IsSwitchOperatorOn(i) && IsCableConnected(i * 2, slot))
                {
                    found = true;
                    lineUsing = i;
                    Debug.Log("lineUsing " + i);
                    SetLineIndicator(2 * i, true);
                    break;
                }
            }
            if (found)
                break;
            yield return new WaitForSeconds(0.1f);
        }

        ChatManager.instance.printChoiceChat("", "음...", "안녕하세요 교환원입니다.", callback1, callback2);
        yield return new WaitForSeconds(1.0f);
        ChatManager.instance.printChat(msg, sender);


        Debug.Log("print chat");

        while (true)
        {
            Debug.Log("line valid "      + LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) + " " + IsCableConnected(lineUsing * 2 + 1, recver.getSlot()));

            if (LineSetScript.instance.IsSwitchTelephoneOn(lineUsing) && IsCableConnected(lineUsing * 2 + 1, recver.getSlot()))
                break;
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("connect correct line");

        {
            var currentTime = Time.time;
            while (Time.time - currentTime < opponentWaiting*dayScale)
            {
                Debug.Log("wait opponent " + (Time.time - currentTime));
                yield return new WaitForSeconds(0.1f);
            }
        }

        Debug.Log("wait done");

        SetSlotIndicator(recver.getSlot(), true);
        SetLineIndicator(2 * lineUsing + 1, true);

        while (true)
        {
            if (!LineSetScript.instance.IsSwitchTelephoneOn(lineUsing))
                break;
            yield return new WaitForSeconds(0.1f);
        }

        {
            SoundManager.instance.transmissionSound();


            var currentTime = Time.time;
            bool blinking = true;
			while (Time.time - currentTime < callDuration*dayScale)
            {
                blinking = !blinking;
                SetSlotIndicator(sender.getSlot(), blinking);
                SetLineIndicator(lineUsing * 2, blinking);
                SetSlotIndicator(recver.getSlot(), !blinking);
                SetLineIndicator(lineUsing * 2 + 1, !blinking);
                yield return new WaitForSeconds(0.1f);
            }
        }
        SetSlotIndicator(sender.getSlot(), true);
        SetLineIndicator(lineUsing * 2, true);
        SetSlotIndicator(recver.getSlot(), true);
        SetLineIndicator(lineUsing * 2 + 1, true);

        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            SetSlotIndicator(sender.getSlot(), false);
            SetLineIndicator(lineUsing * 2, false);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
            SetSlotIndicator(recver.getSlot(), false);
            SetLineIndicator(lineUsing * 2 + 1, false);
        }
        else
        {
            SetSlotIndicator(recver.getSlot(), false);
            SetLineIndicator(lineUsing * 2 + 1, false);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.8f));
            SetSlotIndicator(sender.getSlot(), false);
            SetLineIndicator(lineUsing * 2, false);
        }
    }

	float dayBeginTime;
	int goalCount = -1;
	float dayScale = 1;
    private void InitGame()
    {
		dayBeginTime = Time.time;
		string[] comments = new string[] { " 바꿔주세요.", " 바꿔줘요.", " 바꿔 주십시오." };
		if (Title.Instance.gameStage < 1)
			Title.Instance.gameStage = 1;
		if (Title.Instance.gameStage == 1) {
			PeopleManager.instance.addPerson (new Person ("김개똥", "남성", "010-2737-1928", "1A"));
			PeopleManager.instance.addPerson (new Person ("양창무", "남성", "010-5236-4432", "2B"));
			PeopleManager.instance.addPerson (new Person ("류연아", "여성", "010-3324-8477", "1B"));

			dayScale = 1;
			const int NumCalls = 3;
			goalCount = NumCalls;

			calls.Clear ();
			calls.Add (new object[] {
				"tutorialCall",
				10f, 
				PeopleManager.instance.people [0],
				PeopleManager.instance.people [1],
				PeopleManager.instance.people [1].getName () + comments [UnityEngine.Random.Range (0, comments.Length)],
				5, 10
			});
			calls.Add (new object[] {
				"tutorialCall",
				40f, 
				PeopleManager.instance.people [1],
				PeopleManager.instance.people [2],
				PeopleManager.instance.people [2].getName () + comments [UnityEngine.Random.Range (0, comments.Length)],
				5, 10
			});
			calls.Add (new object[] {
				"tutorialCall",
				70f, 
				PeopleManager.instance.people [2],
				PeopleManager.instance.people [0],
				PeopleManager.instance.people [0].getName () + comments [UnityEngine.Random.Range (0, comments.Length)],
				5, 10
			});
			return;

//			StartCoroutine("tutorialCall", 
//				new object[] { startTime, sender, recver, 
//					recver.getName() + comments[UnityEngine.Random.Range(0, comments.Length)], 
//				opponentWait, duration });
		} else if (Title.Instance.gameStage == 4) {
			dayScale = 1 / Mathf.Sqrt (Title.Instance.gameDay/2f);
			PeopleManager.instance.addPerson (new Person ("김개똥", "남성", "010-2737-1928", "1A"));
			PeopleManager.instance.addPerson (new Person ("양창무", "남성", "010-5236-4432", "1B"));
			PeopleManager.instance.addPerson (new Person ("류연아", "여성", "010-3324-8477", "1C"));
			PeopleManager.instance.addPerson (new Person ("신범길", "남성", "010-3324-8477", "1D"));
			PeopleManager.instance.addPerson (new Person ("구환재", "남성", "010-3324-8477", "1E", "성질이 급하다"));
			PeopleManager.instance.addPerson (new Person ("김다현", "여성", "010-3324-8477", "1F", "중요한 분"));
			PeopleManager.instance.addPerson (new Person ("김다정", "여성", "010-2737-1928", "2A"));
			PeopleManager.instance.addPerson (new Person ("이석주", "남성", "010-5236-4432", "2B"));
			PeopleManager.instance.addPerson (new Person ("안명은", "여성", "010-3324-8477", "2C"));
			//PeopleManager.instance.addPerson (new Person ("류연아", "여성", "010-3324-8477", "2D"));
			PeopleManager.instance.addPerson (new Person ("어수연", "여성", "010-3324-8477", "2E", "성질이 급하다"));
			PeopleManager.instance.addPerson (new Person ("박예린", "여성", "010-3324-8477", "2F"));
			PeopleManager.instance.addPerson (new Person ("조성환", "남성", "010-2737-1928", "3A"));
			PeopleManager.instance.addPerson (new Person ("김철오", "남성", "010-5236-4432", "3B", "멋진 턱수염이 있다"));
			PeopleManager.instance.addPerson (new Person ("강나예", "여성", "010-3324-8477", "3C"));
			PeopleManager.instance.addPerson (new Person ("문보민", "남성", "010-3324-8477", "3D"));
			PeopleManager.instance.addPerson (new Person ("강금호", "남성", "010-5236-4432", "3E", "성질이 급하다"));
			PeopleManager.instance.addPerson (new Person ("최다연", "여성", "010-3324-8477", "3F"));
			PeopleManager.instance.addPerson (new Person ("김유성", "남성", "010-2737-1928", "4A"));
			//PeopleManager.instance.addPerson (new Person ("박창우", "여성", "010-3324-8477", "4B"));
			PeopleManager.instance.addPerson (new Person ("임신영", "여성", "010-3324-8477", "4C"));
			PeopleManager.instance.addPerson (new Person ("안윤서", "여성", "010-3324-8477", "4D"));
			PeopleManager.instance.addPerson (new Person ("신모준", "남성", "010-5719-8477", "4E", "심한 장난꾸러기")); // trickster
			PeopleManager.instance.addPerson (new Person ("김여진", "여성", "010-3324-8477", "4F"));
			PeopleManager.instance.shuffle ();

			// 11개
			// find kim 1, trick 1, angry 3, normal 7, mustache 1, important 1, 
			float baseTime = 35f;
			float scaledTime = baseTime * dayScale;
			float beginTime = 5/dayScale;
			int PN = PeopleManager.instance.people.Count;
			goalCount = 0;
			for (int i = 0; i < 180/scaledTime; i++) {
				goalCount += 1;
				int pick1 = UnityEngine.Random.Range (0, 16);
				if (pick1 < 3) {
					int pick2 = UnityEngine.Random.Range (0, 4);
					if (pick2 == 0) {
						int pi = UnityEngine.Random.Range (0, PN);
						for (pi = 0; pi < PN; pi++) {
							if (PeopleManager.instance.people [pi].getDescription ().IndexOf ("수염") != -1)
								break;
						}
						int pj = UnityEngine.Random.Range (0, PN);
						while (pi == pj) {
							pj = UnityEngine.Random.Range (0, PN);	
						}
						calls.Add (new object[] {
							"mustacheCall",
//							"findKimCall",
							beginTime, 
							PeopleManager.instance.people [pj],
							PeopleManager.instance.people [pi],
							UnityEngine.Random.Range(3, 7),
							UnityEngine.Random.Range(10, 20)
						});
					} else if (pick2 == 1) {
						int pi = UnityEngine.Random.Range (0, PN);
						for (pi = 0; pi < PN; pi++) {
							if (PeopleManager.instance.people [pi].getDescription ().IndexOf ("중요") != -1)
								break;
						}
						int pj = UnityEngine.Random.Range (0, PN);
						while (pi == pj) {
							pj = UnityEngine.Random.Range (0, PN);	
						}
						calls.Add (new object[] {
							"importantCall",
//							"trickCall",
							beginTime, 
							PeopleManager.instance.people [pi],
							PeopleManager.instance.people [pj],
							UnityEngine.Random.Range(3, 7),
							UnityEngine.Random.Range(10, 20)
						});
					} else if (pick2 == 2) {
						int pi = UnityEngine.Random.Range (0, PN);
						for (pi = 0; pi < PN; pi++) {
							if (PeopleManager.instance.people [pi].getDescription ().IndexOf ("수염") != -1)
								break;
						}
						int pj = UnityEngine.Random.Range (0, PN);
						while (pi == pj) {
							pj = UnityEngine.Random.Range (0, PN);	
						}
						calls.Add (new object[] {
							"mustacheCall",
							beginTime, 
							PeopleManager.instance.people [pj],
							PeopleManager.instance.people [pi],
							UnityEngine.Random.Range(3, 7),
							UnityEngine.Random.Range(10, 20)
						});
					} else {
						int pi = UnityEngine.Random.Range (0, PN);
						for (pi = 0; pi < PN; pi++) {
							if (PeopleManager.instance.people [pi].getDescription ().IndexOf ("중요") != -1)
								break;
						}
						int pj = UnityEngine.Random.Range (0, PN);
						while (pi == pj) {
							pj = UnityEngine.Random.Range (0, PN);	
						}
						calls.Add (new object[] {
							"importantCall",
							beginTime, 
							PeopleManager.instance.people [pi],
							PeopleManager.instance.people [pj],
							UnityEngine.Random.Range(3, 7),
							UnityEngine.Random.Range(25, 30)
						});
					}

				} else if (pick1 < 6) {
					int pi = UnityEngine.Random.Range (0, PN);
					int pj = UnityEngine.Random.Range (0, PN);
					while (pi == pj || PeopleManager.instance.people[pj].getDescription().IndexOf("성질")==-1) {
						pj = UnityEngine.Random.Range (0, PN);	
					}
					calls.Add (new object[] {
						"angryCall",
						beginTime, 
						PeopleManager.instance.people [pj],
						PeopleManager.instance.people [pi],
						UnityEngine.Random.Range(1, 3),
						UnityEngine.Random.Range(5, 10)
					});
				} else if (pick1 < 10) {
					int pi = UnityEngine.Random.Range (0, PN);
					int pj = UnityEngine.Random.Range (0, PN);
					while (pi == pj) {
						pj = UnityEngine.Random.Range (0, PN);	
					}
					calls.Add (new object[] {
						"numberCall",
						beginTime, 
						PeopleManager.instance.people [pi],
						PeopleManager.instance.people [pj],
						UnityEngine.Random.Range(3, 7),
						UnityEngine.Random.Range(10, 20)
					});
				} else {
					int pi = UnityEngine.Random.Range (0, PN);
					int pj = UnityEngine.Random.Range (0, PN);
					while (pi == pj) {
						pj = UnityEngine.Random.Range (0, PN);	
					}
					calls.Add (new object[] {
						"normalCall",
						beginTime, 
						PeopleManager.instance.people [pi],
						PeopleManager.instance.people [pj],
						UnityEngine.Random.Range(3, 7),
						UnityEngine.Random.Range(10, 20)
					});


				}
				beginTime += UnityEngine.Random.Range (15,30) * dayScale;
			}
			return;
		}
        //PeopleManager.instance.addPerson(new Person("김철수", "남성", "010-2329-1234", "3A"));
        //PeopleManager.instance.addPerson(new Person("노두열", "남성", "010-1266-6239", "1C"));
        //PeopleManager.instance.addPerson(new Person("김미정", "여성", "010-1216-4532", "2C"));
        //PeopleManager.instance.addPerson(new Person("김효영", "여성", "010-7635-4121", "3B"));
        //PeopleManager.instance.addPerson(new Person("배미나", "여성", "010-6788-4521", "3C"));
        //PeopleManager.instance.addPerson(new Person("강연진", "여성", "010-6616-9819", "1D"));
        //PeopleManager.instance.addPerson(new Person("이우진", "남성", "010-8751-1234", "2D", "요주의 인물"));
		//PeopleManager.instance.addPerson(new Person("남채우", "남성", "010-5666-6532", "2B"));

        List<Person> used = new List<Person>();
        List<int> reviveTime = new List<int>();
        int startTime = 0;
		if (PeopleManager.instance.people.Count < 2)
			return;
        for (int i = 0; i < 10; i ++)
        {
            do
            {
                startTime += UnityEngine.Random.Range(10, 16);
                for (int j = 0; j < used.Count;)
                {
                    if (reviveTime[j] < startTime)
                    {
                        used.RemoveAt(j);
                        reviveTime.RemoveAt(j);
                        break;
                    }
                    else
                    {
                        j++;
                    }
                }

            }
            while (PeopleManager.instance.people.Count - used.Count < 2);

            Person sender = PeopleManager.instance.getRandomPersonExceptionList(used);
            used.Add(sender);
            Person recver = PeopleManager.instance.getRandomPersonExceptionList(used);
            used.Add(recver);
            int opponentWait = UnityEngine.Random.Range(2, 5);
            int duration = UnityEngine.Random.Range(10, 20);
            reviveTime.Add(startTime + 20 + opponentWait + duration + 10);
            reviveTime.Add(startTime + 20 + opponentWait + duration + 10);
            StartCoroutine("normalCall", new object[] { "normalCall", startTime, sender, recver, recver.getName() + comments[UnityEngine.Random.Range(0, comments.Length)],
                opponentWait, duration });
        }


        //Person person = PeopleManager.instance.getRandomPerson();
        //Person recv = PeopleManager.instance.getRandomPerson(person);

        //StartCoroutine("normalCall", new object[] { 5, person, recv, recv.getName() + " 바꿔주세요.", 7, 20 });


    }

    public void newPerson()
    {
        string[] comments = new string[] { " 바꿔주세요.", " 바꿔줘요.", " 바꿔 주십시오." };

        Person person = PeopleManager.instance.getRandomPerson();

        ChatManager.instance.printChat(person.getName() +
            comments[UnityEngine.Random.Range(0, comments.Length)], PeopleManager.instance.getRandomPerson(person));
    }
}
