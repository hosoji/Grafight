using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGrid : MonoBehaviour {


	public Transform prefab;
	public Transform centroid;

	GameObject hexGrid;
	GameObject centroidGrid;

	public static List <GameObject> gridNodes = new List<GameObject>();
	public static List<Transform> centroids = new List<Transform> ();

	public int x = 5;
	public int y = 5;

	public float radius = 0.3f;
	public bool useAsInnerCircleRadius = true;

	private float offsetX, offsetY;

	private int counter = 0;

	public Vector2[] removePoints;
	public int[] removeCentroids;

	void Awake() {
		float unitLength = ( useAsInnerCircleRadius )? (radius / (Mathf.Sqrt(3)/2)) : radius;

		hexGrid = new GameObject ("HexGrid");
		centroidGrid = new GameObject ("Centroids");

		float osPosX = 0.34f;
		float osPosy = 0.18f;


		offsetX = unitLength * Mathf.Sqrt(3);
		offsetY = unitLength * 1.5f;

		// Make Grid
		for( int i = 0; i < y; ++i ) {
			for( int j = 0; j < x; ++j ) {
				Vector2 hexpos = HexOffset( j, i, false );
				Vector3 pos = new Vector3( hexpos.x-2f , hexpos.y-2f, 0 );
				Transform t = Instantiate(prefab, pos, Quaternion.Euler(-90,0,0) );

				t.SetParent (hexGrid.transform);
				t.name = "(" + j.ToString () + ".0, " + i.ToString () + ".0)";

				for (int n = 0; n < removePoints.Length; n++) {
					if (t.name == removePoints [n].ToString ()) {
						//print (t.name + " " + t.ToString ());
						Destroy (t.gameObject);
					} 
				}

			}
		}


		// Make Centroid Grid
		for( int i = 0; i < y; i++ ) {
			for( int j = 0; j < x; j++ ) {

				Vector2 hexPoint = HexOffset (i, j, false);

				Vector3 pos1 = new Vector3( hexPoint.x-2f, (hexPoint.y-2f) + radius, 0 );
				Vector3 pos2 = new Vector3( hexPoint.x-2f, (hexPoint.y-2f) - radius, 0 );

				Transform t1 = Instantiate(centroid, pos1, Quaternion.Euler(-90,0,0) );
				Transform t2 = Instantiate(centroid, pos2, Quaternion.Euler(-90,0,0) );

				t1.SetParent (centroidGrid.transform);
				t2.SetParent (centroidGrid.transform);
				t1.name = counter.ToString ();
				counter++;
				t2.name = counter.ToString ();
				counter++;

				for (int n = 0; n < removeCentroids.Length; n++) {
					if (t1.name == removeCentroids [n].ToString ()) {
						//print (t1.name + " " + t1.ToString ());
						Destroy (t1.gameObject);
					} 

					if (t2.name == removeCentroids [n].ToString ()) {
						//print (t2.name + " " + t2.ToString ());
						Destroy (t2.gameObject);
					} 
				}

			}
		}

		hexGrid.transform.position = new Vector3 (hexGrid.transform.position.x + osPosX, hexGrid.transform.position.y + osPosy, 0);
		centroidGrid.transform.position = new Vector3 (centroidGrid.transform.position.x + osPosX, centroidGrid.transform.position.y + osPosy, 0);

		AddNodesToLists ();

	}

	Vector2 HexOffset( int x, int y, bool center ) {
		Vector2 position = Vector2.zero;

		float centerOffSetX = 1.1f;
		float centerOffSetY = 1.8f;

		if( y % 2 == 0 ) {
			position.x = x * offsetX;
			position.y = y * offsetY;
		}
		else {
			position.x = ( x + 0.5f ) * offsetX;
			position.y = y * offsetY;
		}

		if (center) {
			return new Vector2 (position.x+radius/centerOffSetX, position.y+radius/centerOffSetY);
		} else {
			return position;
		}
	}

	void AddNodesToLists(){

		NodeScript[] nodes = hexGrid.GetComponentsInChildren<NodeScript> ();
		Centroids[] cents = centroidGrid.GetComponentsInChildren<Centroids> ();

		for (int i = 0; i < nodes.Length; i++) {
			gridNodes.Add(nodes[i].gameObject);
		}

		for (int j = 0; j < cents.Length; j++) {
			centroids.Add(cents[j].gameObject.transform);
		}


	}
		

}
