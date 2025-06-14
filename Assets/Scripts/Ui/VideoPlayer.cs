using UnityEngine;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject videoPlayerObject;
    public GameObject gameUICanvas;

    private VideoPlayer videoPlayer;

    void Start()
    {
        // Assure que seul le menu est visible au départ
        mainMenuCanvas.SetActive(true);
        videoPlayerObject.SetActive(false);
        gameUICanvas.SetActive(false);

        videoPlayer = videoPlayerObject.GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    public void OnStartGameClicked()
    {
        mainMenuCanvas.SetActive(false);
        videoPlayerObject.SetActive(true);
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        videoPlayerObject.SetActive(false);
        gameUICanvas.SetActive(true);
    }
}