using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsContainer : MonoBehaviour {

	public static EffectsContainer GetEffectsContainer() {
		return GameObject.FindObjectOfType<EffectsContainer>();
	}
}
