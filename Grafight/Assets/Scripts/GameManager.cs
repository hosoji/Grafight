using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public GameObject player1;
	public GameObject player2;

	public static Dictionary <GameObject, Vector3> globalBlocks = new Dictionary<GameObject, Vector3>();

	public static Dictionary <Vector3, GameObject> globalNodes = new Dictionary<Vector3,GameObject>();

	public Transform [,] gridArray;
	public GameObject grid;
	public int rowLength;
	public int columnLength;

	public float zOffset = -7f;
	public float xOffset = -30f;
	public float yOffset;

	public float gridSize;

	public static float gridSpacing;

	public Transform dotPrefab;

	public Text p1ScoreText;
	public Text p2ScoreText;

	public static int p1Score;
	public static int p2Score;



	void Awake () {
		/*
		
		grid = new GameObject ("Grid");

		gridSpacing = gridSize;

		gridArray = new Transform[rowLength, columnLength];

		for (int c = 0; c < columnLength; c++) {
			for (int r = 0; r < rowLength; r++) {
//				Vector3 pos = new Vector3 (r * gridSize + xOffset,-1f,c * gridSize + zOffset);
				Vector3 pos = new Vector3 (r * gridSize - 2f,c * gridSize - 2f,0.1f);
				gridArray[r,c] = Instantiate (dotPrefab, pos, Quaternion.Euler(-90,0,0), grid.transform) as Transform;
				gridArray [r, c].name = r.ToString () + " " + c.ToString ();

//				if (gridArray [r, c].name == "0 2") {
//					Destroy(gridArray [r, c].gameObject);
//				}
//				gridArray[r,c].transform.localPosition = new Vector3 (90f, 0f, 0f);
//				gridArray[r,c].transform.localScale = new Vector3 (3.4f, 3.4f, 3.4f);
//				print (gridArray [r, c]); 
			}
		}*/

	}

		
	public Vector3 GetWorldPosition(int x, int z){
		return gridArray[x,z].position;
	}


	void Update(){
		p1ScoreText.text = p1Score.ToString ();
		p2ScoreText.text = p2Score.ToString ();

		p1Score = GameObject.Find ("Anchor1").GetComponentsInChildren<NanoScript> ().Length;
		p2Score = GameObject.Find ("Anchor2").GetComponentsInChildren<NanoScript> ().Length;
	}



	public float GetMovementCost(GameObject go){

		float cost = 0.25f;

		print (go.name);
	
		//		Material mat = go.GetComponent<MeshRenderer>().sharedMaterial;
		//		int i;

		//		for(i = 0; i < mats.Length; i++){
		//			if(mat.name.StartsWith(mats[i].name)){
		//				break;
		//			}
		//		}

		return cost;
	}


	public Vector2 Pos2d(PlayerScript target){
		
		return new Vector2 (GridPosition(target).x, GridPosition(target).y);
	}

	public IntVector2 GridPosition(PlayerScript target){
		
		return new IntVector2 (target.X, target.Z);
	}

}
