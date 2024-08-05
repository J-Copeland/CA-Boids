using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriHolder : MonoBehaviour, IDragHandler, IScrollHandler
{

    private Vector3 panelCentre = new Vector2(Screen.width * 0.75f, Screen.height / 2); //centre of the triholder panel: 1/2 down screen, 3/4 across from left
    [SerializeField] private float zoomScaleFactor = 0.05f;

    public void Start()
    {
        Debug.Log("x: " + panelCentre.x + "   y: " + panelCentre.y);
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("pos: " + transform.position.x + eventData.delta.x);
        transform.localPosition = new Vector2(Mathf.Clamp(transform.localPosition.x + eventData.delta.x, -400, 400), Mathf.Clamp(transform.localPosition.y + eventData.delta.y, -400, 400)); //FIX: can behave wierdly with zoom-panning
    }

    public void OnScroll(PointerEventData eventData)
    {
        Vector3 newLocalScale = new Vector3(Mathf.Clamp(transform.GetChild(0).localScale.x + eventData.scrollDelta.y * zoomScaleFactor, 0.5f, 5), Mathf.Clamp(transform.GetChild(0).localScale.y + eventData.scrollDelta.y * zoomScaleFactor, 0.5f, 5), 0);
        
        if (transform.GetChild(0).localScale != newLocalScale) //effects inside only trigger on valid scroll
        {
            float scaleChangeFactor = newLocalScale.x / transform.GetChild(0).localScale.x;
            
            transform.GetChild(0).localScale = newLocalScale;
            transform.GetChild(0).position += new Vector3(Input.mousePosition.x - transform.GetChild(0).position.x, Input.mousePosition.y - transform.GetChild(0).position.y, 0) * (1 - scaleChangeFactor); //diff in position between mouse & obj, mult by relative change in size

        }

    }
    
}
