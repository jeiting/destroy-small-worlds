using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetBehaviour : MonoBehaviour {

	public class HitInfo {
		public float damageDealt;
		public bool coreExposed;
		public bool coreHit;
	}

	[System.Serializable]
	public class PlanetConfiguration {
		public bool isPlayer = false;
		public Color mainColor;
		public int damageResolution = 4;
		public float maxHitPoints = 100.0f;
		public float collisionMassDamageMultiplier = 5.0f;
	}

	public PlanetConfiguration configuration;

	[System.Serializable]
	public class PlanetState {
		public float hitPoints = 100.0f;
	}
	public PlanetState state;

	public ParticleSystem deathExplosionPrefab;
	public Camera mainCamera;

	private Color[] damageMapData;
	private Texture3D damageMapTexture;
	private float planetVolume;
	private bool coreExposed = false;

	private ParticleSystem explosion;

	void createDamageTexture() {

		var damageResolution = configuration.damageResolution;
		var size =  damageResolution * damageResolution * damageResolution;
		planetVolume = size * 0.5f;

		damageMapTexture = new Texture3D (damageResolution, damageResolution, damageResolution, TextureFormat.RGBA32, false);
		damageMapTexture.filterMode = FilterMode.Trilinear;
		damageMapTexture.anisoLevel = 9;
		damageMapTexture.wrapMode = TextureWrapMode.Clamp;

		damageMapData = new Color[size];

		for (int i = 0; i < size; i++) {
			
			damageMapData [i] = new Color (configuration.mainColor.r, configuration.mainColor.g, configuration.mainColor.b, 0.5f);
		}

		damageMapTexture.SetPixels (damageMapData);
		damageMapTexture.Apply ();
	}

	public float mass {
		get {
			return GetComponent<Rigidbody> ().mass;
		}
	}

	void Start () {
		createDamageTexture ();
		var renderer = GetComponent<MeshRenderer> ();
		renderer.material.color = configuration.mainColor;
		renderer.material.SetTexture ("_DamageMap", damageMapTexture);

		explosion = GetComponentInChildren<ParticleSystem> ();
		explosion.Stop ();
		explosion.Clear ();

		state.hitPoints = configuration.maxHitPoints;
	}

	void OnTriggerExit(Collider other) {
		var rocket = other.gameObject.GetComponent<RocketBehaviour> ();
		if (rocket && rocket.configuration.sourcePlanet == this) {
			rocket.state.leftPlanet = true;
		}
	}

	void OnTriggerEnter(Collider other) {
		var rocket = other.gameObject.GetComponent<RocketBehaviour> ();
		var planet = other.gameObject.GetComponent<PlanetBehaviour> ();

		var hitPosition = transform.InverseTransformPoint (other.ClosestPointOnBounds(transform.position)).normalized;

		if (rocket) {
			HandleRocketCollision (rocket, hitPosition);
		} else if (planet) {
			HandlePlanetCollision (hitPosition, planet);
		}
	}

	void HandlePlanetCollision(Vector3 hitPosition, PlanetBehaviour otherPlanet) {
		if (otherPlanet.mass < mass) {
			var damage = configuration.collisionMassDamageMultiplier * otherPlanet.mass;
			ReceiveDamage (damage, hitPosition, otherPlanet.configuration.mainColor);
		} else {
			Explode ();
		}
	}

	public void Explode() {
		var deathExplosion = Instantiate (deathExplosionPrefab);

		deathExplosion.transform.position = transform.position;
		deathExplosion.transform.parent = EffectsContainer.GetEffectsContainer ().transform;
		var main = deathExplosion.main;
		main.startColor = configuration.mainColor;


		var rigidBody = GetComponent<Rigidbody> ();
		var explosionBody = deathExplosion.GetComponent<Rigidbody> ();
		explosionBody.velocity = rigidBody.velocity;

		EventManager.GetEventManager ().planetExplodedEvent.Invoke (this);

		if (configuration.isPlayer) {
			mainCamera.transform.parent = transform.parent;
		}

		Destroy (gameObject);
	}

	void HandleRocketCollision(RocketBehaviour rocket, Vector3 hitPosition) {
		if (!rocket.state.launched) {
			return;
		}

		if (rocket.configuration.sourcePlanet == this && !rocket.state.leftPlanet) {
			return;
		}

		GetComponent<AudioSource> ().Play ();

		HitInfo hitInfo = ReceiveDamage (rocket.damage, hitPosition, rocket.configuration.sourcePlanet.configuration.mainColor);

		var rocketBody = rocket.GetComponent<Rigidbody> ();
		var rigidBody = GetComponent<Rigidbody> ();
		rigidBody.velocity += rocketBody.velocity * rocketBody.mass / rigidBody.mass;

		var eventManager = GameObject.Find ("EventManager").GetComponent<EventManager>();

		eventManager.planetHitEvent.Invoke (this, rocket, hitInfo); 

		Destroy (rocket.gameObject);

		// go boom
		explosion.gameObject.transform.localPosition = hitPosition;
		explosion.gameObject.transform.localRotation = Quaternion.LookRotation (hitPosition);

		var emitParams = new ParticleSystem.EmitParams();

		emitParams.startColor = rocket.configuration.sourcePlanet.configuration.mainColor;
		explosion.Emit(emitParams, 1000 * (int)hitInfo.damageDealt + 100);
	}

	HitInfo ReceiveDamage (float damage, Vector3 hitPosition, Color stainColor)
	{
		HitInfo hitInfo = new HitInfo ();

		var damageResolution = configuration.damageResolution;
		hitPosition = hitPosition.normalized;

		int x = Mathf.Clamp((int)(damageResolution * (hitPosition.x + 1.0f) / 2.0f), 0, damageResolution - 1);
		int y = Mathf.Clamp((int)(damageResolution * (hitPosition.y + 1.0f) / 2.0f), 0, damageResolution - 1);
		int z = Mathf.Clamp((int)(damageResolution * (hitPosition.z + 1.0f) / 2.0f), 0, damageResolution - 1);

		var idx = x * damageResolution * damageResolution + y * damageResolution + z;

		var color = damageMapData [idx];

		float localDamage = damage / planetVolume;

		if (color.a > localDamage) {
			color.a -= localDamage;
		} else if (color.a > 0.0f) {
			localDamage = color.a;
			color.a = 0.0f;

			// Core exposure only happens once
			if (!coreExposed) {
				hitInfo.coreExposed = true;
				coreExposed = true;
			}
		} else {
			// This is a core strike, maybe this does more damage
			localDamage = 2.0f * localDamage;
			hitInfo.coreHit = true;
		}

		hitInfo.damageDealt = localDamage * planetVolume;

		state.hitPoints -= localDamage * planetVolume;

		color.r = stainColor.r;
		color.g = stainColor.g;
		color.b = stainColor.b;

		damageMapData [idx] = color;

		damageMapTexture.SetPixels (damageMapData);
		damageMapTexture.Apply ();

		return hitInfo;
	}

}
