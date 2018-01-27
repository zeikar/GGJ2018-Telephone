using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneBook : MonoBehaviour
{
    Animator animator;
    bool isActive;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        isActive = false;
    }
    
    public void PhoneBookToggle()
    {
        isActive = !isActive;

        animator.SetBool("isActive", isActive);
    }
}
