using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

namespace UnityEngine.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class ComboBoxItem : MonoBehaviour
    {
        public ComboBox parent;
        public Button buttonComponent;
        public Text textComponent;
		public int intData;
		public UnityEvent onSelected;
		
		[HideInInspector]
		public RectTransform rectTransform;
        [HideInInspector]
        public float height;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            height = rectTransform.sizeDelta.y;
            buttonComponent.onClick.AddListener(OnClicked);
            parent.items.Add(this);
        }

        void OnClicked()
        {
            parent.selected = this;
			onSelected.Invoke();
        }
    }
}