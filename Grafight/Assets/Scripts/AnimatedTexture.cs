using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedTexture : MonoBehaviour{
	public int columns = 2;
	public int rows = 2;
	public float framesPerSecond = 10f;

	public Renderer rend;

	//the current frame to display
	private int index = 0;

	void Start(){
		rend = GetComponent<Renderer>();
		rend.enabled = true;

		StartCoroutine(updateTiling());

		//set the tile size of the texture (in UV units), based on the rows and columns
		Vector2 size = new Vector2(1f / columns, 1f / rows);
		rend.sharedMaterial.SetTextureScale("_MainTex", size);
	}

	private IEnumerator updateTiling(){
		while (true)
		{
			
//			index++;

			index = Random.Range (0, rows * columns);
			if (index >= rows * columns)
				index = 0;

			//split into x and y indexes
			Vector2 offset = new Vector2((float)index / columns - (index / columns), //x index
				(index / columns) / (float)rows);          //y index

			rend.sharedMaterial.SetTextureOffset("_MainTex", offset);

			yield return new WaitForSeconds(1f / framesPerSecond);
		}

	}
}﻿
