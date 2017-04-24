using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerups : MonoBehaviour {

	[System.Serializable]
	public class PowerupsConfiguration {
		public float powerUpsPerMinute = 4.0f;

		public GameObject powerupPrefab;
	}
	public PowerupsConfiguration configuration;

	void Update() {
		float roll = Random.Range (0.0f, 1.0f);
		if (roll < Time.deltaTime * configuration.powerUpsPerMinute / 60.0) {
			var powerup = Instantiate (configuration.powerupPrefab);
			powerup.transform.parent = transform;

			Vector3 spawnLocation = 50.0f * Random.insideUnitCircle;
			spawnLocation.z = spawnLocation.y;
			spawnLocation.y = 0;

			powerup.transform.position = spawnLocation;

			Vector3 toLocation = powerup.transform.position + new Vector3 (1.0f, 0.0f, 1.0f) * Random.Range (1.0f, 2.0f) * Random.Range (-1.0f, -2.0f);

			powerup.GetComponent<Rigidbody> ().velocity = -toLocation.normalized * Random.Range(1.0f, 5.0f);
		}
	}
}
