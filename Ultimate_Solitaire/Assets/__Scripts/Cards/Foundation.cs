using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Foundation : MonoBehaviour {
	public int curRank = 0;
	public string suit ;
	//public List<Card> pile;
	[SerializeField]
	public int pileID;
	public int size;

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