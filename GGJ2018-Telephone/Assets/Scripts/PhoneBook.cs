using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneBook : MonoBehaviour
{
    bool isActive;

    // Use this for initialization
    void Start()
    {
        isActive = false;
        this.gameObject.SetActive(isActive);
    }
    
    public void PhoneBookToggle()
    {
        isActive = !isActive;

        this.gameObject.SetActive(isActive);
    }
}
