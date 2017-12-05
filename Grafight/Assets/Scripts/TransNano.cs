using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransNano : NanoScript {

	protected override void Start (){
		base.Start ();
	}

	protected override void SetupTrailRend (){

		tr.time = Random.Range (2.5f, 3f);

		lifetime = 65;
	}

	protected override void Update () {
		MoveToPosition ();


		if (Time.time - startTime >= lifetime) {
			Destroy (gameObject);
		}
		if (transform.position == pos ) {

			NanoScript[] nanos = nanoHolder.GetComponentsInChildren<NanoScript> ();

			if (transform.parent == player.transform) {
				Vector3 circle = Random.insideUnitCircle * 0.1f;
				pos = new Vector3 (player.transform.position.x + circle.x, player.transform.position.y + circle.y, 0f);
			} else {

				Vector3 circle = Random.insideUnitCircle * 0.2f;
				pos = new Vector3 (pos.x + circle.x, pos.y + circle.y, 0f);
			
			}

			foreach (NanoScript nano in nanos) {
				if (nano.player != player) {
					
					if (Vector3.Distance (transform.position, nano.transform.position) < 0.5f) {

						pos = new Vector3 (nano.transform.position.x, nano.transform.position.y, 0f);
						break;
					} 
				}
			}

		}

		if (Vector3.Distance (transform.position, player.transform.position) < 0.4f) {
			gameObject.transform.SetParent (player.transform);
		} else {
			transform.SetParent (nanoHolder.transform);
		}

	}

	protected override void MoveToPosition (){
		transform.position = Vector3.MoveTowards (transform.position, pos , Random.Range (speedMin, speedMax) * Time.deltaTime);
	}

	void OnTriggerEnter(Collider col){
		print (col.name);
		if (col.gameObject.tag == "Cell") {
			if (col.gameObject.GetComponent<NanoScript> ().player != player) {
				Destroy (col.gameObject);
			} else {
				//print ("Same cell type");
			}

		} 
	}
}
