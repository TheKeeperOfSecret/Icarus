using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FinalCutsceneController : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoClip videoClip;
    public string videoPath;

    [Header("UI Settings")]
    public RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
    public bool skipable = true;

    private VideoPlayer videoPlayer;
    private RawImage rawImage;
    private GameObject videoCanvas;
    private bool isPlaying = false;

    void Update()
    {
        if (isPlaying && skipable && Input.anyKeyDown && videoPlayer != null && videoPlayer.isPlaying)
        {
            SkipCutscene();
        }
    }

    public void PlayCutscene()
    {
        if (isPlaying) return;

        isPlaying = true;
        CreateVideoCanvas();
        SetupVideoPlayer();
        PlayVideo();
    }

    void CreateVideoCanvas()
    {
        videoCanvas = new GameObject("VideoCutsceneCanvas");
        Canvas canvas = videoCanvas.AddComponent<Canvas>();
        canvas.renderMode = renderMode;
        canvas.sortingOrder = 9999;

        GameObject rawImageObj = new GameObject("VideoRawImage");
        rawImageObj.transform.SetParent(videoCanvas.transform);
        rawImage = rawImageObj.AddComponent<RawImage>();

        RectTransform rect = rawImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void SetupVideoPlayer()
    {
        videoPlayer = videoCanvas.AddComponent<VideoPlayer>();

        videoPlayer.playOnAwake = false;
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClip;

        // Альтернативно: загрузка по пути
        // videoPlayer.source = VideoSource.Url;
        // videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoPath);

        videoPlayer.renderMode = VideoRenderMode.RenderTexture;

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        videoPlayer.targetTexture = renderTexture;
        rawImage.texture = renderTexture;

        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void PlayVideo()
    {
        BlockGameInput(true);

        Time.timeScale = 0f;

        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        EndCutscene();
    }

    public void SkipCutscene()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            EndCutscene();
        }
    }

    void EndCutscene()
    {
        isPlaying = false;

        BlockGameInput(false);

        Time.timeScale = 1f;

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            if (videoPlayer.targetTexture != null)
            {
                videoPlayer.targetTexture.Release();
                Destroy(videoPlayer.targetTexture);
            }
            Destroy(videoPlayer);
        }

        if (videoCanvas != null)
            Destroy(videoCanvas);

        OnCutsceneFinished?.Invoke();

        GameManager.Instance.RestartLevel();
    }

    void BlockGameInput(bool block)
    {
        GameManager.Instance.player.GetComponent<Movement>().enabled = false;

        Cursor.visible = block;
    }

    public System.Action OnCutsceneFinished;
}
