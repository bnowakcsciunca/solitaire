using UnityEngine;
using System.Collections;

public class Splash : MonoBehaviour {

	public float		splashTime = 2.0f; // How long we want the splash screen to last
	public bool			_________________;

	void Update () {
		splashTime -= Time.deltaTime;
		if (splashTime <= 0) {
			Application.LoadLevel ("2_Start");
		}
	}
}
