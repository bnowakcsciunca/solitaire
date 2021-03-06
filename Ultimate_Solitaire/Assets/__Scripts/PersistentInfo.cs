﻿using UnityEngine;
using System.Collections;

// This class defines an object that persists throughout scene changes. In this context it is used
// to retain the score from the game scene to the end game scene.
public class PersistentInfo : MonoBehaviour {
	public bool			wonGame;
	public int			score;


	// It's vital that this object persists through scene changes. Make it not destroy on new level load
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		wonGame = false; // wonGame is false unless game is won
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Nab the score
		score = Ultimate_Solitaire.S.score;
		wonGame = Ultimate_Solitaire.S.winning;
	}
}
