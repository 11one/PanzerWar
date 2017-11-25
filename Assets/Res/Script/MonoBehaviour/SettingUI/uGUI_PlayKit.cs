using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_IOS
using UnityEngine.iOS;
using UnityEngine.Apple.ReplayKit;
#endif
public class uGUI_PlayKit : MonoBehaviour {
	public Transform OnPlaying, OnNotPlay, Discard,Preview,ReplayKitUI;
	#if UNITY_IOS
	void Start () {
		UpdateReplayStatues();
	}
	void Update(){
		UpdateReplayStatues();
	}
	public void UpdateReplayStatues(){
		ReplayKitUI.gameObject.SetActive(ReplayKit.APIAvailable);
		Discard.gameObject.SetActive(ReplayKit.recordingAvailable);
		Preview.gameObject.SetActive(ReplayKit.recordingAvailable);
		OnPlaying.gameObject.SetActive(ReplayKit.isRecording);
		OnNotPlay.gameObject.SetActive(!ReplayKit.isRecording);
	}
	public void StartRecording(){
		ReplayKit.StartRecording();
	}
	public void StopRecording(){
		ReplayKit.StopRecording();
	}
	public void RecordingPreview(){
		ReplayKit.Preview();
	}
	public void RecordingDiscard(){
		ReplayKit.Discard();
	}
	#else
	void Start(){
		ReplayKitUI.gameObject.SetActive(false);
	}
	#endif

}
