using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DipSwitch : MonoBehaviour {
    public int index;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseUp()
    {
        LineSetScript.ToggleSwitch(index);
    }
}
