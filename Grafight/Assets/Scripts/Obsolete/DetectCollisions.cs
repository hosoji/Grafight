using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisions : MonoBehaviour {

	void OnCollisionEnter (Collision col){
		for (int i = 0; i < col.contacts.Length; ++i) {
//			print ("point: " + col.contacts [i].point);
//			print ("normal: " + col.contacts [i].normal);
			print (col.gameObject.name);

		}
	}
}
