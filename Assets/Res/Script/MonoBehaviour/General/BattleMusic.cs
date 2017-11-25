using UnityEngine;
using System.Collections;
namespace Waroftanks.Music{
	public class BattleMusic : MonoBehaviour {
		public AudioClip[]  BGMs;
		AudioSource audioSource;
		public static BattleMusic Self; 
		public bool InStopBGMState = false; 
		void Start(){
			Self = this;
			audioSource = GetComponent<AudioSource> ();
			audioSource.spread = 360;
			audioSource.volume =0.5f;
			audioSource.minDistance = 1000;
			audioSource.maxDistance =1000;
		}
		public static void StopBGM(){
			Self.audioSource.Stop ();
			Self.InStopBGMState = true;
		}
		public static void PlayBGMinFight(){
			if (PlayerPrefs.GetInt ("BGMDuringBattle") == 1) {
				return;
			}	
			if (Self.InStopBGMState) {
				Self.InStopBGMState = false;
				return;
			}
			if (Self.audioSource.isPlaying!=true) {
				Self.audioSource.clip = Self.BGMs [Random.Range (0, Self.BGMs.Length)];
				Self.audioSource.Play ();
			}
		}
	}
	
}