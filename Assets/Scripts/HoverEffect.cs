using UnityEngine.EventSystems;
using UnityEngine;


public class HoverEffect : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    Vector3 orginalScale;
    
    void Start()
    {
        orginalScale = transform.localScale; //Store the orginal scale of the panel
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = orginalScale * 1.1f; //Increase the size by 10%
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = orginalScale; // Return to the orginal scale
    }
}
