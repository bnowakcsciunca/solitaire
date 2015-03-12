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

public class Card : MonoBehaviour {
<<<<<<< HEAD
	// CARD INFORMATION
	public string 			suit; 								// ( C D H or S)
	public int 				rank; 								// Can be a value from 1 - 14
	public Color 			color = Color.black;
	public string 			colS = "Black";
	public List<GameObject> decoGOs = new List<GameObject>(); 	// Decorators list
	public List<GameObject> pipGOs = new List<GameObject>();	// Pips list
	public SpriteRenderer[] spriteRenderers;
	
	
	public GameObject 		back;  								// the back of the card
	public CardState 		state = CardState.drawpile;
	public List<Card> 		hiddenBy = new List<Card>();
	public int 				layoutID;
	public SlotDef 			slotDef;
	bool 					drawn = false;
	bool 					valid = false;
	bool 					fMove = false;
	bool 					disableFon = false;					// this prevents setting two foundations to the same suit
	
	public string 			prevSortingLayer;					// Stores the previous sorting layer so that we can return the card to it if necessary
	public string			newSortingLayer;
	
	public CardState		prevState = CardState.empty;		// Stores the previous state of the card when moved, so that it can be returned should the trigger no longer be called
	public int				prevTableau;						// If the previous state was in the tableau, this stores the tableau that used to store this
	public bool				validCol = false;
	public bool				isColliding = false;				// Check to see whether or not a valid move is already being tried
	
	public 					CardDefinition def; 				// parsed from DeckXML.xml
	
=======
	public string suit; // ( C D H or S)
	public int rank; // value 1 - 14
	public Color color = Color.black;
	public string colS = "Black";
	// this list holds all the decorators
	public List<GameObject> decoGOs = new List<GameObject>();
	// this list holds all the pips
	public List<GameObject> pipGOs = new List<GameObject>();
	public SpriteRenderer[] spriteRenderers;
	public GameObject back;  // the back of the card
	public CardState state = CardState.drawpile;
	public List<Card> hiddenBy = new List<Card>();
	public int layoutID;
	public SlotDef slotDef;
	bool drawn = false;
	bool valid = false;
	bool fMove = false;
	bool disableFon = false;// this prevents setting two foundations to the same suit

	public string prevSortingLayer;		// Stores the previous sorting layer so that we can return the card to it if necessary


	public CardDefinition def; // parsed from DeckXML.xml

>>>>>>> origin/master
	public bool faceUp{
		get {
			return (!back.activeSelf);
		}
		set{
			back.SetActive(!value);
		}
	}
<<<<<<< HEAD
	
