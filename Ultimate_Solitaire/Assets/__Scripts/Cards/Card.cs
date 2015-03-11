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

	public bool faceUp{
		get {
			return (!back.activeSelf);
		}
		set{
			back.SetActive(!value);
		}
	}

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
		if (king.state == CardState.discard) {
			Ultimate_Solitaire.S.discardPile.Remove(king);
			king.state= CardState.tableau;
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
				}
		}
	}

	void MoveCard(Card clickedcard, Card otherCard){
		if(otherCard.state== CardState.tableau){

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
				passThrough = true;
//						Card[]tem = Ultimate_Solitaire.S.discardPile.ToArray();
				//tem[tem.Length-1].SetSortOrder(100 * Ultimate_Solitaire.S.discardPile.Count);
				}
				else if (clickedcard.state == CardState.tableau && passThrough == false) {
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
						}	
						//print (Ultimate_Solitaire.S.tableaus [tabl2].Count); 
				}

		}


	}

	void OnMouseDown(){
		if (this.state == CardState.tableau) {
			Ultimate_Solitaire.S.clicked = true;
			Ultimate_Solitaire.S.clickedCard = this;
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

		if (drawn == false) {
			// Return to original sorting layer
			if (valid == false)
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
			Ultimate_Solitaire.S.tempCard = null;
			Ultimate_Solitaire.S.pos = Vector3.zero;

		}
		drawn = false;
		valid = false;
		fMove = false;
		disableFon = false;

	}

	// Grab the current sorting layer name
	public string GetSortingLayerName() {
		foreach (SpriteRenderer tSR in spriteRenderers) {
			return (tSR.sortingLayerName);		
		}
		return null;
	}

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

