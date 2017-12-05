using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NanoCollection : MonoBehaviour {

	public GameObject mainNanoPrefabP1, mainNanoPrefabP2;

	public GameObject[] mutationPrefabs;

	public GameObject messengerPrefabP1, messengerPrefabP2;


	public static GameObject nanoHolder;

	[SerializeField] float mutationChance = 0.1f;

	// Use this for initialization
	void Start () {
		nanoHolder = GameObject.Find ("NanoHolder");

	}

	GameObject MutationPossibility(GameObject go){
		GameObject cell = go;
		if (Random.Range (1f, 100f) <= 100f-(mutationChance * 100f)) {
			//cell.AddComponent<NanoScript>();
//			cell = mainNanoPrefabP1;
//			cell.GetComponent<NanoIdentifier>().myType = NanoIdentifier.nanoType.NORMAL;
			return cell;
		} else {
			// Mutation code here
			//cell.AddComponent<NanoScript>();
			cell = mutationPrefabs[0];
			cell.GetComponent<NanoIdentifier>().myType = NanoIdentifier.nanoType.TRANS;
			return cell;
		}
	}

	public GameObject GetPrefab(string player){
		if (player == "Player1") {
			return MutationPossibility (mainNanoPrefabP1);
		} else if (player == "Player2") {
			return MutationPossibility (mainNanoPrefabP2);
		} else {
			print ("No Player Assigned");
			return null;
		}
	}

	public GameObject GetNanoScript(GameObject go){
		NanoIdentifier ni = go.GetComponent<NanoIdentifier> ();

		GameObject cell = go;
		switch (ni.myType) {
		case NanoIdentifier.nanoType.NORMAL:
			if (cell.GetComponent<NanoScript> () == null) {
				cell.AddComponent<NanoScript> ();
			}
			break;
		case NanoIdentifier.nanoType.TRANS:
			if (cell.GetComponent<NanoScript> () == null) {
				cell.AddComponent<TransNano> ();
			}
			break;
		}

		return cell;
	}
}
