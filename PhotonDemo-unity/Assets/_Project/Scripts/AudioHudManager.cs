using AudioUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioHudManager : MonoBehaviour
{
    #region Variables

    [Header("Settings")]
    [SerializeField]
    private Slider m_slider;

    #endregion

    public void ShowSlider()
    {
        AudioManager.Instance.MasterAudioMixer.GetFloat(string.Format("{0}_Volume", AudioMixerGroupNames.Master), out float volumeValue);

        m_slider.value = volumeValue;
        m_slider.gameObject.SetActive(!m_slider.gameObject.activeSelf);
    }

    public void ChangeVolume()
    {
        AudioManager.Instance.OnVolumeChangedHandler(AudioMixerGroupNames.Master, m_slider.value);
    }
}
