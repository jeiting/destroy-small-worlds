using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBehaviour : MonoBehaviour {

	private const string FIRE_MISSILE_BUTTON = "Fire1";
	private const string MOVE_CAMERA_BUTTON = "Fire2";

	private const string CAMERA_X_AXIS = "Mouse X";
	private const string CAMERA_Y_AXIS = "Mouse Y";

	[System.Serializable]
	public class PlayerConfiguration {
		public float mouseSensitivity = 3.0f;
		public float fuelCapacity = 50.0f;
		public float minFuel = 1.0f;
	}
	public PlayerConfiguration configuration;

	[System.Serializable]
	public class PlayerState {
		public float fuelRemaining = 0.0f;
		public bool isGameOver = false;

	}
	public PlayerState state;

	public CameraBehaviour playerCamera;
	public PlanetBehaviour playerPlanet;
	public RocketLaunchBehaviour rocketLaunchController;

	private float viewAngle = 0.0f;
	private float viewPitch = 0.0f;

	// Use this for initialization
	void Start () {
		state.fuelRemaining = configuration.fuelCapacity;

		FillMinimumFuel ();
	}

	public void Reset ()
	{
		playerPlanet.transform.position = Vector3.zero;
		playerPlanet.GetComponent<Rigidbody> ().velocity = Vector3.zero;
	}

	void FillMinimumFuel() {
		var fuelLoadad = rocketLaunchController.LoadMinimumFuel ();
		state.fuelRemaining -= fuelLoadad;
	}

	void FillFuel(float fuel) {
		var fuelLoaded = rocketLaunchController.LoadFuel (fuel);
		state.fuelRemaining -= fuelLoaded;
	}

	public float fuelRemainingPercentage {
		get {
			return state.fuelRemaining / configuration.fuelCapacity;
		}
	}

	public bool fuelInRocket {
		get {
			return rocketLaunchController.fuelInRocket;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (state.isGameOver) {
			return;
		}
		
		if (Input.GetButton(MOVE_CAMERA_BUTTON)) {
			viewAngle += configuration.mouseSensitivity * Input.GetAxis (CAMERA_X_AXIS);
			//viewPitch += configuration.mouseSensitivity * Input.GetAxis (CAMERA_Y_AXIS);
		}

		if (Input.GetButton(FIRE_MISSILE_BUTTON) && state.fuelRemaining > 0.0) {
			FillMinimumFuel ();
			FillFuel (Time.deltaTime);
		}

		if (Input.GetButtonUp(FIRE_MISSILE_BUTTON)) {
			rocketLaunchController.Fire(state.fuelRemaining >= 0.0);
		}

		rocketLaunchController.transform.rotation = Quaternion.Euler (Vector3.up * viewAngle + Vector3.right * viewPitch);
	}
}
