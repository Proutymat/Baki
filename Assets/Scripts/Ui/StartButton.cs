using UnityEngine;
using UnityEngine.EventSystems;

public class StartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Menu/UI_M_HoverIn");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Menu/UI_M_HoverOut");
    }
}
