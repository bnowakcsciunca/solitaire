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
		Ultimate_Solitaire.S.hover = true;
		//print (this.name);
		
	}

	void OnMouseExit(){
		Ultimate_Solitaire.S.hover = false;
	}

	void OnMouseDown(){
		Ultimate_Solitaire.S.clicked = true;
		Ultimate_Solitaire.S.clickedCard = this;
		Ultimate_Solitaire.S.pos = this.transform.position;

		}
	void OnMouseUp(){
		Ultimate_Solitaire.S.clicked = false;
		this.transform.position = Ultimate_Solitaire.S.pos;
		Ultimate_Solitaire.S.clickedCard = null;

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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

