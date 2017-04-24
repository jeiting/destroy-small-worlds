using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLaunchBehaviour : MonoBehaviour {

	public GameObject rocketPrefab;
	public float respawnTimer = 0.3f;

	public GameObject rocketsContainer;

	public bool isGameOver = false;

	private GameObject currentRocket;

	void Start () 
	{
		GenerateRocket ();	
	}

	public bool fuelInRocket {
		get {
			return (currentRocket != null) && currentRocket.GetComponent<RocketBehaviour> ().state.fuelLoaded > 0.0;
		}
	}

	void GenerateRocket() {
		currentRocket = Instantiate (rocketPrefab);
		currentRocket.GetComponent<RocketBehaviour> ().configuration.sourcePlanet = GetComponentInParent<PlanetBehaviour> ();
		currentRocket.transform.parent = transform;
		currentRocket.transform.localRotation = Quaternion.LookRotation (Vector3.forward);
		currentRocket.transform.localPosition = Vector3.forward;
	}

	IEnumerator GenerateRocketAfterTime() {
		yield return new WaitForSecondsRealtime(respawnTimer);
		if (currentRocket == null) {
			GenerateRocket();
		}
	}

	public float LoadMinimumFuel() {
		if (currentRocket == null) {
			return 0.0f;
		}
		var rocket = currentRocket.GetComponent<RocketBehaviour> ();

		if (rocket.state.fuelLoaded > rocket.configuration.minFuel) {
			return 0.0f;
		}

		return LoadFuel (rocket.configuration.minFuel - rocket.state.fuelLoaded);
	}

	public float LoadFuel(float fuel) {
		if (currentRocket == null) {
			return 0.0f;
		}

		var rocket = currentRocket.GetComponent<RocketBehaviour> ();
		var availableSpace = rocket.configuration.maxFuel - rocket.state.fuelLoaded;
		var fuelToLoad = Mathf.Min (availableSpace, fuel);

		rocket.AddFuel (fuelToLoad);

		return fuelToLoad;
	}

	public void Fire(bool generateAfter = true) 
	{
		if (currentRocket == null) {
			return;
		}

		var rocket = currentRocket.GetComponent<RocketBehaviour> ();
		rocket.transform.parent = rocketsContainer.transform;
		rocket.Launch ();

		currentRocket = null;
		if (generateAfter) {
			StartCoroutine (GenerateRocketAfterTime ());
		}
	}
}
