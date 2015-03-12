// code from textbook prospector chapter recoded for this project
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CardState{
	drawpile,
	tableau,
	target,
	discard,
	foundation,
	empty
}

public enum CollisionType {
	foundation,
	empty,
	tableau,
	notColliding
}

public class Card : MonoBehaviour {

	// CARD INFORMATION
	public string 			suit; 								// ( C D H or S)
	public int 				rank; 								// Can be a value from 1 - 14
	public Color 			color = Color.black;
	public string 			colS = "Black";
	public List<GameObject> decoGOs = new List<GameObject>(); 	// Decorators list
	public List<GameObject> pipGOs = new List<GameObject>();	// Pips list
	public SpriteRenderer[] spriteRenderers;
	public GameObject 		back;  								// the back of the card
	public int 				layoutID;
	public SlotDef 			slotDef;

	public CardState 		state = CardState.drawpile;
	bool 					disableFon = false;					// this prevents setting two foundations to the same suit

	// TEMPORARY VARIABLES TO USE IN MOVE LOGIC
	// public List<Card> 		hiddenBy = new List<Card>();
	// public CardState		prevState = CardState.empty;		// Stores the previous state of the card when moved, so that it can be returned should the trigger no longer be called
	bool 					drawn = false;
	bool 					valid = false;
	bool 					fMove = false;
	public int				prevTableau;						// If the previous state was in the tableau, this stores the tableau that used to store this
	public bool				validCol = false;
	public bool				isColliding = false;				// Check to see whether or not a valid move is already being tried
	public Collider			colTemp;							// Handle for a valid collision GameObject
	public CollisionType	colType;							// The type of collision currently happening
	public string 			prevSortingLayer;					// Stores the previous sorting layer so that we can return the card to it if necessary
	public string			newSortingLayer;					// Stores the new sorting layer to place a card in when we finish a move
	
	public 					CardDefinition def; 				// parsed from DeckXML.xml

	public bool faceUp{
		get {
			return (!back.activeSelf);
		}
		set{
			back.SetActive(!value);
		}
	}
	// OnMouseEnter() lets the Ultimate_Solitaire code know that a card is being hovered over
	void OnMouseEnter(){
		Ultimate_Solitaire.S.hover = true;
	}
	
	// OnMouseExit() lets the Ultimate_Solitaire code know that a card is no longer being hovered over
	void OnMouseExit(){
		Ultimate_Solitaire.S.hover = false;
	}
	
	/* 	OnTriggerEnter() is called when this card moves over something that is a trigger
		For each valid collision:
		(1) Set the prevState to the card's current state
		(2) Set that this is a validCol
		(3) Set that this isColliding so that weird errors don't happen when moving this over another card (ie, only one collision event can happen at a time)
		(4) Call the relative function
	*/
	void OnTriggerEnter(Collider col){
		if (isColliding == false) {
			if (col.tag == "Empty" && this.rank == 13) { 
				if (col.GetComponent<TableauAnc> ().pactive == true) { 
					colType = CollisionType.empty;
					colTemp = col;
					isColliding = true;
					// print ("Colliding now with " + col + "; collision type is: " + colType);
				}
			} else if (col.name == "Foundation") {
				colType = CollisionType.foundation;
				colTemp = col;
				isColliding = true;
				// print ("Colliding now with " + col + "; collision type is: " + colType);
			} else {
				if (col.tag != "Empty" && col.GetComponent<Card>().faceUp == true) {
					Ultimate_Solitaire.S.tempCard = col.GetComponent<Card> ();
					if (Ultimate_Solitaire.S.tempCard != Ultimate_Solitaire.S.clickedCard && Ultimate_Solitaire.S.tempCard.faceUp == true && this.faceUp == true && this == Ultimate_Solitaire.S.clickedCard) {
						valid = Ultimate_Solitaire.S.CheckValid (Ultimate_Solitaire.S.clickedCard, Ultimate_Solitaire.S.tempCard);
						if (valid == true) {
							colType = CollisionType.tableau;
							colTemp = col;
							isColliding = true;
							// print ("Colliding now with " + col + "; collision type is: " + colType);
						}
					}
				}
			}
		}
	}

