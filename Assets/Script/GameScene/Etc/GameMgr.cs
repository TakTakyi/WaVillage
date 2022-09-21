using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class GameMgr : MonoBehaviour
{
    public static GameMgr Inst = null;

    PlayerCtrl m_Player;
    GameObject m_MidBoss;
    MonTsCtrl m_MidMonBoss;
    GameObject m_EndBoss;
    MonTsCtrl m_EndBossCtrl;
    public bool m_IsOnEndBoss = false;

    //스크린의 월드 좌표
    public static Vector3 m_SceenWMin = new Vector3(-10.0f, -5.0f, 0.0f);
    public static Vector3 m_SceenWMax = new Vector3(10.0f, 5.0f, 0.0f);
    //스크린의 월드 좌표

    //------ UI 관련 변수들
    public Button m_BackBtn;
    public Text m_heartTxt;
    public Text m_UltTxt;
    public Text m_BestScoreTxt;

    public GameObject BossHpBar;
    public Image HpBar;
    public Text BossHpText;
    //------ UI 관련 변수들

    //------ Fade Out 관련 변수들...
    public Image m_FadeImg = null;
    private bool m_StartFade = false;
    private float AniDuring = 1.0f;  //페이드아웃 연출을 시간 설정
    private float m_CacTime = 0.0f;
    private float m_AddTimer = 0.0f;
    public Color m_Color;

    private float m_StVal = 1.0f;
    private float m_EndVal = 0.0f;
    string SceneName = "";
    //------ Fade In 관련 변수들...

    //----- Item 관련 변수들
    public static GameObject m_heartItem = null;
    public static GameObject m_SubPlayerItem = null;
    public static GameObject m_UltimateItem = null;
    public static GameObject m_MagnetItem = null;
    //----- Item 관련 변수들

    //----- GameOver 관련 변수들
    [Header("---- Game Over ----")]
    public GameObject GameOverPanel;
    public Text GameOverTxt;
    public Button ReplayBtn;
    public Button GoLobbbyBtn;
    public Button GameQuitBtn;
    public Button StopVideoBtn;
    public RawImage m_Meme;
    public RawImage mScreen;
    public VideoPlayer mVideoPlayer;
    //[HideInInspector]
    public bool IsPlayVideo = false;
    string m_ASceneName = "";
    //----- GameOver 관련 변수들

    //---------------------------- 환경설정 Dlg 관련 변수
    [Header("-------- Config DialogBox --------")]
    public GameObject Canvas_Dialog = null;
    private GameObject m_ConfigBoxObj = null;
    public bool ISConfig = true;
    //---------------------------- 환경설정 Dlg 관련 변수

    void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SoundMgr.Instance.PlayBGM("cyber city 2-b", 1.0f);

        m_ASceneName = SceneManager.GetActiveScene().name;
        Time.timeScale = 1.0f;

        m_Player = GameObject.FindObjectOfType<PlayerCtrl>();

        m_MidBoss = GameObject.FindGameObjectWithTag("MiddleBoss");
        m_MidMonBoss = m_MidBoss.GetComponent<MonTsCtrl>();

        m_EndBoss = GameObject.FindGameObjectWithTag("EndBoss");
        m_EndBossCtrl = m_EndBoss.GetComponent<MonTsCtrl>();

        //------스크린의 월드 좌표 구하기
        Vector3 a_ScMin = new Vector3(0.0f, 0.0f, 0.0f); //ScreenViewPort 좌측하단
        m_SceenWMin = Camera.main.ViewportToWorldPoint(a_ScMin);
        //카메라 화면 좌측하단 코너의 월드 좌표

        Vector3 a_ScMax = new Vector3(1.0f, 0.8f, 0.0f); //ScreenViewPort 우측상단
        m_SceenWMax = Camera.main.ViewportToWorldPoint(a_ScMax);
        //카메라 화면 우측상단 코너의 월드 좌표
        //------스크린의 월드 좌표 구하기

        //----- Item Resources불러오기
        m_heartItem = Resources.Load("Prefab/Item/HeartItem") as GameObject;
        m_MagnetItem = Resources.Load("Prefab/Item/MagnetItem") as GameObject;

        if (SceneManager.GetActiveScene().name == "WananaGameScene")
        {
            m_SubPlayerItem = Resources.Load("Prefab/Item/WaSubPlayerItem") as GameObject;
            m_UltimateItem = Resources.Load("Prefab/Item/WaUltimateItem") as GameObject;
        }

        if (SceneManager.GetActiveScene().name == "BamGameScene")
        {
            m_SubPlayerItem = Resources.Load("Prefab/Item/BamSubPlayerItem") as GameObject;
            m_UltimateItem = Resources.Load("Prefab/Item/BamUltimateItem") as GameObject;
        }

        if (SceneManager.GetActiveScene().name == "KimGameScene")
        {
            m_SubPlayerItem = Resources.Load("Prefab/Item/KimSubPlayerItem") as GameObject;
            m_UltimateItem = Resources.Load("Prefab/Item/KimUltimateItem") as GameObject;
        }

        if (SceneManager.GetActiveScene().name == "GuGameScene")
        {
            m_SubPlayerItem = Resources.Load("Prefab/Item/GuSubPlayerItem") as GameObject;
            m_UltimateItem = Resources.Load("Prefab/Item/GuUltimateItem") as GameObject;
        }

        if (SceneManager.GetActiveScene().name == "MimiGameScene")
        {
            m_SubPlayerItem = Resources.Load("Prefab/Item/MiSubPlayerItem") as GameObject;
            m_UltimateItem = Resources.Load("Prefab/Item/MiUltimateItem") as GameObject;
        }
        //----- Item Resources불러오기

        BossHpBar.SetActive(false);
        UIUpdate();

        //------ 게임으로 들어올 때 Fade In 설정 
        m_StVal = 1.0f;
        m_EndVal = 0.0f;
        m_FadeImg.gameObject.SetActive(true);
        m_StartFade = true;
        //------ 게임으로 들어올 때 Fade In 설정 

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() =>
            {
                SceneOut("LobbyScene");
            });

        if (ReplayBtn != null)
            ReplayBtn.onClick.AddListener(() =>
            {
                Time.timeScale = 1.0f;
                SceneOut(m_ASceneName);
            });

        if (GoLobbbyBtn != null)
            GoLobbbyBtn.onClick.AddListener(() =>
            {
                Time.timeScale = 1.0f;
                SceneOut("LobbyScene");
            });

        if (GameQuitBtn != null)
            GameQuitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

        if (StopVideoBtn != null)
            StopVideoBtn.onClick.AddListener(() =>
            {
                IsPlayVideo = !IsPlayVideo;
                StopVideo();
            });
    }

    // Update is called once per frame
    void Update()
    {
        UIUpdate();

        if (BossHpBar.activeSelf == true)
        {
            if (m_IsOnEndBoss == true)
                EndBossUIUpdate();
            else
                MidBossUIUpdate();
        }

        FadeUpdate();

        //---------------------------- 환경설정 Dlg 관련 구현 부분
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (ISConfig == false)
                return;

            if (ISConfig == true)
            {
                ISConfig = false;

                if (m_ConfigBoxObj == null)
                    m_ConfigBoxObj = Resources.Load("SoundsPanel") as GameObject;

                GameObject a_CfgBoxObj = (GameObject)Instantiate(m_ConfigBoxObj);
                a_CfgBoxObj.transform.SetParent(Canvas_Dialog.transform, false);
                //false 로 해야 로컬 프리랩에 설정된 좌표를 유지한채 차일드로 붙게된다.
                Time.timeScale = 0.0f; //일시정지
            }
        }
        //---------------------------- 환경설정 Dlg 관련 구현 부분

        if (mScreen != null && mVideoPlayer != null)
        {
            // 비디오 준비 코루틴 호출
            StartCoroutine(PrepareVideo());
        }
    }

    void UIUpdate()
    {
        m_heartTxt.text = "X " + m_Player.m_PlayerHP.ToString();
        m_UltTxt.text = "X " + m_Player.m_UltCount.ToString();
        m_BestScoreTxt.text = "KillCount : " + MonsterSpawn.Inst.m_MonKillCount.ToString();
    }

    void MidBossUIUpdate()
    {
        HpBar.fillAmount = m_MidMonBoss.m_CurHP / m_MidMonBoss.m_MaxHP;
        BossHpText.text = m_MidMonBoss.m_CurHP.ToString() + " / " + m_MidMonBoss.m_MaxHP.ToString();
    }

    void EndBossUIUpdate()
    {
        HpBar.fillAmount = m_EndBossCtrl.m_CurHP / m_EndBossCtrl.m_MaxHP;
        BossHpText.text = m_EndBossCtrl.m_CurHP.ToString() + " / " + m_EndBossCtrl.m_MaxHP.ToString();
    }

    void FadeUpdate()  //FadeInOut
    {
        if (m_StartFade == false)
            return;

        if (m_CacTime < 1.0f)
        {
            m_AddTimer = m_AddTimer + Time.deltaTime;
            m_CacTime = m_AddTimer / AniDuring;
            m_Color = m_FadeImg.color;
            m_Color.a = Mathf.Lerp(m_StVal, m_EndVal, m_CacTime);
            m_FadeImg.color = m_Color;
            if (1.0f <= m_CacTime)
            {
                if (m_StVal == 1.0f && m_EndVal == 0.0f)// 들어올 때 
                {
                    m_Color.a = 0.0f;
                    m_FadeImg.color = m_Color;
                    m_FadeImg.gameObject.SetActive(false);
                    m_StartFade = false;
                }
                else if (m_StVal == 0.0f && m_EndVal == 1.0f) //나갈 때
                {
                    SceneManager.LoadScene(SceneName);
                }
            } //if (1.0f <= m_CacTime)
        } //if (m_CacTime < 1.0f)

    }//void FadeUpdate()

    void SceneOut(string a_ScName)
    {
        SceneName = a_ScName;

        m_CacTime = 0.0f;
        m_AddTimer = 0.0f;
        m_StVal = 0.0f;
        m_EndVal = 1.0f;
        m_FadeImg.gameObject.SetActive(true);
        m_StartFade = true;
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
        if (IsPlayVideo == false)
            return;

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
