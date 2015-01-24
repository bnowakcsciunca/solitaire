using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {
	public bool ________________;
	public PT_XMLReader xmlr;
	public List<string> cardNames;
	public List<Card>   cards;
	public List<Decorator> decorators;
	public List<CardDefinition> cardDefs;
	public Transform deckAnchor;
	public Dictionary<string,Sprite> dictSprites;

	// init deck called by solitare when it is ready
	public void InitDeck(string deckXMLText){
		ReadDeck (deckXMLText);
	}
	// this parses the xml file
	public void ReadDeck(string deckXMLText){
		xmlr = new PT_XMLReader ();
		xmlr.Parse (deckXMLText);

		//this prints a test line
		string s = "xml[0] decorator[0] ";
		s += "type=" + xmlr.xml["xml"][0]["decorator"][0].att ("type");
		s += " x=" + xmlr.xml ["xml"] [0] ["decorator"] [0].att ("x");
		s += " y=" + xmlr.xml ["xml"] [0] ["decorator"] [0].att ("y");
		s += " scale="+xmlr.xml["xml"][0]["decorator"][0].att("scale");
		//print (s);


		//read decorators for all cards
		decorators = new List<Decorator> ();
	}



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
