using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerCtrl : MonoBehaviour
{
    public GameObject m_Player;
    public int m_PlayerHP = 1;

    //---------- 키보드 입력값 변수 선언
    private float h = 0.0f;
    private float v = 0.0f;

    private float moveSpeed = 7.0f;
    Vector3 moveDir = Vector3.zero;
    //---------- 키보드 입력값 변수 선언

    //----------------주인공이 지형 밖으로 나갈 수 없도록 막기 위한 변수 
    Vector3 HalfSize = Vector3.zero;
    Vector3 m_CacCurPos = Vector3.zero;

    float a_LmtBdLeft = 0;
    float a_LmtBdTop = 0;
    float a_LmtBdRight = 0;
    float a_LmtBdBottom = 0;
    //----------------주인공이 지형 밖으로 나갈 수 없도록 막기 위한 변수 

    //---------- 총알 발사 관련 변수 선언
    public GameObject m_BulletObj = null;
    float m_AttSpeed = 0.2f;   //주인공 공속
    float m_CacAtTick = 0.0f;   //기관총 발사 틱 만들기....
    GameObject a_NewObj = null;
    BulletCtrl a_BulletSC = null;
    //---------- 총알 발사 관련 변수 선언

    //------ Sub Hero
    public int sub_Count = 4;
    int isIndexcount = 0;
    public GameObject sub_Obj = null;
    public GameObject sub_Parent = null;
    public GameObject[] sub_PlayerObj = new GameObject[4];
    public SubPlayerCtrl[] m_Sub = new SubPlayerCtrl[4];
    //------ Sub Hero

    //---- 아이템 관련 변수
    ItemCtrl m_ItemCtrl = null;
    public int m_UltCount = 1;
    bool m_UseUltOnOff = true;
    float m_UltCoolTime = 3.0f;
    public GameObject m_Ult_Obj = null;
    public int m_MagCount = 0;
    public bool homing_OnOff = false;
    //---- 아이템 관련 변수

    //---- 데미지 연출 관련 변수
    Animator _animator;
    float m_AliveTime = 2.0f;
    bool isNoDie = false;
    //---- 데미지 연출 관련 변수

    //시작연출관련
    bool IsPlay = false;

    // Start is called before the first frame update
    void Start()
    {
        //------ 캐릭터의 가로 반사이즈, 세로 반사이즈 구하기
        //월드에 그려진 스프라이트 사이즈 얻어오기
        MeshRenderer sprRend = gameObject.GetComponentInChildren<MeshRenderer>();
        //sprRend.transform.localScale <-- 스프라이트는 이걸로 사이즈를 구하면 안된다.
        HalfSize.x = sprRend.bounds.size.x / 2.0f - 0.23f;
        //나중에 주인공 캐릭터 외형을 바꾸면 다시 계산해 준다.
        HalfSize.y = sprRend.bounds.size.y / 2.0f - 0.05f;
        //여백이 커서 조금 줄여 주자
        HalfSize.z = 1.0f;
        //------ 캐릭터의 가로 반사이즈, 세로 반사이즈 구하기

        SubPlayerItem();
        for (int ii = 0; ii < sub_Count; ii++)
        {
            sub_PlayerObj[ii].GetComponent<SpriteRenderer>().enabled = false;
        }

        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlay == false)
            StartChar();

        if (IsPlay == true)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            if (h != 0.0f || v != 0.0f)
            {
                moveDir = new Vector3(h, v, 0);
                if (1.0f < moveDir.magnitude)
                    moveDir.Normalize();
                transform.position +=
                    moveDir * moveSpeed * Time.deltaTime;
            }//if (h != 0.0f || v != 0.0f)

            m_Player.transform.Rotate(0, 0, 400.0f * Time.deltaTime, Space.Self);

            LimitMove();
            FireUpdate();

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (m_UltCount >= 1 && m_UseUltOnOff == true)
                    UseUltimate();

                if (m_UltCount <= 0)
                    m_UltCount = 0;
            }

            if (m_UltCoolTime >= 0.0f && m_UseUltOnOff == false)
            {
                m_UltCoolTime -= Time.deltaTime;
                if (m_UltCoolTime <= 0.0f)
                {
                    m_UltCoolTime = 3.0f;
                    m_UseUltOnOff = true;
                }
            }

            if (isNoDie == true && m_AliveTime >= 0.0f)
            {
                m_AliveTime -= Time.deltaTime;
                //Debug.Log(m_AliveTime);
                if (m_AliveTime <= 0.0f)
                {
                    isNoDie = false;
                    m_AliveTime = 2.0f;
                }
            }
        }
    }

    //시작 연출
    void StartChar()
    {
        if (IsPlay == true)
            return;

        if (TutorialMgr.Inst.Panel.activeSelf == false)
        {
            this.transform.position += Vector3.right * Time.deltaTime * moveSpeed;

            if (this.transform.position.x >= 0.0f)
            {
                IsPlay = true;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "EnemyBullet")
        {
            //목숨 하나 차감 및 초기화
            if (isNoDie == false)
            {
                m_PlayerHP -= 1;
                ResetState();
                Nodie();
            }

            Destroy(coll.gameObject);
        }
        else if (coll.tag == "MonsterEnemy")
        {
            //목숨 하나 차감 및 초기화/몬스터 파괴
            if (isNoDie == false)
            {
                m_PlayerHP -= 1;
                ResetState();
                Nodie();
            }

            Destroy(coll.gameObject);
        }
        else if (coll.tag == "MiddleBoss")
        {
            if (isNoDie == false)
            {
                m_PlayerHP -= 1;
                ResetState();
                Nodie();
            }
        }
        else if (coll.tag == "Item")
        {
            //아이템 적용
            m_ItemCtrl = coll.gameObject.GetComponent<ItemCtrl>();
            ItemUpdate(coll);
            Destroy(coll.gameObject);
        }

        if (m_PlayerHP <= 0)
        {
            Time.timeScale = 0.0f;
            GameMgr.Inst.GameOverPanel.SetActive(true);
            GameMgr.Inst.IsPlayVideo = true;
            Destroy(this.gameObject);
        }
    }

    void LimitMove()
    {
        m_CacCurPos = transform.position;

        a_LmtBdLeft = TutorialMgr.m_SceenWMin.x + HalfSize.x;
        a_LmtBdTop = TutorialMgr.m_SceenWMin.y + HalfSize.y;
        a_LmtBdRight = TutorialMgr.m_SceenWMax.x - HalfSize.x;
        a_LmtBdBottom = TutorialMgr.m_SceenWMax.y - HalfSize.y;

        if (m_CacCurPos.x < a_LmtBdLeft)
            m_CacCurPos.x = a_LmtBdLeft;

        if (a_LmtBdRight < m_CacCurPos.x)
            m_CacCurPos.x = a_LmtBdRight;

        if (m_CacCurPos.y < a_LmtBdTop)
            m_CacCurPos.y = a_LmtBdTop;

        if (a_LmtBdBottom < m_CacCurPos.y)
            m_CacCurPos.y = a_LmtBdBottom;

        transform.position = m_CacCurPos;
    }

    void FireUpdate()
    { //--------------- 총알 발사 코드
        if (0.0f < m_CacAtTick)
            m_CacAtTick = m_CacAtTick - Time.deltaTime;

        if (m_CacAtTick <= 0.0f)
        {
            a_NewObj = (GameObject)Instantiate(m_BulletObj);
            //오브젝트의 클론(복사체) 생성 함수   
            a_BulletSC = a_NewObj.GetComponent<BulletCtrl>();
            a_BulletSC.m_BulletType = BulletType.Player;
            a_BulletSC.BulletSpawn(this.transform.position, Vector3.right);
            a_BulletSC.homing_OnOff = homing_OnOff;

            m_CacAtTick = m_AttSpeed;
        }// if (m_CacAtTick <= 0.0f)
    }//void FireUpdate()

    void ItemUpdate(Collider2D coll)
    {
        if (m_ItemCtrl.m_ItemType == ItemType.Heart)
        {
            m_PlayerHP++;
            if (m_PlayerHP > 10)
            {
                m_PlayerHP = 10;
            }
        }
        else if (m_ItemCtrl.m_ItemType == ItemType.SubPlayer)
        {
            if (isIndexcount >= 4)
                Destroy(coll.gameObject);

            if (isIndexcount < 4)
            {
                sub_PlayerObj[isIndexcount].GetComponent<SpriteRenderer>().enabled = true;
                m_Sub[isIndexcount].m_AttSpeed = m_AttSpeed;
                m_Sub[isIndexcount].IsFire = true;

                isIndexcount++;
            }

        }
        else if (m_ItemCtrl.m_ItemType == ItemType.Ultimate)
        {
            //궁극기 최대 값 5
            m_UltCount++;
            if (m_UltCount >= 6)
                m_UltCount = 5;
        }
        else if (m_ItemCtrl.m_ItemType == ItemType.Magnet)
        {
            //자석 최대 값 2
            //1번 먹으면 아이템 자석으로 끌어들임
            //2번 먹으면 호밍샷
            m_MagCount++;
            if (m_MagCount > 2)
                m_MagCount = 2;

            if (m_MagCount == 2)
            {
                homing_OnOff = true;
                for (int ii = 0; ii < isIndexcount; ii++)
                {
                    m_Sub[ii].homing_OnOff = homing_OnOff;
                }
            }

        }
    }

    public void SubPlayerItem()
    {
        for (int ii = 0; ii < sub_Count; ii++)
        {
            GameObject obj = Instantiate(sub_Obj) as GameObject;
            obj.transform.SetParent(sub_Parent.transform);
            SubPlayerCtrl sub = obj.GetComponent<SubPlayerCtrl>();
            sub.SubHeroSpwan(sub_Parent, (360 / sub_Count) * ii);
            sub_PlayerObj[ii] = obj;
            m_Sub[ii] = sub_PlayerObj[ii].GetComponent<SubPlayerCtrl>();
        }
    }

    public void UseUltimate()
    {
        m_UltCount--;
        GameObject obj = Instantiate(m_Ult_Obj) as GameObject;
        obj.transform.position =
            new Vector3(GameMgr.m_SceenWMin.x - 1.0f, 0.0f, 0.0f);
        m_UseUltOnOff = false;
    }

    void ResetState()
    {
        if (isIndexcount > 0)
        {
            for (int ii = 0; ii < isIndexcount; ii++)
            {
                sub_PlayerObj[ii].GetComponent<SpriteRenderer>().enabled = false;
                m_Sub[ii].IsFire = false;
                m_Sub[ii].homing_OnOff = false;
            }
        }

        if (m_ItemCtrl != null)
        {
            m_ItemCtrl.isMagnet = false;
        }

        homing_OnOff = false;
        isIndexcount = 0;
        m_UltCount = 0;
        m_MagCount = 0;
    }

    void Nodie()
    {
        isNoDie = true;
        _animator.SetTrigger("Die");
    }
}
