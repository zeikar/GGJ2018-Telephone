using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSetScript : MonoBehaviour {
	public int N = 3; // max = 6
	public GameObject LineSet;
	public GameObject LineSetArray;
	public GameObject CableEnd;

	public List<GameObject> lines;
	public List<GameObject> cableEnds;

	// Use this for initialization
	void Start () {
		float l = - (N-1) / 2f * 2.1f;
		for (var i = 0; i < N; i++) {
			var o = Instantiate (LineSet, LineSetArray.transform);
			var p = o.transform.localPosition;
			p.x = l + i * 2.1f;
			p.y = 1f;
			o.transform.localPosition = p;
			var ci = Instantiate (CableEnd, o.transform);
			var co = Instantiate (CableEnd, o.transform);
			ci.transform.localPosition = new Vector3 (-0.4f, 0, 0);
			float angle = Random.Range (30f, 70f);
			if (0 == Random.Range (0, 2))
				angle = -angle;
			ci.transform.localRotation = Quaternion.Euler(0, 0, angle);
			co.transform.localPosition = new Vector3 (0.4f, 0, 0);
			angle = Random.Range (30f, 70f);
			if (0 == Random.Range (0, 2))
				angle = -angle;
			co.transform.localRotation = Quaternion.Euler (0, 0, angle);
			ci.GetComponent<CableEndScript> ().Init (ci);
			co.GetComponent<CableEndScript> ().Init (co);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
