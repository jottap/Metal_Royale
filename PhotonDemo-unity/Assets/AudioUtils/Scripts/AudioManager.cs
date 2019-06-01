using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioUtils
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager m_Instance;
        public static AudioManager Instance
        {
            get
            {
                return m_Instance;
            }

            set
            {
                m_Instance = value;
            }
        }

        private const string EMPTY_AUDIO = "audios/empty_audio";

        [SerializeField]
        private AudioMixer m_MasterAudioMixer;
        public AudioMixer MasterAudioMixer
        {
            get
            {
                return m_MasterAudioMixer;
            }

            set
            {
                m_MasterAudioMixer = value;
            }
        }

        public static int? lastVolume;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Instance = this;
            
            SubscribeEvents();
        }

        private void Start()
        {
            if(lastVolume == null)
                OnVolumeChangedHandler(AudioMixerGroupNames.Master, -20);
            else
                OnVolumeChangedHandler(AudioMixerGroupNames.Master, lastVolume.Value);
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            AudioSourceConfiguration.OnVolumeChanged_Action += OnVolumeChangedHandler;
        }

        private void UnsubscribeEvents()
        {
            //AudioSourceConfiguration.OnVolumeChanged_Action -= OnVolumeChangedHandler;
        }

        private AudioMixerGroup GetAudioMixerGroupByName(AudioMixerGroupNames audioMixerGroupName)
        {
            AudioMixerGroup audioMixerGroup = MasterAudioMixer.FindMatchingGroups("Master").FirstOrDefault(x => x.name.Equals(audioMixerGroupName.ToString()));

            return audioMixerGroup;
        }

        /// <summary>
        /// Configurar o volume de uma determinado grupo de Audio.
        /// </summary>
        /// <param name="audioMixerGroupName">Nome do grupo de Audio.</param>
        /// <param name="volume">[0 dB] = Normal volume. [-80 dB] = Minimum volume. [20 dB] = Maximum volume.</param>
        public void OnVolumeChangedHandler(AudioMixerGroupNames audioMixerGroupName, float volume)
        {
            string audioMixerParameterName = string.Format("{0}_Volume", audioMixerGroupName);

            lastVolume = (int)volume;

            MasterAudioMixer.SetFloat(audioMixerParameterName, volume);
        }

        /// <summary>
        /// Tocar um AudioClip.
        /// OBS.: O AudioSource vai ter Loop caso o MixerGroup seja Background. Caso haja necessidade de ter Backgrouns sem loop, outro MixerGroup deve ser criado na estrutura.
        /// </summary>
        /// <param name="audioClip">AudioClip para ser tocado.</param>
        /// <param name="audioMixerGroupName">Tipo de AudioClip.</param>
        /// <param name="delay">Delay em segundos para tocar o AudioClip.</param>
        public static AudioSource Play(AudioClip audioClip, AudioMixerGroupNames audioMixerGroupName, float delay = 0)
        {
            if (audioClip == null)
                return null;

            AudioSource audioSource = AudioPooler.AudioSource;

            try
            {
                AudioMixerGroup audioMixerGroup = Instance.GetAudioMixerGroupByName(audioMixerGroupName);
                audioSource.outputAudioMixerGroup = audioMixerGroup;

                audioSource.clip = audioClip;
                audioSource.PlayDelayed(delay);
                audioSource.loop = audioMixerGroupName == AudioMixerGroupNames.Background;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
            }

            return audioSource;
        }

        /// <summary>
        /// Tocar um Audio baseado no caminho dentro da pasta Resources.
        /// OBS.: O AudioSource vai ter Loop caso o MixerGroup seja Background. Caso haja necessidade de ter Backgrouns sem loop, outro MixerGroup deve ser criado na estrutura.
        /// </summary>
        /// <param name="resourcesPath">Caminho do AudioClip para ser tocado.</param>
        /// <param name="audioMixerGroupName">Tipo de AudioClip.</param>
        /// <param name="delay">Delay em segundos para tocar o AudioClip.</param>
        public static AudioSource Play(string resourcesPath, AudioMixerGroupNames audioMixerGroupName, float delay = 0)
        {
            AudioSource audioSource = null;

            try
            {
                AudioClip audioClip = Resources.Load<AudioClip>(resourcesPath);

                // Se nao encontrar nenhum audioClip, pegar um vazio.
                if (audioClip == null)
                {
                    audioClip = Resources.Load<AudioClip>(EMPTY_AUDIO);
                }

                audioSource = Play(audioClip, audioMixerGroupName, delay);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
            }

            return audioSource;
        }

        /// <summary>
        /// Dar Stop em todos os AudioSources do Pooler que estao nesse MixerGroup.
        /// </summary>
        /// <param name="audioMixerGroupName"></param>
        /// <param name="useFade"></param>
        public static void Stop(AudioMixerGroupNames audioMixerGroupName, bool useFade = true)
        {
            try
            {
                List<AudioSource> list = AudioPooler.AudioSourcesList.FindAll(x => x.outputAudioMixerGroup == Instance.GetAudioMixerGroupByName(audioMixerGroupName) && !x.IsStopped());

                float duration = useFade ? 0.5f : 0.0f;

                list.ForEach(x =>
                {
                    AudioPooler.AudioSourcesList.Remove(x);

                    x.DOKill();

                    x.DOFade(0, duration).OnComplete(() => x.Stop());
                });
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
            }

        }

        /// <summary>
        /// Dar Stop em todos os AudioSources do Pooler que possuam esse audioClip.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="useFade"></param>
        public static void Stop(AudioClip audioClip, bool useFade = true)
        {
            try
            {
                List<AudioSource> list = AudioPooler.AudioSourcesList.FindAll(x => x.clip == audioClip && !x.IsStopped());

                float duration = useFade ? 0.5f : 0.0f;

                list.ForEach(x =>
                {
                    AudioPooler.AudioSourcesList.Remove(x);

                    x.DOKill();

                    x.DOFade(0, duration).OnComplete(() => x.Stop());
                });
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
            }
            
        }

        /// <summary>
        /// Dar Pause em todos os AudioSources do Pooler que estao nesse MixerGroup.
        /// </summary>
        /// <param name="audioMixerGroupName"></param>
        /// <param name="useFade"></param>
        /// <returns>Lista de AudioSources que foram pausados.</returns>
        public static List<AudioSource> Pause(AudioMixerGroupNames audioMixerGroupName, bool useFade = true)
        {
            List<AudioSource> list = AudioPooler.AudioSourcesList.FindAll(x => x.outputAudioMixerGroup == Instance.GetAudioMixerGroupByName(audioMixerGroupName) && !x.IsStopped());

            float duration = useFade ? 0.5f : 0.0f;

            list.ForEach(x =>
            {
                x.DOKill();

                x.DOFade(0, duration).OnComplete(() => x.Pause());
            });

            return list;
        }

        /// <summary>
        /// Dar Pause em todos os AudioSources do Pooler que possuam esse audioClip.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="useFade"></param>
        /// <returns>Lista de AudioSources que foram pausados.</returns>
        public static List<AudioSource> Pause(AudioClip audioClip, bool useFade = true)
        {
            if (audioClip == null)
                return null;

            List<AudioSource> list = AudioPooler.AudioSourcesList.FindAll(x => x.clip == audioClip && !x.IsStopped());

            float duration = useFade ? 0.5f : 0.0f;

            foreach (AudioSource x in list)
            {
                if(x != null)
                {
                    //x.DOKill();

                    //x.DOFade(0, duration).OnComplete(() => x.Pause());
                }
            }

            return list;
        }

        /// <summary>
        /// Dar Pause em todos os AudioSources do Pooler que estao nesse MixerGroup.
        /// </summary>
        /// <param name="audioMixerGroupName"></param>
        /// <param name="useFade"></param>
        /// <returns>Lista de AudioSources que foram pausados.</returns>
        public static List<AudioSource> UnPause(AudioMixerGroupNames audioMixerGroupName, bool useFade = true)
        {
            List<AudioSource> list = AudioPooler.AudioSourcesList.FindAll(x => x.outputAudioMixerGroup == Instance.GetAudioMixerGroupByName(audioMixerGroupName) && x.IsPaused());

            float duration = useFade ? 0.5f : 0.0f;

            list.ForEach(x =>
            {
                x.DOKill();

                x.DOFade(1, duration).OnComplete(() => x.UnPause());
            });

            return list;
        }

        /// <summary>
        /// Dar Pause em todos os AudioSources do Pooler que possuam esse audioClip.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="useFade"></param>
        /// <returns>Lista de AudioSources que foram pausados.</returns>
        public static List<AudioSource> UnPause(AudioClip audioClip, bool useFade = true)
        {
            List<AudioSource> list = AudioPooler.AudioSourcesList.FindAll(x => x.clip == audioClip && x.IsPaused());

            float duration = useFade ? 0.5f : 0.0f;

            list.ForEach(x =>
            {
                x.DOKill();

                x.DOFade(1, duration).OnComplete(() => x.UnPause());
            });

            return list;
        }

        /// <summary>
        /// Verifica se o MixerGroup esta tocando algum audio.
        /// </summary>
        /// <param name="audioMixerGroupName"></param>
        /// <returns></returns>
        public static bool IsPlaying(AudioMixerGroupNames audioMixerGroupName)
        {
            bool exists = AudioPooler.AudioSourcesList.Exists(x => x.outputAudioMixerGroup == Instance.GetAudioMixerGroupByName(audioMixerGroupName) && !x.IsStopped());

            return exists;
        }

        /// <summary>
        /// Verifica se o audioClip esta tocando.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <returns></returns>
        public static bool IsPlaying(AudioClip audioClip)
        {
            bool exists = AudioPooler.AudioSourcesList.Exists(x => x.clip == audioClip && !x.IsStopped());

            return exists;
        }
    }
}