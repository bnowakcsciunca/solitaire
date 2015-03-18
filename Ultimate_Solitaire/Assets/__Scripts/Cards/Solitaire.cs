//-------------------------------------
// this is an unused script
//-------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Solitaire : MonoBehaviour {
	static public Solitaire S;
	public Deck deck;
	public TextAsset deckXML;

	void Awake(){

		S = this;
	}

	// Use this for initialization
	void Start () {
		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);
		Deck.Shuffle (ref deck.cards); // shuffles deck
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
