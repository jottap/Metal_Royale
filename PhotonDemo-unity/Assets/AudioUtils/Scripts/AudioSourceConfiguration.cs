using System;
using System.Collections.Generic;
using UnityEngine;

namespace AudioUtils
{
    /// <summary>
    /// Classe com metodos estaticos para Salvar/Carregar o volume dos objetos do jogo.
    /// </summary>

    public class AudioSourceConfiguration
    {
        public static Action<AudioMixerGroupNames, float> OnVolumeChanged_Action { get; set; }

        private static Dictionary<AudioMixerGroupNames, float> m_VolumesDict;
        private static Dictionary<AudioMixerGroupNames, float> VolumesDict
        {
            get
            {
                if (m_VolumesDict == null)
                {
                    m_VolumesDict = new Dictionary<AudioMixerGroupNames, float>();
                }

                return m_VolumesDict;
            }

            set
            {
                m_VolumesDict = value;
            }
        }

        public static float GetVolume(AudioMixerGroupNames audioMixerGroupName)
        {
            if (!VolumesDict.ContainsKey(audioMixerGroupName))
            {
                LoadSavedConfigs();
            }

            return VolumesDict[audioMixerGroupName];
        }

        public static void LoadSavedConfigs()
        {
            foreach (AudioMixerGroupNames audioMixerGroupName in Enum.GetValues(typeof(AudioMixerGroupNames)))
            {
                string playerPrefsKey = string.Format("{0}_AudioSource_Volume", audioMixerGroupName.ToString());

                float volume = PlayerPrefs.HasKey(playerPrefsKey) ?
                    PlayerPrefs.GetFloat(playerPrefsKey) :
                    1.0f; // Valor default

                if (VolumesDict.ContainsKey(audioMixerGroupName))
                {
                    VolumesDict[audioMixerGroupName] = volume;
                }
                else
                {
                    VolumesDict.Add(audioMixerGroupName, volume);
                }

                //Debug.Log("LOAD: " + playerPrefsKey + " = " + VolumesDict[audioSourceType]);
            }
        }

        public static void SaveVolume(AudioMixerGroupNames audioMixerGroupName, float volume)
        {
            string playerPrefsKey = string.Format("{0}_AudioSource_Volume", audioMixerGroupName.ToString());

            PlayerPrefs.SetFloat(playerPrefsKey, volume);

            if (VolumesDict.ContainsKey(audioMixerGroupName))
            {
                VolumesDict[audioMixerGroupName] = volume;
            }
            else
            {
                VolumesDict.Add(audioMixerGroupName, volume);
            }

            //Debug.Log("SAVE: " + playerPrefsKey + " = " + VolumesDict[audioSourceType]);

            if (OnVolumeChanged_Action != null)
            {
                OnVolumeChanged_Action(audioMixerGroupName, volume);
            }
        }
    }
}