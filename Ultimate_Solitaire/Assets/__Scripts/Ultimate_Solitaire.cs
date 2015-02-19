using UnityEngine;
using System.Collections;

public class Ultimate_Solitaire : MonoBehaviour {
	static public Ultimate_Solitaire S;
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
