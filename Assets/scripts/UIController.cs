using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public GameObject fuelRemainingDisplay;
	public Text scoreLabel;
	public Text gameOverLabel;
	public Text levelLabel;
	public Text flashLabel;

	public void SetFuelRemainingPercentage (float fuelRemaining) {
		var localScale = fuelRemainingDisplay.transform.localScale;
		localScale.x = fuelRemaining;
		fuelRemainingDisplay.transform.localScale = localScale;
	}

	public void SetScore (int score) {
		scoreLabel.text = score.ToString ();
	}

	public void ShowGameOver() {
		gameOverLabel.gameObject.SetActive (true);
	}

	public void SetLevel(int levelNumber, string name) {
		var level = "Level " + levelNumber;
		levelLabel.text = level;
		FlashMessage (name);

	}

	public void FlashMessage(string message) {
		StartCoroutine (_FlashMessage (message));
	}

	private IEnumerator _FlashMessage(string message) {

		flashLabel.text = message;
		flashLabel.gameObject.SetActive (true);

		yield return new WaitForSeconds (1.5f);

		flashLabel.gameObject.SetActive (false);
	}
}
