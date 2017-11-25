//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;

namespace MaterialUI
{
    [CustomEditor(typeof(DragEventSender))]
    public class DragEventSenderEditor : Editor
    {
        private SerializedProperty m_HorizontalTargetObject;
        private SerializedProperty m_VerticalTargetObject;
        private SerializedProperty m_AnyDirectionTargetObject;

        void OnEnable()
        {
            m_HorizontalTargetObject = serializedObject.FindProperty("m_HorizontalTargetObject");
            m_VerticalTargetObject = serializedObject.FindProperty("m_VerticalTargetObject");
            m_AnyDirectionTargetObject = serializedObject.FindProperty("m_AnyDirectionTargetObject");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_HorizontalTargetObject);
            EditorGUILayout.PropertyField(m_VerticalTargetObject);
            EditorGUILayout.PropertyField(m_AnyDirectionTargetObject);

            serializedObject.ApplyModifiedProperties();
        }
    }
}