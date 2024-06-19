using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriHolder : MonoBehaviour, IDragHandler, IScrollHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("pos: " + transform.position.x + eventData.delta.x);
        transform.localPosition = new Vector2(Mathf.Clamp(transform.localPosition.x + eventData.delta.x, -400, 400), Mathf.Clamp(transform.localPosition.y + eventData.delta.y, -400, 400));
    }

    public void OnScroll(PointerEventData eventData)
    {
        transform.GetChild(0).localScale = new Vector2(Mathf.Clamp(transform.GetChild(0).localScale.x + eventData.scrollDelta.y * 0.05f, 0, 5), Mathf.Clamp(transform.GetChild(0).localScale.y + eventData.scrollDelta.y * 0.05f, 0, 5));
    }
    
}
