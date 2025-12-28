using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PanelMoPhongDongCo : MonoBehaviour
{
    public List<VideoClip> videoClips;

    public VideoPlayer videoPlayer;

    public GameObject scene;

    public void ChonVideoMoPhong(GameObject go)
    {
        scene.SetActive(true);

        int idVideo = int.Parse(go.name);

        videoPlayer.clip = videoClips[idVideo];

        PlayVideo();
    }

    public void BackFromScene()
    {
        scene.SetActive(false);
    }

    public void PlayVideo()
    {
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
        }
    }

    public void StopVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
    }

    public void InActiveAll()
    {
        scene.SetActive(false);
    }
}
