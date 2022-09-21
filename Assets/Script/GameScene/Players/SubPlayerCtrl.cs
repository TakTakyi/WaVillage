using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPlayerCtrl : MonoBehaviour
{
    PlayerCtrl m_RefPlayer = null;

    float angle = 0.0f;
    float radius = 1.0f;
    float speed = 100.0f;

    GameObject parent_Obj = null;
    Vector3 parent_Pos = Vector3.zero;

    //----- 공격 관련 변수
    public GameObject m_BulletObj = null;
    public float m_AttSpeed;
    float m_CacAtTick = 0.0f;

    GameObject a_NewObj = null;
    BulletCtrl a_BulletSC = null;

    public bool IsFire = false;
    public bool homing_OnOff = false;
    //----- 공격 관련 변수

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_RefPlayer != null)
        {
            //homing_OnOff = m_RefPlayer.homing_OnOff;
        }

        angle += Time.deltaTime * speed;
        if (360.0f < angle)
            angle -= 360.0f;

        parent_Pos = parent_Obj.transform.position;

        transform.position = parent_Pos +
            new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad),
            radius * Mathf.Sin(angle * Mathf.Deg2Rad));

        if (IsFire == true)
        {
            Attack_Update();
        }
    }

    public void SubHeroSpwan(GameObject a_Paren, float a_Angle)
    {
        parent_Obj = a_Paren;
        m_RefPlayer = a_Paren.GetComponent<PlayerCtrl>();
        angle = a_Angle;
    }

    void Attack_Update()
    {
        if (0.0f < m_CacAtTick)
            m_CacAtTick = m_CacAtTick - Time.deltaTime;

        if (m_CacAtTick <= 0.0f)
        {
            a_NewObj = (GameObject)Instantiate(m_BulletObj);
            a_BulletSC = a_NewObj.GetComponent<BulletCtrl>();
            a_BulletSC.m_BulletType = BulletType.Player;
            a_BulletSC.BulletSpawn(this.transform.position, Vector3.right);
            a_BulletSC.homing_OnOff = homing_OnOff;

            m_CacAtTick = m_AttSpeed;
        }//if (m_CacAtTick <= 0.0f)

    }//void Attack_Update()
}
