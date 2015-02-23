using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class End_Screen : MonoBehaviour {

	public Text title;
	public Text Score;
	public Text HScore;
	public Text congrats;
	public bool _____________________________;

	int score;
	int highscore;
	GameObject winScreen;
	GameObject loseScreen;

	// Use this for initialization
	void Awake () {
		highscore = PlayerPrefs.GetInt ("UltimateHighScore");

		// Testing here
		//WonGame ();
		LostGame();
	}

	public void WonGame() {
		title.text = "Congratulations, You Won!";
		Score.text = score.ToString ();
		HScore.text = highscore.ToString ();

		if (score > highscore) {
			congrats.text = "Congratulations, you beat the high score!";
			highscore = score;
			PlayerPrefs.SetInt ("UltimateHighScore", highscore);
		}

	}
	
	public void LostGame() {
		title.text = "You Lost. Try again!";
		Score.text = score.ToString ();
		HScore.text = highscore.ToString ();

		if (score > highscore) {
			congrats.text = "Congratulations, you beat the high score!";
			highscore = score;
			PlayerPrefs.SetInt ("UltimateHighScore", highscore);
		}
	}

	public void PlayAgain() {
		Application.LoadLevel ("3_Game");
	}

	public void MainMenu() {
		Application.LoadLevel ("2_Start");
	}

	public void EndGame() {
		Application.Quit ();
	}
}
