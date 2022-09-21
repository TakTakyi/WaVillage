using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    public static MonsterSpawn Inst = null;

    public GameObject[] MonPrefab;

    public int m_MonKillCount = 0;  //난이동 계산을 위한 몬스터 킬 카운트
    public float m_SpDelta = 0.0f;         //스폰 주기 계산용 변수
    float m_DiffSpawn = 3.0f;       //난이도에 따른 몬스터 스폰주기 변수
    public float m_ShotDelay = 1.5f;       //난이도에 따른 몬스터 총알 발사 주기

    //--- 아이템 확률 변수들
    public int m_HItemRate = 0;     //목숨 아이템 드랍 확률
    public int m_SPItemRate = 0;    //소환수 아이템 드랍 확률
    public int m_UItemRate = 0;     //궁극기 아이템 드랍 확률
    public int m_MgItemRate = 0;    //자석 아이템 드랍 확률
    public int m_NDItemRate = 0;    //아이템 미드랍 확률
    //--- 아이템 확률 변수들


    void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_SpDelta = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_MonKillCount < 25)
        {
            m_HItemRate = 10;
            m_SPItemRate = 30;
            m_UItemRate = 40;
            m_MgItemRate = 50;
            m_NDItemRate = 100;
            m_DiffSpawn = 3.0f;
            m_ShotDelay = 1.5f;
        }
        else if (m_MonKillCount >= 25 && m_MonKillCount < 50)
        {
            m_HItemRate = 10;
            m_SPItemRate = 20;
            m_UItemRate = 40;
            m_MgItemRate = 50;
            m_NDItemRate = 100;
            m_DiffSpawn = 2.0f;
            m_ShotDelay = 0.8f;
        }
        else if (m_MonKillCount >= 50 && m_MonKillCount < 75)
        {
            //중간 보스 생성
            m_HItemRate = 10;
            m_SPItemRate = 20;
            m_UItemRate = 40;
            m_MgItemRate = 50;
            m_NDItemRate = 100;
            m_DiffSpawn = 1.0f;
            m_ShotDelay = 1.1f;
        }
        else if (m_MonKillCount >= 75 && m_MonKillCount < 100)
        {
            m_HItemRate = 10;
            m_SPItemRate = 30;
            m_UItemRate = 40;
            m_MgItemRate = 50;
            m_NDItemRate = 100;
            m_DiffSpawn = 0.5f;
            m_ShotDelay = 0.5f;
        }
        //최종보스생성

        if (GameMgr.Inst.m_Color.a <= 0.0f)
            MonSpawnFunc();
    }

    void MonSpawnFunc()
    {
        if (m_MonKillCount == 50)
            return;

        if (m_MonKillCount >= 100)
            return;

        m_SpDelta -= Time.deltaTime;
        if (m_SpDelta < 0.0f)
        {
            GameObject go = null;

            int idx = 0;
            if (m_MonKillCount >= 51)
                idx = Random.Range(0, 2);

            go = Instantiate(MonPrefab[idx]) as GameObject;  //몬스터 스폰

            float py = Random.Range(-3.0f, 2.0f);
            go.transform.position =
                new Vector3(GameMgr.m_SceenWMax.x + 2.0f, py, -1);


            m_SpDelta = m_DiffSpawn;
        }
    }
}