	// OnMouseEnter() lets the Ultimate_Solitaire code know that a card is being hovered over
=======

>>>>>>> origin/master
	void OnMouseEnter(){
		/*if (this.state == CardState.tableau) {
			print ("tableau");	
		}
		if (this.state == CardState.drawpile) {
			print ("DECK");	
		}*/
		Ultimate_Solitaire.S.hover = true;
	}
<<<<<<< HEAD
	
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
		// You can move a King to an empty spot on the tableau
			if (col.tag == "Empty" && this.rank == 13) { 
				if (col.GetComponent<TableauAnc> ().pactive == true) { 
					prevState = this.state;
					// print ("empty");
					validCol = true;
					isColliding = true;
					MoveToEmpty (Ultimate_Solitaire.S.clickedCard, col.GetComponent<TableauAnc> ()); 
				}	
			}
			// You can also move cards to foundation in numerical order sorted by suit
			else if (col.name == "Foundation") {
				prevState = this.state;
				print ("Previous state is: " + prevState);
				validCol = true;
				isColliding = true;
				MoveToFoundation (Ultimate_Solitaire.S.clickedCard, col.GetComponent<Foundation> ());
				// Finally, you can also move cards to tableaus
			} else {
				if (col.tag != "Empty") {
					prevState = this.state;
					Ultimate_Solitaire.S.tempCard = col.GetComponent<Card> ();
					if (Ultimate_Solitaire.S.tempCard != Ultimate_Solitaire.S.clickedCard && Ultimate_Solitaire.S.tempCard.faceUp == true && this.faceUp == true && this == Ultimate_Solitaire.S.clickedCard) {
						valid = Ultimate_Solitaire.S.CheckValid (Ultimate_Solitaire.S.clickedCard, Ultimate_Solitaire.S.tempCard);
						if (valid == true) {
							validCol = true;
							isColliding = true;
							print ("Trying MoveCard()");
							MoveCard (Ultimate_Solitaire.S.clickedCard, Ultimate_Solitaire.S.tempCard); 
						}
					}
				}
			}
		}
	}
	
	// Move the card back to its original spot once it is no longer above the triggering location
	// Each function will have to reset everything that was previously changed
	void OnTriggerExit(Collider col) {
		// print ("No longer colliding with " + col);
		// WWprint ("Collider is " + col);
		
		// Only do this is validCol is true, and this has been over something that we want it to move to
		if (validCol == true) {
			if (col.tag == "Empty" && this.rank == 13) { 
				if (col.GetComponent<TableauAnc> ().pactive == true) {
					isColliding = false;
					ReturnFromEmpty (Ultimate_Solitaire.S.clickedCard, col.GetComponent<TableauAnc> ()); 
				}	
			} else if (col.tag != "Empty") {
				Ultimate_Solitaire.S.tempCard = col.GetComponent<Card> ();
				if (Ultimate_Solitaire.S.tempCard != Ultimate_Solitaire.S.clickedCard && Ultimate_Solitaire.S.tempCard.faceUp == true && this.faceUp == true && this == Ultimate_Solitaire.S.clickedCard) {
					valid = Ultimate_Solitaire.S.CheckValid (Ultimate_Solitaire.S.clickedCard, Ultimate_Solitaire.S.tempCard);
					if (valid == true) {
						isColliding = false;
						ReturnFromMove (Ultimate_Solitaire.S.clickedCard, Ultimate_Solitaire.S.tempCard); 
					}
				}
			} else if (col.tag == "Foundation") {
				isColliding = false;
				ReturnFromFoundation (Ultimate_Solitaire.S.clickedCard, col.GetComponent<Foundation> ());
			}
			validCol = false;
		}
	}
	
	void ReturnFromEmpty(Card king, TableauAnc anc) {
		
	}
	
	void ReturnFromFoundation(Card clickedC, Foundation fon) {
		// print ("Returning the card from the foundation");
		disableFon = false;
		fMove = false;
		fon.pile.Remove (clickedC);
		
		fon.curRank = 0;
		fon.suit = "none";
		clickedC.state = prevState;
		
		if (clickedC.state == CardState.tableau)
			Ultimate_Solitaire.S.tableaus [prevTableau].Add (clickedC);
		if (clickedC.state == CardState.discard)
			Ultimate_Solitaire.S.discardPile.Add (clickedC); 
	}
	
	void ReturnFromMove(Card clickedCard, Card otherCard) {
		
	}
	
	// MoveToEmpty() moves Card king to TableauAnc anc
	void MoveToEmpty(Card king, TableauAnc anc){
		Ultimate_Solitaire.S.pos = anc.transform.position;
		Ultimate_Solitaire.S.pos.z -= 2;
		
		// If the King was in the discard pile, remove it from the list and set it's state to the tableau pile
=======

	void OnMouseExit(){
		Ultimate_Solitaire.S.hover = false;
	}

	void OnTriggerEnter(Collider col){
		//print (col.name);
		if (col.tag == "Empty" && this.rank == 13) { 
			if(col.GetComponent<TableauAnc>().pactive== true){ 
			print ("empty");
				MoveToEmpty(Ultimate_Solitaire.S.clickedCard,col.GetComponent<TableauAnc>()); 
			}	
		}


		else if (col.name == "Foundation") {
			//Ultimate_Solitaire.S.clickedCard = this;
						//print ("fff");	
			MoveToFoundation(Ultimate_Solitaire.S.clickedCard,col.GetComponent<Foundation>());
				} else {
						if (col.tag != "Empty"){

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
	//	print ("DERP");
	}

	void MoveToEmpty(Card king,TableauAnc anc){
		Ultimate_Solitaire.S.pos = anc.transform.position;
		Ultimate_Solitaire.S.pos.z -= 2;
>>>>>>> origin/master
		if (king.state == CardState.discard) {
			Ultimate_Solitaire.S.discardPile.Remove(king);
			king.state= CardState.tableau;
			Ultimate_Solitaire.S.tableaus[anc.pileID].Add(king);
			king.slotDef.TableauNum = anc.pileID;
			// print ("new size " +  Ultimate_Solitaire.S.tableaus[anc.pileID].Count); 
			
		}

		Card[] x = Ultimate_Solitaire.S.tableaus [king.slotDef.TableauNum].ToArray ();
		int q = 0;
		for (int i = 0; i<x.Length; i++) {
			if (x[i].name == king.name){
				q = i;
			}	
		}
		if (king.state == CardState.tableau && q == x.Length-1) {
			int temp = king.slotDef.TableauNum;	
			Ultimate_Solitaire.S.tableaus[temp].Remove(king);
			Ultimate_Solitaire.S.tableaus[anc.pileID].Add(king);
			king.slotDef.TableauNum = anc.pileID;
			// print ("new size " +  Ultimate_Solitaire.S.tableaus[anc.pileID].Count);
		}
<<<<<<< HEAD
		
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
=======

		if (king.state == CardState.tableau && q != x.Length - 1) {
			int temp = king.slotDef.TableauNum;	
			Ultimate_Solitaire.S.tableaus[temp].Remove(king);
			Ultimate_Solitaire.S.tableaus[anc.pileID].Add(king);
			king.slotDef.TableauNum = anc.pileID;
			Card[] tempr = Ultimate_Solitaire.S.multiMov;
			Vector3 kpos = anc.transform.position;
			kpos.z -=2;
			for (int i = 1; i<tempr.Length;i++){
				Ultimate_Solitaire.S.tableaus[tempr[i].slotDef.TableauNum].Remove(tempr[i]);
				Ultimate_Solitaire.S.tableaus[anc.pileID].Add(tempr[i]);
				tempr[i].slotDef.TableauNum= anc.pileID;
				kpos.z -= 1;
				kpos.y -= .5f;
				tempr[i].transform.position = kpos;


			}


		}

	
	}



	void MoveToFoundation(Card clickedC,Foundation fon){

				if (disableFon == false){
				if (clickedC.rank == fon.curRank + 1) {
						//print ("YYYY");

			
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
>>>>>>> origin/master
				}
		}
	}
<<<<<<< HEAD
	
	void MoveCard(Card clickedcard, Card otherCard) {
		// If the card you're moving this card to is in the tableau
		prevSortingLayer = clickedcard.GetSortingLayerName ();
		// newSortingLayer = prevSortingLayer;
		if(otherCard.state== CardState.tableau){
			Ultimate_Solitaire.S.pos = otherCard.transform.position;
			Ultimate_Solitaire.S.pos.z -= 1;
			Ultimate_Solitaire.S.pos.y -= .5f;
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
					
					// SET THE LAYER. The row number is equal to the card's new place in the tableau
					valid = true;
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
			// If you're moving a stack from the tableau
			if (clickedcard.state == CardState.tableau && Ultimate_Solitaire.S.multi == true) {
				Ultimate_Solitaire.S.pos = otherCard.transform.position;
				Ultimate_Solitaire.S.pos.z -= 1;
				Ultimate_Solitaire.S.pos.y -= .5f;
				int tabl1 = clickedcard.slotDef.TableauNum;
				int tabl2 = otherCard.slotDef.TableauNum;
				if (tabl1 != tabl2){
					
					// print (Ultimate_Solitaire.S.tableaus [tabl2].Count);
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
	}
	
	// OnMouseDown() is called when this card is clicked
=======

	void MoveCard(Card clickedcard, Card otherCard){
		if(otherCard.state== CardState.tableau&&Ultimate_Solitaire.S.multi == false){

				Ultimate_Solitaire.S.pos = otherCard.transform.position;
				Ultimate_Solitaire.S.pos.z -= 1;
				Ultimate_Solitaire.S.pos.y -= .5f;
			bool passThrough = false;

				//print (Ultimate_Solitaire.S.pos); 
				if (clickedcard.state == CardState.discard) {
						Ultimate_Solitaire.S.discardPile.Remove (clickedcard);	
						clickedcard.state = CardState.tableau;
						int tableaunumb = otherCard.slotDef.TableauNum;
						Ultimate_Solitaire.S.tableaus [tableaunumb].Add (clickedcard);
				clickedcard.slotDef.TableauNum = tableaunumb;
				passThrough = true;
//						Card[]tem = Ultimate_Solitaire.S.discardPile.ToArray();
				//tem[tem.Length-1].SetSortOrder(100 * Ultimate_Solitaire.S.discardPile.Count);
				}
			else if (clickedcard.state == CardState.tableau && passThrough == false && Ultimate_Solitaire.S.multi == false) {
						int tabl1 = clickedcard.slotDef.TableauNum;
						int tabl2 = otherCard.slotDef.TableauNum;
				//print (tabl1); &&
				//print(tabl2);

						if (tabl1 != tabl2){
					
							//print (Ultimate_Solitaire.S.tableaus [tabl2].Count);
							Ultimate_Solitaire.S.tableaus [tabl1].Remove (clickedcard);
							Ultimate_Solitaire.S.tableaus [tabl2].Add (clickedcard); 
							
					clickedcard.slotDef.TableauNum = otherCard.slotDef.TableauNum;
					clickedcard.SetSortOrder(Ultimate_Solitaire.S.tableaus[clickedcard.slotDef.TableauNum].Count);   
						}	
						//print (Ultimate_Solitaire.S.tableaus [tabl2].Count); 
				}


		}
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

>>>>>>> origin/master
	void OnMouseDown(){
		prevSortingLayer = GetSortingLayerName ();
		this.SetSortingLayerName("MovingCard");
		
		print ("The current sorting layer is: " + prevSortingLayer);
		if (this.state == CardState.tableau) {
			// Change Layer to MovingCard, which is the highest sorting 
			this.SetSortingLayerName("MovingCard");
			
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
			// print ("The position in the tableau is " + q);
			if (q != x.Length-1 && x[q].faceUp==true){
				// print ("multi");
				Ultimate_Solitaire.S.multi = true;
			}
			if (Ultimate_Solitaire.S.multi == true){
				Ultimate_Solitaire.S.multiMov = new Card[x.Length-q];
				for (int i = q ; i<x.Length; i++){
					// print (x[i].name);
					Ultimate_Solitaire.S.multiMov[j] = x[i];
					j++;
					
				}
				
			}
<<<<<<< HEAD
			Ultimate_Solitaire.S.pos = this.transform.position; 
		}
		// If the discard card is clicked
		if (this.state == CardState.discard) {
=======
			Ultimate_Solitaire.S.pos = this.transform.position;  

			// Change Layer to be above other cards
			// prevSortingLayer = this.Set
			prevSortingLayer = GetSortingLayerName();
			//print ("derrr");
			this.SetSortingLayerName("MovingCard");

			}
		if (this.state == CardState.discard) {
			prevSortingLayer = GetSortingLayerName();
			//print ("derrr");
>>>>>>> origin/master
			this.SetSortingLayerName("MovingCard");
			
			Card[] tem = Ultimate_Solitaire.S.discardPile.ToArray();
			Ultimate_Solitaire.S.tp = tem[tem.Length-1];
			Ultimate_Solitaire.S.clicked = true;
			Ultimate_Solitaire.S.clickedCard = Ultimate_Solitaire.S.tp;
			if (Ultimate_Solitaire.S.pos == Vector3.zero)
				Ultimate_Solitaire.S.pos = Ultimate_Solitaire.S.tp.transform.position;
		}
<<<<<<< HEAD
		// If the DrawPile is clicked on
		if (this.state == CardState.drawpile&& this.faceUp == false) {
=======
		 if (this.state == CardState.drawpile&& this.faceUp == false) {
			// Here I think we should check if the card has children and if so, move the entire pile



>>>>>>> origin/master
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
<<<<<<< HEAD
		// If the card clicked is NOT the DrawPile
		if (drawn == false) {
			// If it's not a valid move, then there is no newSortingLayer. Change newSortingLayer to be prevSortingLayer
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
=======

		if (drawn == false) {
			// Return to original sorting layer
			if (valid == false&&fMove == false)
			this.SetSortingLayerName(prevSortingLayer);

			Ultimate_Solitaire.S.clicked = false;
			if (this.state == CardState.tableau)
			this.transform.position = Ultimate_Solitaire.S.pos;
			if (this.state == CardState.discard){
				Ultimate_Solitaire.S.tp.transform.position = Ultimate_Solitaire.S.pos;

			}
			if (fMove == true)
				Ultimate_Solitaire.S.clickedCard.state= CardState.foundation;
			Ultimate_Solitaire.S.clickedCard = null;
>>>>>>> origin/master
			Ultimate_Solitaire.S.tempCard = null;
			Ultimate_Solitaire.S.pos = Vector3.zero;	// Reset the move vector for the game
			
		}
		
		// Reset all logic variables to false / null
		ResetMoveLogic ();

		
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
	}
	
	// Grab the current sorting layer name
	public string GetSortingLayerName() {
		foreach (SpriteRenderer tSR in spriteRenderers) {
			return (tSR.sortingLayerName);
		}
		return null;
		
	}
<<<<<<< HEAD
	
	// Set the sorting layer name to string tSLN
=======

>>>>>>> origin/master
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
	
<<<<<<< HEAD
	// SetTableauSortingOrder sets the layer of this card to one higher than the layer of the card it is on top of
	public void SetTableauSortingOrder(Card otherCard) {
		string temp = otherCard.GetSortingLayerName ();
		int otherNum = int.Parse (temp);
		print (otherNum);
		SetSortingLayerName ("Row" + (otherNum + 1));
	}
=======
	

>>>>>>> origin/master
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

