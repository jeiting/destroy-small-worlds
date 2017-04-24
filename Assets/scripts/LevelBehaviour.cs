using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBehaviour : MonoBehaviour {

	public int numPlanets;
	public string levelName = "";

	// Use this for initialization
	void Start () {
		numPlanets = transform.childCount;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
