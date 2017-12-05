using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour {

	public enum Team {
		PLAYER1,
		PLAYER2,
	}

	List <Vector3> temp1 = new List<Vector3> ();

	MeshRenderer rend;

	public Material[] mat;

	public Team myTeam;

	public KeyframeScript keyAttached;

	public float recTimRegain = 0.2f;

	public float health;

	void Start(){
		rend = GetComponent<MeshRenderer> ();
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Bullet") {

			PlayerStats stats = col.gameObject.GetComponent<ProjectileScript> ().player.gameObject.GetComponent<PlayerStats> ();
			stats.maxRecTime += recTimRegain;
			col.gameObject.GetComponent<ProjectileScript> ().DestroyBullet (col.gameObject);

			keyAttached.activeBlocks.Remove (this.gameObject);
			keyAttached.blockPoints.Remove (transform.position);

			Destroy (gameObject);
		}

		if (col.gameObject.tag == "Deflector") {
			Debug.Log ("Overlapping blocks!");
			Destroy (gameObject);
		}

		if (col.gameObject.tag == "Player") {

			if (myTeam == Team.PLAYER1) {
				if (col.gameObject.GetComponent<TeamAssignment> ().myTeam == TeamAssignment.Team.TEAM_B) {
					col.gameObject.GetComponent<PlayerScript> ().ResetPlayerPos ();
					col.gameObject.GetComponent<KeyframeScript> ().ReplenishSegments ();
					
				} else {
					keyAttached.Segments = keyAttached.maxSegments;
				}
			} else {
				if (col.gameObject.GetComponent<TeamAssignment> ().myTeam == TeamAssignment.Team.TEAM_A) {
					col.gameObject.GetComponent<PlayerScript> ().ResetPlayerPos ();
					col.gameObject.GetComponent<KeyframeScript> ().ReplenishSegments ();

				} else {
					keyAttached.Segments = keyAttached.maxSegments;
				}
			}

	

			foreach (GameObject node in keyAttached.areaNodeMarkers) {
				if (Mathf.Abs (Vector3.Distance (transform.position, node.transform.position)) < GameManager.gridSpacing) {
					node.GetComponent<AreaNode>().ChangeNodeBlockToNode();
				}
			}


			keyAttached.activeBlocks.Remove (gameObject);
			keyAttached.blockPoints.Remove (transform.position);
			GameManager.globalBlocks.Remove (gameObject);


		
		
			Destroy (gameObject);
		}


	}

	void Update(){
		if (myTeam == Team.PLAYER1) {
			rend.material = mat [0];

		} else if (myTeam == Team.PLAYER2) {

			rend.material = mat [1];

		}



	

		foreach (GameObject node in keyAttached.areaNodeMarkers) {
			if (Mathf.Abs (Vector3.Distance (transform.position, node.transform.position)) < GameManager.gridSpacing) {

				if (myTeam == Team.PLAYER1) {
					if (node.GetComponent<AreaNode> ().myPlayerNum == AreaNode.PlayerNum.ONE) {
						Debug.Log (node.name + " " + node.transform.position);
						break;
					} 
				} else {
					if (node.GetComponent<AreaNode> ().myPlayerNum == AreaNode.PlayerNum.TWO) {
						Debug.Log (node.name + " " + node.transform.position);
						break;
					} 

				}
			} else {
				if (!temp1.Contains(node.transform.position)){
					temp1.Add (node.transform.position);
				}
			}
		}

		if (temp1.Count == keyAttached.areaNodeMarkers.Count) {
			Debug.Log (" not found");
			keyAttached.activeBlocks.Remove (gameObject);
			keyAttached.blockPoints.Remove (transform.position);
			GameManager.globalBlocks.Remove (gameObject);
			Destroy (gameObject);
		}
	}



}
