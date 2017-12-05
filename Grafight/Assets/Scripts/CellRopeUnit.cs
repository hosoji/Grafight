using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellRopeUnit : MonoBehaviour {

	public GameObject target;
	public GameObject anchor;
	float dist = 0.1f;
	float aDist = 0.2f;
	float speed = 1f;

	void Update() {
		if (Vector3.Distance (transform.position, target.transform.position) > dist)  {
			transform.position = Vector3.MoveTowards (transform.position, target.transform.position, Time.deltaTime * 0.1f);
		}
	}
}
