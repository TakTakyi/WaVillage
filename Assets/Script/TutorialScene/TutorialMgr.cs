using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialMgr : MonoBehaviour
{
    public static TutorialMgr Inst = null;

    public Button m_SkipBtn;
    public Text m_GameInfoTxt;
    float m_DelayTime = 0.0f;
    float m_FixTime = 4.0f;

    public GameObject Panel;
    public Button PanelCloseBtn;

    //---------------------------- 환경설정 Dlg 관련 변수
    [Header("-------- Config DialogBox --------")]
    public GameObject Canvas_Dialog = null;
    private GameObject m_ConfigBoxObj = null;
    public bool ISConfig = true;
    //---------------------------- 환경설정 Dlg 관련 변수

    //------ Fade Out 관련 변수들...
    public Image m_FadeImg = null;
    private float AniDuring = 1.0f;  //페이드아웃 연출을 시간 설정
    private bool m_StartFade = false;
    private float m_CacTime = 0.0f;
    private float m_AddTimer = 0.0f;
    public Color m_Color;

    private float m_StVal = 1.0f;
    private float m_EndVal = 0.0f;
    string SceneName = "";
    //------ Fade In 관련 변수들...

    //스크린의 월드 좌표
    public static Vector3 m_SceenWMin = new Vector3(-10.0f, -5.0f, 0.0f);
    public static Vector3 m_SceenWMax = new Vector3(10.0f, 5.0f, 0.0f);
    //스크린의 월드 좌표

    //----- Item 관련 변수들
    ItemCtrl m_HItemCtrl;
    ItemCtrl m_SItemCtrl;
    ItemCtrl m_UItemCtrl;
    ItemCtrl m_MItemCtrl;
    public GameObject m_heartItem = null;
    public GameObject m_SubPlayerItem = null;
    public GameObject m_UltimateItem = null;
    public GameObject m_MagnetItem = null;
    //----- Item 관련 변수들

    //---- UI 관련 변수들
    TutorialPlayerCtrl m_Player;
    public Text m_KillCountTxt;
    public Text m_HeartTxt;
    public Text m_UltTxt;
    //---- UI 관련 변수들

    int Index = 0;

    void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //------스크린의 월드 좌표 구하기
        Vector3 a_ScMin = new Vector3(0.0f, 0.0f, 0.0f); //ScreenViewPort 좌측하단
        m_SceenWMin = Camera.main.ViewportToWorldPoint(a_ScMin);
        //카메라 화면 좌측하단 코너의 월드 좌표

        Vector3 a_ScMax = new Vector3(1.0f, 0.8f, 0.0f); //ScreenViewPort 우측상단
        m_SceenWMax = Camera.main.ViewportToWorldPoint(a_ScMax);
        //카메라 화면 우측상단 코너의 월드 좌표
        //------스크린의 월드 좌표 구하기

        SoundMgr.Instance.PlayBGM("cyberpunk city 2", 1.0f);

        m_Player = GameObject.FindObjectOfType<TutorialPlayerCtrl>();

        //------ 로비로 들어올 때 Fade In 설정 
        m_StVal = 1.0f;
        m_EndVal = 0.0f;
        m_FadeImg.gameObject.SetActive(true);
        m_StartFade = true;
        //------ 로비로 들어올 때 Fade In 설정 

        //--- 아이템 별로 스크립트 불러오기
        m_HItemCtrl = m_heartItem.GetComponent<ItemCtrl>();
        m_SItemCtrl = m_SubPlayerItem.GetComponent<ItemCtrl>();
        m_UItemCtrl = m_UltimateItem.GetComponent<ItemCtrl>();
        m_MItemCtrl = m_MagnetItem.GetComponent<ItemCtrl>();
        //--- 아이템 별로 스크립트 불러오기

        m_DelayTime = m_FixTime;

        if (m_SkipBtn != null)
            m_SkipBtn.onClick.AddListener(() =>
            {
                Time.timeScale = 1.0f;
                SceneOut("LobbyScene");
            });

        if (PanelCloseBtn != null)
            PanelCloseBtn.onClick.AddListener(() =>
            {
                Panel.SetActive(false);
                PanelCloseBtn.gameObject.SetActive(false);
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

                Panel.SetActive(false);

                if (m_ConfigBoxObj == null)
                    m_ConfigBoxObj = Resources.Load("SoundsPanel") as GameObject;

                GameObject a_CfgBoxObj = (GameObject)Instantiate(m_ConfigBoxObj);
                a_CfgBoxObj.transform.SetParent(Canvas_Dialog.transform, false);
                //false 로 해야 로컬 프리랩에 설정된 좌표를 유지한채 차일드로 붙게된다.
                Time.timeScale = 0.0f; //일시정지
            }
        }
        //---------------------------- 환경설정 Dlg 관련 구현 부분

        if (m_DelayTime >= 0.0f)
        {
            m_DelayTime -= Time.deltaTime;

            if (m_DelayTime <= 0.0f)
            {
                m_GameInfoTxt.text = "다 읽으셨으면 X 버튼을 \n 클릭해 주세요";
            }
        }

        TutorialUpdate();
        UIUpdate();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Panel.activeSelf == true)
                return;

            Index++;
            if (Index > 9)
                Index = 9;
            Debug.Log(Index);
        }
    }

    void TutorialUpdate()
    {
        if (PanelCloseBtn.gameObject.activeSelf == true)
            return;

        if (Index == 0)
            m_GameInfoTxt.text = "첫번째 아이템은 목숨 아이템입니다. (Enter)";
        else if (Index == 1)
        {
            m_HItemCtrl.IsTutorial = true;
            m_GameInfoTxt.text = "목숨 아이템은 좌측 상단에 표시됩니다. \n 목숨은 최대 5개입니다(Enter)";
        }
        else if (Index == 2)
        {
            m_GameInfoTxt.text = "두번째 아이템은 소환수 아이템입니다. (Enter)";
        }
        else if (Index == 3)
        {
            m_SItemCtrl.IsTutorial = true;
            m_GameInfoTxt.text = "소환수는 플레이어 주위를 돌면서 총알을 발사합니다 \n 소환수는 최대 4개까지 소환됩니다. (Enter)";
        }
        else if (Index == 4)
        {
            m_GameInfoTxt.text = "세번째 아이템은 궁극기 아이템입니다. \n 궁극기 아이템은 R키를 눌러 사용합니다.(Enter)";
        }
        else if (Index == 5)
        {
            m_UItemCtrl.IsTutorial = true;
            m_GameInfoTxt.text = "최대 5개까지 소유합니다. \n 궁극기는 모든 총알을 맞을수 있고 \n 일반 몬스터를 파괴합니다 (Enter)";
        }
        else if (Index == 6)
        {
            m_GameInfoTxt.text = "네번째 아이템은 자석 아이템입니다. \n 자석 아이템은 아이템을 자동으로 획득합니다. (Enter)";
        }
        else if (Index == 7)
        {
            m_MItemCtrl.IsTutorial = true;
            m_GameInfoTxt.text = "자석 아이템을 두번 획득하게 되면 \n 유도탄이 적용 됩니다. (Enter)";
        }
        else if (Index == 8)
        {
            m_GameInfoTxt.text = "마지막으로 캐릭터 사망시 목숨을  \n 제외한 모든 아이템 효과는 사라집니다 \n 행운을 빌겠습니다.(화살표클릭)";
        }
        else
        {
            m_GameInfoTxt.text = "Good Luck";
        }
    }

    void UIUpdate()
    {
        m_KillCountTxt.text = "KillCount = 0";
        m_HeartTxt.text = "X " + m_Player.m_PlayerHP;
        m_UltTxt.text = "X " + m_Player.m_UltCount;
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
