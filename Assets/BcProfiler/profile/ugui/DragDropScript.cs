using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragDropScript : MonoBehaviour,IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    RectTransform rectTrans = null;
    void Start()
    {
        rectTrans = GetComponent<RectTransform>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        rectTrans.pivot.Set(0, 0);
        transform.position = Input.mousePosition;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        //transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
