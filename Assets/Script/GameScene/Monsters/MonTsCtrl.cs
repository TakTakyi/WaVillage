using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonType
{
    MonTs,
    MonBt,
    MiddleBoss,
    EndBoss
}

public enum BossAttState
{
    BS_SLEEP,       //InActive
    BS_NORMAL_ACT,  //Active
    BS_FEVER01_ACT, //피버1타입
};

public class MonTsCtrl : MonoBehaviour
{
    PlayerCtrl m_Player;
    public MonType m_MonType = MonType.MonTs;


    //--- 보스의 공격 패턴 관련 변수
    public BossAttState m_BossState = BossAttState.BS_SLEEP;
    public int m_ShootCount = 0;
    //--- 보스의 공격 패턴 관련 변수

    public float m_MaxHP = 10.0f;      //최대 체력치
    public float m_CurHP = 10.0f;      //현재 체력
    float enemy_Att = 20.0f;     //공격력

    Vector3 m_CurPos;       //위치 계산용 변수
    Vector3 m_SpawnPos;     //스폰 위치
    Vector3 HalfSize = Vector3.zero;
    Vector3 m_DirVec;       //이동 방향
    float m_Speed = 4.0f;   //이동 속도
    float m_CacPosY = 0.0f;
    float m_Rand_Y = 0.0f;
    float a_LmtBdBottom = 0;

    //---------- 총알 발사 관련 변수 선언
    public GameObject m_BulletObj = null;
    GameObject a_NewObj = null;
    BulletCtrl a_BulletSC = null;
    float shoot_Time = 0.0f;
    float shoot_Delay = 1.5f;
    float BulletMvSpeed = 10.0f; //총알 이동 속도
    //---------- 총알 발사 관련 변수 선언

    Color m_color = Color.clear;

    // Start is called before the first frame update
    void Start()
    {
        m_Player = GameObject.FindObjectOfType<PlayerCtrl>();

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

        m_SpawnPos = this.transform.position;
        m_Rand_Y = Random.Range(0.2f, 2.6f);  //Sin함수의 랜덤 진폭

        if (m_MonType == MonType.MonTs)
        {
            m_MaxHP = 10;
            if (MonsterSpawn.Inst.m_MonKillCount >= 25)
                m_MaxHP = 20;
            if (MonsterSpawn.Inst.m_MonKillCount >= 75)
                m_MaxHP = 30;

            m_CurHP = m_MaxHP;
        }

        if (m_MonType == MonType.MonBt)
        {
            m_MaxHP = 30;
           
            if (MonsterSpawn.Inst.m_MonKillCount >= 75)
                m_MaxHP = 40;

            m_CurHP = m_MaxHP;
        }

        if (m_MonType == MonType.MiddleBoss)
        {
            m_MaxHP = 1000;
            m_CurHP = m_MaxHP;
        }

        if (m_MonType == MonType.EndBoss)
        {
            m_MaxHP = 10000;
            m_CurHP = m_MaxHP;
        }
    }

    // Update is called once per frame
    void Update()
    {
        shoot_Delay = MonsterSpawn.Inst.m_ShotDelay;

        if (m_MonType == MonType.MonTs)
            MonTS_AI_Update();
        else if (m_MonType == MonType.MonBt)
            MonBt_AI_Update();
        else if (m_MonType == MonType.MiddleBoss)
            MiddleBoss_AI_Update();
        else if (m_MonType == MonType.EndBoss)
            EndBoss_AI_Update();

        if (this.transform.position.x < GameMgr.m_SceenWMin.x - 2.0f)
            Destroy(gameObject); //왼쪽 화면을 벗어나는 즉시 제거
    }

