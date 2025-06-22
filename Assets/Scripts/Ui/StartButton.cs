using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


public class StartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float beatingCooldown = 1;
    private bool isBeating;
    private Sequence beatingSequence;
    
    void Start()
    {
        isBeating = false;
        StartBeating();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Menu/UI_M_HoverIn");
        StopBeating();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Menu/UI_M_HoverOut");
        StartBeating();
    }

    private void StartBeating()
    {
        if (isBeating) return;
        isBeating = true;
        
        beatingSequence = DOTween.Sequence()
            .AppendInterval(beatingCooldown / 2)
            .AppendCallback(() =>
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Menu/UI_M_Pulse");
            })
            .Append(this.transform.DOScale(1.2f, 0.1f).SetEase(Ease.OutQuad))
            .Append(this.transform.DOScale(1f, 0.1f).SetEase(Ease.InOutQuad))
            .Append(this.transform.DOScale(1.3f, 0.1f).SetEase(Ease.OutQuad))
            .Append(this.transform.DOScale(1f, 0.1f).SetEase(Ease.InOutQuad))
            .AppendInterval(beatingCooldown / 2)
            .SetLoops(-1);
    }
    
    private void StopBeating()
    {
        if (!isBeating) return;
        isBeating = false;
        beatingSequence?.Kill();
        this.transform.DOScale(1f, 0.2f);
    }
}
