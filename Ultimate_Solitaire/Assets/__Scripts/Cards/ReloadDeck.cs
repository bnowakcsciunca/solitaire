//-----------------------------------------------------------------------------------
// this class creates a way to 'restock' the deck after it has been filed through
//-----------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReloadDeck : MonoBehaviour {
	public GameObject Box; // the actual gameobject
	Material x;  // the material attached to Box
	bool Tenabled = false; // flag for telling if this is enabled
	int count = 0; // counter for number of times the deck has been passed through
	bool isRed = false;

	public Game_Screen gScript;

	// Use this for initialization
	void Start () {
		x = Box.renderer.material;
		x = Box.renderer.material;
		x.color = Color.white;
		}
	// Update is called once per frame
	void Update () {


		if (count == 3) {
			x.color = Color.red;
			Tenabled = false;
			isRed = true;
		}

		else if (Ultimate_Solitaire.S.drawPile.Count == 0 && count < 3) {
			x.color = Color.green;	
			Tenabled = true;
		}
		else {
			Tenabled = false;
			x.color = Color.white; 
		}

	
	}
	public void OnMouseDown(){
		if (Tenabled == true) {
			print ("deck will be restocked now");
			Card[] tem = Ultimate_Solitaire.S.discardPile.ToArray();
			int con = tem.Length;
			for (int i = 0; i < con ; i++){
				Ultimate_Solitaire.S.discardPile.Remove(tem[i]);
				Ultimate_Solitaire.S.drawPile.Add(tem[i]);
				tem[i].state = CardState.drawpile;
			}
			Ultimate_Solitaire.S.DrawUpdate();  
			count++;

			// Decrease the score by 50% for going through the pile and update the score
			Ultimate_Solitaire.S.score = Ultimate_Solitaire.S.score / 2;
			Ultimate_Solitaire.S.UpdateScore ();
		} else if (isRed == true){
			gScript.MakeLoseButtonVisible();
		}
		Tenabled = false;
	}

}

