using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour {
	public int Width = 4;
	public int Height = 3;
	int ViewWidth = 14;
	int ViewHeight = 7;

	public GameObject CableSlot;
	public GameObject Switchboard;

	public static List<GameObject> cableSlots = new List<GameObject>();

	// Use this for initialization
	void Start () {
		float l = -ViewWidth / 2.0f-2;
		float t = ViewHeight / 2.0f+1.5f;
		float sx = ViewWidth *1.0f / Width;
		float sy = ViewHeight *1.0f / Height;
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				var o = Instantiate (CableSlot, Switchboard.transform);

				var p = o.transform.position;
				p.x = l + sx * (x+0.5f);
				p.y = t - sy * (y+0.5f);
				o.transform.position = p;
				cableSlots.Add (o);
				Debug.Log (p.x + " "+ p.y);
			}
		}
	}
	
	// Update is called once per frame
	void Update () { 
		
	}
}
