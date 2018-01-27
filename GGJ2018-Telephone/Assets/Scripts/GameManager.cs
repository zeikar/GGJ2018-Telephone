using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int gameStage;

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
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

    void SetIndicator(int slot, bool onoff)
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
        int slot = (int)args[1];
        string msg = (string)args[2];
        int opponentWaiting = (int)args[3];
        int callDuration = (int)args[4];
        yield return new WaitForSeconds(secondsToBegin);
        SetIndicator(slot, true); 
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

        StartCoroutine("normalCall", new object[] { 5, 4, "개똥이 바꿔주세요.", 7, 20 });


    }

    public void newPerson()
    {
        string[] comments = new string[] { " 바꿔주세요.", " 바꿔줘요.", " 바꿔 주십시오." };

        Person person = PeopleManager.instance.getRandomPerson();

        ChatManager.instance.printChat(person.getName() +
            comments[UnityEngine.Random.Range(0, comments.Length)], PeopleManager.instance.getRandomPerson(person));
    }
}
