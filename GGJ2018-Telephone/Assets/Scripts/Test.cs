using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ChatManager.instance.printChat("testing...");
        ChatManager.instance.printChat("Hello World...");
        ChatManager.instance.printChat("전화교환수");

        ChatManager.instance.printChoiceChat("1234567", "A", "B");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
