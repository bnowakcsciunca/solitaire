using UnityEngine;
using System.Collections;

public class Start_Screen : MonoBehaviour {

	public GameObject		mainMenu;	// Reference for the Main_Menu canvas
	public GameObject		hMenu;		// Reference for the Help_Menu canvas

	// When the scene loads, set the help menu to false
	void Awake() {
		mainMenu = GameObject.Find ("Main_Menu");

		hMenu = GameObject.Find ("Help_Menu");
		hMenu.SetActive (false);
	}

	public void NewClicked() {
		Application.LoadLevel ("3_Game");
	}

	public void HelpClicked() {
		mainMenu.SetActive (false);
		hMenu.SetActive (true);
	}

	public void ExitClicked() {
		Application.Quit();
	}

	public void BackClicked() {
		mainMenu.SetActive(true);
		hMenu.SetActive (false);
	}
}
