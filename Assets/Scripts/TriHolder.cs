using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriHolder : MonoBehaviour, IDragHandler, IScrollHandler
{

    private Vector3 panelCentre = new Vector2(Screen.width * 0.75f, Screen.height / 2); //centre of the triholder panel: 1/2 down screen, 3/4 across from left
    [SerializeField] private float zoomScaleFactor = 0.05f;

    [SerializeField] private float maxLimitX, minLimitX, maxLimitY, minLimitY;

    public void scaleChanged()
    {
        float triWidth = transform.GetChild(0).localScale.x * transform.GetChild(0).GetComponent<RectTransform>().rect.width;
        float triHeight = transform.GetChild(0).localScale.y * transform.GetChild(0).GetComponent<RectTransform>().rect.height;
        maxLimitX = panelCentre.x + Screen.width / 4 + triWidth / 4;
        minLimitX = panelCentre.x - Screen.width / 4 - triWidth / 4;
        maxLimitY = panelCentre.y + Screen.height / 2 + triHeight / 4;
        minLimitY = panelCentre.y - Screen.height / 2 - triHeight / 4;
    }

    public void Start()
    {
        Debug.Log("x: " + panelCentre.x + "   y: " + panelCentre.y);
        scaleChanged();
    }

    public void OnDrag(PointerEventData eventData)
    {
        float newXPos = Mathf.Clamp(transform.GetChild(0).position.x + eventData.delta.x, minLimitX, maxLimitX);
        float newYPos = Mathf.Clamp(transform.GetChild(0).position.y + eventData.delta.y, minLimitY, maxLimitY);
        transform.GetChild(0).position = new Vector2(newXPos, newYPos);
    }

    public void OnScroll(PointerEventData eventData)
    {

        float newXScale = Mathf.Clamp(transform.GetChild(0).localScale.x + eventData.scrollDelta.y * zoomScaleFactor, 0.5f, 5);
        float newYScale = Mathf.Clamp(transform.GetChild(0).localScale.y + eventData.scrollDelta.y * zoomScaleFactor, 0.5f, 5);
        Vector3 newLocalScale = new Vector3(newXScale, newYScale, 0);
        
        if (transform.GetChild(0).localScale != newLocalScale) //effects inside only trigger on valid scroll
        {
            float scaleChangeFactor = newLocalScale.x / transform.GetChild(0).localScale.x;

            transform.GetChild(0).localScale = newLocalScale;
            scaleChanged();

            //diff in position between mouse & obj, multiplied by relative change in size
            float newXPos = Mathf.Clamp(transform.GetChild(0).position.x + ((Input.mousePosition.x - transform.GetChild(0).position.x) * (1 - scaleChangeFactor)), minLimitX, maxLimitX);
            float newYPos = Mathf.Clamp(transform.GetChild(0).position.y + ((Input.mousePosition.y - transform.GetChild(0).position.y) * (1 - scaleChangeFactor)), minLimitY, maxLimitY);
            transform.GetChild(0).position = new Vector2(newXPos, newYPos);
        }

    }
    
}
