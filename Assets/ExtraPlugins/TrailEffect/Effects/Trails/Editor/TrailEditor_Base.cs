using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace PigeonCoopToolkit.Effects.Trails.Editor
{
    
    public class TrailEditor_Base : UnityEditor.Editor
    {
        

        public static TrailPreviewUtillity win;

        public override void OnInspectorGUI()
        {

            TrailRenderer_Base t = (TrailRenderer_Base)serializedObject.targetObject;
            if (t == null)
                return;

            float defaultLabelWidth = EditorGUIUtility.labelWidth;
            float defaultFieldWidth = EditorGUIUtility.fieldWidth;
            GUILayout.Space(5);

            GUILayout.BeginVertical();
            

            EditorGUILayout.PropertyField(serializedObject.FindProperty("TrailData.Lifetime"));
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            EditorGUIUtility.fieldWidth = defaultFieldWidth;

            GUILayout.BeginHorizontal();
            EditorGUIUtility.fieldWidth = defaultLabelWidth - 80;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TrailData.TrailMaterial"));
            GUILayout.Space(10);
            EditorGUIUtility.labelWidth = 30;
            EditorGUIUtility.fieldWidth = 40;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TrailData.MaterialTileLength"), new GUIContent("Tile"), GUILayout.Width(70));
            GUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = defaultLabelWidth;
            EditorGUIUtility.fieldWidth = defaultFieldWidth;


            GUILayout.BeginHorizontal();
            EditorGUIUtility.fieldWidth = defaultLabelWidth - 80;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TrailData.SizeOverLife"));
            GUILayout.Space(10);
            EditorGUIUtility.labelWidth = 50;
            EditorGUIUtility.fieldWidth = 20;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TrailData.StretchSizeToFit"), new GUIContent("Stretch"), GUILayout.Width(70));
            GUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = defaultLabelWidth;
            EditorGUIUtility.fieldWidth = defaultFieldWidth;

            GUILayout.BeginHorizontal();
            EditorGUIUtility.fieldWidth = defaultLabelWidth - 80;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TrailData.ColorOverLife"));
            GUILayout.Space(10);
            EditorGUIUtility.labelWidth = 50;
            EditorGUIUtility.fieldWidth = 20;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TrailData.StretchColorToFit"), new GUIContent("Stretch"), GUILayout.Width(70));
            GUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            EditorGUIUtility.fieldWidth = defaultFieldWidth;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("TrailData.UseForwardOverride"));
            if(t.TrailData.UseForwardOverride)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("TrailData.ForwardOverride"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TrailData.ForwardOverrideRelative"), new GUIContent("Override Relative"));


                EditorGUI.indentLevel--;

            }


            DrawTrailSpecificGUI();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxNumberOfPoints"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Emit"));


            GUILayout.EndVertical();

         


            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfDirtyOrScript();


            GUILayout.Space(5);
            if (GUILayout.Button("Open preview"))
            {
                // Get existing open window or if none, make a new one:
                win = (TrailPreviewUtillity)EditorWindow.GetWindow(typeof(TrailPreviewUtillity), true, "Normalized Trail Preview");
                win.minSize = new Vector2(900, 140);
                win.maxSize = new Vector2(900, 140);
                win.Trail = t;
            }
            
        }

        protected virtual void DrawTrailSpecificGUI()
        {}
    }
}
