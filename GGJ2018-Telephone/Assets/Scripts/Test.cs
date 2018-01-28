using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    int cnt = 0;
	// Use this for initialization
	void Start () {
        ChatManager.instance.printChat("testing...");
        ChatManager.instance.printChat("Hello World...");
        ChatManager.instance.printChat("전화교환수");

        ChatManager.instance.printChoiceChat("sasdasd", "asdasdad", "agafg s", null, null);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void asdf()
    {
        ChatManager.instance.printChoiceChat("sasdasd" + cnt++, "asdasdad", "agafg s", null, null);
    }

    public void asdfg()
    {
        ChatManager.instance.printChat("Hello World..." + cnt++);
    }
}
