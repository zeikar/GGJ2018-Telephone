using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int gameStage = -1;

    void Awake()
    {
		gameStage = -1;
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);

		}
		else if (Instance != this) {
			Destroy (gameObject);
		}

		if (Instance.gameStage == -1)
			Instance.gameStage = Title.firstStage;
    }
    
	float life = 900;
    void Start()
    {
        InitGame();
    }

	void OnGUI()
	{
		if (Time.time - stageBeginTime < 3) {
			Rect r = new Rect (Screen.width/10, Screen.height/10, Screen.width*8/10, Screen.height*8/10);
			//GUI.backgroundColor = new Color (0, 0, 0, 0.7f);
			var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
			texture.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f, 0.8f));
			texture.Apply();

			GUIStyle style = GUI.skin.GetStyle ("label");
			style.fontSize = 50;
			style.alignment = TextAnchor.MiddleCenter;
			GUI.Box (r, texture);
			GUI.Label (r, "DAY " + gameStage, style);
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
		var now = Time.time - stageBeginTime;
		if (life < 1000)
			life += now/30;
		if (life > 1000)
			life = 1000;
		{
			// life gauge
			var s = GameObject.Find("LifeGauge").transform.localScale;
			s.y = 2000-life * 2 + Mathf.Sin (now / 3) * 20 + UnityEngine.Random.Range (-2.5f, 2.5f);
			if (s.y < 0)
				s.y = 0;
            GameObject.Find("LifeGauge").transform.localScale = s;
		}
		for (; callsIndex < calls.Count; callsIndex++) {
			var row = calls [callsIndex];
			if ((int)row [1] > now) {
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
			gameStage ++;
			SetPreStageVariable ();
            goalCount = -1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

	void SetPreStageVariable()
	{
		if (gameStage == 1) {
			Title.Instance.BoardHeight = 2;
			Title.Instance.BoardWidth = 2;
			Title.Instance.SwitchN = 2;
		} else if (gameStage == 2) {
		} else if (gameStage == 3) {
		} else if (gameStage >= 4) {
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

	IEnumerator tutorialCall(object[] args)
	{
		int secondsToBegin = (int)args[1];
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
		yield return new WaitForSeconds(secondsToBegin-Time.time+stageBeginTime);

		SetSlotIndicator(slot, true);

		int lineUsing = -1;
		var lineSet = GameObject.Find("LineSetArray").GetComponent<LineSetScript>();
		while(true)
		{
			bool found = false;
			for(int i = 0; i < lineSet.N; i ++)
			{
				//Debug.Log(i + " " + LineSetScript.IsSwitchOperatorOn(i) + " " + IsCableConnected(i*2, slot));
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
			//Debug.Log("line valid " + LineSetScript.IsSwitchTelephoneOn(lineUsing) + " " + IsCableConnected(lineUsing * 2 + 1, recver.getSlot()));

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
			while(Time.time - currentTime < opponentWaiting)
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
				LineSetScript.instance.IsSwitchOperatorOn (lineUsing) && IsCableConnected (lineUsing * 2, sender.getSlot ()) &&
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
			var currentTime = Time.time;
			bool blinking = true;
			while (Time.time - currentTime < callDuration)
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

    IEnumerator normalCall(object[] args)
    {
        int secondsToBegin = (int)args[0];
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
                //Debug.Log(i + " " + LineSetScript.IsSwitchOperatorOn(i) + " " + IsCableConnected(i*2, slot));
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
            while (Time.time - currentTime < opponentWaiting)
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
            var currentTime = Time.time;
            bool blinking = true;
            while (Time.time - currentTime < callDuration)
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

	float stageBeginTime;
	int goalCount = -1;

    private void InitGame()
    {
		string[] comments = new string[] { " 바꿔주세요.", " 바꿔줘요.", " 바꿔 주십시오." };
		if (gameStage < 1)
			gameStage = 1;
		if (gameStage == 1) {
			stageBeginTime = Time.time;
			PeopleManager.instance.addPerson(new Person("김개똥", "남성", "010-2737-1928", "1A"));
			PeopleManager.instance.addPerson(new Person("양창무", "남성", "010-5236-4432", "2B"));
			PeopleManager.instance.addPerson(new Person("류연아", "여성", "010-3324-8477", "1B"));

			const int NumCalls = 3;
			goalCount = NumCalls;

			calls.Clear ();
			calls.Add (new object[] {
				"tutorialCall",
				10, 
				PeopleManager.instance.people [0],
				PeopleManager.instance.people [1],
				PeopleManager.instance.people [1].getName () + comments [UnityEngine.Random.Range (0, comments.Length)],
				5, 10
			});
			calls.Add (new object[] {
				"tutorialCall",
				40, 
				PeopleManager.instance.people [1],
				PeopleManager.instance.people [2],
				PeopleManager.instance.people [2].getName () + comments [UnityEngine.Random.Range (0, comments.Length)],
				5, 10
			});
			calls.Add (new object[] {
				"tutorialCall",
				70, 
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
            StartCoroutine("normalCall", new object[] { startTime, sender, recver, recver.getName() + comments[UnityEngine.Random.Range(0, comments.Length)],
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