	void OnTriggerExit(Collider col) {
		if (colTemp == col) {
			colTemp = null;
			isColliding = false;
			colType = CollisionType.notColliding;
		}
	}

	// OnMouseDown() is called when this card is clicked
	void OnMouseDown() {
		Card[] x;
		prevSortingLayer = GetSortingLayerName ();
		// this.SetSortingLayerName("MovingCard");
		
		// print ("The current sorting layer is: " + prevSortingLayer);
		// If face-up on the tableau, make this card movable
		if (this.state == CardState.tableau) {
			// Change Layer to MovingCard, which is the highest sorting 
			this.SetSortingLayerName("MovingCard");
			
			int q = 0;
			int j = 0;
			Ultimate_Solitaire.S.clicked = true;
			Ultimate_Solitaire.S.clickedCard = this;
			int tabl = this.slotDef.TableauNum;
			x = Ultimate_Solitaire.S.tableaus[tabl].ToArray();
			for (int i = 0;i<x.Length;i++){
				if (x[i].name == this.name)
					q = i;
			}
			
			// If there are multiple cards beneath this one, set multi to true
			if (q != x.Length-1 && x[q].faceUp==true){
				// print ("multi");
				Ultimate_Solitaire.S.multi = true;
			}
			
			// Then, if multi is true add them to multiMov array
			if (Ultimate_Solitaire.S.multi == true){
				Ultimate_Solitaire.S.multiMov = new Card[x.Length-q];
				for (int i = q ; i<x.Length; i++){
					// print (x[i].name);
					Ultimate_Solitaire.S.multiMov[j] = x[i];
					/*
					Ultimate_Solitaire.S.multiLayers[j] = x[i].GetSortingLayerName ();
					Vector3 v = x[i].transform.position;
					Ultimate_Solitaire.S.multiOriginal[j] = v; */
					j++;
					
				}
				
			}
			Ultimate_Solitaire.S.pos = this.transform.position;
		}
		if (this.state == CardState.discard) {
			prevSortingLayer = GetSortingLayerName();
			this.SetSortingLayerName("MovingCard");
			
			Card[] tem = Ultimate_Solitaire.S.discardPile.ToArray();
			Ultimate_Solitaire.S.tp = tem[tem.Length-1];
			Ultimate_Solitaire.S.clicked = true;
			Ultimate_Solitaire.S.clickedCard = Ultimate_Solitaire.S.tp;
			if (Ultimate_Solitaire.S.pos == Vector3.zero)
				Ultimate_Solitaire.S.pos = Ultimate_Solitaire.S.tp.transform.position;
		}
		
		// If the DrawPile is clicked on
		if (this.state == CardState.drawpile&& this.faceUp == false) {
			// Here I think we should check if the card has children and if so, move the entire pile
			Card tem = Ultimate_Solitaire.S.DrawCall();
			Ultimate_Solitaire.S.discardPile.Add(tem);
			tem.state = CardState.discard;
			tem.faceUp = true;
			tem.transform.parent = Ultimate_Solitaire.S.layoutAnchor;
			tem.transform.localPosition = new Vector3(Ultimate_Solitaire.S.layout.discardPile.x,
			                                          Ultimate_Solitaire.S.layout.discardPile.y,
			                                          .05f);
			tem.SetSortOrder(100 * Ultimate_Solitaire.S.discardPile.Count);
			drawn = true;
			// Set the sorting layer to discard so that these things draw correctly
			tem.SetSortingLayerName ("Discard");
			
		}
		// If the card is at the top of it's tableau stack but face down, make it go face up
		if (this.state == CardState.tableau && this.faceUp == false) {
			int tem = this.slotDef.TableauNum;
			x = Ultimate_Solitaire.S.tableaus[tem].ToArray();
			for (int i = 0;i <x.Length;i++){
				if (x[i].name == this.name&&i == x.Length-1){
					this.faceUp=true;
					
				}
			}
		}
		x = null;
	}

