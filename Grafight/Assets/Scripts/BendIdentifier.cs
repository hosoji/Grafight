using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendIdentifier : MonoBehaviour {
	public GameObject player;

	public string identity; 

	// Use this for initialization
	void Update () {

		identity = player.name;
		
	}
	

}
