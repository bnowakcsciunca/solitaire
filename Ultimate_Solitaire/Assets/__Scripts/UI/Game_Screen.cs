using UnityEngine;
using System.Collections;

public class Game_Screen : MonoBehaviour {

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