	void OnMouseUp() {
		// If the card is colliding with a valid move, do the game logic
		if (colTemp != null) {
			DoMoveLogic();
			colTemp = null;
		}

		// If the card clicked is NOT the DrawPile, then proceed
		if (drawn == false) {
			// If it's not a valid move, then there is no newSortingLayer. newSortingLayer = prevSortingLayer so that it will return to the correct position in the tableau
			if (valid == false) {
				// Return to original sorting layer
				// print ("Going back to original place");
				// print ("Previous sorting layer should be: " + prevSortingLayer);
				newSortingLayer = prevSortingLayer;
			}
			if (this.state == CardState.tableau) {
				this.transform.position = Ultimate_Solitaire.S.pos;
				this.SetSortingLayerName (newSortingLayer); // newSortingLayer is created in MoveCard, this allows it to use the tableau num values easily
			}
			else if (this.state == CardState.discard){
				Ultimate_Solitaire.S.tp.transform.position = Ultimate_Solitaire.S.pos;
				this.SetSortingLayerName ("Discard");
			}
			else if (fMove == true) {
				Ultimate_Solitaire.S.clickedCard.state = CardState.foundation;
				this.SetSortingLayerName ("Foundation");
			}
			else {
				print ("Something bad happened and this card is not working correctly.");
			}
			
			Ultimate_Solitaire.S.clicked = false; 		// Set the clicked boolean to false in Ultimate_Solitaire.cs
			Ultimate_Solitaire.S.clickedCard = null;	// Card has been moved so remove the reference to it
			Ultimate_Solitaire.S.tempCard = null;
			Ultimate_Solitaire.S.pos = Vector3.zero;	// Reset the move vector for the game
			
		}
		
		// Reset all logic variables to false / null
		ResetMoveLogic ();
	}

	// DoMoveLogic() does a specific card moving method depending on the type of collision the trigger that the card is over is generating
	void DoMoveLogic(){
		if (colType == CollisionType.empty) {
			print ("Trying MoveToEmpty()");
			MoveToEmpty (Ultimate_Solitaire.S.clickedCard, colTemp.GetComponent<TableauAnc> ()); 
		}
		if (colType == CollisionType.foundation) {
			print ("Trying MoveToFoundation()");
			MoveToFoundation (Ultimate_Solitaire.S.clickedCard, colTemp.GetComponent<Foundation> ());
		}
		if (colType == CollisionType.tableau) {
			Ultimate_Solitaire.S.tempCard = colTemp.GetComponent<Card> ();
			print ("Trying MoveCard()");
			MoveCard (Ultimate_Solitaire.S.clickedCard, Ultimate_Solitaire.S.tempCard); 
		}
	}

	// ******************************************************************************************************
	// *************************************Specific Card Moving Methods*************************************
	// ******************************************************************************************************

	// MoveToEmpty() moves Card king to TableauAnc anc
	void MoveToEmpty(Card king, TableauAnc anc){
		Ultimate_Solitaire.S.pos = anc.transform.position;
		Ultimate_Solitaire.S.pos.z -= 2;
		
		// If the King was in the discard pile, remove it from the list and set it's state to the tableau pile
		if (king.state == CardState.discard) {
			Ultimate_Solitaire.S.discardPile.Remove(king);
			king.state= CardState.tableau;
			Ultimate_Solitaire.S.tableaus[anc.pileID].Add(king);
			king.slotDef.TableauNum = anc.pileID;
			// print ("new size " +  Ultimate_Solitaire.S.tableaus[anc.pileID].Count); 
			
		}
		if (king.state == CardState.tableau) {
			int temp = king.slotDef.TableauNum;	
			Ultimate_Solitaire.S.tableaus[temp].Remove(king);
			Ultimate_Solitaire.S.tableaus[anc.pileID].Add(king);
			king.slotDef.TableauNum = anc.pileID;
			// print ("new size " +  Ultimate_Solitaire.S.tableaus[anc.pileID].Count);
		}
		// Set the King's sorting layer to be the bottom
		king.SetSortingLayerName ("Row0");
	}

