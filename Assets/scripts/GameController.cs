using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	[System.Serializable]
	public class GameConfiguration {
		public float damageScoreMultiplier = 100.0f;

		public AudioClip levelCompleteSound;
		public AudioClip gameOverSound;
	}
	public GameConfiguration configuration;

	public PlayerControllerBehaviour playerController;
	public UIController uiController;

	public int currentLevel = 0;
	public GameObject level0 = null;
	public GameObject level1 = null;
	public GameObject level2 = null;
	public GameObject level3 = null;
	public GameObject level4 = null;
	public GameObject level5 = null;
	public GameObject level6 = null;
	public GameObject level7 = null;
	public GameObject level8 = null;
	public GameObject level9 = null;

	private GameObject[] levels;

	private bool gameIsOver = false;

	public int score = 0;

	// Use this for initialization
	void Start () 
	{
		var eventManager = EventManager.GetEventManager ();

		eventManager.planetHitEvent.AddListener (PlanetHit); 
		eventManager.planetExplodedEvent.AddListener (PlanetExploded);
		eventManager.powerupCollectedEvent.AddListener (PowerupCollected);

		levels = new GameObject[] {level0, level1, level2, level3, level4, level5, level6, level7, level8, level9};

		DisableAllLevels ();
		LoadLevel ();
	}

	void AddPoints(int points) {
		score += points;
		uiController.SetScore (score);
	}

	void PowerupCollected(Powerup powerup) {
		playerController.state.fuelRemaining += powerup.configuration.additionalFuel;
	}

	void PlanetHit(PlanetBehaviour planet, RocketBehaviour rocket, PlanetBehaviour.HitInfo hitInfo) {
		if (planet != playerController.playerPlanet) {
			AddPoints ((int)(configuration.damageScoreMultiplier * hitInfo.damageDealt));

			if (hitInfo.coreExposed) {
				uiController.FlashMessage ("CORE EXPOSED");
				AddPoints (500);
			}

			if (hitInfo.coreHit) {
				uiController.FlashMessage ("CORE HIT");
				AddPoints (1000);
			}

			if (planet.state.hitPoints <= 0.0f) {

				planet.Explode ();
			}
		} else {
			if (playerController.playerPlanet.state.hitPoints <= 0.0f) {
				GameOver ();
			}
		}
	}

	void PlanetExploded(PlanetBehaviour planet) {
		
		levels [currentLevel].GetComponent<LevelBehaviour> ().numPlanets--;
		if (planet.configuration.isPlayer) {
			GameOver ();
		} else if (levels [currentLevel].GetComponent<LevelBehaviour> ().numPlanets == 0) {
			

			currentLevel++;
			LoadLevel ();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		uiController.SetFuelRemainingPercentage (playerController.fuelRemainingPercentage);

		if (playerController.fuelRemainingPercentage <= 0.0f 
			&& !playerController.fuelInRocket 
			&& GameObject.FindObjectOfType<RocketBehaviour>() == null) {
			GameOver ();	
		}
	}

	void GameOver() {
		if (gameIsOver)
			return;
		gameIsOver = true;
		playerController.state.isGameOver = true;
		playerController.playerPlanet.GetComponent<AudioSource> ().Stop ();
		GetComponent<AudioSource> ().PlayOneShot (configuration.gameOverSound);
		uiController.ShowGameOver ();
	}

	void DisableAllLevels() {
		foreach (var level in levels) {
			if (level == null)
				continue;
			level.SetActive (false);
		}
	}

	void LoadLevel() {
		DisableAllLevels ();
		if (currentLevel < levels.Length) {
			var level = levels [currentLevel];
			level.SetActive (true);

			if (currentLevel != 0) {
				GetComponent<AudioSource> ().PlayOneShot (configuration.levelCompleteSound);
			}

			uiController.SetLevel (currentLevel + 1, level.GetComponent<LevelBehaviour>().levelName);

			playerController.Reset ();
		} else {
			GameOver ();
		}
	}
}
