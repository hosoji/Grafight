using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centroids : MonoBehaviour {

	[SerializeField] GameObject [] nodes = new GameObject[3];

	NanoCollection nanoCollection;

	float t;
	float delay = 50;

	void Start(){
		for (int i = 0; i < nodes.Length; i++) {
			nodes [i] = null;
		}

		nanoCollection = GameObject.Find ("[GameManager]").GetComponent<NanoCollection> ();
	}

	public void AssignNodes(GameObject go){
		for (int i = 0; i < nodes.Length; i++) {
			if (nodes [i] == null) {
				nodes [i] = go;
				break;
			} 
		}
	}

	void Update(){
		for (int i = 0; i < nodes.Length; ++i) {
			if (nodes [i] != null) {
				if (nodes [i].GetComponent<NodeScript> ().nodeScore > 0) {
					nodes [i].name = "Player1";
				} else if (nodes [i].GetComponent<NodeScript> ().nodeScore < 0) {
					nodes [i].name = "Player2";
				} else {
					nodes [i].name = null;
				}
			}
		}

		for (int i = 0; i < nodes.Length; i++) {
			if (nodes [i] != null) {
				CheckAllNodes ();
			}
		}
		
	}

	void CheckAllNodes(){
		if (nodes [0].name == "Player1" && nodes [1].name == "Player1" && nodes [2].name == "Player1") {
			CreateMessenger ("Player1");
		} else if (nodes [0].name == "Player2" && nodes [1].name == "Player2" && nodes [2].name == "Player2") {
			CreateMessenger ("Player2");
		} else {
			
		}
	}
		

	void CreateMessenger(string player){

		if (t > 0) {
			t -= Time.deltaTime;
		} else {
			if (player == "Player1") {
				GameObject obj = Instantiate (nanoCollection.messengerPrefabP1, transform.position, Quaternion.identity,  NanoCollection.nanoHolder.transform);
				obj.GetComponent<MessengerNano> ().player = GameObject.Find (player);
			} else {
				GameObject obj = Instantiate (nanoCollection.messengerPrefabP2, transform.position, Quaternion.identity,  NanoCollection.nanoHolder.transform);
				obj.GetComponent<MessengerNano> ().player = GameObject.Find (player);
			}

			t = Random.Range (40, delay);
		}
	}

}
