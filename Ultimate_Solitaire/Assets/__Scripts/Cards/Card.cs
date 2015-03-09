// code from textbook prospector chapter recoded for this project
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CardState{
	drawpile,
	tableau,
	target,
	discard
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
		Ultimate_Solitaire.S.tempCard = col.GetComponent<Card> ();
		if (Ultimate_Solitaire.S.tempCard != Ultimate_Solitaire.S.clickedCard && Ultimate_Solitaire.S.tempCard.faceUp == true && this.faceUp == true && this == Ultimate_Solitaire.S.clickedCard) {
			print (Ultimate_Solitaire.S.tempCard.name);
			 valid = Ultimate_Solitaire.S.CheckValid(Ultimate_Solitaire.S.clickedCard,Ultimate_Solitaire.S.tempCard);
				print (valid);
			if (valid == true){
				MoveCard(Ultimate_Solitaire.S.clickedCard,Ultimate_Solitaire.S.tempCard); 
			}
				
		}


	//	print ("DERP");
	}

	void MoveCard(Card clickedcard, Card otherCard){
		if(otherCard.state== CardState.tableau){

				Ultimate_Solitaire.S.pos = otherCard.transform.position;
				Ultimate_Solitaire.S.pos.z -= 1;
				Ultimate_Solitaire.S.pos.y -= .5f;
				print (Ultimate_Solitaire.S.pos);
				if (clickedcard.state == CardState.discard) {
						Ultimate_Solitaire.S.discardPile.Remove (clickedcard);	
						clickedcard.state = CardState.tableau;
						int tableaunumb = otherCard.slotDef.TableauNum;
						Ultimate_Solitaire.S.tableaus [tableaunumb].Add (clickedcard);
				}
				if (clickedcard.state == CardState.tableau) {
						int tabl1 = clickedcard.slotDef.TableauNum;
						int tabl2 = otherCard.slotDef.TableauNum;
						print (Ultimate_Solitaire.S.tableaus [tabl2].Count);
						Ultimate_Solitaire.S.tableaus [tabl1].Remove (clickedcard);
						Ultimate_Solitaire.S.tableaus [tabl2].Add (clickedcard);
						print (Ultimate_Solitaire.S.tableaus [tabl2].Count);
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
			this.SetSortingLayerName("MovingCard");
			}
		if (this.state == CardState.discard) {
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
			Ultimate_Solitaire.S.clickedCard = null;
			Ultimate_Solitaire.S.tempCard = null;
			Ultimate_Solitaire.S.pos = Vector3.zero;
		}
		drawn = false;
		valid = false;

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

