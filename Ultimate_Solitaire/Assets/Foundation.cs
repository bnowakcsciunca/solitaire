using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Foundation : MonoBehaviour {
	public int curRank = 0;
	public string suit ;
	public List<Card> pile;

	// Use this for initialization
	void Start () {
		pile = new List<Card>();
		suit = "none";
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
