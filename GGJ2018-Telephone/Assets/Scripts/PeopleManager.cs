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

	public void shuffle()
	{
		for (int i = 0; i < people.Count; i++) {
			int n = people.Count - i - 1;
			if (n <= 1)
				continue;
			int j = Random.Range (i + 1, people.Count);
			var t = people [i];
			people [i] = people [j];
			people [j] = t;
		}
		for (int i = 0; i < people.Count; i++) {
			int n = people.Count - i - 1;
			if (n <= 1)
				continue;
			int j = Random.Range (i + 1, people.Count);
			var t = people [i].code;
			people [i].code = people [j].code;
			people [j].code = t;
		}
		for (int i = 0; i < people.Count; i++) {
			people [i].number = "010-"+Random.Range(1000, 10000) + "-" + Random.Range(1000, 10000);
		}
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
