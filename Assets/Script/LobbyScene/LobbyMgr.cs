using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyMgr : MonoBehaviour
{
    public static LobbyMgr Inst = null;

    //------ Fade Out 관련 변수들...
    public Image m_FadeImg = null;
    private float AniDuring = 1.0f;  //페이드아웃 연출을 시간 설정
    private bool m_StartFade = false;
    private float m_CacTime = 0.0f;
    private float m_AddTimer = 0.0f;
    private Color m_Color; 
    
    private float m_StVal = 1.0f;
    private float m_EndVal = 0.0f;
    string SceneName = "";
    //------ Fade In 관련 변수들...

    public Button WaPlayBtn;
    public Button BamPlayBtn;
    public Button KimPlayBtn;
    public Button GuPlayBtn;
    public Button MiPlayBtn;

    public Button QuitBtn;

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
        SoundMgr.Instance.PlayBGM("cyberpunk city 2", 1.0f);

        //------ 로비로 들어올 때 Fade In 설정 
        m_StVal = 1.0f;
        m_EndVal = 0.0f;
        m_FadeImg.gameObject.SetActive(true);
        m_StartFade = true;
        //------ 로비로 들어올 때 Fade In 설정 

        if (WaPlayBtn != null)
            WaPlayBtn.onClick.AddListener(() =>
            {
                SceneOut("WananaGameScene");
            });

        if (BamPlayBtn != null)
            BamPlayBtn.onClick.AddListener(() =>
            {
                SceneOut("BamGameScene");
            });


        if (KimPlayBtn != null)
            KimPlayBtn.onClick.AddListener(() =>
            {
                SceneOut("KimGameScene");
            });

        if (GuPlayBtn != null)
            GuPlayBtn.onClick.AddListener(() =>
            {
                SceneOut("GuGameScene");
            });

        if (MiPlayBtn != null)
            MiPlayBtn.onClick.AddListener(() =>
            {
                SceneOut("MimiGameScene");
            });

        if (QuitBtn != null)
            QuitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });
    }

    // Update is called once per frame
    void Update()
    {
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
}
