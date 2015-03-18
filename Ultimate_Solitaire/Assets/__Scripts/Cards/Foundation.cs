//---------------------------------------------------------------------------
// this script is for the block that acts as the foundation pile
// most of what is here is for tracking various values and conditions
//---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Foundation : MonoBehaviour {
	public int curRank = 0; // the current highest card on the foundation
	public string suit ; // the suit on this foundation
	//public List<Card> pile;
	[SerializeField]
	public int pileID;// used to diferentiate between foundations/ index into array of linked lists
	public int size; // the size of the pile on this foundation

	// Use this for initialization
	void Start () {
		//pile = new List<Card>();
		suit = "none";
		Ultimate_Solitaire.S.foundations[pileID]=new List<Card>();
	
	}
	
	// Update is called once per frame
	void Update () {
		size = Ultimate_Solitaire.S.foundations [pileID].Count;
	
	}
}
