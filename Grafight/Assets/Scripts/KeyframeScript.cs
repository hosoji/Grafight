using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyframeScript : MonoBehaviour {

	[SerializeField] GameObject nodePrefab;
	[SerializeField] GameObject attkNodePrefab;
	[SerializeField] GameObject blockPrefab;

//	Transform[] astarNodes;
	float nodeDepleteDelay;

	GameObject key;
	GameObject area;
	GameObject node;
	GameObject block; 
	GameObject attackObj;
	public Transform keyFrameHolder;

	GameManager gameManager;
	PlayerScript player;
	TeamAssignment input;
//	Recording record;

	// For tracking positions
	public List<Vector3> recPos = new List <Vector3> ();
	public List<IntVector2> keyFrames = new List<IntVector2>();

	//For tracking territory nodes
	public List<Vector3> areaNodes = new List<Vector3>();
	public List<GameObject> areaNodeMarkers = new List<GameObject> ();


//	public List <Vector3> nodes = new List<Vector3>();

	public List <Vector3> attkNode = new List<Vector3> ();
//	public List <bool> ready = new List<bool>();
//	public List <bool> attkReady = new List<bool> ();

	//For tracking Pattern checking points and Path drawing points
	public Vector3 [] perimeterPoints = new Vector3 [4] ;
	public List <Vector3> pathPoints = new List<Vector3>();
	public int pathLength = 4;
	Vector3 pointPos;

	// For tracking Blocker GOs and positions
	public List <GameObject> activeBlocks = new List<GameObject> ();
	public List <Vector3> blockPoints = new List <Vector3> ();


	//For tracking Attacking GOs and positions

	public List <GameObject> activeAttacks = new List <GameObject> ();
	public List <Vector3> attackPoints = new List <Vector3> ();

//	public bool isReady = false;
//
//	public int blockIndex = 0;
//	public int attackIndex = 0;

	float yOffset = -0.8f;

	public Text segmentText;

	public int maxSegments;
	int initSegments;
	public bool segmentRemoved = false;

	[SerializeField] private int segments;
	public int Segments{
		get { 
			return segments; 
		}
		set{
			segments = value;
			if(segments <= 0)
			{
				segments = 0;
			}
			else if (segments > maxSegments)
			{
				segments = maxSegments;
			}
		}
	}

	int [] moveDirections = new int [3] ;


	int lastPositionX;
	int lastPositionY;

	public LineRenderer lr;
	public Material mat;



	void Start(){
		gameManager = GameObject.Find ("[GameManager]").GetComponent<GameManager> ();
		player = GetComponent<PlayerScript> ();
		input = GetComponent<TeamAssignment> ();

		player.enabled = true;
		AssignDefaultKeyframes ();

		for (int i = 0; i < perimeterPoints.Length; i++) {
			perimeterPoints [i] = Vector3.zero;
		}

		pathPoints.Add (player.gridPos);
			

		AssignDefaultKeyframes ();

		// Set up Line Renderer 

		lr = player.gameObject.GetComponentInChildren<LineRenderer> ();
		lr.enabled = false;
		lr.widthMultiplier = 0.2f;
		lr.material = mat;
		lr.receiveShadows = true;
		lr.alignment = LineAlignment.View;
		lr.textureMode = LineTextureMode.Tile;

		Segments = maxSegments;
		initSegments = maxSegments;

		lastPositionX = gameManager.GridPosition(player).x;
		lastPositionY = gameManager.GridPosition(player).y;

	}

	void Update(){
		ClearPath ();
		FadeAreaNodes ();
//		print (areaNodes.Count + " " + areaNodeMarkers.Count);

		pathLength = maxSegments + 1;



		if (segmentText != null){
		segmentText.text = Segments.ToString () + "/" + maxSegments.ToString ();
		}
	}

	public void AreaNodeAssignment(Vector3 pos){
//		
		keyFrames.Add (gameManager.GridPosition(player));

		if (!GameManager.globalNodes.ContainsKey (pos)) {
			
			Vector3 areaPos = new Vector3 (pos.x, pos.y + yOffset, pos.z);
			area = Instantiate (nodePrefab, areaPos, Quaternion.identity, keyFrameHolder);

			if (input.myTeam == TeamAssignment.Team.TEAM_A) {
				area.GetComponent<AreaNode> ().myPlayerNum = AreaNode.PlayerNum.ONE;
	

			} else if (input.myTeam == TeamAssignment.Team.TEAM_B) {
				area.GetComponent<AreaNode> ().myPlayerNum = AreaNode.PlayerNum.TWO;
			}


			area.transform.localEulerAngles = new Vector3 (90f, 0f, 0f);
		
//			area.transform.localScale = new Vector3 (3.4f, 3.4f, 3.4f);

			GameManager.globalNodes.Add (pos, area);
			areaNodes.Add (pos);
			areaNodeMarkers.Add (area);

			print (GameManager.globalNodes[pos]);

		} else {

			if (input.myTeam == TeamAssignment.Team.TEAM_A) {
				if (GameManager.globalNodes [pos].GetComponent<AreaNode> ().myPlayerNum == AreaNode.PlayerNum.TWO) {
					
					areaNodes.Add (pos);
					areaNodeMarkers.Add (GameManager.globalNodes [pos]);
					player.otherPlayer.GetComponent<KeyframeScript> ().areaNodes.Remove (pos);
					player.otherPlayer.GetComponent<KeyframeScript> ().areaNodeMarkers.Remove (GameManager.globalNodes [pos]);
					GameManager.globalNodes [pos].GetComponent<AreaNode> ().myPlayerNum = AreaNode.PlayerNum.ONE;


				} else {
					segmentRemoved = true;
				}
				
			} else {
				if (GameManager.globalNodes [pos].GetComponent<AreaNode> ().myPlayerNum == AreaNode.PlayerNum.ONE) {
					
					areaNodes.Add (pos);
					areaNodeMarkers.Add (GameManager.globalNodes [pos]);
					player.otherPlayer.GetComponent<KeyframeScript> ().areaNodes.Remove (pos);
					player.otherPlayer.GetComponent<KeyframeScript> ().areaNodeMarkers.Remove (GameManager.globalNodes [pos]);
					GameManager.globalNodes [pos].GetComponent<AreaNode> ().myPlayerNum = AreaNode.PlayerNum.TWO;
				} else {
					segmentRemoved = true;
				}
			}
//
//			areaNodes.Add (pos);
//			areaNodeMarkers.Add (GameManager.globalNodes [pos]);
//			player.otherPlayer.GetComponent<KeyframeScript> ().areaNodes.Remove (pos);
//			player.otherPlayer.GetComponent<KeyframeScript> ().areaNodeMarkers.Remove (GameManager.globalNodes [pos]);
//			
		}

	}

	void FadeAreaNodes(){
		GameObject[] nodeObjs = GameObject.FindGameObjectsWithTag ("AreaNode"); 

		for (int n = areaNodes.Count; n > 0; n--) {
			nodeDepleteDelay += Time.deltaTime;

			if (nodeDepleteDelay > 5) {
//				areaNodes.RemoveAt(n);

			}


		}

		for (int i = 0; i < nodeObjs.Length; i++) {

			if (areaNodes.Contains(nodeObjs[i].transform.position)){
				Debug.Log (nodeObjs [i].name);
				Destroy (nodeObjs[i]);
			}


		}


	
	}

	void AssignDefaultKeyframes(){
		for (int i = 0; i < gameManager.columnLength; i++) {
			AreaNodeAssignment (gameManager.gridArray[player.X,i].position);
			keyFrames.Clear ();


		}


	}

	public void KeyFrameAssignment(){
		if (!keyFrames.Contains (gameManager.GridPosition(player))) {
	
			keyFrames.Add (gameManager.GridPosition(player));
		}
		
	}



	public void ClearKeyFrames(){

//		nodes.Clear ();
		keyFrames.Clear ();
//		blockPoints.Clear ();

//		activeBlocks.Clear ();
//		ready.Clear ();
		for (int i = 0; i < activeAttacks.Count; i++) {
			Destroy (activeAttacks [i]);
		}
		attkNode.Clear ();
		activeAttacks.Clear ();
		attackPoints.Clear ();
//		attkReady.Clear ();


	}

//	public void ResetIndex(){
//		blockIndex = 0;
//		attackIndex = 0;
//	}


//	public void ActivateQueuedBlocker(){
//
//		if (activeBlocks.Count > 0) {
////			Debug.Log ("Enabling bloc");
//
//			GameObject currentBlock = activeBlocks [blockIndex];
//
//			currentBlock.GetComponent<Blocker> ().EnableBlock ();
//		}
//
//		if (blockIndex < activeBlocks.Count - 1) {
//			blockIndex++;
//		}
//
//
//	}

//	public void ActivateQueuedAttack(){
//
//		if (activeAttacks.Count > 0) {
//			//			Debug.Log ("Enabling bloc");
//
////			player.PlayerAction();
//		}
//
//		if (attackIndex < activeAttacks.Count - 1) {
//			attackIndex++;
//		}
//	}

	public void QueueBlocker(){
		float yOffset = -2f;

		Vector3 centroid = UtilScript.CalculateCentroid (perimeterPoints, yOffset);
	
		if (!GameManager.globalBlocks.ContainsValue(centroid)){


			//!blockPoints.Contains(centroid)
//			isReady = true;

//			nodes.Add (player.gridPos);
//			ready.Add (isReady);


			block = Instantiate (blockPrefab, centroid, Quaternion.identity) as GameObject;
			block.GetComponent<Blocker> ().keyAttached = this;

			if (input.myTeam == TeamAssignment.Team.TEAM_A) {
				block.GetComponent<Blocker> ().myTeam = Blocker.Team.PLAYER1;
			} else {
				block.GetComponent<Blocker> ().myTeam = Blocker.Team.PLAYER2;
			}


			activeBlocks.Add (block);

			blockPoints.Add (centroid);

			GameManager.globalBlocks.Add (block, centroid);

	
		

			for (int i = 0; i < areaNodes.Count; i++) {
				for (int n = 0; n < perimeterPoints.Length; n++) {
					if (areaNodes[i] == (perimeterPoints [n])) {
						areaNodeMarkers [i].GetComponent<AreaNode> ().ChangeNodeToNodeBlock();
						break;
					}
				}
			}
		}
	}


	public void QueueAttack(){
		float yOffset = -2f;

		Vector3 centroid = UtilScript.CalculateCentroid (perimeterPoints, yOffset);

		if (!attackPoints.Contains(centroid)){

//			isReady = true;
//
			attkNode.Add (player.gridPos);
//			attkReady.Add (isReady);

			attackObj = Instantiate (attkNodePrefab, centroid, Quaternion.identity) as GameObject;
//			block.GetComponent<Blocker> ().DisableBlock();
			activeAttacks.Add (attackObj);
//			activeBlockers.Add (centroid, block);
			attackPoints.Add (centroid);

		}

	}


	public void StorePerimeterPoints(Vector3 pos){
		bool isPlaced = false;


		for (int i = 0; i < perimeterPoints.Length; i++) {
			if (perimeterPoints [i] == Vector3.zero) {
				perimeterPoints [i] = pos;
				isPlaced = true;

				break;

			}
		}
			
		if (!isPlaced) {
			perimeterPoints [0] = perimeterPoints [1];
			perimeterPoints [1] = perimeterPoints [2];
			perimeterPoints [2] = perimeterPoints [3];
			perimeterPoints [3] = pos;
		}



	}

	public void StorePathPoints(Vector3 pos){

		bool isPlaced = false;

		if (pathPoints.Count < pathLength) {

			pathPoints.Add (pos);
			isPlaced = true;
		} else {
			
			for (int i = 1; i < pathPoints.Count; i++){
				if (i <= lr.positionCount) {
					pointPos = lr.GetPosition (i);
				}
				if (i != lr.positionCount) {
					lr.SetPosition (i - 1, pointPos);
				} else {
					lr.SetPosition (i, pos);
				}
			}
			pathPoints.RemoveAt (0);
			pathPoints.Add (pos);

		}
	}

	public void KeyFrameSettings(Vector3 pos){
		//Clears node spritre moved on

//		if (gameObject.name != "Bullet") {
//			for (int i = 0; i < astarNodes.Length; i++) {
//				if (pos == astarNodes [i].position) {
//					astarNodes [i].gameObject.GetComponent<SpriteRenderer> ().enabled = false;
//				}
//			}
//		}

		maxSegments = initSegments + activeBlocks.Count;

		AreaNodeAssignment (pos);
//		isReady = false;
	}

	public void PathDrawing(){

		lr.positionCount = pathPoints.Count;

		float yOffset = -0.9f;

		for (int i = 0; i < pathPoints.Count; i++) {
			Vector3 path = new Vector3 (pathPoints [i].x, pathPoints [i].y + yOffset, pathPoints [i].z);
			lr.SetPosition (i, path); 
//			if (perimeterPoints [i] != Vector3.zero) {
//				Vector3 path = new Vector3 (perimeterPoints [i].x, perimeterPoints [i].y + yOffset, perimeterPoints [i].z);
//				lr.SetPosition (i, path);
//			}
		}
	}

	public void PathCheck(){


		int dir = 9;
		//Debug.Log(key
		//Debug.Log(gameObject.name +" " + key.GridPosition().x +", "+lastPositionX);
		//Debug.Log(gameObject.name +" " + key.GridPosition().x +", "+lastPositionX);
		if (Mathf.Abs (gameManager.GridPosition(player).x - lastPositionX) == 1 && gameManager.GridPosition(player).y - lastPositionY == 0 ) {


			if (gameManager.GridPosition(player).x - lastPositionX > 0) {
				//Debug.Log (gameObject.name+"Moved Right");
				dir = 1;
				TrackMoves (dir);

			} else {
				//Debug.Log (gameObject.name+"Moved Left");
				dir = -1;
				TrackMoves (dir);
			}
			lastPositionX = gameManager.GridPosition(player).x;
		}

		if (Mathf.Abs (gameManager.GridPosition(player).y - lastPositionY) == 1 && gameManager.GridPosition(player).x - lastPositionX == 0) {

			if (gameManager.GridPosition(player).y - lastPositionY > 0) {
				//Debug.Log (gameObject.name+"Moved Up");
				dir = 2 ;
				TrackMoves (dir);

			} else {
				//Debug.Log (gameObject.name+"Moved Down");
				dir = -2 ;
				TrackMoves (dir);
			}
			lastPositionY = gameManager.GridPosition(player).y;
		}

		if (Mathf.Abs (gameManager.GridPosition(player).y - lastPositionY) == 1 &&  Mathf.Abs(gameManager.GridPosition(player).x - lastPositionX) == 1) {

			if (gameManager.GridPosition(player).y - lastPositionY > 0 && gameManager.GridPosition(player).x - lastPositionX > 0) {
				//				Debug.Log ("Moved UpRight");
				dir = 3;
				TrackMoves (dir);

			} else if (gameManager.GridPosition(player).y - lastPositionY > 0 && gameManager.GridPosition(player).x - lastPositionX < 0) {
				//				Debug.Log ("Moved UpLeft");
				dir = 3 ;
				TrackMoves (dir);

			} else if (gameManager.GridPosition(player).y - lastPositionY < 0 && gameManager.GridPosition(player).x - lastPositionX > 0) {
				//				Debug.Log ("Moved DownRight");
				dir = -3;
				TrackMoves (dir);
			} else {
				//				Debug.Log ("Moved DownLeft");
				dir = -3;
				TrackMoves (dir);
			}
			lastPositionY = gameManager.GridPosition(player).y;
			lastPositionX = gameManager.GridPosition(player).x;
		}






	} 

	void TrackMoves(int dir){
		bool isPlaced = false;


		for (int i = 0; i < moveDirections.Length; i++) {
			if (moveDirections [i] == 0) {
				moveDirections [i] = dir;
				isPlaced = true;

				break;

			}
		}


		if (!isPlaced) {
			moveDirections [0] = moveDirections [1];
			moveDirections [1] = moveDirections [2];
			moveDirections [2] = dir;
		}

		if (AttackPatternCheck()) {
			Debug.Log (gameObject.name + " Attack Formation Created");
			QueueAttack ();
		}

		if (DefensePatternCheck()) {
			Debug.Log (gameObject.name + "Defensive Formation Created");
			QueueBlocker ();
		}



		//		Debug.Log (moveDirections[0] + " " + moveDirections[1] + " " + moveDirections[2]);


	}



	void ClearPath(){
		if (Input.GetButtonDown (input.cancel)) {


			for (int i = 0; i < perimeterPoints.Length; i ++){
				perimeterPoints [i] = Vector3.zero;
			}
			ClearKeyFrames ();




			moveDirections[0] = 0;
			moveDirections [1] = 0;
			moveDirections [2] = 0;


			lr.enabled = false;
			recPos.Clear ();
			pathPoints.Clear ();

			player.ResetPlayerPos ();


			ReplenishSegments ();


			lastPositionX = player.X;

			lastPositionY = player.Z;

			StorePerimeterPoints (player.gridPos);
			StorePathPoints (player.gridPos);


		}
	}


	public void UseSegments(int cost){

//				Debug.Log ("Being called");
		Segments = segments - (1 + cost);
		segmentRemoved = true;


	}


	public void SegmentCheck(Vector3 pos){
		if (!GameManager.globalNodes.ContainsKey (pos)) {
			if (!segmentRemoved && Segments > 0) {
				
				UseSegments (0);
			} 
		} else {

			if (input.myTeam == TeamAssignment.Team.TEAM_A) {
				if (GameManager.globalNodes [pos].GetComponent<AreaNode> ().myPlayerNum == AreaNode.PlayerNum.TWO) {
					if (!segmentRemoved && Segments > GameManager.globalNodes [pos].GetComponent<AreaNode> ().cost) {
						UseSegments (GameManager.globalNodes [pos].GetComponent<AreaNode> ().cost);
						GameManager.globalNodes [pos].GetComponent<AreaNode> ().ChangeNodeBlockToNode();
					}

				} else {
					segmentRemoved = true;
				}
			} else {
				if (GameManager.globalNodes [pos].GetComponent<AreaNode> ().myPlayerNum == AreaNode.PlayerNum.ONE) {
					if (!segmentRemoved && Segments > GameManager.globalNodes [pos].GetComponent<AreaNode> ().cost) {
						UseSegments (GameManager.globalNodes [pos].GetComponent<AreaNode> ().cost);
						GameManager.globalNodes [pos].GetComponent<AreaNode> ().ChangeNodeBlockToNode();
					}
				} else {
					segmentRemoved = true;
				}
				
			}
			
		}
			

	}


	public void ReplenishSegments(){
		Segments = maxSegments;

	}

	public bool DefensePatternCheck(){
		bool patternMade = false;

		int [] defeseFormation = new int [3] ;

		defeseFormation [0] = 1;
		defeseFormation [1] = 2;
		defeseFormation [2] = -1;

		for (int index = 0; index < moveDirections.Length; index++) {

			if (Mathf.Abs (moveDirections [index]) != Mathf.Abs (defeseFormation [index])) {


				patternMade = false;
				break;

			} else {
				patternMade = true;
			}
		}

		if (moveDirections [2] != moveDirections[0] * -1 ) {

			patternMade = false;

		} 


		return patternMade;
	}



	public bool AttackPatternCheck(){
		bool patternMade = false;

		int [] attackFormation = new int [3] ;

		attackFormation [0] = 3;

		if (moveDirections [0] > 0) {
			attackFormation [1] = -2;

		} else {
			attackFormation [1] = 2;
		}

		attackFormation [2] = -3;


		for (int index = 0; index < moveDirections.Length; index++) {

			if (Mathf.Abs (moveDirections [index]) != Mathf.Abs (attackFormation [index])) {


				patternMade = false;
				break;

			} else {
				patternMade = true;
				//				Debug.Log ("pattern true");
			}

		}

		if (moveDirections [1] != attackFormation[1] ) {
			patternMade = false;
		} 


		if (moveDirections [2] != moveDirections[0] ) {
			patternMade = false;
		} 


		return patternMade;
	}




}
