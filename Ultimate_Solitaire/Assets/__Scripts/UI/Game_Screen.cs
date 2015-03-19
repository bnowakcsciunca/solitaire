﻿using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class Game_Screen : MonoBehaviour {

	public GameObject			scoreT;
	public GameObject			loseT;

	void Awake() {
		scoreT = GameObject.Find ("scoreText");
		scoreT.GetComponent<Text>().text = "Score: 0";

		loseT = GameObject.Find ("loseButton");
		loseT.SetActive (false);
	}

	public void UpdateScore() {
		scoreT.GetComponent<Text>().text = "Score: " + Ultimate_Solitaire.S.score;
	}

	public void ShuffleButton() {
		Application.LoadLevel ("3_Game");
	}

	public void MenuButton() {
		Application.LoadLevel ("2_Start");
	}

	public void ExitButton() {
		Application.Quit ();
	}

	public void MakeLoseButtonVisible() {
		loseT.SetActive (true);
	}
}
