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
    }

    public void addPerson(Person person)
    {
        people.Add(person);
    }

    public Person getRandomPerson(Person self = null)
    {
        Person person = people[Random.Range(0, people.Count)];

        while (person == self)
        {
            person = people[Random.Range(0, people.Count)];
        }

        return person;
    }
    public Person getRandomPersonExceptionList(List<Person> exceptions)
    {
        if (exceptions.Count >= people.Count)
            return null;
        Person person = people[Random.Range(0, people.Count)];

        while (exceptions.Exists(x => x == person))
        {
            person = people[Random.Range(0, people.Count)];
        }

        return person;
    }
}
