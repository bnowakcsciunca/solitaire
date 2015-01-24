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
		decorators = new List<Decorator> (); //init the list
		// grab a PT_XMLHashlist of all decorators in XML file
		PT_XMLHashList xDecos = xmlr.xml ["xml"] [0] ["decorator"];
		Decorator deco;
		for (int i = 0; i < xDecos.Count; i++) {
		// for each decorator
			deco =  new Decorator();
			//copy attributes of the <deorator> to the Decorator
			deco.type = xDecos[i].att ("type");
			// set the flip bool based on if the text is 1
			deco.flip = (xDecos[i].att ("flip") =="1");
			// floats need to be parsed from the attribute strings
			deco.scale = float.Parse(xDecos[i].att ("scale"));

			// location vectors
			deco.loc.x = float.Parse(xDecos[i].att ("x"));
			deco.loc.y = float.Parse(xDecos[i].att ("y"));
			deco.loc.z = float.Parse(xDecos[i].att ("z"));
			// add the temporary decorator to the list
			decorators.Add(deco);

		
		
		
		
		}


		// read pip locations for each card num
		cardDefs = new List<CardDefinition> ();// init list of cards
		// grab a PT_XMLHashlist of all the cards
		PT_XMLHashList xCardDefs = xmlr.xml ["xml"] [0] ["card"];
		for (int i =0; i < xCardDefs.Count; i++) {
			// for each card
			// create a card defintion
			CardDefinition cDef = new CardDefinition();
			// parse attribute values and add them to Cdf
			cDef.rank = int.Parse(xCardDefs[i].att ("rank"));
			// grab  a PT_XMLHashtable of all the pips on the card
			PT_XMLHashList xPips = xCardDefs[i]["pip"];
			if (xPips != null){
				for(int j = 0; j < xPips.Count; j++){
					// iterate through all pips
					deco = new Decorator();
					// pips on the card are handled through the decorator class
					deco.type = "pip";
					deco.flip = (xPips[j].att ("flip") == "1");
					deco.loc.x = float.Parse(xPips[j].att ("x"));
					deco.loc.y = float.Parse(xPips[j].att ("y"));
					deco.loc.z = float.Parse(xPips[j].att ("z"));
					if (xPips[j].HasAtt("scale")){
						deco.scale = float.Parse(xPips[j].att ("scale"));
					}
					cDef.pips.Add(deco);


					

				}

			}
			// face cards have a face attribute
			// cdef.face is the name of this
			if (xCardDefs[i].HasAtt("face")){
				cDef.face = xCardDefs[i].att ("face");
			}
			cardDefs.Add (cDef);
				
		
		
		}
	}



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
