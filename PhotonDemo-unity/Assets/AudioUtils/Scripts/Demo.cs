using UnityEngine;

namespace AudioUtils
{
    public class Demo : MonoBehaviour
    {
        [SerializeField]
        private AudioMixerGroupNames m_AudioMixerGroupName;

        [SerializeField, Range(-80, 20)]
        private float m_Volume = 0.0f;

        [SerializeField]
        private float m_Delay = 1.0f;

        [SerializeField]
        private AudioClip m_AudioClip;

        [ContextMenu("SetVolume")]
        public void SetVolume()
        {
            AudioSourceConfiguration.SaveVolume(m_AudioMixerGroupName, m_Volume);
        }

        [ContextMenu("GetVolume")]
        public void GetVolume()
        {
            Debug.Log("GetFXVolume: " + AudioSourceConfiguration.GetVolume(m_AudioMixerGroupName));
        }

        [ContextMenu("PlayAudio")]
        public void PlayAudio()
        {
            AudioManager.Play(m_AudioClip, m_AudioMixerGroupName, m_Delay);
        }
    }
}