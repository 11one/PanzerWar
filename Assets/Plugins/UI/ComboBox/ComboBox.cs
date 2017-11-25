using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace UnityEngine.UI
{
    public enum PointerState { Inside, Outside }
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class ComboBox : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        #region Enumerations
        public enum PanelState { Opened, Closed, Opening, Closing }
        #endregion
        #region Variables
        public Transform parent;
        public ComboBoxHeader header;
        public ScrollRect panel;
        public PanelState state;
        public PointerState pointerState;
        public ComboBoxItem selected;
		[Tooltip("The speed of transitions between two states.")]
        public float stateTransitionSpeed = 6f;
		[Tooltip("Max items in the view box.")]
        public int maxItemsInView = 5;
		[Tooltip("Should the first item be selected if the selection was no set?")]
		public bool autoSelect;
		[Tooltip("Should the SelectionChanged event caused by the autoSelect property be captured?")]
		public bool sendInitialSelectionEvent;
        public List<ComboBoxItem> items;
        #endregion
        #region Static Variables
        static ComboBox selectedComboBox;
        static List<ComboBox> allComboBoxes;
        #endregion
        #region Private Variables
		[HideInInspector]
		private bool initialSelect = false;
        [HideInInspector]
        private RectTransform panelContent;
        [HideInInspector]
        private GameObject panelGO;
        [HideInInspector]
        public float height;
        [HideInInspector]
        private float viewHeight = 0;
        [HideInInspector]
        private RectTransform self;
        [HideInInspector]
        private ComboBoxItem lastSelected;
        [HideInInspector]
        private bool canCallOpeningStarted = true;
        [HideInInspector]
        private bool canCallClosingStarted = true;
        #endregion
		#region EventListeners
		public UnityEvent onSelectionChanged;
		#endregion
		#region Initialization
		void Awake()
        {
			if (state != PanelState.Closing) state = PanelState.Closing;
			if (onSelectionChanged == null) onSelectionChanged = new UnityEvent();
            if (allComboBoxes == null) allComboBoxes = new List<ComboBox>();
            allComboBoxes.Add(this);
            self = GetComponent<RectTransform>();
            //if (parent == null) parent = self.parent;
            if (panel != null)
            {
                panelGO = panel.gameObject;
                panelContent = panel.content;
            }
            header.parent = this;
            header.buttonComponent.onClick.AddListener(OnClick);

			
        }
        void Start()
        {
            CalculateHeight();
			panel.gameObject.SetActive(false);
            if (selectedComboBox == null)
                selectedComboBox = this;
			if (autoSelect && selected == null)
			{
				#if DEBUG
				Debug.Log("[UI.ComboBox] Setting initial selection.");
				#endif
				if (sendInitialSelectionEvent) initialSelect = true;
				selected = items[items.Count-1];
			}

			onSelectionChanged.AddListener(OnSelectionChanged);
        }
		#endregion
		#region Methods
		void Update()
		{
			#region Panel states
			if (state == PanelState.Opening)
            {
                if (canCallOpeningStarted) OnOpeningStarted();
                float delta = stateTransitionSpeed * 100 * Time.deltaTime;
                float maxViewHeight = viewHeight + header.height;
                float currentViewHeight = Mathf.Clamp(self.sizeDelta.y + delta,header.height, maxViewHeight);
                self.sizeDelta = new Vector2(self.sizeDelta.x,currentViewHeight);
                self.anchoredPosition = new Vector2(self.anchoredPosition.x,-self.sizeDelta.y);

                float currentHeight = Mathf.Clamp(panelContent.sizeDelta.y + delta, 0, height);
                panelContent.sizeDelta = new Vector2(panelContent.sizeDelta.x, currentHeight);
                panelContent.anchoredPosition = new Vector2(panelContent.anchoredPosition.x, -panelContent.sizeDelta.y);

                if (self.sizeDelta.y == maxViewHeight && panelContent.sizeDelta.y == height) state = PanelState.Opened;
            }
            else if (state == PanelState.Closing)
            {
                if (canCallClosingStarted) OnClosingStarted();
                float delta = -stateTransitionSpeed * 100 * Time.deltaTime;
                float maxViewHeight = viewHeight + header.height;
                float currentViewHeight = Mathf.Clamp(self.sizeDelta.y + delta, header.height, maxViewHeight);
                self.sizeDelta = new Vector2(self.sizeDelta.x, currentViewHeight);
                self.anchoredPosition = new Vector2(self.anchoredPosition.x, -self.sizeDelta.y);

                float currentHeight = Mathf.Clamp(panelContent.sizeDelta.y + delta, 0, height);
                panelContent.sizeDelta = new Vector2(panelContent.sizeDelta.x, currentHeight);
                panelContent.anchoredPosition = new Vector2(panelContent.anchoredPosition.x, -panelContent.sizeDelta.y);

                if (self.sizeDelta.y == header.height && panelContent.sizeDelta.y == 0)
                {
                    state = PanelState.Closed;
                    OnClosed();
                }
			}
			if (state != PanelState.Closed && state != PanelState.Closing)
			{
				//if (Input.GetMouseButtonUp(0))
				//{
				//	OnPointerUp();
				//}
			}
			#endregion
			#region Selection
			if (lastSelected != selected)
            {
                lastSelected = selected;
                header.textComponent.text = selected.textComponent.text;
                for (int index = 0; index < items.Count; index++)
                {
                    if (items[index] != selected) items[index].buttonComponent.interactable = true;
                    else selected.buttonComponent.interactable = false;
                }
				if (!initialSelect)
					onSelectionChanged.Invoke();
				else initialSelect = false;
            }
            if (selectedComboBox == this && state == PanelState.Opened)
            {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    state = PanelState.Closing;
                }
            }            
            #endregion
            #region Singletoning
            if (state == PanelState.Opened && selectedComboBox != this)
            {
                state = PanelState.Closing;
            }
            #endregion
        }
        void CalculateHeight()
        {
            viewHeight = 0f;
            height = 0f;
            for (int index = 0; index < items.Count; index++)
            {
                height += items[index].height;

                if (index < maxItemsInView)
                    viewHeight += items[index].height;
            }
        }
		public void ClosePanel() {
			if (state != PanelState.Closed || state != PanelState.Closing) state = PanelState.Closing;
		}
		public void OpenPanel() {
			if (state != PanelState.Opened || state != PanelState.Opening) state = PanelState.Opening;
		}
		public void SetMaxVisibleItems(int max) {
			maxItemsInView = max;
			CalculateHeight();
		}
		#region Data management
		public GameObject AddItem(string text) {
			return AddItem(text,-1);
		}
		public GameObject AddItem(string text,int data)
        {
			if (items.Count >= 1) {
            GameObject original = items[items.Count - 1].gameObject;
            RectTransform originalRect = original.GetComponent<RectTransform>();

            GameObject obj = (GameObject)GameObject.Instantiate(original);
            RectTransform rectTransfom = obj.GetComponent<RectTransform>();
            ComboBoxItem comboBoxItem = obj.GetComponent<ComboBoxItem>();

            obj.name = text;
            comboBoxItem.textComponent.text = text;
			comboBoxItem.intData = data;

            rectTransfom.SetParent(originalRect.parent, false);
            rectTransfom.anchoredPosition = new Vector2(rectTransfom.anchoredPosition.x, rectTransfom.anchoredPosition.y - 28);

            CalculateHeight();
			return obj;
			}
			else {
				Debug.LogError("[UI.ComboBox] You need to have an item in the ComboBox, no items were found.",this);
				return null;
			}
        }
		public bool SetSelectedWhereData(int data)
		{
			for (int index = 0; index < items.Count; index++)
			{
				if (items[index].intData == data)
				{
					#if DEBUG 
					Debug.Log("[UI.ComboBox] Item selected where data == " + data.ToString(), this);
					#endif
					selected = items[index];
					return true;
				}
			}
			#if DEBUG
			Debug.Log("[UI.ComboBox] no item found with data == " + data.ToString(), this);
			#endif
			return false;
		}
		public bool SetSelectedWhereText(string text)
		{
			for (int index = 0; index < items.Count; index++)
			{
				if (items[index].textComponent.text == text)
				{
					selected = items[index];
					return true;
				}
			}
			return false;
		}
		public void SetSelectedWhereIndex(int index)
		{
			selected = items[index];
		}
		public bool SetSelected(string text, int data)
		{
			for (int index = 0; index < items.Count; index++)
			{
				if (items[index].textComponent.text == text && items[index].intData == data)
				{
					selected = items[index];
					return true;
				}
			}
			return false;
		}	
		#endregion
		#endregion
		#region Events
		private void OnClosed()
        {
            panelGO.SetActive(false);
            if (parent != null) parent.SetAsFirstSibling();
        }
        private void OnClosingStarted()
        {
            header.imageComponent.rectTransform.DORotate(new Vector3(0, 0, header.initialRotation), 0.35f, RotateMode.Fast);
            if (selectedComboBox == this) selectedComboBox = null;
            canCallOpeningStarted = true;
            canCallClosingStarted = false;
        }
        private void OnOpeningStarted()
        {
            header.imageComponent.rectTransform.DORotate(new Vector3(0, 0, header.desiredRotation), 0.35f, RotateMode.Fast);
            selectedComboBox = this;
            canCallOpeningStarted = false;
            canCallClosingStarted = true;
            panelGO.SetActive(true);
            if (parent != null) parent.SetAsLastSibling();
        }
        private void OnClick()
        {
            if (state == PanelState.Opening || state == PanelState.Opened) state = PanelState.Closing;
            else if (state == PanelState.Closing || state == PanelState.Closed) state = PanelState.Opening;
            else throw new NotImplementedException();
        }
        public void OnPointerUp()
        {
            if (pointerState == PointerState.Outside)
            {
                if (state != PanelState.Closed)
                    state = PanelState.Closing;
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            pointerState = PointerState.Outside;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerState = PointerState.Inside;
        }
		public void OnSelectionChanged()
		{
			#if DEBUG
			Debug.Log("[UI.ComboBox] Selection changed.",this);
			#endif
		}
		/*private void OnLanguageDirectionChanged(Direction dir)
		{
			bool panelActive = panelGO.activeSelf;
			if (!panelActive) panelGO.SetActive(true);
			if (dir == Direction.LeftToRight)
			{
				header.textComponent.alignment = TextAnchor.MiddleLeft;
				for (int index = 0; index < items.Count; index++)
				{
					items[index].textComponent.alignment = TextAnchor.MiddleLeft;
				}
			}
			else
			{
				header.textComponent.alignment = TextAnchor.MiddleRight;
				for (int index = 0; index < items.Count; index++)
				{
					items[index].textComponent.alignment = TextAnchor.MiddleRight;
				}
			}
			if (!panelActive) panelGO.SetActive(false);
		}*/
        #endregion
    }
}