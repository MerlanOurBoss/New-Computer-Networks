using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    [SerializeField] private Image pauseAndresumeButton;
    [SerializeField] private Image muteButton;
    [SerializeField] private VideoClip[] videoClips;

    private VideoPlayer _myVideoPlayer;
    private bool _isPlaying = false;
    private bool _isMuted = false;
    private int videoIndex = 0;

    private void Start()
    {
        _myVideoPlayer = GetComponent<VideoPlayer>();
    }

    public void VideoIndexPlay(int index)
    {
        _myVideoPlayer.clip = videoClips[index];
    }
    public void PauseAndPlay()
    {
        if (!_isPlaying)
        {
            _myVideoPlayer.Pause();
            pauseAndresumeButton.sprite = Resources.Load<Sprite>("UI/VideoPanel/Play-Button");
            _isPlaying = true;
        }
        else
        {
            _myVideoPlayer.Play();
            pauseAndresumeButton.sprite = Resources.Load<Sprite>("UI/VideoPanel/Pause-Button");
            _isPlaying = false;
        }
    }

    public void MuteVideo()
    {
        if (_myVideoPlayer)
        {
            _isMuted = _myVideoPlayer.GetDirectAudioMute(0);
            _myVideoPlayer.SetDirectAudioMute(0, !_isMuted);
            if (_isMuted) { muteButton.sprite = Resources.Load<Sprite>("UI/VideoPanel/Volume-Button"); }
            else { muteButton.sprite = Resources.Load<Sprite>("UI/VideoPanel/Mute-Button"); }
        }
    }
}