	// MoveToFoundation moves Card clickedC to Foundation fon
	void MoveToFoundation(Card clickedC, Foundation fon){
		prevSortingLayer = clickedC.GetSortingLayerName ();
		if (disableFon == false){
			if (clickedC.rank == fon.curRank + 1) {
				if (fon.suit == "none" || fon.suit == clickedC.suit) {
					Vector3 transfer = fon.transform.position;
					transfer.z -= 1;
					Ultimate_Solitaire.S.pos = transfer;
					
					
					fon.curRank = clickedC.rank;
					fon.suit = clickedC.suit;
					int tem = clickedC.slotDef.TableauNum;
					if (clickedC.state == CardState.tableau)
						prevTableau = tem;
					Ultimate_Solitaire.S.tableaus [tem].Remove (clickedC);
					if (clickedC.state == CardState.discard)
						Ultimate_Solitaire.S.discardPile.Remove (clickedC); 
					fon.pile.Add (clickedC);
					// print (fon.pile.Count);
					clickedC.SetSortOrder (100 * fon.pile.Count);
					fMove = true;
					disableFon = true;
				}
			}
		}
	}
	
	void MoveCard(Card clickedcard, Card otherCard) {
		
		prevSortingLayer = clickedcard.GetSortingLayerName ();
		
		// If you're moving a single card
		if(otherCard.state == CardState.tableau && Ultimate_Solitaire.S.multi == false){
			Ultimate_Solitaire.S.pos = otherCard.transform.position;
			Ultimate_Solitaire.S.pos.z -= 1;
			Ultimate_Solitaire.S.pos.y -= .5f;
			bool passThrough = false;
			
			// If you're moving the card from the Discard pile
			if (clickedcard.state == CardState.discard) {
				Ultimate_Solitaire.S.discardPile.Remove (clickedcard);	
				clickedcard.state = CardState.tableau;
				int tableaunumb = otherCard.slotDef.TableauNum;
				Ultimate_Solitaire.S.tableaus [tableaunumb].Add (clickedcard);
				clickedcard.slotDef.TableauNum = tableaunumb;
				passThrough = true;
				valid = true;

				// SET THE LAYER. The row number is equal to the card's new place in the tableau
				newSortingLayer = "Row" + (Ultimate_Solitaire.S.tableaus [tableaunumb].Count - 1);
				print ("Moved card from discard with new sorting layer of " + newSortingLayer);
			}
			
			//else if (clickedcard.state == CardState.tableau && passThrough == false) {
			//	int tabl1 = clickedcard.slotDef.TableauNum;
			//	int tabl2 = otherCard.slotDef.TableauNum;
			//}
			// If you're moving the card from another Tableau
			else if (clickedcard.state == CardState.tableau && passThrough == false && Ultimate_Solitaire.S.multi == false) {
				int tabl1 = clickedcard.slotDef.TableauNum;
				int tabl2 = otherCard.slotDef.TableauNum;
				
				if (tabl1 != tabl2){
					// print (Ultimate_Solitaire.S.tableaus [tabl2].Count);
					Ultimate_Solitaire.S.tableaus [tabl1].Remove (clickedcard);
					Ultimate_Solitaire.S.tableaus [tabl2].Add (clickedcard); 
					clickedcard.slotDef.TableauNum = otherCard.slotDef.TableauNum;
					clickedcard.SetSortOrder(Ultimate_Solitaire.S.tableaus[clickedcard.slotDef.TableauNum].Count);  
					
					// SET THE LAYER. The row number is equal to the card's new place in the tableau
					newSortingLayer = "Row" + (Ultimate_Solitaire.S.tableaus [clickedcard.slotDef.TableauNum].Count - 1);
					print ("Moved card from other tableau with new sorting layer of " + newSortingLayer);
				}	
				//print (Ultimate_Solitaire.S.tableaus [tabl2].Count); 
			}
		}

		// If you're moving multiple cards
		if (clickedcard.state == CardState.tableau && Ultimate_Solitaire.S.multi == true) {
			Ultimate_Solitaire.S.pos = otherCard.transform.position;
			Ultimate_Solitaire.S.pos.z -= 1;
			Ultimate_Solitaire.S.pos.y -= .5f;
			int tabl1 = clickedcard.slotDef.TableauNum;
			int tabl2 = otherCard.slotDef.TableauNum;
			if (tabl1 != tabl2){
				
				//print (Ultimate_Solitaire.S.tableaus [tabl2].Count);
				Ultimate_Solitaire.S.tableaus [tabl1].Remove (clickedcard);
				Ultimate_Solitaire.S.tableaus [tabl2].Add (clickedcard); 
				
				clickedcard.slotDef.TableauNum = otherCard.slotDef.TableauNum;
				clickedcard.SetSortOrder(Ultimate_Solitaire.S.tableaus[clickedcard.slotDef.TableauNum].Count);   
			}	
			Vector3 tpos = Ultimate_Solitaire.S.pos;
			Card[]x = Ultimate_Solitaire.S.multiMov;
			for (int i = 1; i<x.Length;i++){
				tpos.z -=1;
				tpos.y -=.5f;
				x[i].transform.position = tpos;
				Ultimate_Solitaire.S.tableaus [tabl1].Remove (x[i]);
				Ultimate_Solitaire.S.tableaus [tabl2].Add (x[i]); 
				x[i].slotDef.TableauNum = tabl2;
			}	
		}
		
	}

