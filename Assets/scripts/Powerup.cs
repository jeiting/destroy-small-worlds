using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {
	[System.Serializable]
	public class PowerupConfiguration {
		public Texture texture;
		public float additionalFuel;

		public AudioClip spawnClip;
		public AudioClip pickupClip;

	}
	public PowerupConfiguration configuration;

	public MeshRenderer meshRenderer;

	void Start() {
		meshRenderer.material.mainTexture = configuration.texture;
		var audioSource = GetComponent<AudioSource> ();
		audioSource.PlayOneShot (configuration.spawnClip);
	}

	IEnumerator OnTriggerEnter(Collider other) {
		var rocket = other.gameObject.GetComponent<RocketBehaviour> ();

		if (rocket) {

			meshRenderer.enabled = false;

			var audioSource = GetComponent<AudioSource> ();
			audioSource.PlayOneShot (configuration.pickupClip);

			EventManager.GetEventManager ().powerupCollectedEvent.Invoke (this);

			yield return new WaitForSeconds (configuration.pickupClip.length);

			Destroy (gameObject);

		}
	}
}
