using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.Video;


public class StartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float beatingCooldown = 1;
    [SerializeField] private bool setObjectsInInspector = false;
    [Header("------- SET IN INSPECTOR -------"), ShowIf("setObjectsInInspector")] 
    [SerializeField, ShowIf("setObjectsInInspector")] private VideoPlayer videoPlayer;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject introInterface;
    [SerializeField, ShowIf("setObjectsInInspector")] private GameObject questionsInterface;

    private bool isBeating;
    private Sequence beatingSequence;
    private bool hasBeenClicked = false;
    
    void Start()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/AMB/AMB_Menu/AMB_M");
        isBeating = false;
        hasBeenClicked = false;
        StartBeating();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hasBeenClicked) return;
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Menu/UI_M_HoverIn");
        StopBeating();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hasBeenClicked) return;
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

    public void ButtonClick()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Menu/UI_M_Click");
        StopBeating();
        hasBeenClicked = true;
        
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.Prepare();
    }
    
    private void DisableIntroInterface()
    {
        introInterface.SetActive(false);
    }
    
    private void OnVideoPrepared(VideoPlayer vp)
    {
        videoPlayer.Play();
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_Menu/UI_M_Intro");
        Invoke(nameof(DisableIntroInterface), 0.2f);
    }
    
    private void OnVideoEnd(VideoPlayer vp)
    {
        videoPlayer.gameObject.SetActive(false);
        questionsInterface.SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_StartGame");
    }
}
