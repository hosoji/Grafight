using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CordControl : MonoBehaviour {

	public GameObject Rope;
	private Rope _ropeEntity;

	public float minCord;
	public float maxCord;

	public float cordLength;

	Rigidbody rb;

	public float speed = 2f;

	public GameObject anchor;



	TeamAssignment input;

	private bool[] flags = new bool[] { false, false };

	void Start(){
		_ropeEntity = Rope.GetComponent<Rope>();
		input = GetComponent<TeamAssignment> ();
		rb = GetComponent<Rigidbody> ();
	}

	void FixedUpdate()
	{
//		cordLength = Mathf.Clamp(_ropeEntity.GetRopeLength (), minCord, maxCord) ;
		//print (_ropeEntity.GetRopeLength());
//		cordLength = Mathf.Clamp(_ropeEntity.GetRopeLength (), minCord, maxCord) ;
		//_update = true;
		if (_ropeEntity == null)
			return;
		if (flags[0] == true)
		{
			_ropeEntity.CutRope(1.5f * Time.fixedDeltaTime, Direction.FrontToBack);
		}
		else
			if (flags[1] == true)
			{
				_ropeEntity.CutRope(-0.5f * Time.fixedDeltaTime, Direction.FrontToBack);
			}
	}


	void Update () {

		cordLength = _ropeEntity.GetRopeLength ();

		//print ("Cut: " + flags [0] + " Extend: " + flags [1]);
		float controllerVertical = Input.GetAxis (input.vertical);
		float controllerHorizontal = Input.GetAxis (input.horizontal);
//		print (controllerVertical);
		float buffer = 0.02f;


		flags[0] = false;
		flags[1] = false;

		if (Input.GetButton(input.cut)) {
			flags [0] =  flags[0] | true;
			flags [1] = false;
		
		} else if (rb.velocity.magnitude != 0) {
			flags[0] = false;
			flags[1] = flags[1] | true;

		}
	}

//	public void ExtendedCord(){
//
//		if (cordLength < maxCord) {
//			print ("Wtf");
//			flags [0] = false;
//			flags [1] = true;
//		} 
//
//		
//	}
//
//	public void RetractCord(){
//		if (cordLength > minCord) {
//			flags [0] =  true;
//			flags [1] = false;
//		}
//	}
//
//	public void IdleCord(){
//		flags [1] = false;
//		flags [0] = false;
//	}



}
