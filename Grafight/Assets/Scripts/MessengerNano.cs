using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessengerNano : NanoScript {

	int numOfPoints = 5;
	int index = 0;
	bool incremented;

	float offset = 0.15f;

	List <Vector3> pathPoints = new List<Vector3>();

	protected override void Start (){
		base.Start ();

		Vector3 heading = (a.transform.position - initPos);
		float dist = heading.magnitude;

		Vector3 dir = heading / dist;

		for (int i = 1; i < numOfPoints; i++) {
			float f = (float)i* dist/(numOfPoints-1);
			Vector3 circle = Random.insideUnitCircle * offset;
			Vector3 p = new Vector3 (transform.position.x + (dir.x * f) + circle.x, transform.position.y + (dir.y * f) + circle.y, 0f);
			Vector3 point = transform.position + (dir * f);

			pathPoints.Add (p);
		}
	}

	protected override void SetupTrailRend (){

		tr.time = Random.Range (1f, 1.5f);
	}

	protected override void Update () {
		
		MoveToPosition ();
		if (transform.position == pos) {
			//incremented = false;

			if (index < numOfPoints-1) {
//				if (!incremented) {
//					index++;
//					incremented = true;
//				}

				pos = pathPoints [index++];
			}
		}
	}

	protected override void MoveToPosition (){
		transform.position = Vector3.MoveTowards (transform.position, pos , Random.Range (speedMin, speedMax) * Time.deltaTime);
	}

//	void OnTriggerEnter(Collider col){
//		print (col.name);
//		if (col.gameObject.tag == "Cell") {
//			if (col.gameObject.GetComponent<NanoScript> ().player != player) {
//				Destroy (col.gameObject);
//			} else {
//				//print ("Same cell type");
//			}
//
//		} 
//	}
}
