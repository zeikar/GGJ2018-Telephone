using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleManager : MonoBehaviour
{
    public static PeopleManager instance;
    public List<Person> people;


    // Use this for initialization
    void Awake()
    {
        instance = this;
        people = new List<Person>();

        people.Add(new Person("김개똥", "남성", "010-2737-1928"));
        people.Add(new Person("asd", "남성", "010-2329-1234"));
        people.Add(new Person("werwer", "남성", "010-5666-6532"));
    }
}
