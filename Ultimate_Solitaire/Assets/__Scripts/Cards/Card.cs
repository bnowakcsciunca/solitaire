// code from textbook prospector chapter recoded for this project
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CardState{
	drawpile,
	tableau,
	target,
	discard,
	foundation
}

public class Card : MonoBehaviour {
	// CARD INFORMATION
	public string 			suit; // ( C D H or S)
	public int 				rank; // value 1 - 14
	public Color 			color = Color.black;
	public string 			colS = "Black";
	public List<GameObject> decoGOs = new List<GameObject>(); 	// Decorators list
	public List<GameObject> pipGOs = new List<GameObject>();	// Pips list
	public SpriteRenderer[] spriteRenderers;


	public GameObject 		back;  // the back of the card
	public CardState 		state = CardState.drawpile;
	public List<Card> 		hiddenBy = new List<Card>();
	public int 				layoutID;
	public SlotDef 			slotDef;
	bool 					drawn = false;
	bool 					valid = false;
	bool 					fMove = false;
	bool 					disableFon = false;// this prevents setting two foundations to the same suit

	public string 			prevSortingLayer;		// Stores the previous sorting layer so that we can return the card to it if necessary


	public 					CardDefinition def; // parsed from DeckXML.xml

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
		if (this.state == CardState.tableau) {
			print ("tableau");	
		}
		if (this.state == CardState.drawpile) {
			print ("DECK");	
		}
		Ultimate_Solitaire.S.hover = true;



