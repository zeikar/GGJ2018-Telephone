using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneBook : MonoBehaviour
{
    public static PhoneBook instance;
    public GameObject bookPagePrefab;
    Animator animator;
    bool isActive;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
        isActive = false;
    }

    void Start()
    {
        string buffer = "";

        for (int i = 0; i < PeopleManager.instance.people.Count; ++i)
        {
            Person person = PeopleManager.instance.people[i];

            buffer += person.getName() + " : " + person.getNumber() + '\n';

            if (i % 2 == 1)
            {
                Text bookPage = Instantiate(bookPagePrefab, this.transform).GetComponent<Text>();
                bookPage.text = buffer;
                Book.instance.bookPages.Add(bookPage);

                buffer = "";
            }
        }

        if(buffer != "")
        {
            Text bookPage = Instantiate(bookPagePrefab, this.transform).GetComponent<Text>();
            bookPage.text = buffer;
            Book.instance.bookPages.Add(bookPage);
        }

        Book.instance.Init();
    }
    
    public void PhoneBookToggle()
    {
        isActive = !isActive;

        animator.SetBool("isActive", isActive);
    }
}
