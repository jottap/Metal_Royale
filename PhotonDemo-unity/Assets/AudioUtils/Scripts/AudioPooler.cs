using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace AudioUtils
{
    /// <summary>
    /// Classe usada para criar/usar AudioSource com reaproveitamento.
    /// </summary>

    public class AudioPooler : MonoBehaviour
    {
        private static AudioPooler m_Instance;
        private static AudioPooler Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject go = new GameObject(typeof(AudioPooler).Name);
                    m_Instance = go.AddComponent<AudioPooler>();
                }

                return m_Instance;
            }
        }

        private List<AudioSource> m_AudioSourceList;
        public static List<AudioSource> AudioSourcesList
        {
            get
            {
                if (Instance.m_AudioSourceList == null)
                {
                    Instance.m_AudioSourceList = new List<AudioSource>();
                }

                return Instance.m_AudioSourceList.FindAll(x => x != null);
            }

            set
            {
                Instance.m_AudioSourceList = value;
            }
        }

        private AudioSource AvailableAudioSource
        {
            get
            {
                if (m_AudioSourceList == null)
                {
                    m_AudioSourceList = new List<AudioSource>();
                }

                AudioSource audioSource = m_AudioSourceList.Find(x => x != null && x.IsStopped());
                if (audioSource == null)
                {
                    audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
                    audioSource.transform.SetParent(this.transform);

                    m_AudioSourceList.Add(audioSource);
                }

                audioSource.enabled = true;
                audioSource.mute = false;
                audioSource.playOnAwake = false;
                audioSource.loop = false;
                audioSource.volume = 1.0f;

                return audioSource;
            }
        }

        public static AudioSource AudioSource
        {
            get
            {
                AudioSource audioSource = Instance.AvailableAudioSource;
                audioSource.DOKill();
                audioSource.volume = 1.0f;

                return audioSource;
            }
        }
    }
}