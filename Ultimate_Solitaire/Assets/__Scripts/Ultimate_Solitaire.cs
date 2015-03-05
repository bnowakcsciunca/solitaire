using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ultimate_Solitaire : MonoBehaviour {
	static public Ultimate_Solitaire S;
	public Deck deck;
	public TextAsset deckXML;
	Vector3 mousePos2D;
	Vector3 mousePos3D;
	public bool clicked = false; // true when card is clicked
	public bool hover = false; // true when mouse is over card
	public Card  clickedCard;// the card being moved
	public Vector3 pos;// clicked card's original position 
	public Card tp; // temp card for moving discard cards
	public Card tempCard; // a slot for the other card in a movement action


	
	public Layout layout;
	public TextAsset layoutXML;
	public List<Card> drawPile;
	public Vector3 layoutCenter;
	public float xOffset = 3;
	public float yOffset = 2.5f;
	public Transform layoutAnchor;
	

	public List<Card>[] tableaus = new List<Card>[7];// the linked list of tableaus
	public List<Card> discardPile;
	void Awake(){
		
		S = this;
	}
	
	// Use this for initialization
	void Start () {
		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);
		Deck.Shuffle (ref deck.cards); // shuffles deck
		layout = GetComponent<Layout> ();
		layout.ReadLayout (layoutXML.text);
		drawPile = (deck.cards);
		LayoutGame ();
		//print(tableaus[1].Count);
		
	}
	public Card DrawCall(){// a method to call draw to avoid issues with calling methods from non static context (workaround getter method)
		return Draw();}
	
	// Update is called once per frame
	void Update () {
		mousePos2D = Input.mousePosition; // get pointer position
		mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D); //Screen to world space
		mousePos3D.z++;
		// next lines basicly move the card if the conditions provided are true
		if (clicked = true&& clickedCard != null&& (clickedCard.state == CardState.tableau || clickedCard.state == CardState.discard) && clickedCard.faceUp == true) {
			clickedCard.transform.position = mousePos3D;
			//print (mousePos3D);

				
		}
		
	}
	//everything below here was copied from prospetor ((with slight modifcations to layoutGame()------------------------------
	void UpdateDrawPile(){
		Card cd;
		for (int i = 0; i<drawPile.Count; i++) {
			cd = drawPile[i];
			cd.transform.parent= layoutAnchor;
			Vector2 dpStagger = layout.drawPile.stagger;
			cd.transform.localPosition = new Vector3(layout.multiplier.x * (layout.drawPile.x + i*dpStagger.x),
			                                         layout.multiplier.y * (layout.drawPile.y + i*dpStagger.y),
			                                         -layout.drawPile.layerID+0.1f*i);
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
		Card cp;// this is an addition 
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
			int temp = cp.slotDef.TableauNum;// get tableau number from slotdef
			tableaus[temp].Add (cp);// add the card to it's proper tableau
			
			
		}



		
		UpdateDrawPile ();
	}


}
