using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class End_Screen : MonoBehaviour {

	public Text 			title;
	public Text 			Score;
	public Text 			HScore;
	public Text 			congrats;
	public bool 			_____________________________;

	public GameObject		persistentStuff;

	int 					score;
	int 					highscore;
	bool 					didIWin;
	GameObject 				winScreen;
	GameObject 				loseScreen;

	// Use this for initialization
	void Awake () {
		highscore = PlayerPrefs.GetInt ("UltimateHighScore");

		// Grab the persistent information
		persistentStuff = GameObject.Find ("Persistence_Object");
		PersistentInfo pScript = persistentStuff.GetComponent<PersistentInfo>();
		score = pScript.score;

		didIWin = pScript.wonGame;
		if (didIWin == false) {
			LostGame();
		} else {
			WonGame();
		}
	}

	public void WonGame() {
		title.text = "Congratulations, You Won!";
		Score.text = score.ToString ();
		HScore.text = highscore.ToString ();

		if (score > highscore) {
			congrats.text = "Congratulations, you beat the high score!";
			highscore = score;
			HScore.text = highscore.ToString ();
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
			HScore.text = highscore.ToString ();
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
