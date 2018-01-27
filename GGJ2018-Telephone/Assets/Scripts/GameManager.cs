using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int gameStage;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitGame();
    }

    void Update()
    {
    }

    Dictionary<int,int> cableEndConnections = new Dictionary<int, int>();
    bool IsCableConnected(int cableEnd, int cableSlot)
    {
        foreach(var kv in cableEndConnections)
        {
            Debug.Log(kv.Key + " : " + kv.Value);
        }
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
            cableEndConnections.Remove(cableEnd);
        else
            cableEndConnections[cableEnd] = cableSlot;
    }

    void SetLineIndicator(int lineIndex, bool onoff)
    {
        if (onoff)
        {
            LineSetScript.indicators[lineIndex].color = new Color(1, 237f / 255, 0);
        }
        else
        {
            LineSetScript.indicators[lineIndex].color = new Color(103f/255, 97f / 255, 14f/255);
        }
    }

    void SetSlotIndicator(int slot, bool onoff)
    {
        if (onoff)
        {
            SwitchScript.indicators[slot].color = new Color(51f / 255, 1, 0);
        }
        else
        {
            SwitchScript.indicators[slot].color = new Color(15f / 255, 64f / 255, 0);
        }
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
        while(true)
        {
            bool found = false;
            for(int i = 0; i < lineSet.N; i ++)
            {
                //Debug.Log(i + " " + LineSetScript.IsSwitchOperatorOn(i) + " " + IsCableConnected(i*2, slot));
                if (LineSetScript.IsSwitchOperatorOn(i) && IsCableConnected(i*2, slot))
                {
                    found = true;
                    lineUsing = i;
                    Debug.Log("lineUsing " + i);
                    SetLineIndicator(2*i, true);
                    break;
                }
            }
            if (found)
                break;
            yield return new WaitForSeconds(0.1f);
        }

        ChatManager.instance.printChat(msg, sender);

        Debug.Log("print chat");

        while(true)
        {
            Debug.Log("line valid " + LineSetScript.IsSwitchTelephoneOn(lineUsing) + " " + IsCableConnected(lineUsing * 2 + 1, recver.getSlot()));

            if (LineSetScript.IsSwitchTelephoneOn(lineUsing) && IsCableConnected(lineUsing * 2+1, recver.getSlot()))
                break;
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("connect correct line");

        {
            var currentTime = Time.time;
            while(Time.time - currentTime < opponentWaiting)
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
            if (!LineSetScript.IsSwitchTelephoneOn(lineUsing))
                break;
            yield return new WaitForSeconds(0.1f);
        }

        {
            var currentTime = Time.time;
            while (Time.time - currentTime < callDuration)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

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
    }

    private void InitGame()
    {
        gameStage = 1;

        PeopleManager.instance.addPerson(new Person("김개똥", "남성", "010-2737-1928", "1A"));
        PeopleManager.instance.addPerson(new Person("김철수", "남성", "010-2329-1234", "3A"));
        PeopleManager.instance.addPerson(new Person("남채우", "남성", "010-5666-6532", "2B"));
        PeopleManager.instance.addPerson(new Person("노두열", "남성", "010-1266-6239", "1C"));
        PeopleManager.instance.addPerson(new Person("양창무", "남성", "010-5236-4432", "2B"));
        PeopleManager.instance.addPerson(new Person("김미정", "여성", "010-1216-4532", "2C"));
        PeopleManager.instance.addPerson(new Person("류연아", "여성", "010-3324-8477", "1B"));
        PeopleManager.instance.addPerson(new Person("김효영", "여성", "010-7635-4121", "3B"));
        PeopleManager.instance.addPerson(new Person("배미나", "여성", "010-6788-4521", "3C"));
        PeopleManager.instance.addPerson(new Person("강연진", "여성", "010-6616-9819", "1D"));
        PeopleManager.instance.addPerson(new Person("이우진", "남성", "010-8751-1234", "2D", "요주의 인물"));

        Person person = PeopleManager.instance.getRandomPerson();
        Person recv = PeopleManager.instance.getRandomPerson(person);

        StartCoroutine("normalCall", new object[] { 5, person, recv, recv.getName() + " 바꿔주세요.", 7, 20 });


    }

    public void newPerson()
    {
        string[] comments = new string[] { " 바꿔주세요.", " 바꿔줘요.", " 바꿔 주십시오." };

        Person person = PeopleManager.instance.getRandomPerson();

        ChatManager.instance.printChat(person.getName() +
            comments[UnityEngine.Random.Range(0, comments.Length)], PeopleManager.instance.getRandomPerson(person));
    }
}
