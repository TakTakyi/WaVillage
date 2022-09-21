using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TitleMgr : MonoBehaviour
{
    public RawImage mScreen;
    public VideoPlayer mVideoPlayer;
    public Button GoLobbyBtn;
    
    float VideoLength;
    float m_Time;

    //------ Fade Out 관련 변수들...
    public Image m_FadeImg = null;
    private float AniDuring = 1.0f;  //페이드아웃 연출을 시간 설정
    private bool m_StartFade = false;
    private float m_CacTime = 0.0f;
    private float m_AddTimer = 0.0f;
    private Color m_Color;
    //------ Fade Out 관련 변수들... 

    // Start is called before the first frame update
    void Start()
    {
        VideoLength = (float)mVideoPlayer.length;

        if (GoLobbyBtn != null)
            GoLobbyBtn.onClick.AddListener(() =>
            {
                StopVideo();
                m_FadeImg.gameObject.SetActive(true);
                m_StartFade = true;
            });

        if (mScreen != null && mVideoPlayer != null)
        {
            // 비디오 준비 코루틴 호출
            StartCoroutine(PrepareVideo());
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_Time += Time.deltaTime;
        if (VideoLength <= m_Time)
        {
            m_FadeImg.gameObject.SetActive(true);
            m_StartFade = true;
            StopVideo();
        }

        if (m_StartFade == true)
        {
            if (m_CacTime < 1.0f)
            {
                m_AddTimer = m_AddTimer + Time.deltaTime;
                m_CacTime = m_AddTimer / AniDuring;
                m_Color = m_FadeImg.color;
                m_Color.a = m_CacTime;
                m_FadeImg.color = m_Color;
                if (1.0f <= m_CacTime)
                {
                    SceneManager.LoadScene("TutorialScene");
                }
            }
        }//if (m_StartFade == false)  
    }

    protected IEnumerator PrepareVideo()
    {
        // 비디오 준비
        mVideoPlayer.Prepare();

        // 비디오가 준비되는 것을 기다림
        while (!mVideoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // VideoPlayer의 출력 texture를 RawImage의 texture로 설정한다
        mScreen.texture = mVideoPlayer.texture;
        PlayVideo();
    }

    public void PlayVideo()
    {
        if (mVideoPlayer != null && mVideoPlayer.isPrepared)
        {
            // 비디오 재생
            mVideoPlayer.Play();
        }
    }

    public void StopVideo()
    {
        if (mVideoPlayer != null && mVideoPlayer.isPrepared)
        {
            // 비디오 멈춤
            mVideoPlayer.Stop();
        }
    }
}