		//print (this.name);
		
	}

	// OnMouseExit() lets the Ultimate_Solitaire code know that a card is no longer being hovered over
	void OnMouseExit(){
		Ultimate_Solitaire.S.hover = false;
	}

	// OnTriggerEnter() is called when this card is dropped onto something else
	void OnTriggerEnter(Collider col){

		// You can move a King to an empty spot on the tableau
		if (col.tag == "Empty" && this.rank == 13) { 
			if (col.GetComponent<TableauAnc> ().pactive == true) { 
				print ("empty");
				MoveToEmpty (Ultimate_Solitaire.S.clickedCard, col.GetComponent<TableauAnc> ()); 
			}	
		}

		// You can also move cards to foundation in numerical order sorted by suit
		else if (col.name == "Foundation") {
			MoveToFoundation (Ultimate_Solitaire.S.clickedCard, col.GetComponent<Foundation> ());
		} else {
			if (col.tag != "Empty") {
				Ultimate_Solitaire.S.tempCard = col.GetComponent<Card> ();
				if (Ultimate_Solitaire.S.tempCard != Ultimate_Solitaire.S.clickedCard && Ultimate_Solitaire.S.tempCard.faceUp == true && this.faceUp == true && this == Ultimate_Solitaire.S.clickedCard) {
					print (Ultimate_Solitaire.S.tempCard.name);
					valid = Ultimate_Solitaire.S.CheckValid (Ultimate_Solitaire.S.clickedCard, Ultimate_Solitaire.S.tempCard);
					print (valid);
					if (valid == true) {
						MoveCard (Ultimate_Solitaire.S.clickedCard, Ultimate_Solitaire.S.tempCard); 
					}
				}
			}
		}
	}

	// MoveToEmpty() moves Card king to TableauAnc anc
	void MoveToEmpty(Card king, TableauAnc anc){
		Ultimate_Solitaire.S.pos = anc.transform.position;
		Ultimate_Solitaire.S.pos.z -= 2;

		// If the King was in the discard pile, remove it from the list and set it's state to the tableau pile
		if (king.state == CardState.discard) {
			Ultimate_Solitaire.S.discardPile.Remove(king);
			king.state = CardState.tableau;
			Ultimate_Solitaire.S.tableaus[anc.pileID].Add(king);
			king.slotDef.TableauNum = anc.pileID;
			print ("new size " +  Ultimate_Solitaire.S.tableaus[anc.pileID].Count); 

		}
		if (king.state == CardState.tableau) {
			int temp = king.slotDef.TableauNum;	
			Ultimate_Solitaire.S.tableaus[temp].Remove(king);
			Ultimate_Solitaire.S.tableaus[anc.pileID].Add(king);
			king.slotDef.TableauNum = anc.pileID;
			print ("new size " +  Ultimate_Solitaire.S.tableaus[anc.pileID].Count);
		}

		// Set the King's sorting layer to be the bottom
		king.SetSortingLayerName ("Row0");
	}


	// MoveToFoundation moves Card clickedC to Foundation fon
	void MoveToFoundation(Card clickedC,Foundation fon){
		if (disableFon == false){
			if (clickedC.rank == fon.curRank + 1) {
				if (fon.suit == "none" || fon.suit == clickedC.suit) {
					Vector3 transfer = fon.transform.position;
					transfer.z -= 1;
					Ultimate_Solitaire.S.pos = transfer;
					//print (Ultimate_Solitaire.S.pos);
					fon.curRank = clickedC.rank;
					fon.suit = clickedC.suit;
					int tem = clickedC.slotDef.TableauNum;
					if (clickedC.state == CardState.tableau)
						Ultimate_Solitaire.S.tableaus [tem].Remove (clickedC);
					if (clickedC.state == CardState.discard)
						Ultimate_Solitaire.S.discardPile.Remove (clickedC); 
					fon.pile.Add (clickedC);
					print (fon.pile.Count);
					clickedC.SetSortOrder (100 * fon.pile.Count);
					fMove = true;
					disableFon = true;

				}
			}
		}
	}

	void MoveCard(Card clickedcard, Card otherCard){
		// If the card you're moving this card to is in the tableau
		if(otherCard.state== CardState.tableau){
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

				// Set the layer
				clickedcard.SetSortingLayerName("Row" + (tableaunumb));
			}
			// If you're moving the card from another Tableau
			else if (clickedcard.state == CardState.tableau && passThrough == false) {
				prevSortingLayer = clickedcard.GetSortingLayerName ();
				int tabl1 = clickedcard.slotDef.TableauNum;
				int tabl2 = otherCard.slotDef.TableauNum;
				//print (tabl1);
				//print(tabl2);

				if (tabl1 != tabl2){
					
					//print (Ultimate_Solitaire.S.tableaus [tabl2].Count);
					Ultimate_Solitaire.S.tableaus [tabl1].Remove (clickedcard);
					Ultimate_Solitaire.S.tableaus [tabl2].Add (clickedcard); 
							
					clickedcard.slotDef.TableauNum = otherCard.slotDef.TableauNum;
					clickedcard.SetSortOrder(Ultimate_Solitaire.S.tableaus[clickedcard.slotDef.TableauNum].Count);  

					// Set the layer
					clickedcard.SetSortingLayerName("Row" + (clickedcard.slotDef.TableauNum));
				}	
			}

		}
	}

	// OnMouseDown() is called when this card is clicked
	void OnMouseDown(){
		if (this.state == CardState.tableau) {
			int q = 0;
			int j = 0;
			Ultimate_Solitaire.S.clicked = true;
			Ultimate_Solitaire.S.clickedCard = this;
			int tabl = this.slotDef.TableauNum;
			Card[]x = Ultimate_Solitaire.S.tableaus[tabl].ToArray();
			for (int i = 0;i<x.Length;i++){
				if (x[i].name == this.name)
					q = i;
			}
			print (q);
			if (q != x.Length-1 && x[q].faceUp==true){
				print ("multi");
				Ultimate_Solitaire.S.multi = true;
			}
			if (Ultimate_Solitaire.S.multi == true){
				Ultimate_Solitaire.S.multiMov = new Card[x.Length-q];
				for (int i = q ; i<x.Length; i++){
					//print (x[i].name);
					Ultimate_Solitaire.S.multiMov[j] = x[i];
					j++;

				}

			}
			Ultimate_Solitaire.S.pos = this.transform.position;  

			// Change Layer to be above other cards. MovingCard is the highest sorting layer. Make sure to store 
			// Current Layer for later use
			prevSortingLayer = GetSortingLayerName();
			this.SetSortingLayerName("MovingCard");

		}
		if (this.state == CardState.discard) {
			// Grab the current layer for later use
			prevSortingLayer = GetSortingLayerName();
			this.SetSortingLayerName("MovingCard");
			Card[] tem = Ultimate_Solitaire.S.discardPile.ToArray();
			Ultimate_Solitaire.S.tp = tem[tem.Length-1];
			Ultimate_Solitaire.S.clicked = true;
			Ultimate_Solitaire.S.clickedCard = Ultimate_Solitaire.S.tp;
			if (Ultimate_Solitaire.S.pos == Vector3.zero)
			Ultimate_Solitaire.S.pos = Ultimate_Solitaire.S.tp.transform.position;
		}
		 if (this.state == CardState.drawpile&& this.faceUp == false) {
			// Here I think we should check if the card has children and if so, move the entire pile
			prevSortingLayer = GetSortingLayerName();
			this.SetSortingLayerName("MovingCard");


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

		}
		// If this is the top face-down card in a tableau
		if (this.state == CardState.tableau && this.faceUp == false) {
			int tem = this.slotDef.TableauNum;
			Card[] x = Ultimate_Solitaire.S.tableaus[tem].ToArray();
			for (int i = 0;i <x.Length;i++){
				if (x[i].name == this.name&&i == x.Length-1){
					this.faceUp=true;

				}
			}
		}
	}
	void OnMouseUp(){
		this.SetSortingLayerName (prevSortingLayer);
		if (drawn == false) {

			if (valid == false && fMove == false)
				// Return to original sorting layer
				this.SetSortingLayerName(prevSortingLayer);
			Ultimate_Solitaire.S.clicked = false; // Set the clicked boolean to false in Ultimate_Solitaire.cs

		
			if (this.state == CardState.tableau)
				this.transform.position = Ultimate_Solitaire.S.pos;
			if (this.state == CardState.discard){
				Ultimate_Solitaire.S.tp.transform.position = Ultimate_Solitaire.S.pos;
			}
			if (fMove == true)
				Ultimate_Solitaire.S.clickedCard.state = CardState.foundation;
			Ultimate_Solitaire.S.clickedCard = null;
			Ultimate_Solitaire.S.tempCard = null;
			Ultimate_Solitaire.S.pos = Vector3.zero;

		}
		drawn = false;
		valid = false;
		fMove = false;
		disableFon = false;
		Ultimate_Solitaire.S.multi = false;
		Ultimate_Solitaire.S.multiMov = null;

	}

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
		print (otherNum);
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

