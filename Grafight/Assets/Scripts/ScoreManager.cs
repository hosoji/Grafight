using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	GameManager gameManager;

	public Text p1ScoreText;
	public Text p2ScoreText;

	public static int p1Score;
	public static int p2Score;

	// Use this for initialization
	void Start () {
		gameManager = GameObject.Find ("[GameManager]").GetComponent<GameManager> ();

		p1Score = 0;
		p2Score = 0;
	}
	
	// Update is called once per frame
	void Update () {

		p1ScoreText.text = p1Score.ToString();
		p2ScoreText.text = p2Score.ToString();

		foreach (GameObject node in GameManager.globalNodes.Values) {
			if (node.GetComponent<AreaNode> ().myPlayerNum == AreaNode.PlayerNum.ONE) {
				p1Score = CalculatePercentages(gameManager.player1.GetComponent<KeyframeScript> ().areaNodes.Count);
			}
			if (node.GetComponent<AreaNode> ().myPlayerNum == AreaNode.PlayerNum.TWO) {
				p2Score = CalculatePercentages(gameManager.player2.GetComponent<KeyframeScript> ().areaNodes.Count);
			}
		}
		
	}



	int CalculatePercentages(int value){

		SphereCollider[] grid = gameManager.grid.GetComponentsInChildren<SphereCollider> (); 

		float n  = UtilScript.remapRange (value, 0, grid.Length, 0, 100);
		Mathf.Clamp (n, 0, 100);

		return (int)n;
	}
}
