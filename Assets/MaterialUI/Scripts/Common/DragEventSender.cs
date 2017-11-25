//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    /// <summary> Captures drag events from an object and sends them to other handlers, depending on the orientation of the drag. </summary>
    public class DragEventSender : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        /// <summary> This object will recieve events if the current object is dragged horizontally. </summary>
        [SerializeField]
        private GameObject m_HorizontalTargetObject;
        /// <summary> This object will recieve events if the current object is dragged horizontally. </summary>
        public GameObject horizontalTargetObject
        {
            get { return m_HorizontalTargetObject; }
            set { m_HorizontalTargetObject = value; }
        }

        /// <summary> This object will recieve events if the current object is dragged vertically. </summary>
        [SerializeField]
        private GameObject m_VerticalTargetObject;
        /// <summary> This object will recieve events if the current object is dragged vertically. </summary>
        public GameObject verticalTargetObject
        {
            get { return m_VerticalTargetObject; }
            set { m_VerticalTargetObject = value; }
        }

        /// <summary> This object will recieve events regardless of whether the current object is dragged horizontally or vertically. </summary>
        [SerializeField]
        private GameObject m_AnyDirectionTargetObject;
        /// <summary> This object will recieve events regardless of whether the current object is dragged horizontally or vertically. </summary>
        public GameObject anyDirectionTargetObject
        {
            get { return m_AnyDirectionTargetObject; }
            set { m_AnyDirectionTargetObject = value; }
        }

        /// <summary> On the last OnBeginDrag event, was the current object dragged horizontally? </summary>
        private bool m_CurrentDragIsHorizontal;

        public void OnDrag(PointerEventData eventData)
        {
            if (m_CurrentDragIsHorizontal && m_HorizontalTargetObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(m_HorizontalTargetObject, eventData, ExecuteEvents.dragHandler);
            }

            if (!m_CurrentDragIsHorizontal && m_VerticalTargetObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(m_VerticalTargetObject, eventData, ExecuteEvents.dragHandler);
            }

            if (m_AnyDirectionTargetObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(m_AnyDirectionTargetObject, eventData, ExecuteEvents.dragHandler);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_CurrentDragIsHorizontal = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

            if (m_CurrentDragIsHorizontal && m_HorizontalTargetObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(m_HorizontalTargetObject, eventData, ExecuteEvents.beginDragHandler);
            }

            if (!m_CurrentDragIsHorizontal && m_VerticalTargetObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(m_VerticalTargetObject, eventData, ExecuteEvents.beginDragHandler);
            }

            if (m_AnyDirectionTargetObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(m_AnyDirectionTargetObject, eventData, ExecuteEvents.beginDragHandler);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (m_CurrentDragIsHorizontal && m_HorizontalTargetObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(m_HorizontalTargetObject, eventData, ExecuteEvents.endDragHandler);
            }

            if (!m_CurrentDragIsHorizontal && m_VerticalTargetObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(m_VerticalTargetObject, eventData, ExecuteEvents.endDragHandler);
            }

            if (m_AnyDirectionTargetObject != null)
            {
                ExecuteEvents.ExecuteHierarchy(m_AnyDirectionTargetObject, eventData, ExecuteEvents.endDragHandler);
            }
        }
    }
}