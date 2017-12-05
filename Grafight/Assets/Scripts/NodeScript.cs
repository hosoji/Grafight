using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour {

	public BendIdentifier[] spheres;

	public GameObject innerPrefab;

//	public GameObject nanoPrefabP1, nanoPrefabP2;
//
//	public static GameObject nanoHolder;

	public float nanoLimit = 10f;

	public float nanoCount = 0;
	GameObject currentNano;

	NanoCollection nanoCollection;


	public Material [] mats0, mats1, mats2;

	MeshRenderer rend;

	[SerializeField]
	List<GameObject> p1Spheres = new List<GameObject>();
	List<GameObject> p2Spheres = new List<GameObject>();

	List<GameObject> nanoPods = new List<GameObject>();


	public int nodeScore;

	public List<Transform> centroidTransform = new List<Transform> ();

	float distanceToNode = 0.2f;
	public float distanceToCentrois = 0.5f;

	public float delay = 15f;

	float t = 0f; 

	// Use this for initialization
	void Start () {

		//nanoHolder = GameObject.Find ("NanoHolder");
		
		rend = GetComponent<MeshRenderer> ();

		nanoCollection = GameObject.Find ("[GameManager]").GetComponent<NanoCollection> ();
	

		p1Spheres = this.p1Spheres;
		p2Spheres = this.p2Spheres;
		t = Random.Range (0, delay);

	}
	
	// Update is called once per frame
	void Update () {


		nodeScore = p1Spheres.Count - p2Spheres.Count;

		AssignNodeToPlayer ();
		DetectNodeInfluence ();

		foreach (Transform centroid in CircleGrid.centroids) {
			if (centroid != null) {
				if (Vector3.Distance (centroid.position, transform.position) < distanceToCentrois) {
					if (!centroidTransform.Contains (centroid)) {
						centroidTransform.Add (centroid);
						centroid.gameObject.GetComponent<Centroids> ().AssignNodes (this.gameObject);
					}
				}
			}
		}

		if (nanoCount >= nanoLimit) {
			GetComponentsInChildren<MeshRenderer> ()[0].enabled = false;
			GetComponentsInChildren<MeshRenderer> ()[1].enabled = false;
			GetComponent<MeshCollider> ().enabled = false;
			nodeScore = 0;
			//Destroy (gameObject);
		}
	}

	void AssignNodeToPlayer(){

		if (nodeScore > 0) {
			rend.materials = mats2;
			SpawnNanoNodes (p1Spheres);
			
		} else if (nodeScore < 0) {
			rend.materials = mats1;
			SpawnNanoNodes (p2Spheres);
			
		} else {
			rend.materials = mats0;

		}
		
	}


	void DetectNodeInfluence (){


		// Finds bend instances near it to determine which Player's rope is wrapped around it more
		spheres = FindObjectsOfType<BendIdentifier> ();

		//		print (spheres.Length);

		for (int i = 0; i < spheres.Length; ++i) {
			if (Vector3.Distance (transform.position, spheres [i].gameObject.transform.position) < distanceToNode) {

				//print (spheres[i].identity);


				if (spheres [i].identity == "Player1") {
					if (!p1Spheres.Contains (spheres [i].gameObject)) {
						p1Spheres.Add (spheres [i].gameObject);
					}
				}

				if (spheres [i].identity == "Player2") {
					if (!p2Spheres.Contains (spheres [i].gameObject)) {
						p2Spheres.Add (spheres [i].gameObject);
					}
				}
			} 
		}


		for (int n = p1Spheres.Count-1; n >= 0; n--) {
			if (p1Spheres[n] == null) {
				p1Spheres.RemoveAt (n);
			}
		}

		for (int n = p2Spheres.Count-1; n >= 0; n--) {
			if (p2Spheres[n] == null) {
				p2Spheres.RemoveAt (n);
			}
		}


		if (spheres.Length == 0) {
			p1Spheres.Clear ();
		}

		if (spheres.Length == 0) {
			p2Spheres.Clear ();
		}
			
	}

	void SpawnNanoNodes(List <GameObject> p){

		if (nanoCount < nanoLimit) {

			for (int i = 0; i < p.Count - 1; i++) {
			
				if (t > 0) {
					t -= Time.deltaTime;
				} else {
					if (p == p1Spheres) {
						CreateNanoNodes ("Player1");
					} else {
						CreateNanoNodes ("Player2");
					}

				}
			}
		}
	}

	void CreateNanoNodes(string player){

		currentNano = nanoCollection.GetPrefab (player);

		Vector3 circle = Random.insideUnitCircle * 0.2f;
		Vector3 pos = new Vector3 (transform.position.x + circle.x, transform.position.y + circle.y, 0f);
		GameObject nano = Instantiate (currentNano, pos, Quaternion.identity,  NanoCollection.nanoHolder.transform);
		t = Random.Range (0, delay);
		nanoCount++;

		nanoCollection.GetNanoScript (nano);

		nano.GetComponent<NanoScript> ().player = GameObject.Find (player);


	}
		
}
