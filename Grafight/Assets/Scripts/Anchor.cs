using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour {

	[SerializeField] private float depletionRate;

	MeshRenderer rend;

	public Rope rope;

	public float sizeMin, sizeMax;

	public float ropeMin, ropeMax;

	Vector3 startScale;
	//Vector3 currentScale;

	void Start(){
		startScale = transform.localScale;

		rend = GetComponent<MeshRenderer> ();
	}

	void Update(){
		transform.localScale = new Vector3 (CalculateSize (rope.GetRopeLength ()), CalculateSize (rope.GetRopeLength ()), CalculateSize (rope.GetRopeLength ()));


	}

	public float CalculateSize(float value){
		
		return UtilScript.remapRange (value, ropeMin, ropeMax, sizeMax, sizeMin);
	}
}
