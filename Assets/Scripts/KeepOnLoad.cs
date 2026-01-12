using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class KeepOnLoad : MonoBehaviour {
	private static Dictionary<string, GameObject> _instances = new Dictionary<string, GameObject>();
	public         string id;

	void Awake() {
		if (_instances.TryGetValue(id, out var existing)) {
			// A null result indicates the other object was destroyed for some reason
			if (existing != null) {
				if (ReferenceEquals(gameObject, existing)) return;

				Destroy(gameObject);

				// Return to skip the following registration code
				return;
			}
		}

		// The following code registers this GameObject regardless of whether it's new or replacing
		_instances[id] = gameObject;

		DontDestroyOnLoad(gameObject);
	}
}