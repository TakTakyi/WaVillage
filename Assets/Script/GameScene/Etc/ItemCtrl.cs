using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ItemType
{
    Heart,
    SubPlayer,
    Ultimate,
    Magnet
}

public class ItemCtrl : MonoBehaviour
{
    public ItemType m_ItemType = ItemType.Heart;
    float m_MoveSpeed = 4.0f;
    float a_MagnetSpeed = 9.0f;
    Vector3 a_MoveDir;
    [HideInInspector] public PlayerCtrl m_RefPlayer = null;
    public bool isMagnet;
    public bool IsTutorial;
    string m_ScName;

    // Start is called before the first frame update
    void Start()
    {
        m_ScName = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ScName.Contains("Tutorial") && IsTutorial == true)
        {
            transform.position += Vector3.left * Time.deltaTime * m_MoveSpeed;

            // 총알 화면 밖으로 나가면 제거해 주기
            if (transform.position.x < GameMgr.m_SceenWMin.x - 0.5f)
            {
                Destroy(gameObject);
            }
        }

        if (m_ScName.Contains("Game"))
        {
            if (m_RefPlayer != null && isMagnet == true)
            {
                a_MoveDir = m_RefPlayer.transform.position - transform.position;
                a_MoveDir.z = -1.0f;
                if (a_MoveDir.magnitude <= 20.0f)
                {
                    a_MoveDir.Normalize();
                    transform.position += a_MoveDir * Time.deltaTime * a_MagnetSpeed;
                    isMagnet = true;
                }
            }//if (m_RefPlayer != null)  

            if (isMagnet == false)
                transform.position += Vector3.left * Time.deltaTime * m_MoveSpeed;

            // 총알 화면 밖으로 나가면 제거해 주기
            if (transform.position.x < GameMgr.m_SceenWMin.x - 0.5f)
            {
                Destroy(gameObject);
            }
        }
    }
}
