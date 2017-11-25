using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class ComboBoxHeader : MonoBehaviour
    {
        [HideInInspector]
        public ComboBox parent;
        public Button buttonComponent;
        public Text textComponent;
        public Image imageComponent;
        [HideInInspector]
        public float height;
        public float initialRotation;
        public float desiredRotation;
        public float rotationSpeed;

        void Awake()
        {
            initialRotation = imageComponent.rectTransform.eulerAngles.z;
            height = GetComponent<RectTransform>().sizeDelta.y;
        }
        //void Update()
        //{
        //    
        //    Vector3 Euler = imageComponent.rectTransform.rotation.eulerAngles;
        //    float delta = Time.deltaTime * 10f * rotationSpeed;
        //    imageComponent.rectTransform.rotation = Quaternion.Euler(euler.x, euler.y, Mathf.Clamp(euler.z + delta, (desiredRotation < initialRotation) ? desiredRotation : initialRotation, (desiredRotation > initialRotation ? desiredRotation : initialRotation)));
        //}
    }
}