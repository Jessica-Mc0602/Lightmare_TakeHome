using UnityEngine.EventSystems;
using UnityEngine;


public class HoverEffect : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    Vector3 orginalScale;
    AudioSource audioSource;
    
    void Start()
    {
        orginalScale = transform.localScale; //Store the orginal scale of the panel
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogError("AudioSource component not found on " + gameObject.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = orginalScale * 1.1f; //Increase the size by 10%
        if (audioSource != null)
            audioSource.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = orginalScale; // Return to the orginal scale
    }
}
