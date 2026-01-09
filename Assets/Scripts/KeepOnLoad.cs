using UnityEngine;

public class KeepOnLoad : MonoBehaviour {
	private void Start() {
		DontDestroyOnLoad(gameObject);
		Debug.Log("[DEBUG]: Keeping " + gameObject.name + " on load");
	}
}