    void MonTS_AI_Update()
    {
        m_CurPos = this.transform.position;
        a_LmtBdBottom = GameMgr.m_SceenWMax.y - HalfSize.y;

        m_CurPos.x += (-1.0f * Time.deltaTime * m_Speed);
        m_CacPosY += Time.deltaTime * (m_Speed / 2.2f);
        m_CurPos.y = m_SpawnPos.y + Mathf.Sin(m_CacPosY) * m_Rand_Y;

        if (a_LmtBdBottom < m_CurPos.y)
        {
            m_Rand_Y = m_Rand_Y / 2.0f;
            m_CurPos.y = m_SpawnPos.y + Mathf.Sin(m_CacPosY) * m_Rand_Y;
        }

        this.transform.position = m_CurPos;

        //----- 총알 발사 
        if (m_BulletObj == null)
            return;

        shoot_Time -= Time.deltaTime;
        if (shoot_Time <= 0.0f)
        {
            a_NewObj = (GameObject)Instantiate(m_BulletObj);
            //오브젝트의 클론(복사체) 생성 함수   
            a_BulletSC = a_NewObj.GetComponent<BulletCtrl>();
            a_BulletSC.m_BulletType = BulletType.Enemy;
            a_BulletSC.BulletSpawn(this.transform.position, Vector3.left,
                                    BulletMvSpeed, enemy_Att);

            shoot_Time = shoot_Delay;
        }
        //----- 총알 발사 
    }

    void MonBt_AI_Update()
    {
        if (m_Player == null)
            return;

        m_CurPos = this.transform.position;
        Vector3 a_CacVec = m_Player.transform.position - this.transform.position;

        m_DirVec = a_CacVec;  //몬스터의 이동 방향 벡터
        if (a_CacVec.x < -3.5f) //미사일이 주인공보다 우측 3.5m 전방에 있을 때만...
            m_DirVec.y = 0.0f;

        m_DirVec.Normalize();
        m_DirVec.x = -1.0f;
        m_DirVec.z = 0.0f;

        m_CurPos = m_CurPos + (m_DirVec * Time.deltaTime * m_Speed);

        this.transform.position = m_CurPos;
    }// void Missile_AI_Update()

