using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Player,
    Enemy
}

public class BulletCtrl : MonoBehaviour
{
    //Bullet Type 변수
    public BulletType m_BulletType = BulletType.Player;

    //--이동 관련 변수들
    Vector3 m_DirTgVec = Vector3.right; //날아 가야할 방향 계산용 변수
    Vector3 a_StartPos = Vector3.zero;  //시작 위치 계산용 변수
    private float m_MoveSpeed = 1.0f;  //한플레임당 이동 시키고 싶은 거리 (이동속도)
    private float m_RotSpeed = 400.0f;   //객체 회전 속도
    //--이동 관련 변수들
    [HideInInspector] public float bullet_Att = 20.0f;

    //--유도탄 변수 
    [HideInInspector] public bool homing_OnOff = false;   //유도탄 OnOff 
    [HideInInspector] public bool isTaget = false;
    //한번이라도 타겟이 잡힌 적인 있는지 확인 하는 변수
    [HideInInspector] public GameObject taget_Obj = null; //타겟 참조 변수
    Vector3 m_DesiredDir;  //타겟을 향하는 방향 변수
    //--유도탄 변수 

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 10.0f); //안전장치
    }

    // Update is called once per frame
    void Update()
    {
        if (m_BulletType == BulletType.Player)
        {
            if (homing_OnOff == true) //유도탄인 경우...
            {
                if (taget_Obj == null && isTaget == false) //타겟이 없으면...
                {
                    FindEnemy();
                }

                if (taget_Obj != null) //타겟이 존재하면...
                    BulletHoming();
                else //타겟이 사망했다면...
                {
                    transform.Rotate(0, 0, -m_RotSpeed * Time.deltaTime, Space.Self);
                    transform.position += m_DirTgVec * Time.deltaTime * m_MoveSpeed;
                }
            }
            else
            {
                //총알 회전하기
                transform.Rotate(0, 0, -m_RotSpeed * Time.deltaTime, Space.Self);
                transform.position += m_DirTgVec * Time.deltaTime * m_MoveSpeed;
            }
        }
        else if (m_BulletType == BulletType.Enemy)
        {
            transform.position += m_DirTgVec * Time.deltaTime * m_MoveSpeed;
        }

        BulletLimit();
    }

    void BulletLimit()
    {
        //if (GameMgr.m_SceenWMax.x + 0.5f < this.transform.position.x)
        // 총알 화면 밖으로 나가면 제거해 주기
        if ((GameMgr.m_SceenWMax.x + 0.5f < this.transform.position.x) ||
             (this.transform.position.x < GameMgr.m_SceenWMin.x - 1.0f) ||
             (GameMgr.m_SceenWMax.y + 1.0f < this.transform.position.y) ||
             (this.transform.position.y < GameMgr.m_SceenWMin.y - 1.0f))
        {
            Destroy(gameObject);
        }

        if (this.transform.position.x < GameMgr.m_SceenWMin.x - 1.0f)
        {
            Destroy(gameObject);
        }
    }

    public void BulletSpawn(Vector3 a_OwnPos, Vector3 a_DirTgVec,
                        float a_MvSpeed = 15.0f, float att = 20.0f)
    {
        m_DirTgVec = a_DirTgVec;
        a_StartPos = a_OwnPos + (m_DirTgVec * 0.5f);
        transform.position = new Vector3(a_StartPos.x,
                                         a_StartPos.y, 0.0f);

        m_MoveSpeed = a_MvSpeed;
        bullet_Att = att;
    }

    void FindEnemy()
    {
        GameObject[] a_EnemyList = GameObject.FindGameObjectsWithTag("MonsterEnemy");

        if (a_EnemyList.Length <= 0) //그냥 먼 어딘 가를 추적하게 한다.
            return;

        GameObject a_Find_Mon = null;
        float a_CacDist = 0.0f;
        Vector3 a_CacVec = Vector3.zero;
        for (int i = 0; i < a_EnemyList.Length; ++i)
        {
            a_CacVec = a_EnemyList[i].transform.position - transform.position;
            a_CacVec.z = -1.0f;
            a_CacDist = a_CacVec.magnitude;

            if (4.0f < a_CacDist) 
                continue;

            a_Find_Mon = a_EnemyList[i].gameObject;
            break;
        }//for (int i = 0; i < a_EnemyList.Length; ++i)

        taget_Obj = a_Find_Mon;
        if (taget_Obj != null)
            isTaget = true;
    }//void FindEnemy()

    void BulletHoming()
    {
        m_DesiredDir = taget_Obj.transform.position - transform.position;
        m_DesiredDir.z = -1.0f;
        m_DesiredDir.Normalize();

        //적을 향해 회전 이동하는 방법
        float angle = Mathf.Atan2(m_DesiredDir.y, m_DesiredDir.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = angleAxis;
        m_DirTgVec = transform.right;
        transform.Rotate(0, 0, -m_RotSpeed * Time.deltaTime, Space.Self);
        transform.Translate(Vector3.right * m_MoveSpeed * Time.deltaTime);
    }
}
