using UnityEngine;
using System.Collections;

public class Start_Screen : MonoBehaviour {

	public void newClicked() {
		Application.LoadLevel ("3_Game");
	}

	public void helpClicked() {
		// Switch the UI to the help UI
	}

	public void exitClicked() {
		Application.Quit();
	}
}
