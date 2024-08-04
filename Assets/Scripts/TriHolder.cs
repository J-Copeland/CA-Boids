using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriHolder : MonoBehaviour, IDragHandler, IScrollHandler
{

    private Vector2 panelCentre = new Vector2(Screen.width * 0.75f, Screen.height / 2); //centre of the triholder panel: 1/2 down screen, 3/4 across from left
    [SerializeField] private float zoomScaleFactor = 0.05f;
    [SerializeField] private float zoomPositionScaleFactor = 0.01f;

    public void Start()
    {
        Debug.Log("x: " + panelCentre.x + "   y: " + panelCentre.y);
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("pos: " + transform.position.x + eventData.delta.x);
        transform.localPosition = new Vector2(Mathf.Clamp(transform.localPosition.x + eventData.delta.x, -400, 400), Mathf.Clamp(transform.localPosition.y + eventData.delta.y, -400, 400));
    }

    public void OnScroll(PointerEventData eventData)
    {
        Vector3 newLocalScale = new Vector3(Mathf.Clamp(transform.GetChild(0).localScale.x + eventData.scrollDelta.y * zoomScaleFactor, 0, 5), Mathf.Clamp(transform.GetChild(0).localScale.y + eventData.scrollDelta.y * zoomScaleFactor, 0, 5), 0);
        float Xdifference = transform.GetChild(0).localScale.x - newLocalScale.x;
        Debug.Log(transform.GetChild(0).localScale.x + "...   " + newLocalScale.x);
        if (transform.GetChild(0).localScale != newLocalScale) //effects inside only trigger on valid scroll
        {
            
            transform.GetChild(0).localScale = newLocalScale;
            if(eventData.scrollDelta.y > 0)
            {
                transform.GetChild(0).position += (new Vector3(Input.mousePosition.x - panelCentre.x, Input.mousePosition.y - panelCentre.y, 0) * (Xdifference)); //subtract difference between mouse position & panelCentre
            }
            else
            {
                transform.GetChild(0).position += (new Vector3(Input.mousePosition.x - panelCentre.x, Input.mousePosition.y - panelCentre.y, 0) * (Xdifference)); //add ^
            }
            Debug.Log(new Vector3(Input.mousePosition.x - panelCentre.x, Input.mousePosition.y - panelCentre.y, 0) * Xdifference + "...   " + Mathf.Abs(Xdifference));
        }
        
    }
    
}