    void MiddleBoss_AI_Update()
    {
        if (m_Player == null)
            return;

        if (MonsterSpawn.Inst.m_MonKillCount >= 50 && m_BossState == BossAttState.BS_SLEEP)
        {
            GameMgr.Inst.BossHpBar.SetActive(true);
            this.transform.position += Vector3.left * Time.deltaTime * m_Speed;
            m_CurPos = this.transform.position;

            if (m_CurPos.x <= 6.0f)
            {
                m_CurPos.x = 6.0f;
                this.transform.position = m_CurPos;
                m_BossState = BossAttState.BS_FEVER01_ACT;
            }
        }

        //-- 공격패턴 만들기
        //----- 총알 발사 
        if (m_BulletObj == null)
            return;

        if (m_BossState == BossAttState.BS_NORMAL_ACT) //기본 공격
        {
            shoot_Time = shoot_Time - Time.deltaTime;
            if (shoot_Time <= 0.0f)
            {
                Vector3 a_TargetV =
                        m_Player.transform.position - this.transform.position;
                a_TargetV.Normalize();
                a_NewObj = (GameObject)Instantiate(m_BulletObj);
                //오브젝트의 클론(복사체) 생성 함수   
                a_BulletSC = a_NewObj.GetComponent<BulletCtrl>();
                a_BulletSC.m_BulletType = BulletType.Enemy;
                a_BulletSC.BulletSpawn(this.transform.position,
                                        a_TargetV, BulletMvSpeed);
                a_BulletSC.isTaget = true;
                float angle = Mathf.Atan2(a_TargetV.y, a_TargetV.x) * Mathf.Rad2Deg;
                angle += 180.0f;
                a_NewObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);

                m_ShootCount++;
                if (m_ShootCount < 7) //일반공격 7번까지의 공격 주기 
                    shoot_Time = 0.7f;
                else  //궁극기로 넘어갈 때 1.5초 딜레이 후에
                {
                    m_ShootCount = 0;
                    shoot_Time = 2.0f;
                    m_BossState = BossAttState.BS_FEVER01_ACT;
                }
            }//if (shoot_Time <= 0.0f)
        }
        else if (m_BossState == BossAttState.BS_FEVER01_ACT) //피버 공격
        {
            shoot_Time = shoot_Time - Time.deltaTime;
            if (shoot_Time <= 0.0f)
            {
                float Radius = 100.0f;
                Vector3 a_TargetV = Vector3.zero;
                for (float Angle = 0.0f; Angle < 360.0f; Angle += 15.0f)
                {
                    a_TargetV.x = Radius * Mathf.Cos(Angle * Mathf.Deg2Rad);
                    a_TargetV.y = Radius * Mathf.Sin(Angle * Mathf.Deg2Rad);
                    a_TargetV.Normalize();
                    a_NewObj = (GameObject)Instantiate(m_BulletObj);
                    //오브젝트의 클론(복사체) 생성 함수   
                    a_BulletSC = a_NewObj.GetComponent<BulletCtrl>();
                    a_BulletSC.m_BulletType = BulletType.Enemy;
                    a_BulletSC.BulletSpawn(this.transform.position,
                                            a_TargetV, BulletMvSpeed);
                    a_BulletSC.isTaget = true;
                    float angle = Mathf.Atan2(a_TargetV.y, a_TargetV.x) * Mathf.Rad2Deg;
                    angle += 180.0f;
                    a_NewObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
                    //a_NewObj.transform.rotation = 
                    //                  Quaternion.AngleAxis(angle, Vector3.forward);
                }//for (Angle = 0; Angle < 360.0f; Angle += 15)

                m_ShootCount++;
                if (m_ShootCount < 3) //궁극기 3번까지의 공격 주기 
                    shoot_Time = 1.0f;
                else  //궁극기로 넘어갈 때 1.5초 딜레이 후에
                {
                    m_ShootCount = 0;
                    shoot_Time = 1.5f;
                    m_BossState = BossAttState.BS_NORMAL_ACT;
                }

            }//if (shoot_Time <= 0.0f)
        }//if (m_BossState == BossAttState.BS_FEVER01_ACT) //피버 공격
        //-- 공격패턴 만들기
    }

    void EndBoss_AI_Update()
    {
        if (m_Player == null)
            return;

        if (MonsterSpawn.Inst.m_MonKillCount >= 100 && m_BossState == BossAttState.BS_SLEEP)
        {
            GameMgr.Inst.BossHpBar.SetActive(true);
            GameMgr.Inst.m_IsOnEndBoss = true;
            this.transform.position += Vector3.left * Time.deltaTime * m_Speed;
            m_CurPos = this.transform.position;

            if (m_CurPos.x <= 5.5f)
            {
                m_CurPos.x = 5.5f;
                this.transform.position = m_CurPos;
                m_BossState = BossAttState.BS_FEVER01_ACT;
            }
        }

        //-- 공격패턴 만들기
        //----- 총알 발사 
        if (m_BulletObj == null)
            return;

        if (m_BossState == BossAttState.BS_NORMAL_ACT) //기본 공격
        {
            shoot_Time = shoot_Time - Time.deltaTime;
            if (shoot_Time <= 0.0f)
            {
                Vector3 a_TargetV =
                        m_Player.transform.position - this.transform.position;
                a_TargetV.Normalize();
                a_NewObj = (GameObject)Instantiate(m_BulletObj);
                //오브젝트의 클론(복사체) 생성 함수   
                a_BulletSC = a_NewObj.GetComponent<BulletCtrl>();
                a_BulletSC.m_BulletType = BulletType.Enemy;
                a_BulletSC.BulletSpawn(this.transform.position,
                                        a_TargetV, BulletMvSpeed);
                a_BulletSC.isTaget = true;
                float angle = Mathf.Atan2(a_TargetV.y, a_TargetV.x) * Mathf.Rad2Deg;
                angle += 180.0f;
                a_NewObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);

                m_ShootCount++;
                if (m_ShootCount < 7) //일반공격 7번까지의 공격 주기 
                    shoot_Time = 0.7f;
                else  //궁극기로 넘어갈 때 1.5초 딜레이 후에
                {
                    m_ShootCount = 0;
                    shoot_Time = 2.0f;
                    m_BossState = BossAttState.BS_FEVER01_ACT;
                }
            }//if (shoot_Time <= 0.0f)
        }
        else if (m_BossState == BossAttState.BS_FEVER01_ACT) //피버 공격
        {
            shoot_Time = shoot_Time - Time.deltaTime;
            if (shoot_Time <= 0.0f)
            {
                float Radius = 100.0f;
                Vector3 a_TargetV = Vector3.zero;
                for (float Angle = 0.0f; Angle < 360.0f; Angle += 15.0f)
                {
                    a_TargetV.x = Radius * Mathf.Cos(Angle * Mathf.Deg2Rad);
                    a_TargetV.y = Radius * Mathf.Sin(Angle * Mathf.Deg2Rad);
                    a_TargetV.Normalize();
                    a_NewObj = (GameObject)Instantiate(m_BulletObj);
                    //오브젝트의 클론(복사체) 생성 함수   
                    a_BulletSC = a_NewObj.GetComponent<BulletCtrl>();
                    a_BulletSC.m_BulletType = BulletType.Enemy;
                    a_BulletSC.BulletSpawn(this.transform.position,
                                            a_TargetV, BulletMvSpeed);
                    a_BulletSC.isTaget = true;
                    float angle = Mathf.Atan2(a_TargetV.y, a_TargetV.x) * Mathf.Rad2Deg;
                    angle += 180.0f;
                    a_NewObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
                    //a_NewObj.transform.rotation = 
                    //                  Quaternion.AngleAxis(angle, Vector3.forward);
                }//for (Angle = 0; Angle < 360.0f; Angle += 15)

                m_ShootCount++;
                if (m_ShootCount < 3) //궁극기 3번까지의 공격 주기 
                    shoot_Time = 1.0f;
                else  //궁극기로 넘어갈 때 1.5초 딜레이 후에
                {
                    m_ShootCount = 0;
                    shoot_Time = 1.5f;
                    m_BossState = BossAttState.BS_NORMAL_ACT;
                }

            }//if (shoot_Time <= 0.0f)
        }//if (m_BossState == BossAttState.BS_FEVER01_ACT) //피버 공격
        //-- 공격패턴 만들기
    }

    public void TakeDamage(float a_Value)
    {
        if (this.gameObject.transform.position.x >= GameMgr.m_SceenWMax.x - 1.0f)
            return;

        if (m_MonType == MonType.MiddleBoss && m_BossState == BossAttState.BS_SLEEP)
            return;

        if (m_MonType == MonType.EndBoss && m_BossState == BossAttState.BS_SLEEP)
            return;

        if (m_CurHP <= 0.0f) //이렇게 하면 사망 처리는 한번만 될 것이다.
            return;

        m_CurHP -= a_Value;

        if (m_CurHP <= 0.0f)
        {
            MonsterSpawn.Inst.m_MonKillCount++;

            if (m_MonType == MonType.MonTs)
            {
                DropItem();
            }

            if (m_MonType == MonType.MonBt)
            {
                DropItem();
            }

            if (m_MonType == MonType.MiddleBoss)
            {

                GameMgr.Inst.BossHpBar.SetActive(false);
            }

            if (m_MonType == MonType.EndBoss)
            {
                GameMgr.Inst.BossHpBar.SetActive(false);
                GameMgr.Inst.m_IsOnEndBoss = false;
                GameMgr.Inst.GameOverTxt.text = "Game Clear";
                GameMgr.Inst.StopVideoBtn.gameObject.SetActive(false);
                GameMgr.Inst.m_Meme.color = m_color;
                GameMgr.Inst.GameOverPanel.SetActive(true);
                GameMgr.Inst.IsPlayVideo = false;
            }
            
            Destroy(this.gameObject);
        }
    }

    public void DropItem()
    {
        //---- 보상으로 아이템 드롭 
        int dice = Random.Range(0, 101);     //0 ~ 100 랜덤값 발생
        if (dice < MonsterSpawn.Inst.m_HItemRate)  
        {
            if (GameMgr.m_heartItem != null)
            {
                GameObject a_CoinObj = (GameObject)Instantiate(GameMgr.m_heartItem);
                a_CoinObj.transform.position = this.transform.position;
                ItemCtrl a_CoinCtrl = a_CoinObj.GetComponent<ItemCtrl>();
                if (a_CoinCtrl != null)
                {
                    a_CoinCtrl.m_RefPlayer = m_Player;
                    if (m_Player.m_MagCount >= 1)
                    {
                        a_CoinCtrl.isMagnet = true;
                    }
                }
                Destroy(a_CoinObj, 10.0f);  //10초내에 먹어야 한다.
            }
        }
        else if (dice >= MonsterSpawn.Inst.m_HItemRate && dice < MonsterSpawn.Inst.m_SPItemRate)
        {
            if (GameMgr.m_SubPlayerItem != null)
            {
                GameObject a_CoinObj = (GameObject)Instantiate(GameMgr.m_SubPlayerItem);
                a_CoinObj.transform.position = this.transform.position;
                ItemCtrl a_CoinCtrl = a_CoinObj.GetComponent<ItemCtrl>();
                if (a_CoinCtrl != null)
                {
                    a_CoinCtrl.m_RefPlayer = m_Player;
                    if (m_Player.m_MagCount >= 1)
                    {
                        a_CoinCtrl.isMagnet = true;
                    }
                }
                Destroy(a_CoinObj, 10.0f);  //10초내에 먹어야 한다.
            }
        }
        else if (dice >= MonsterSpawn.Inst.m_SPItemRate && dice < MonsterSpawn.Inst.m_UItemRate)
        {
            if (GameMgr.m_UltimateItem != null)
            {
                GameObject a_CoinObj = (GameObject)Instantiate(GameMgr.m_UltimateItem);
                a_CoinObj.transform.position = this.transform.position;
                ItemCtrl a_CoinCtrl = a_CoinObj.GetComponent<ItemCtrl>();
                if (a_CoinCtrl != null)
                {
                    a_CoinCtrl.m_RefPlayer = m_Player;
                    if (m_Player.m_MagCount >= 1)
                    {
                        a_CoinCtrl.isMagnet = true;
                    }
                }
                Destroy(a_CoinObj, 10.0f);  //10초내에 먹어야 한다.
            }
        }
        else if (dice >= MonsterSpawn.Inst.m_UItemRate && dice < MonsterSpawn.Inst.m_MgItemRate)
        {
            if (GameMgr.m_MagnetItem != null)
            {
                GameObject a_CoinObj = (GameObject)Instantiate(GameMgr.m_MagnetItem);
                a_CoinObj.transform.position = this.transform.position;
                ItemCtrl a_CoinCtrl = a_CoinObj.GetComponent<ItemCtrl>();
                if (a_CoinCtrl != null)
                {
                    a_CoinCtrl.m_RefPlayer = m_Player;
                    if (m_Player.m_MagCount >= 1)
                    {
                        a_CoinCtrl.isMagnet = true;
                    }
                }

                Destroy(a_CoinObj, 10.0f);  //10초내에 먹어야 한다.
            }
        }
        //---- 보상으로 아이템 드롭 
    }

    void OnTriggerEnter2D(Collider2D col)
    { //몬스터에 뭔가 충돌 되었을 때 발생되는 함수
        if (col.tag == "PlayerBullet")
        {
            TakeDamage(10.0f);
            Destroy(col.gameObject);
        }
    }
}
