using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateCtrl : MonoBehaviour
{
    float m_MoveSpeed = 7.0f;
    string m_name = "";

    // Start is called before the first frame update
    void Start()
    {
        m_name = this.gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_name.Contains("MiMiUltmate"))
        {
            transform.Rotate(0, 0, -400.0f * Time.deltaTime, Space.Self);
        }

        transform.position += Vector3.right * Time.deltaTime * m_MoveSpeed;

        if ((GameMgr.m_SceenWMax.x + 0.5f < this.transform.position.x))
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D col) //뭔가 충돌 되었을 때 발생되는 함수
    {
        if (col.tag == "MonsterEnemy")
        {
            MonTsCtrl a_Enemy = col.gameObject.GetComponent<MonTsCtrl>();
            if (a_Enemy != null)
                a_Enemy.TakeDamage(700);
        }

        if (col.tag == "MiddleBoss")
        {
            MonTsCtrl a_Enemy = col.gameObject.GetComponent<MonTsCtrl>();
            if (a_Enemy != null)
                a_Enemy.TakeDamage(200);

            Destroy(this.gameObject);
        }

        if (col.tag == "EndBoss")
        {
            MonTsCtrl a_Enemy = col.gameObject.GetComponent<MonTsCtrl>();
            if (a_Enemy != null)
                a_Enemy.TakeDamage(2000);

            Destroy(this.gameObject);
        }

        if (col.tag == "EnemyBullet")
        {
            Destroy(col.gameObject);
        }
    } //void OnTriggerEnter2D(Collider2D col)
}
