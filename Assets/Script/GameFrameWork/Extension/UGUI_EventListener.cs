using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GFW
{

    public sealed class UGUI_EventListener : UnityEngine.EventSystems.EventTrigger
    {
        public delegate void VoidDelegate(GameObject go);

        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;
        public VoidDelegate onLongPress;
        private Coroutine last_press_co;
        private YieldInstruction waitForSecond = new WaitForSeconds(0.5f);

        private static readonly List<RaycastResult> list = new List<RaycastResult>();
        private static PointerEventData eventData;

        public static bool IsUIEvent
        {
            get
            {
                if (eventData == null)
                {
                    eventData = new PointerEventData(EventSystem.current);
                }

                list.Clear();
                eventData.pressPosition = Input.mousePosition;
                eventData.position = Input.mousePosition;
                EventSystem.current.RaycastAll(eventData, list);
                return list.Count > 0;
            }
        }

        public static UGUI_EventListener Get(GameObject go)
        {
            UGUI_EventListener listener = go.GetComponent<UGUI_EventListener>();
            if (listener == null) listener = go.AddComponent<UGUI_EventListener>();
            return listener;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null) onClick(gameObject);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null) onDown(gameObject);
            if (this.onLongPress != null) last_press_co = StartCoroutine(CO_LongPress());
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null) onEnter(gameObject);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null) onExit(gameObject);
            if (last_press_co != null)
            {
                StopCoroutine(last_press_co);
                last_press_co = null;
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null) onUp(gameObject);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null) onSelect(gameObject);
        }

        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (onUpdateSelect != null) onUpdateSelect(gameObject);
        }


        IEnumerator CO_LongPress()
        {
            yield return waitForSecond;
            last_press_co = null;
            if (this.onLongPress != null)
            {
                this.onLongPress(this.gameObject);
            }
        }
    }
}