// code from textbook prospector chapter recoded for this project
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour {
	public string suit; // ( C D H or S)
	public int rank; // value 1 - 14
	public Color color = Color.black;
	public string colS = "Black";
	// this list holds all the decorators
	public List<GameObject> decoGOs = new List<GameObject>();
	// this list holds all the pips
	public List<GameObject> pipGOs = new List<GameObject>();

	public GameObject back;  // the back of the card

	public CardDefinition def; // parsed from DeckXML.xml

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