	// ResetMoveLogic() should return all move variables to the conditions they exist in before a move is attempted (ie, the card is clicked)
	void ResetMoveLogic() {
		drawn = false;
		valid = false;
		fMove = false;
		disableFon = false;
		Ultimate_Solitaire.S.multi = false;
		Ultimate_Solitaire.S.multiMov = null;
		prevSortingLayer = null;
		newSortingLayer = null;
		colType = CollisionType.notColliding;
		colTemp = null;
		validCol = false;
		isColliding = false;
	}

	// ******************************************************************************************************
	// *************************************Utility Methods**************************************************
	// ******************************************************************************************************

	// Grab the current sorting layer name
	public string GetSortingLayerName() {
		foreach (SpriteRenderer tSR in spriteRenderers) {
			return (tSR.sortingLayerName);
		}
		return null;
		
	}

	// Set the sorting layer name to string tSLN
	public void SetSortingLayerName(string tSLN){
		PopulateSpriteRenderers ();
		foreach (SpriteRenderer tSR in spriteRenderers) {
			tSR.sortingLayerName = tSLN;		
		}
		
	}
	
	public void PopulateSpriteRenderers(){
		if (spriteRenderers == null || spriteRenderers.Length == 0) {
			spriteRenderers = GetComponentsInChildren<SpriteRenderer>();	
		}
	}
	
	public void SetSortOrder(int sOrd){
		PopulateSpriteRenderers ();
		foreach (SpriteRenderer tSR in spriteRenderers) {
			if (tSR.gameObject == this.gameObject){
				tSR.sortingOrder = sOrd;
				continue;
			}
			switch(tSR.gameObject.name){
			case "back":
				tSR.sortingOrder = sOrd+2;
				break;
			case "face":
			default: tSR.sortingOrder = sOrd+1;
				break;
			}
		}
		
	}
	
	// SetTableauSortingOrder sets the layer of this card to one higher than the layer of the card it is on top of
	public void SetTableauSortingOrder(Card otherCard) {
		string temp = otherCard.GetSortingLayerName ();
		int otherNum = int.Parse (temp);
		// print (otherNum);
		SetSortingLayerName ("Row" + (otherNum + 1));
	}
}

[System.Serializable]
public class Decorator{
	
	public string type; // for card pips
	public Vector3 loc; // location of pip on the card
	public bool flip = false; // Wheather the sprite is inverted
	public float scale = 1f; // the scale of the sprite
}

[System.Serializable]
public class CardDefinition{
	public string   face; //used for face card
	public int      rank; // a number 1-13
	public List<Decorator> pips = new List<Decorator> (); //pips used
}