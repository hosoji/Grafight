using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellRope : MonoBehaviour {

	float dist = 0.3f;
	float bufferDist;

	public GameObject anchor;
	public GameObject player;

	GameObject a;

	public Transform unitHolder;

	public GameObject unitPrefab;

	CellRopeUnit lastUnit;
	GameObject currentUnit;

	bool spawned;

	List <CellRopeUnit> units = new List<CellRopeUnit>();

	// Use this for initialization
	void Start () {
		lastUnit = anchor.GetComponentInChildren<CellRopeUnit> ();

	
		//lastUnit.target = MidPoint (lastUnit.transform.position,player.transform.position,  0);
		lastUnit.target = player;
		lastUnit.anchor = anchor;
		units.Add (lastUnit);
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (player.transform.position, lastUnit.transform.position) > dist) {
			spawned = false;
			if (!spawned) {
				SpawnUnit ();
				spawned = true;
			}
		}
		//print (lastUnit.gameObject.name);

		if (Vector3.Distance (player.transform.position, lastUnit.transform.position) < 0.1f) {
			if (units.Count > 2) {
				Destroy (units [units.Count - 1].gameObject);
				units.RemoveAt (units.Count - 1);

				lastUnit = units [units.Count - 1];
			}

		}

		if (units.Count > 2) {
			for (int i = 0; i < units.Count; i++) {

				if (i == 0) {
					units [i].anchor = anchor;
					units [i].target = units [i + 1].gameObject;
				} else if (i == units.Count - 1) {
					units [i].anchor = units [i - 1].gameObject;
					units [i].target = player;
				
				} else {
					units [i].anchor = units [i - 1].gameObject;
					units [i].target = units [i + 1].gameObject;
				
				}

				print (units [i].name + " is targeting " + units [i].target.name);
			}
		}

	}

	void SpawnUnit(){

		Vector3 pos = MidPoint(player.transform.position,lastUnit.transform.position) ;

		currentUnit = Instantiate (unitPrefab, pos, Quaternion.identity, unitHolder);
		currentUnit.gameObject.name = "unit: " + (units.Count - 1).ToString ();
		units.Add (currentUnit.GetComponent<CellRopeUnit> ());

		lastUnit.target = currentUnit;
		currentUnit.GetComponent<CellRopeUnit> ().anchor = lastUnit.gameObject;

		lastUnit = currentUnit.GetComponent<CellRopeUnit> ();
		
	}

	Vector3 MidPoint(Vector3 a, Vector3 b){
		Vector3[] t = new Vector3[2];
		t [0] = a;
		t [1] = b;
		
		return UtilScript.CalculateCentroid (t, 0);
	}


}
