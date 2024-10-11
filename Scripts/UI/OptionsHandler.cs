using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsHandler : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] Slider m_masterSlider;
    [SerializeField] Slider m_musicSlider;
    [SerializeField] Slider m_ambiantSlider;
    [SerializeField] Slider m_FXSlider;

    SoundManagerFMOD m_manager;

    enum m_enumSliders { Brightness, Master, Music, Ambiant, FX};

    // Start is called before the first frame update
    void Start()
    {
        m_manager = SoundManagerFMOD.GetInstance();

        InitSliders();
    }

    // Update is called once per frame
    void Update()
    {
        m_masterSlider.onValueChanged.AddListener(delegate { VolumeChange(m_enumSliders.Master, m_masterSlider.value); });
        m_musicSlider.onValueChanged.AddListener(delegate { VolumeChange(m_enumSliders.Music, m_musicSlider.value); });
        m_ambiantSlider.onValueChanged.AddListener(delegate { VolumeChange(m_enumSliders.Ambiant, m_ambiantSlider.value); });
        m_FXSlider.onValueChanged.AddListener(delegate { VolumeChange(m_enumSliders.FX, m_FXSlider.value); });
    }



    private void VolumeChange(m_enumSliders _slider, float _value)
    {
        switch (_slider)
        {
            case m_enumSliders.Master:
                m_manager.SetVolumeMaster(_value);
                break;
            case m_enumSliders.Music:
                m_manager.SetVolumeSoundTrack(_value);
                break;
            case m_enumSliders.Ambiant:
                m_manager.SetVolumeAmbiant(_value);
                break;
            case m_enumSliders.FX:
                m_manager.SetVolumeFX(_value);
                break;
        }
    }

    private void InitSliders()
    {
        m_masterSlider.value = m_manager.GetVolumeMaster();
        m_musicSlider.value = m_manager.GetVolumeSoundTrack();
        m_ambiantSlider.value = m_manager.GetVolumeAmbiant();
        m_FXSlider.value = m_manager.GetVolumeFX();
    }
}
