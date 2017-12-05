using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour {

	Rigidbody rb;
	TeamAssignment input;

	CordControl cord;

	public Anchor anchor;

	Piece backPiece;
	Vector3 dir;
	Vector3 bendDir;
	float lastBendDir;

	List <GameObject> tempSpheres = new List<GameObject> ();

//	int moveMod;

	public float speed = 30f;

	float minSpeed = 0f;
	float maxSpeed;
	public float gravityScale; 


	// Use this for initialization
	void Start () {
		maxSpeed = speed;
		rb = GetComponent<Rigidbody> ();
		input = GetComponent<TeamAssignment> ();
		cord = GetComponent<CordControl> ();

		backPiece = anchor.rope.FrontPiece;

//		if (input.myTeam == TeamAssignment.Team.TEAM_A) {
//			moveMod = 1;
//		} else {
//			moveMod = -1;
//		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		float lastDir = dir.magnitude;

		if (GetLastBendInstance () != null) {
			lastBendDir = bendDir.magnitude;
		}

//		print ("direction 1: " + (backPiece.transform.position - transform.position));
		Physics.gravity = new Vector3 (0, gravityScale, 0);

		float controllerVertical = Input.GetAxis (input.vertical);
		float controllerHorizontal = Input.GetAxis (input.horizontal);

//		float controllerVertical = Input.GetAxis ("Vertical");
//		float controllerHorizontal = Input.GetAxis ("Horizontal");
//		print (controllerVertical);

		dir = backPiece.transform.position - transform.position;
		//print (gameObject.name + ": " + dir.magnitude);
		if (GetLastBendInstance () != null) {

			bendDir = GetLastBendInstance ().transform.position - transform.position;
			
			if (bendDir.magnitude - lastBendDir > 0) {
				speed = CalculateSpeed (anchor.rope.GetRopeLength ());
				//print ("extending rope");
			} else {
				speed = maxSpeed;
			}
		} else {
			if (dir.magnitude - lastDir > 0) {
				speed = CalculateSpeed (anchor.rope.GetRopeLength ());
				//print ("extending rope");
			} else {
				speed = maxSpeed;
			}
		}

		rb.AddForce (controllerHorizontal * Time.deltaTime * speed, controllerVertical * Time.deltaTime *speed, 0f ,ForceMode.Force );	

	}

	public float CalculateSpeed(float value){

		return UtilScript.remapRange (value, anchor.ropeMin, anchor.ropeMax, maxSpeed, minSpeed );
	}

	GameObject GetLastBendInstance(){
		BendIdentifier[] spheres = FindObjectsOfType<BendIdentifier> ();

		for (int i = 0; i < spheres.Length; ++i) {
			if (spheres [i].player == gameObject) {
				if (!tempSpheres.Contains (spheres [i].gameObject)) {
					tempSpheres.Add (spheres [i].gameObject);
				}
			}
		}

		for (int n = tempSpheres.Count-1; n >= 0; n--) {
			if (tempSpheres[n] == null) {
				tempSpheres.RemoveAt (n);
			}
		}

		if (tempSpheres.Count > 0) {
			return tempSpheres [tempSpheres.Count - 1];
		} else {
			return null;
		}
	}
}
