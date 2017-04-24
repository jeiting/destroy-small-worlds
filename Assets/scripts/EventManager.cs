using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class PlanetHitEvent : UnityEvent<PlanetBehaviour, RocketBehaviour, PlanetBehaviour.HitInfo> {
	
}

[System.Serializable]
public class PlanetExplodedEvent : UnityEvent<PlanetBehaviour> {
	
}

[System.Serializable]
public class PowerUpCollectedEvent : UnityEvent<Powerup> { 
}

public class EventManager : MonoBehaviour {

	public static EventManager GetEventManager() {
		return GameObject.Find ("EventManager").GetComponent<EventManager> ();
	}

	public PlanetHitEvent planetHitEvent;
	public PlanetExplodedEvent planetExplodedEvent;
	public PowerUpCollectedEvent powerupCollectedEvent;

	// Use this for initialization
	void Awake () {
		planetHitEvent = new PlanetHitEvent ();
		planetExplodedEvent = new PlanetExplodedEvent ();
		powerupCollectedEvent = new PowerUpCollectedEvent ();
	}
}
