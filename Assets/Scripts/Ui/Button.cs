using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_Hover");
    }
}
