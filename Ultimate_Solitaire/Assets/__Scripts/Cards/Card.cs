// code from textbook prospector chapter recoded for this project
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

	public CardState 		state = CardState.drawpile;			// The state the card is in
	bool 					disableFon = false;					// this prevents setting two foundations to the same suit

	// TEMPORARY VARIABLES TO USE IN MOVE LOGIC
	// public List<Card> 		hiddenBy = new List<Card>();
	// public CardState		prevState = CardState.empty;		// Stores the previous state of the card when moved, so that it can be returned should the trigger no longer be called
	bool 					drawn = false;						// Flag for clicking on the draw pile
	bool 					valid = false;						// Flag for a valid move
	bool 					fMove = false;						// Flag for a foundation move
	public int				prevTableau;						// If the previous state was in the tableau, this stores the tableau that used to store this
	public bool				validCol = false;
	public bool				isColliding = false;				// Check to see whether or not a valid move is already being tried
	public Collider			colTemp;							// Handle for a valid collision GameObject
	public CollisionType	colType;							// The type of collision currently happening
	public string 			prevSortingLayer;					// Stores the previous sorting layer so that we can return the card to it if necessary
	public string			newSortingLayer;					// Stores the new sorting layer to place a card in when we finish a move
	
	public 					CardDefinition def; 				// parsed from DeckXML.xml

	public          		Color startcolor;
	public          		Color highlight;

	public bool faceUp{
		get {
			return (!back.activeSelf);
		}
		set{
			back.SetActive(!value);
		}
	}

	void Start() {
		startcolor = renderer.material.color;
	}

	// OnMouseEnter() lets the Ultimate_Solitaire code know that a card is being hovered over
	void OnMouseEnter(){
		if (this.state != CardState.drawpile && this.faceUp == true)
			renderer.material.color = highlight;
		Ultimate_Solitaire.S.hover = true;
	}
	
	// OnMouseExit() lets the Ultimate_Solitaire code know that a card is no longer being hovered over
	void OnMouseExit(){
		Ultimate_Solitaire.S.hover = false;
		renderer.material.color = startcolor;
	}

	void OnTriggerEnter(Collider col){
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

	void OnTriggerExit(Collider col) {
		if (colTemp == col) {
			colTemp = null;
			// isColliding = false;
			colType = CollisionType.notColliding;
		}
	}

	// OnMouseDown() is called when this card is clicked. Depending on the state of the card, it will then call
	// an appropriate helper method.
	void OnMouseDown() {
		bool flag=false;
		// If this card is in a tableau pile
		if (this.state == CardState.tableau) {
			CreateTableauMove();
			flag = true;
		}
		// If this card is in the Discard pile
		if (this.state == CardState.discard) {
			CreateDiscardMove();
			flag = true;
		}
		
		// If the DrawPile is clicked on
		if (this.state == CardState.drawpile&& this.faceUp == false) {
			DrawCard();
			flag = true;
		}
		if (this.state == CardState.tableau && this.faceUp == false) {
			FlipCard ();
			flag = true;
		}
		if (flag == false) {
			print ("This message should not have been reached. If it has, the card's state is wonky and none of the click" +
			       "methods are returning.");
		}
	}

	void OnMouseUp() {
		// If the card is colliding with a valid move, do the game logic
		if (colTemp != null) {
			// print ("Valid move");
			DoMoveLogic();
			colTemp = null;
		} else {
			// print ("Not a valid move; colTemp is " + colTemp);
			// Reset all logic variables to false / null
			ResetMoveLogic ();
		}
	}

	// DoMoveLogic() does a specific card moving method depending on 
	// the type of collision experienced by the card when mouse button is released
	void DoMoveLogic(){
		if (colType == CollisionType.empty) {
			// print ("Trying MoveToEmpty()");
			MoveToEmpty (Ultimate_Solitaire.S.clickedCard, colTemp.GetComponent<TableauAnc> ());
			ResetMoveLogic ();
			return;
		}
		if (colType == CollisionType.foundation) {
			// print ("Trying MoveToFoundation()");
			MoveToFoundation (Ultimate_Solitaire.S.clickedCard, colTemp.GetComponent<Foundation> ());
			ResetMoveLogic ();
			return;
		}
		if (colType == CollisionType.tableau) {
			Ultimate_Solitaire.S.tempCard = colTemp.GetComponent<Card> ();
			// print ("Trying MoveCard()");
			MoveCard (Ultimate_Solitaire.S.clickedCard, Ultimate_Solitaire.S.tempCard);
			ResetMoveLogic ();
			return;
		}
	}

	// ******************************************************************************************************
	// *************************************Specific Card Moving Methods*************************************
	// ******************************************************************************************************
	void MoveCard(Card clickedcard, Card otherCard) {
		
		prevSortingLayer = clickedcard.GetSortingLayerName ();
		
		// Move a single card to the tableau
		if(otherCard.state == CardState.tableau && Ultimate_Solitaire.S.multi == false){
			Ultimate_Solitaire.S.pos = otherCard.transform.position;
			Ultimate_Solitaire.S.pos.z -= 1;	// Adjusts the z position relative to the parent card
			Ultimate_Solitaire.S.pos.y -= .5f;	// Adjusts y position relative to the parent card in the tableau
			bool passThrough = false;			// Prevents bugs between moving discard / moving tableau
			
			// Move from discard pile
			if (clickedcard.state == CardState.discard) {
				Ultimate_Solitaire.S.discardPile.Remove (clickedcard);	
				clickedcard.state = CardState.tableau;
				int tableaunumb = otherCard.slotDef.TableauNum;// retains tableau number
				Ultimate_Solitaire.S.tableaus [tableaunumb].Add (clickedcard);
				clickedcard.slotDef.TableauNum = tableaunumb;
				passThrough = true;
				valid = true;
				
				// SET THE LAYER. The row number is equal to the card's new place in the tableau
				newSortingLayer = "Row" + (Ultimate_Solitaire.S.tableaus [tableaunumb].Count - 1);
				// print ("Moved card from discard with new sorting layer of " + newSortingLayer);
				
				// Add some points and update the score
				Ultimate_Solitaire.S.score += 3;
				Ultimate_Solitaire.S.UpdateScore ();
			}
			// Move from tableau pile
			else if (clickedcard.state == CardState.tableau && passThrough == false && Ultimate_Solitaire.S.multi == false) {
				int tabl1 = clickedcard.slotDef.TableauNum; // retains clicked card's tableau number
				int tabl2 = otherCard.slotDef.TableauNum; // retains the other card's tableau number
				
				if (tabl1 != tabl2){
					// print (Ultimate_Solitaire.S.tableaus [tabl2].Count);
					Ultimate_Solitaire.S.tableaus [tabl1].Remove (clickedcard);
					Ultimate_Solitaire.S.tableaus [tabl2].Add (clickedcard); 
					clickedcard.slotDef.TableauNum = otherCard.slotDef.TableauNum;
					clickedcard.SetSortOrder(Ultimate_Solitaire.S.tableaus[clickedcard.slotDef.TableauNum].Count);  
					
					// SET THE LAYER. The row number is equal to the card's new place in the tableau
					newSortingLayer = "Row" + (Ultimate_Solitaire.S.tableaus [clickedcard.slotDef.TableauNum].Count - 1);
					// print ("Moved card from other tableau with new sorting layer of " + newSortingLayer);
				}	
				//print (Ultimate_Solitaire.S.tableaus [tabl2].Count); 
			}
		}
		
		// Move multiple cards
		if (clickedcard.state == CardState.tableau && Ultimate_Solitaire.S.multi == true) {
			valid = true; // Definitely a valid move

			Ultimate_Solitaire.S.pos = otherCard.transform.position;
			Ultimate_Solitaire.S.pos.z -= 1;
			Ultimate_Solitaire.S.pos.y -= .5f;
			int tabl1 = clickedcard.slotDef.TableauNum; // retains clicked card's tableau number
			int tabl2 = otherCard.slotDef.TableauNum;// retains the other card's tableau number
			if (tabl1 != tabl2){
				
				//print (Ultimate_Solitaire.S.tableaus [tabl2].Count);
				Ultimate_Solitaire.S.tableaus [tabl1].Remove (clickedcard);
				Ultimate_Solitaire.S.tableaus [tabl2].Add (clickedcard); 
				
				clickedcard.slotDef.TableauNum = otherCard.slotDef.TableauNum;
				clickedcard.SetSortOrder(Ultimate_Solitaire.S.tableaus[clickedcard.slotDef.TableauNum].Count);   
			}	
			Vector3 tpos = Ultimate_Solitaire.S.pos; // a vector3 used to calculate the final position of all involved cards 
			Card[] x = Ultimate_Solitaire.S.multiMov; // an arrya used to track the cards involved in this move

			// Get the sorting layer of the other card, and parse it.
			string otherLayer = otherCard.GetSortingLayerName();
			string otherLayerNum = Regex.Replace (otherLayer, "[^0-9]", ""); // Use Regex.Replace to remove all non integer characters from the otherLayer
			print (otherLayer + " " + otherLayerNum);
			int oL = 0;
			oL = int.Parse (otherLayerNum);
			oL++;


			// Place each card
			print ("This should try to place each remaining card in the multiMov array correctly.");
			for (int i = 0; i<x.Length;i++){
				tpos.z -=1;
				tpos.y -=.5f;

				x[i].SetSortingLayerName ("Row" + (oL + i)); // Set the sorting layer name to be greater than otherCard
				print ("Card " + i + " should be at " + x[i].GetSortingLayerName ());
				Ultimate_Solitaire.S.tableaus [tabl1].Remove (x[i]);
				Ultimate_Solitaire.S.tableaus [tabl2].Add (x[i]); 
				x[i].slotDef.TableauNum = tabl2;
			}	
		}
		
	}
	// MoveToEmpty() moves Card king to TableauAnc anc
	void MoveToEmpty(Card king, TableauAnc anc){
		valid = true; // Setting this makes layering work

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
		newSortingLayer = "Row0";
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
					int tem = clickedC.slotDef.TableauNum;// retains this cards tableau number
					if (clickedC.state == CardState.tableau)
						prevTableau = tem;
					Ultimate_Solitaire.S.tableaus [tem].Remove (clickedC);
					if (clickedC.state == CardState.discard)
						Ultimate_Solitaire.S.discardPile.Remove (clickedC); 
					Ultimate_Solitaire.S.foundations[fon.pileID].Add (clickedC);
					// print (fon.pile.Count);
					clickedC.SetSortOrder (100 * Ultimate_Solitaire.S.foundations[fon.pileID].Count);

					fMove = true;
					disableFon = true;// prevents multiple foundations from grabbing this card
					//this.state = CardState.foundation;

					// Update the score. Double the run multiplier. Then call UpdateScore() to update the UI
					Ultimate_Solitaire.S.score += 1 * Ultimate_Solitaire.S.runMult;
					Ultimate_Solitaire.S.runMult *= 2;
					Ultimate_Solitaire.S.UpdateScore();
				}
			}
		}
	}

	void CreateTableauMove() {
		Card[] x; // a temporary array to make this code shorter
		prevSortingLayer = GetSortingLayerName ();

		int q = 0;//the index in the temporay array of the clicked card
		int j = 0;// an index used to iterate through the array sans the clicked card
		Ultimate_Solitaire.S.clicked = true;
		Ultimate_Solitaire.S.clickedCard = this;
		int tabl = this.slotDef.TableauNum;    // retains tableau number
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
				
				// Grab the cards
				Ultimate_Solitaire.S.multiMov[j] = x[i];
				// Grab the original layers
				Ultimate_Solitaire.S.multiLayers[j] = x[i].GetSortingLayerName ();
				print ("Card " + j + " sorting layer is " + Ultimate_Solitaire.S.multiLayers[j]);
				// Then grab the original position
				Vector3 v = x[i].transform.position;
				Ultimate_Solitaire.S.multiPos[j] = v; 

				// Put each on the "Moving" layer
				Ultimate_Solitaire.S.multiMov[j].SetSortingLayerName ("MovingCard");

				// Ultimate_Solitaire.S.multiMov[j].SetSortingLayerName ("MovingCard");
				// Ultimate_Solitaire.S.multiMov[j].SetSortOrder (j);
				j++; // J is used because it starts at 0, and i starts at q. It increments through the shrunken array
				
			}
			
		}

		// Change Layer to MovingCard, which is the highest sorting 
		this.SetSortingLayerName("MovingCard");

		Ultimate_Solitaire.S.pos = this.transform.position;
		x = null;
	}

	void CreateDiscardMove() {
		prevSortingLayer = GetSortingLayerName();
		this.SetSortingLayerName("MovingCard");
		
		Card[] tem = Ultimate_Solitaire.S.discardPile.ToArray();// a temporary array for this move
		Ultimate_Solitaire.S.tp = tem[tem.Length-1];
		Ultimate_Solitaire.S.clicked = true;
		Ultimate_Solitaire.S.clickedCard = Ultimate_Solitaire.S.tp;
		if (Ultimate_Solitaire.S.pos == Vector3.zero)
			Ultimate_Solitaire.S.pos = Ultimate_Solitaire.S.tp.transform.position;
	}

	void DrawCard() {
		Card tem = Ultimate_Solitaire.S.DrawCall();// a temporary place to put the card that was drawn
		Ultimate_Solitaire.S.discardPile.Add(tem);
		tem.state = CardState.discard;
		tem.faceUp = true;
		tem.transform.parent = Ultimate_Solitaire.S.layoutAnchor;
		// NEW VARIABLE
		int discardSize = Ultimate_Solitaire.S.discardPile.Count; // the size of the discard pile
		float staggerDist = 0f; // the stagger offset of the base of the pile, incremented for all other cards (this SHOULD BE ZERO here)
		// print (discardSize);
		for (int i=0; i<discardSize; i++) {
			staggerDist -= 0.05f;
		}
		tem.transform.localPosition = new Vector3(Ultimate_Solitaire.S.layout.discardPile.x,
		                                          Ultimate_Solitaire.S.layout.discardPile.y,
		                                          staggerDist);
		tem.SetSortOrder(4 * Ultimate_Solitaire.S.discardPile.Count);
		drawn = true;
		// Set the sorting layer to discard so that these things draw correctly
		tem.SetSortingLayerName ("Discard");

		// Reset the run multiplier since a new card was drawn
		Ultimate_Solitaire.S.runMult = 1;
	}

	void FlipCard() {
		Card[] x;// a temporary array
		//prevSortingLayer = GetSortingLayerName ();
		int tem = this.slotDef.TableauNum;// retains tableau number
		x = Ultimate_Solitaire.S.tableaus[tem].ToArray();
		for (int i = 0;i <x.Length;i++){
			if (x[i].name == this.name&&i == x.Length-1){
				this.faceUp=true;
				
			}
		}
		x = null;
	}

	// ResetMoveLogic() should return all move variables to the conditions they exist in before a move is attempted (ie, the card is clicked)
	void ResetMoveLogic() {
		// If the card clicked is NOT the DrawPile, then proceed
		if (drawn == false) {
			// If it's not a valid move, then there is no newSortingLayer. newSortingLayer = prevSortingLayer so that it will return to the correct position in the tableau
			if (valid == false) {
				// Return to original sorting layer
				// print ("Going back to original place");
				// print ("Previous sorting layer should be: " + prevSortingLayer);
				// print ("Not a valid move.");
				newSortingLayer = prevSortingLayer;
				if (Ultimate_Solitaire.S.multi == true) {
					Ultimate_Solitaire.S.MultiReset ();
					/*this.SetSortingLayerName (newSortingLayer);
					int temp = int.Parse (newSortingLayer);

					for (int i = 0; i<Ultimate_Solitaire.S.multiMov.Length; i++) {
						Ultimate_Solitaire.S.multiMov[i].SetSortingLayerName ("Row" + (temp+i));
					}*/
				}
			}
			if (this.state == CardState.tableau && Ultimate_Solitaire.S.multi == false&&fMove == false) {
				this.transform.position = Ultimate_Solitaire.S.pos;
				print("The new sorting layer should be " + newSortingLayer);
				this.SetSortingLayerName (newSortingLayer); // newSortingLayer is created in MoveCard, this allows it to use the tableau num values easily
			}
			else if (this.state == CardState.discard && fMove == false){
				Ultimate_Solitaire.S.tp.transform.position = Ultimate_Solitaire.S.pos;
				this.SetSortingLayerName ("Discard");
			}
			else if (fMove == true) {
				Ultimate_Solitaire.S.clickedCard.state = CardState.foundation;
				Ultimate_Solitaire.S.clickedCard.transform.position = Ultimate_Solitaire.S.pos;
				this.SetSortingLayerName ("Foundation");

			}
			else if (this.state == CardState.tableau && Ultimate_Solitaire.S.multi == true && fMove == false){
				print ("ResetMoveLogic() - MultiMov");
			} else {
				print ("The type of move just made is unknown. Something bad happened.");
			}
			
			Ultimate_Solitaire.S.clicked = false; 		// Set the clicked boolean to false in Ultimate_Solitaire.cs
			Ultimate_Solitaire.S.clickedCard = null;	// Card has been moved so remove the reference to it
			Ultimate_Solitaire.S.tempCard = null;
			Ultimate_Solitaire.S.pos = Vector3.zero;	// Reset the move vector for the game
			
		}

		drawn = false;
		valid = false;
		fMove = false;
		disableFon = false;

		// Reset variables for multi-moving cards
		Ultimate_Solitaire.S.multi = false;

		// *** PROBABLY DON'T WANT TO NULL THESE
		Ultimate_Solitaire.S.multiMov = null;
		// Ultimate_Solitaire.S.multiPos = null;
		// Ultimate_Solitaire.S.multiLayers = null;

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