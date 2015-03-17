using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ultimate_Solitaire : MonoBehaviour {
	static public Ultimate_Solitaire S;

	// DECK REFERENCE AND PILE LISTS
	public Deck deck;
	public List<Card> 	drawPile;
	public List<Card> 	discardPile;
	[SerializeField]
	public List<Card>[] tableaus = new List<Card>[7];
	public List<Card>[] foundations = new List<Card>[4];

	// CARD REFERENCES
	public Card  		clickedCard;	// the card being moved
	public Card 		tp; 			// temp card for moving discard cards
	public Card 		tempCard; 		// a slot for the other card in a movement action
	
	Vector3 			mousePos2D;
	Vector3 			mousePos3D;
	public Vector3 		pos;
	public bool 		clicked = false;
	public bool 		hover = false;
	public bool 		multi = false;
	
	public Card[] 		multiMov;		// multiMov contains a reference to any multiple cards being moved
	public string[]		multiLayers;	// multiLayers contains a reference to the layers of all multiMov cards
	public Vector3[]	multiPos;		// multiPos contains a reference to the original positions of all multiMov cards

	// LAYOUT INFORMATION
	public 				Layout 		layout;
	public 				TextAsset 	deckXML;
	public 				TextAsset 	layoutXML;
	public 				Vector3 	layoutCenter;
	public 				float 		xOffset = 3;
	public 				float 		yOffset = 2.5f;
	public 				Transform 	layoutAnchor;   

	void Awake(){
		
		S = this;
	}
	
	// Use this for initialization
	void Start () {
		multiLayers = new string[13];
		multiPos = new Vector3[13];

		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);
		Deck.Shuffle (ref deck.cards); // shuffles deck
		layout = GetComponent<Layout> ();
		layout.ReadLayout (layoutXML.text);
		drawPile = (deck.cards);
		LayoutGame ();
		//print(tableaus[1].Count);
		
	}
	public Card DrawCall(){
		return Draw();}
	
	// Update is called once per frame
	void Update () {
		//print (tableaus [0].Count);
		// Move a card or stack of cards 
		mousePos2D = Input.mousePosition;
		mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D);
		mousePos3D.z++;
		if (clicked = true && clickedCard != null&& (clickedCard.state == CardState.tableau || clickedCard.state == CardState.discard) && clickedCard.faceUp == true) {
			clickedCard.transform.position = mousePos3D;

			// New multi-moving code
			// Check that both multi is true AND that a valid multiMov array exists
			if (multi != false && multiMov != null) {
				for (int i = 0; i<multiMov.Length; i++) {
					mousePos3D.y -= 0.5f;
					//mousePos3D.z -= 0.5f;
					multiMov[i].transform.position = mousePos3D;
				}
			}
		}
		
	}

	public void DrawUpdate(){
		UpdateDrawPile ();

	}
	void UpdateDrawPile(){
		Card cd;
		for (int i = 0; i<drawPile.Count; i++) { 
			cd = drawPile[i];
			cd.transform.parent= layoutAnchor;
			Vector2 dpStagger = layout.drawPile.stagger;
			cd.transform.localPosition = new Vector3(layout.multiplier.x * (layout.drawPile.x + i*dpStagger.x),
			                                         layout.multiplier.y * (layout.drawPile.y + i*dpStagger.y),
			                                         -(layout.drawPile.layerID)+0.1f*i);
			cd.faceUp = false;
			cd.state=CardState.drawpile;
			
			cd.SetSortingLayerName(layout.drawPile.layerName);
			cd.SetSortOrder(-10*i);

		}
	}
	Card Draw(){
		Card cd = drawPile [0];
		drawPile.RemoveAt (0);
		return(cd);
	}

	public bool CheckValid(Card clicked, Card collided){
		if (clicked.rank == collided.rank - 1 && clicked.colS != collided.colS) {
			//print ("DERP");
			return true;
		}
		else{
			return false;
		}
	}

	void LayoutGame(){
		if (layoutAnchor == null) {
			GameObject tGO = new GameObject("_LayoutAnchor");
			layoutAnchor = tGO.transform;
			layoutAnchor.transform.position=layoutCenter;
		}
		Card cp;
		for (int i = 0 ; i < tableaus.Length; i++)
			tableaus[i] = new List<Card>();

		foreach (SlotDef tSD in layout.slotDefs) {
			cp = Draw ();
			cp.faceUp = tSD.faceUp;
			cp.transform.parent = layoutAnchor;
			cp.transform.localPosition = new Vector3(layout.multiplier.x * tSD.x,layout.multiplier.y*tSD.y,-tSD.layerID);
			cp.layoutID = tSD.id;
			cp.slotDef = tSD;
			cp.state=CardState.tableau;
			cp.SetSortingLayerName(tSD.layerName);
			int temp = cp.slotDef.TableauNum;
			tableaus[temp].Add (cp);
		}

		UpdateDrawPile ();
	}

	// MultiReset() will reset the positions of all cards in the multiMov.
	// Note that this does not reset any internal logic for Lists, etc. - merely the visual
	// representation. The logic is handled in the Card class
	public void MultiReset() {
		Card c;
		for (int i=0; i<multiMov.Length; i++) {
			c = multiMov[i];
			c.transform.position = multiPos[i];
			c.SetSortingLayerName(multiLayers[i]);
		}
	}
}
