using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NanoScript : MonoBehaviour {

	protected TrailRenderer tr;
	protected GameObject nanoHolder;

	public float speedMin = 0.1f;
	public float speedMax = 0.3f;

	public float lifetime;
	protected float startTime;

	protected Vector3 pos;
	protected Vector3 initPos;

	//Player object set in inspector
	public GameObject player;

	//Anchor object set in inspector
	protected GameObject a;

	float anchorHoverDistance;

	float anchorDistanceMin = 0.05f;
	float anchorDistanceMax = 0.45f;

	float t = 0;


	protected virtual void Start(){
		nanoHolder = GameObject.Find ("NanoHolder");
		initPos = transform.position;
		pos = initPos;
		startTime = Time.time;

		a = player.GetComponent<CordControl> ().anchor;

		tr = GetComponent<TrailRenderer> ();

		SetupTrailRend ();
	}

	protected virtual void SetupTrailRend(){
		tr.endWidth = Random.Range (0.02f, 0.04f);
		tr.startWidth = Random.Range (0.03f, 0.05f);
		tr.time = Random.Range (0.1f, 0.3f);

		lifetime = 90;
	}

	protected virtual void Update(){
		MoveToPosition ();

		if (transform.parent == nanoHolder.transform) {
			if (Time.time - startTime >= lifetime) {
				Destroy (gameObject);
			}
		}
	
		if (Vector3.Distance(transform.position, pos) <= 0.01f ) {
			if (transform.parent == player.transform) {
				Vector3 circle = Random.insideUnitCircle * 0.1f;
				pos = new Vector3 (player.transform.position.x + circle.x, player.transform.position.y + circle.y, 0f);
			} else {
				Vector3 circle = Random.insideUnitCircle * 0.2f;
				pos = new Vector3 (initPos.x + circle.x, initPos.y + circle.y, 0f);
			}
		}

		if (Vector3.Distance (transform.position, player.transform.position) < 0.4f) {
			gameObject.transform.SetParent (player.transform);
		} else {
			transform.SetParent (nanoHolder.transform);
		}


		// Fix following code so that nanos stick to their messenger 

		if (FindObjectOfType<MessengerNano> () != null) {
			if (Vector3.Distance(transform.position, FindObjectOfType<MessengerNano>().transform.position) < 0.3f){
				GameObject msgGO = FindObjectOfType<MessengerNano> ().gameObject;
				if (msgGO.GetComponent<MessengerNano> ().player == player) {
					gameObject.transform.SetParent (msgGO.transform);
					float offset = Random.Range (0.05f, 0.1f);

					pos = new Vector3 (msgGO.transform.position.x + offset, msgGO.transform.position.y + offset, 0f);
				}
			}
		}

		if (Vector3.Distance (transform.position, a.gameObject.transform.position) < AnchorDistance() ) {
			Vector3 circle = Random.insideUnitCircle * 1f;

			pos = new Vector3 (a.transform.position.x + circle.x, a.transform.position.y + circle.y, 0f);
			transform.SetParent (a.transform);
		} 
	}

	protected virtual void MoveToPosition(){
		/*
		if (t > 0) {
			t -= (Random.Range (0.1f, 0.3f) * Time.deltaTime);
		} else {
			t = 1;
		}
		transform.position = Vector3.Lerp(transform.position, pos, t);
		*/

		transform.position = Vector3.MoveTowards (transform.position, pos , Random.Range (speedMin, speedMax) * Time.deltaTime);
	}

	void OnTriggerEnter(Collider col){
		
		if (col.gameObject.tag == "Cell") {
			if (col.gameObject.GetComponent<NanoScript> ().player != player) {
				Debug.Log ("Nanos Collided");
				Destroy (col.gameObject);
				Destroy (gameObject);
			} else {
				//print ("Same cell type");
			}
		} 
	}

	float AnchorDistance(){
		return UtilScript.remapRange (a.GetComponent<Anchor>().rope.GetRopeLength(), a.GetComponent<Anchor>().ropeMin, a.GetComponent<Anchor>().ropeMax, anchorDistanceMax, anchorDistanceMin);
	}


}
