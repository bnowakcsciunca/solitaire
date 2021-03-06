﻿//-----------------------------------------------------------------------------------------
// This script contains the main game logic flow including data structures for cards, piles, and movement.
// Most of the card functions are contained within the cards themselves.
//-----------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Ultimate_Solitaire : MonoBehaviour {
	static public Ultimate_Solitaire S;

	// DECK REFERENCE AND PILE LISTS
	public Deck 		deck;
	public List<Card> 	drawPile;
	public List<Card> 	discardPile;

	[SerializeField]
	public List<Card>[] tableaus = new List<Card>[7]; // a linked list containing all the tableau piles. the index is retrieved from Card.slotdef.tableaunum
	public List<Card>[] foundations = new List<Card>[4]; // a linked list containing all the foundation piles. the index MUST come from Foundation.pileNum

	// CARD REFERENCES
	public Card  		clickedCard;	// the card being moved
	public Card 		tp; 			// temp card for moving discard cards
	public Card 		tempCard; 		// a slot for the other card in a movement action
	
	Vector3 			mousePos2D;		// used for retrieving mouse position
	Vector3 			mousePos3D;		// used for retrieving mouse position
	public Vector3 		pos;			// a value for retaining the original position of a card or for changing it's position after a valid move 

	public bool 		clicked = false;	// A flag telling if a card has been clicked or dragged around
	public bool 		hover = false;		// a flag telling if the pointer is over a card
	public bool 		multi = false;		// a flag telling if the attempted move involves more then one card
	
	public Card[] 		multiMov;		// multiMov contains a reference to any multiple cards being moved
	public string[]		multiLayers;	// multiLayers contains a reference to the layers of all multiMov cards
	public Vector3[]	multiPos;		// multiPos contains a reference to the original positions of all multiMov cards

	// LAYOUT INFORMATION
	/*
	 * Most variables here pertain to the initial game layout and are used to set up the game
	 */
	public Layout 		layout;   // the layout script reference 
	public TextAsset 	deckXML;  // the xml doc with the card layout perameters 
	public TextAsset 	layoutXML; // the xml with the game layout info
	public Vector3 		layoutCenter; // a pointer to the center of the layout
	public float 		xOffset = 3; // offset between piles
	public float 		yOffset = 2.5f; 
	public Transform 	layoutAnchor; // the base of the game layout


	// SCORE INFORMATION
	public int			score;		// Stores the score
	public int			runMult;	// Stores the multiplier that doubles the points added for each card moved to the foundation in a row
	public Game_Screen	gScript;	// Direct reference to Game_Screen.cs in the holding object (set in the Inspector)

	public bool			winning;	// False is lose, true is win

	void Awake(){
		S = this;
	}
	
	// Use this for initialization
	void Start () {
		winning = false;

		multiLayers = new string[13];
		multiPos = new Vector3[13];

		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);
		Deck.Shuffle (ref deck.cards); // shuffles deck
		layout = GetComponent<Layout> ();
		layout.ReadLayout (layoutXML.text);
		drawPile = (deck.cards);
		LayoutGame ();

		runMult = 1; // This is always 1 at the start of the game
		
	}

	// a wrapper method created to avoid issues with static-nonstatic conflicts
	public Card DrawCall(){
		return Draw();}
	
	// Update is called once per frame
	void Update () {

		// Move a card or stack of cards 
		mousePos2D = Input.mousePosition;
		mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D);
		mousePos3D.z++;
		if (clicked = true && clickedCard != null&& (clickedCard.state == CardState.tableau || clickedCard.state == CardState.discard ) && clickedCard.faceUp == true) {
			clickedCard.transform.position = mousePos3D;

			// MULTI-MOVE CODE
			// Check that both multi is true AND that a valid multiMov array exists
			if (multi != false && multiMov != null) {
				for (int i = 0; i<multiMov.Length; i++) {
					mousePos3D.y -= 0.5f;
					multiMov[i].transform.position = mousePos3D;
					multiMov[i].SetSortOrder (4 * i); // Each card is on the MovingCard sort layer so this should work
				}
			}
		}
		
	}

	// a wrapper method created to avoid issues with static-nonstatic conflicts
	public void DrawUpdate(){
		UpdateDrawPile ();
	}

	// UpdateDrawPile() updates the draw pile
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

	// Draw() draws the top card from the drawpile.
	Card Draw(){
		Card cd = drawPile [0];
		drawPile.RemoveAt (0);
		return(cd);
	}

	// CheckValid returns whether or not there is a valid move between this clicked card and the card it is
	// colliding with.
	public bool CheckValid(Card clicked, Card collided){
		if (clicked.rank == collided.rank - 1 && clicked.colS != collided.colS) {
			//print ("DERP");
			return true;
		}
		else{
			return false;
		}
	}

	// LayoutGame() does the initial card layout.
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
		multiMov = null;
	}

	// UpdateScore() calls UpdateScore() in the script handling the game screen's UI. It updates the image based on
	// the score variable stored here.
	public void UpdateScore() {
		gScript.UpdateScore ();
	}
	
	/**
	// CheckWin() checks to see if the game has been won by checking each foundation pile for the bool full = true.
	// If so, it sets the game's winning flag to true and loads the EndScreen.
	public void CheckWin(){
		bool won = true;
		for (int i = 0; i < foundations.Length; i++) {
			if (foundations[i] == false)
				won = false;
		}
		if (won == true) {
			winning = true;
			Application.LoadLevel ("4_EndScreen");
		}

	} **/
	/**
    // CheckLose() checks to see if the game is lost by CheckingValidMoveRemains(). If lost, winning flag is set to false
    // and the game loads the EndScreen.
	public void CheckLose() {}
	**/
}
