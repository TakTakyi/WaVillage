using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : G_Singleton<SoundMgr>
{
    [HideInInspector] public AudioSource m_AudioSrc = null;
    Dictionary<string, AudioClip> m_ADClipList = new Dictionary<string, AudioClip>();

    //--- 효과음 최적화를 위한 버퍼 변수
    int m_EffSdCount = 5;       //지금은 5개 레이어로 플레이...
    int m_iSndCount = 0;
    //최대 5개까지 재생되게 제어 렉방지(Andround:5ro, PC:무제한)
    //-------- 조건 아래 배열들은 m_EffSdCount 보다 커야 한다
    List<GameObject> m_sndObjList = new List<GameObject>();     //효과음 오브젝트
    List<AudioSource> m_sndSrcList = new List<AudioSource>();
    //각 차일드 오브젝트에 붙을 AudioSource Component 레퍼런스를 저장하기 위한 리스트
    float[] m_EffVolume = new float[10];
    //--- 효과음 최적화를 위한 버퍼 변수

    float m_bgmVolume = 0.2f;
    [HideInInspector] public bool m_SoundOnOff = true;
    [HideInInspector] public float m_SoundVolume = 1.0f;

    protected override void Init()  //Awake 대신 사용
    {
        base.Init();

        LoadChildGameObj();  //순서상
    }

    // Start is called before the first frame update
    void Start()
    {
        //----- 사운드 미리 로드
        AudioClip a_GAudioClip = null;
        object[] temp = Resources.LoadAll("Sounds");
        for (int a_ii = 0; a_ii < temp.Length; a_ii++)
        {
            a_GAudioClip = temp[a_ii] as AudioClip;

            if (m_ADClipList.ContainsKey(a_GAudioClip.name) == true)
                continue;

            m_ADClipList.Add(a_GAudioClip.name, a_GAudioClip);
        }
        //----- 사운드 미리 로드
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public void LoadChildGameObj()
    {
        m_AudioSrc = this.gameObject.AddComponent<AudioSource>();

        for (int a_ii = 0; a_ii < m_EffSdCount; a_ii++)
        {
            // 최대 5개까지 재생되게 제어 렉방지(Andround:5ro, PC:무제한)
            GameObject newSoundOBJ = new GameObject();
            newSoundOBJ.transform.SetParent(this.transform);
            newSoundOBJ.transform.localPosition = Vector3.zero;
            AudioSource a_AudioSrc = newSoundOBJ.AddComponent<AudioSource>();
            a_AudioSrc.playOnAwake = false;
            a_AudioSrc.loop = false;
            newSoundOBJ.name = "SoundEffObj";

            m_sndSrcList.Add(a_AudioSrc);
            m_sndObjList.Add(newSoundOBJ);
        }
    }

    public void PlayBGM(string a_FileName, float fVolume = 0.2f)
    {
        if (m_SoundOnOff == false)
        {
            return;
        }

        AudioClip a_GAudioClip = null;
        if (m_ADClipList.ContainsKey(a_FileName) == true)
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        if (m_AudioSrc == null)
        {
            return;
        }

        if (m_AudioSrc.clip != null && m_AudioSrc.clip.name == a_FileName)
        {
            return;
        }

        m_AudioSrc.clip = a_GAudioClip;
        m_AudioSrc.volume = fVolume * m_SoundVolume;
        m_bgmVolume = fVolume;
        m_AudioSrc.loop = true;
        m_AudioSrc.Play();
    }

    public void PlayEffSound(string a_FileName, float fVolume = 0.2f)
    {
        if (m_SoundOnOff == false)
        {
            return;
        }

        AudioClip a_GAudioClip = null;
        if (m_ADClipList.ContainsKey(a_FileName) == true)
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        if (a_GAudioClip != null && m_sndSrcList[m_iSndCount] != null)
        {
            m_sndSrcList[m_iSndCount].clip = a_GAudioClip;
            m_sndSrcList[m_iSndCount].volume = fVolume * m_SoundVolume;
            m_sndSrcList[m_iSndCount].loop = false;
            m_sndSrcList[m_iSndCount].Play();
            //m_AudioSrc.PlayClipAtPoint(clip, Vector3(5, 1, 2));  //위치에 따라서 사운드를 플레이 시켜주는 함수
            m_EffVolume[m_iSndCount] = fVolume;

            m_iSndCount++;
            if (m_EffSdCount <= m_iSndCount)
                m_iSndCount = 0;
        }
    }

    public void PlayGUISound(string a_FileName, float fVolume = 0.2f)
    {
        if (m_SoundOnOff == false)
        {
            return;
        }

        AudioClip a_GAudioClip = null;
        if (m_ADClipList.ContainsKey(a_FileName) == true)
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        //---- PlayOneShot() 함수를 이용해서 플레이 하는 경우
        if (m_AudioSrc == null)
        {
            return;
        }

        m_AudioSrc.PlayOneShot(a_GAudioClip, fVolume * m_SoundVolume);
        //---- PlayOneShot() 함수를 이용해서 플레이 하는 경우
    }

    public void SoundOnOff(bool a_OnOff = true)
    {
        bool a_MuteOnOff = !a_OnOff;

        if (m_AudioSrc != null)
        {
            m_AudioSrc.mute = a_MuteOnOff;  //mute == true 끄기 Mute == false 켜지기
            if (a_MuteOnOff == false)
            {
                m_AudioSrc.time = 0;  //처음부터 다시 플레이
            }
        }

        for (int a_ii = 0; a_ii < m_sndSrcList.Count; a_ii++)
        {
            if (m_sndSrcList[a_ii] != null)
            {
                m_sndSrcList[a_ii].mute = a_MuteOnOff;

                if (a_MuteOnOff == false)
                {
                    m_sndSrcList[a_ii].time = 0;  //처음부터 다시 플레이
                }
            }
        }

        m_SoundOnOff = a_OnOff;
    }

    //배경음은 지금 볼륨을 가져온 다음에 플레이 해준다
    public void SoundVolume(float fVolume)
    {
        if (m_AudioSrc != null)
        {
            m_AudioSrc.volume = m_bgmVolume * fVolume;
        }

        for (int a_ii = 0; a_ii < m_sndSrcList.Count; a_ii++)
        {
            if (m_sndSrcList[a_ii] != null)
            {
                m_sndSrcList[a_ii].volume = m_EffVolume[a_ii] * fVolume;
            }
        }

        m_SoundVolume = fVolume;
    }
}
