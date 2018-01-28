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
    public static List<SpriteRenderer> indicators = new List<SpriteRenderer>();
    public static List<GameObject> switchOns = new List<GameObject>();
    public static List<GameObject> switchOffs = new List<GameObject>();

    public static void SwitchOn(int index, bool onoff)
    {
        switchOns[index].SetActive(onoff);
        switchOffs[index].SetActive(!onoff);
    }

    public static bool IsSwitchOperatorOn(int lineIndex)
    {
        return switchOns[lineIndex * 2].activeInHierarchy;
    }
    public static bool IsSwitchTelephoneOn(int lineIndex)
    {
        return switchOns[lineIndex * 2+1].activeInHierarchy;
    }

    public static void ToggleSwitch(int index)
    {
        //Debug.Log("TS "+index);
        if (switchOffs[index].activeInHierarchy)
        {
            SwitchOn(index, true);
            SoundManager.instance.SwitchSound(true);
        }
        else
        {
            SwitchOn(index, false);
            SoundManager.instance.SwitchSound(false);
        }
    }
		
    // Use this for initialization
    void Start () {
		if (Title.Instance != null)
			if (Title.Instance.SwitchN != -1)
			N = Title.Instance.SwitchN;
		float l = - (N-1) / 2f * 2.1f;
		for (var i = 0; i < N; i++) {
			var o = Instantiate (LineSet, LineSetArray.transform);
			var p = o.transform.localPosition;
			p.x = l + i * 2.1f;
			p.y = 1f;
			o.transform.localPosition = p;

            indicators.Add(o.transform.Find("CableIn/CableSlot/Indicator").gameObject.GetComponent<SpriteRenderer>());
            indicators.Add(o.transform.Find("CableOut/CableSlot/Indicator").gameObject.GetComponent<SpriteRenderer>());
            switchOffs.Add(o.transform.Find("DipOperator/SwitchOff").gameObject);
            switchOns.Add(o.transform.Find("DipOperator/SwitchOn").gameObject);
            o.transform.Find("DipOperator").gameObject.GetComponent<DipSwitch>().index = i * 2;

            switchOffs.Add(o.transform.Find("DipTelephone/SwitchOff").gameObject);
            switchOns.Add(o.transform.Find("DipTelephone/SwitchOn").gameObject);
            o.transform.Find("DipTelephone").gameObject.GetComponent<DipSwitch>().index = i * 2+1;

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
			ci.GetComponent<CableEndScript> ().Init (ci, i*2);
			co.GetComponent<CableEndScript> ().Init (co, i*2+1);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
