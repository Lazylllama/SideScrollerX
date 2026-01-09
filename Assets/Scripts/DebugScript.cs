using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugScript : MonoBehaviour {
	float timer;
	
	void Start() {
		
	}

	void Update() {
		timer += Time.deltaTime;

		if (timer > 10) SceneManager.LoadScene("Scenes/LevelTest");
	}
}