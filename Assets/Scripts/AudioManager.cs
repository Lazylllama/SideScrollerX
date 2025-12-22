using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	#region Fields

	public static           AudioManager    Instance;
	
	//? Clip list public to be able to get the length of a clip in other scripts
	[SerializeField] public List<AudioClip> audioClipList;
	[SerializeField] private AudioSource     sfxSource;

	public enum AudioName {
		CoinCollect,      //0
		Explosion,        //1
		BlobDie,          //2
		BeeDie,           //3
		ButtonPress,      //4
		CollectItem,      //5
		GoldenKeyCollect, //6
		LowHealthWarn,    //7
		PauseOpen,        //8
		PauseClose,       //9
		UseKey,           //10
		PlayerHurt,       //11
		PlayerDie,        //12
		PlayerFallDie,    //13
		HealthReplenish,  //14
		CursorPress,      //15
		ThrowBomb,        //16
		BirdCallEnd,      //17
		OpenDoor,         //18
	};

	private readonly Dictionary<AudioName, AudioClip> audioClips = new();

	#endregion

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(gameObject);
			return;
		}

		Instance = this;

		InitializeDictionary();
	}

	private void InitializeDictionary() {
		for (var i = 0; i < audioClipList.Count; i++) {
			if (i < System.Enum.GetValues(typeof(AudioName)).Length) {
				audioClips[(AudioName)i] = audioClipList[i];
			}
		}
	}

	/// <summary>
	/// Plays a sound effect
	/// </summary>
	/// <param name="audioName">Audio Clip Name - AudioManager.AudioName[]</param>
	/// <param name="volume">Sound Volume - 0 to 1 (float)</param>
	public void PlaySfx(AudioName audioName, float volume = 1f) {
		if (audioClips.TryGetValue(audioName, out var clip)) {
			sfxSource.PlayOneShot(clip, volume);
		} else {
			Debug.LogWarning($"AudioClip {audioName} not found in AudioManager dictionary!");
		}
	}
}