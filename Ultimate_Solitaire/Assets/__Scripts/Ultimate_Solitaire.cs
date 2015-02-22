using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ultimate_Solitaire : MonoBehaviour {
	static public Ultimate_Solitaire S;
	public Deck deck;
	public TextAsset deckXML;



	
	public Layout layout;
	public TextAsset layoutXML;
	public List<Card> drawPile;
	public Vector3 layoutCenter;
	public float xOffset = 3;
	public float yOffset = 2.5f;
	public Transform layoutAnchor;
	

	public List<Card> tableau;
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
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
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

	void LayoutGame(){
		if (layoutAnchor == null) {
			GameObject tGO = new GameObject("_LayoutAnchor");
			layoutAnchor = tGO.transform;
			layoutAnchor.transform.position=layoutCenter;
		}
		Card cp;
		foreach (SlotDef tSD in layout.slotDefs) {
			cp = Draw ();
			cp.faceUp = tSD.faceUp;
			cp.transform.parent = layoutAnchor;
			cp.transform.localPosition = new Vector3(layout.multiplier.x * tSD.x,layout.multiplier.y*tSD.y,-tSD.layerID);
			cp.layoutID = tSD.id;
			cp.slotDef = tSD;
			cp.state=CardState.tableau;
			cp.SetSortingLayerName(tSD.layerName);
			
			tableau.Add (cp);
			
			
		}



		
		UpdateDrawPile ();
	}


}
