using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalBodyBehaviour : MonoBehaviour {

	public const string PLANET_TAG = "Planet";
	private const float GRAVITATIONAL_STRENGTH = 0.1f;

	public Vector3 initalVelocity;
	public float rotationRate;

	// Use this for initialization
	void Start () {
		var rigidBody = GetComponent<Rigidbody> ();
		rigidBody.velocity = initalVelocity;
		rigidBody.angularVelocity = new Vector3 (0.0f, rotationRate, 0.0f);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		var rigidBody = GetComponent<Rigidbody> ();
		var mass = rigidBody.mass;
		// Get other planets
		var otherPlanets = GameObject.FindGameObjectsWithTag(PLANET_TAG);

		foreach (var planet in otherPlanets) {
			if (planet == gameObject || !planet.GetComponent<GravitationalBodyBehaviour>()) {
				continue;
			}

			var direction = planet.transform.position - this.transform.position;
			var otherMass = planet.GetComponent<Rigidbody> ().mass;
			var distance = direction.magnitude;

			var force = GRAVITATIONAL_STRENGTH * mass * otherMass / Mathf.Pow (distance, 2) * direction.normalized;
			rigidBody.AddForce (force);

		};
	}
}
