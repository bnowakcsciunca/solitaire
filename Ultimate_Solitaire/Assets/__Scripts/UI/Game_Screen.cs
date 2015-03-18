using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class Game_Screen : MonoBehaviour {

	public GameObject			scoreT;

	void Awake() {
		scoreT = GameObject.Find ("scoreText");
		scoreT.GetComponent<Text>().text = "Score: 0";
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
}
