using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour {

	[System.Serializable]
	public class RocketConfiguration {
		public float fuelDamageMultiplier = 10.0f;
		public float maxFuel = 4.0f;
		public float minFuel = 1.0f;
		public float thrust = 1.0f;
		public float lifespan = 5.0f;

		public float baseDamage = 1.0f;

		public PlanetBehaviour sourcePlanet;
	}
	public RocketConfiguration configuration;

	[System.Serializable]
	public class RocketState {
		public float fuelLoaded = 0.0f;
		public bool leftPlanet = false;
		public bool launched = false;
	}
	public RocketState state;

	private bool shutdown = false;
	private float currentFuel = 0.0f;
	private float lifetimeRemaining;


	// Use this for initialization
	void Start ()  
	{
		GetComponentInChildren<TrailRenderer> ().enabled = false;
	}

	public float damage {
		get {
			return configuration.baseDamage + currentFuel * configuration.fuelDamageMultiplier;
		}
	}

	public void AddFuel(float fuel) {
		state.fuelLoaded += fuel;
		currentFuel = state.fuelLoaded;
	}

	public void Launch () {

		state.launched = true;

		var rigidBody = GetComponent<Rigidbody> ();
		rigidBody.isKinematic = false;
		GetComponent<Rigidbody>().velocity = configuration.sourcePlanet.gameObject.GetComponent<Rigidbody> ().velocity;

		GetComponentInChildren<TrailRenderer> ().enabled = true;

		currentFuel = state.fuelLoaded;
		lifetimeRemaining = configuration.lifespan;

		GetComponent<AudioSource> ().Play ();
	}

	Vector3 LookDirection() {
		var rigidBody = GetComponent<Rigidbody> ();

		var soiVelocitySum = new Vector3 ();

		foreach (var planet in GameObject.FindGameObjectsWithTag (GravitationalBodyBehaviour.PLANET_TAG)) {
			// SOI weighted relative velocity
			var planetRB = planet.GetComponent<Rigidbody>();
			var mass = planetRB.mass;
			var velocity = planetRB.velocity;

			var relVelocity = rigidBody.velocity - planetRB.velocity;
			var relDistance = (rigidBody.position - planet.transform.position).magnitude;

			float weight = mass / Mathf.Min(0.0001f, Mathf.Pow (relDistance, 2.0f));

			soiVelocitySum += weight * relVelocity;
		}
		soiVelocitySum = soiVelocitySum.normalized;



		// t goes from 0.0 -> 1.0
		float t = Mathf.Pow((state.fuelLoaded - currentFuel) / state.fuelLoaded, 5.0f);

		return (transform.forward * (1.0f - t) + soiVelocitySum * t).normalized;
	}

	void Update() {
		var fuelMaterial = GetComponent<MeshRenderer> ().materials [1];
		fuelMaterial.SetFloat ("_Fuel", Mathf.Min(currentFuel, state.fuelLoaded) / configuration.maxFuel);
	}

	void FixedUpdate () {
		
		if (state.launched) {
			var rigidBody = GetComponent<Rigidbody> ();

			if (currentFuel > 0.0f) {
				rigidBody.AddRelativeForce (Vector3.forward * configuration.thrust);
				currentFuel -= Time.fixedDeltaTime;
			} else if (!shutdown) {
				GetComponentInChildren<TrailRenderer> ().gameObject.transform.parent = GameObject.Find ("Space").transform;
				GetComponent<AudioSource> ().Stop ();
				shutdown = true;
			}

			lifetimeRemaining -= Time.fixedDeltaTime;
			if (lifetimeRemaining < 0.0f) {
				Destroy (gameObject);
			}

			var relativeVelocity = LookDirection();
			transform.rotation = Quaternion.LookRotation (relativeVelocity);

		}
	}
}
