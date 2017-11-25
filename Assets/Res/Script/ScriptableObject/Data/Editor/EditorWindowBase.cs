﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class EditorWindowBase : Editor {
	public string EditorHeadline = "ShanghaiWindy...";
	public bool InEditingSceneObject = false;

	public void BaseGUI(){
		EditorGUILayout.HelpBox (EditorHeadline, MessageType.Info);

        if(GUILayout.Button("Ping Object")){
			EditorGUIUtility.PingObject (target);
		}

		if (InEditingSceneObject) {
            EditorGUILayout.HelpBox ("In Editor Mode",MessageType.Warning);
			if (GUILayout.Button ("Unlock Inspector")) {
				ActiveEditorTracker.sharedTracker.isLocked = false;
				InEditingSceneObject = false;
			}
		}
	
	}


	public void LockEditor(){
		ActiveEditorTracker.sharedTracker.isLocked = true;
		InEditingSceneObject = true;
	} 
	public void UnlockEditor(){
		ActiveEditorTracker.sharedTracker.isLocked = false;
		InEditingSceneObject = true;
	}
	public virtual void Awake (){
		Selection.selectionChanged += OnSelectionChanged;

		SceneView.onSceneGUIDelegate += view =>
		{
			ShortCut();
		};
	}
	public virtual void OnDestroy (){
		ActiveEditorTracker.sharedTracker.isLocked = false;

		Selection.selectionChanged -= OnSelectionChanged;

		SceneView.onSceneGUIDelegate -= view =>
		{
			ShortCut();
		};
	}
	public virtual void OnSelectionChanged (){
	}
	public virtual void ShortCut (){
	}

}
