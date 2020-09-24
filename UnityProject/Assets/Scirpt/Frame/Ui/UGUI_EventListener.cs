using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public sealed class UGUI_EventListener : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler, IPointerClickHandler, IUpdateSelectedHandler, ISelectHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static UGUI_EventListener Get(GameObject go)
    {
        UGUI_EventListener listener = go.GetComponent<UGUI_EventListener>();
        if (listener == null)
            listener = go.AddComponent<UGUI_EventListener>();
        return listener;
    }


    public static bool IsUIEvent
    {
        get
        {
            if (_eventData == null)
                _eventData = new PointerEventData(EventSystem.current);
            _eventData.pressPosition = Input.mousePosition;
            _eventData.position = Input.mousePosition;
            if (_list == null)
                _list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_eventData, _list);
            return _list.Count > 0;
        }
    }

    private static List<RaycastResult> _list;
    private static PointerEventData _eventData;


    public delegate void VoidDelegate(GameObject go);

    public delegate void PointerDelegate(GameObject go, PointerEventData eventData);

    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public PointerDelegate onPointDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public PointerDelegate onPointUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VoidDelegate onLongPress;
    public PointerDelegate onBeginDrag;
    public PointerDelegate onDrag;
    public PointerDelegate onEndDrag;

    private float last_onclick = 0;
    private Coroutine last_press_co;
    private YieldInstruction longPressWait = new WaitForSeconds(0.1f);
    private YieldInstruction longPressStart = new WaitForSeconds(0.5f);
    private bool _isLongPress; //防止长按的同时 触发OnClick


    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isLongPress && onClick != null && Time.unscaledTime - last_onclick > 0.1f)
        {
            Debug.Log("[UGUI] OnPointerClick :" + gameObject.name);
            last_onclick = Time.unscaledTime;
            onClick(gameObject);
        }

        _isLongPress = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isLongPress = false;
        onDown?.Invoke(gameObject);
        onPointDown?.Invoke(gameObject, eventData);
        if (onLongPress != null)
            last_press_co = StartCoroutine(CO_LongPress());
    }

    IEnumerator CO_LongPress()
    {
        yield return longPressStart;
        while (true)
        {
            yield return longPressWait;
            _isLongPress = true;
            if (onLongPress != null)
                onLongPress(gameObject);
            else
                yield break;
            if (last_press_co == null)
                yield break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (last_press_co != null)
        {
            StopCoroutine(last_press_co);
            last_press_co = null;
        }

        onExit?.Invoke(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (last_press_co != null)
        {
            StopCoroutine(last_press_co);
            last_press_co = null;
        }

        onUp?.Invoke(gameObject);
        onPointUp?.Invoke(gameObject, eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        onSelect?.Invoke(gameObject);
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
        onUpdateSelect?.Invoke(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke(gameObject, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(gameObject, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(gameObject, eventData);
    }
}