using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PhoneBook : MonoBehaviour
{
    public static PhoneBook instance;
    public GameObject bookPagePrefab;
    public Transform bookPageParent;
    Animator animator;
    bool isActive;

    // for generating book page
    Text lastPage;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
        isActive = false;

        lastPage = Instantiate(bookPagePrefab, bookPageParent).GetComponent<Text>();
    }

    void Start()
    {

        for (int i = 0; i < PeopleManager.instance.people.Count; ++i)
        {
            string buffer = "";
            Person person = PeopleManager.instance.people[i];

            buffer += person.getName() + " : " + person.getNumber() + '/' + person.getCode() + '\n';

            if (!person.getDescription().Equals(""))
            {
                buffer += person.getDescription() + '\n';
            }

            buffer += '\n';

            AddBook(buffer);
        }

        Book.instance.Init();
    }

    public void AddBook(string str)
    {
        if (lastPage.text.Count(x => x == '\n') > 6)
        {
            lastPage = Instantiate(bookPagePrefab, bookPageParent).GetComponent<Text>();
        }

        lastPage.text += str;
    }

    public void PhoneBookToggle()
    {
        isActive = !isActive;
        SoundManager.instance.bookSound(isActive);
        animator.SetBool("isActive", isActive);
    }
}
