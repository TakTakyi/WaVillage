using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfigBox : MonoBehaviour
{
    public Button m_OK_Btn = null;

    public Toggle m_Sound_Toggle = null;
    public Slider m_Sound_Slider = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_OK_Btn != null)
            m_OK_Btn.onClick.AddListener(OKBtnFunction);

        //--- 체크상태가 변경되었을때 호출되는 함수를 대기하는 코드
        if (m_Sound_Toggle != null)
            m_Sound_Toggle.onValueChanged.AddListener(SoundOnOff);

        //슬라이드 상태가 변경 되었을 때 호출되는 함수 대기하는 코드
        if (m_Sound_Slider != null)
            m_Sound_Slider.onValueChanged.AddListener(SliderChanged);

        //-- 체크상태, 슬라이드 상태, 닉네임 로딩 후 UI컨트롤에 적용
        int a_SoundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if (m_Sound_Toggle != null)
        {
            //m_Sound_Toggle.isOn = (a_SoundOnOff == 1) ? true : false;
            if (a_SoundOnOff == 1)
            {
                m_Sound_Toggle.isOn = true;
            }
            else
            {
                m_Sound_Toggle.isOn = false;
            }
        }

        if (m_Sound_Slider != null)
            m_Sound_Slider.value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
        //-- 체크상태, 슬라이드 상태, 닉네임 로딩 후 UI컨트롤에 적용
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    void OKBtnFunction()
    {
        Time.timeScale = 1.0f;    //일시정지 풀어주기

        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            TutorialMgr.Inst.ISConfig = true;
        }

        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            LobbyMgr.Inst.ISConfig = true;
        }

        string name = SceneManager.GetActiveScene().name;
        if (name.Contains("Game"))
        {
            GameMgr.Inst.ISConfig = true;
        }


        Destroy(this.gameObject);
    }

    void SoundOnOff(bool value)  //체크상태가 변경되었을 때 호출되는 함수
    {
        //---- 체크상태 저장
        if (m_Sound_Toggle != null)
        {
            if (value == true)
            {
                PlayerPrefs.SetInt("SoundOnOff", 1);
            }
            else
            {
                PlayerPrefs.SetInt("SoundOnOff", 0);
            }

            SoundMgr.Instance.SoundOnOff(value);  //"사운드 켜기 / 끄기"
        }
        //---- 체크상태 저장
    }

    public void SliderChanged(float value)
    {
        if (m_Sound_Slider != null)
        {
            //Value 0.0 ~ 1.0f // 슬라이드 상태가 변경 되었을때 호출되는 함수
            //---- 슬라이드 상태 저장
            PlayerPrefs.SetFloat("SoundVolume", value);
            SoundMgr.Instance.SoundVolume(value);
        }
    }
}
