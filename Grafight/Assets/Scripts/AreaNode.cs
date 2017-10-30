using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaNode : MonoBehaviour {

	public Sprite [] sprites;

	public SpriteRenderer sr;

	public int cost;


	public Color[] colors;

	public enum PlayerNum{

		ONE,
		TWO,
	}

	public PlayerNum myPlayerNum;

	// Use this for initialization
	void Start () {

		sr = GetComponent<SpriteRenderer> ();

		myPlayerNum = this.myPlayerNum;

		sr.sprite = sprites [0];
	
	
		
	}
	
	// Update is called once per frame
	void Update () {

		if (myPlayerNum == PlayerNum.ONE) {
			sr.color = colors [0];

		} else if (myPlayerNum == PlayerNum.TWO) {

			sr.color = colors [1];

		}
		
	}

	public void ChangeNodeToNodeBlock(){
		sr.sprite = sprites [1];
		cost = 1;
	}

	public void ChangeNodeBlockToNode(){
		sr.sprite = sprites [0];
		cost = 0;
	}
}
