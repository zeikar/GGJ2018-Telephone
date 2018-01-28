using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableEndScript : MonoBehaviour {
	Vector3 initialPosition;
	GameObject self;
	bool placed = false;
	GameObject connected;
	public Sprite whiteSprite;
    int myIndex;

	// Use this for initialization
	void Start () {
		lineObject = new GameObject ("Line", new System.Type[]{ typeof(SpriteRenderer) });
		var p = lineObject.transform.position;
		p.z = -8;
		lineObject.transform.position = p;
		var sr = lineObject.GetComponent<SpriteRenderer> ();
		sr.sprite = whiteSprite;
		sr.color = new Color (0, 0, 0.5f);
		lineObject.SetActive (false);
	}

	public void Init(GameObject go, int index){
        myIndex = index;
		self = go;
		initialPosition = go.transform.position;
	}

	void ResetConnection() {
		// after animation
		self.transform.position = initialPosition;
		float angle = Random.Range (30f, 70f);
		if (0 == Random.Range (0, 2))
			angle = -angle;
		self.transform.localRotation = Quaternion.Euler(0, 0, angle);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	GameObject lineObject;
	Vector3 offset;

	void OnMouseDown() {
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
		Vector3 curPositionOff = Camera.main.ScreenToWorldPoint(curScreenPoint);
		curPositionOff.z = 0;

		UpdateLineObject (curPositionOff);
		lineObject.SetActive (true);
		if (placed) {
			placed = false;
		}
		//offset = curPositionOff - self.transform.position;
		offset.x = 0;
		offset.y = 0;
		offset.z = 0;
		//self.transform.rotation = Quaternion.identity;
	}

	void UpdateLineObject(Vector3 p)
	{
		var p2 = (p + initialPosition)/2f;
		p2.z = -8;
		lineObject.transform.position = p2;
		var delta = p - initialPosition;
		self.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(-delta.x, delta.y)*180/3.141592f);
		lineObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(delta.y, delta.x)*180/3.141592f);
		lineObject.transform.localScale = new Vector3 (delta.magnitude*100, 10, 1);
			
	}

	GameObject hit = null;
	void OnMouseDrag() {
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
		curPosition.z = 0;
		self.transform.position = curPosition+offset;
		UpdateLineObject (curPosition);

		hit = null;
		var c2 = self.GetComponent<BoxCollider2D> ();
		foreach (var slot in SwitchScript.Instance.cableSlots) {
			var c1 = slot.GetComponent<CircleCollider2D> ();
			var dist = Physics2D.Distance (c1, c2);
			if (dist.isOverlapped) {
				hit = slot;
				break;
			}
		}
	}

	void OnMouseUp() {
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);

		if (hit) {
			placed = true;
			self.transform.position = hit.transform.position;
			self.transform.rotation = Quaternion.identity;

			UpdateLineObject (self.transform.position);

            GameManager.Instance.OnConnect(myIndex, hit.GetComponent<CableSlot>().Index);
		} else {
			lineObject.SetActive (false);
			ResetConnection();
            GameManager.Instance.OnConnect(myIndex, -1);
		}
	}
}
