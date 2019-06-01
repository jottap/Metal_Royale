using UnityEngine;

namespace AudioUtils
{
    public static class ExtensionMethods
    {
        public static bool IsPaused(this AudioSource audioSource)
        {
            return !audioSource.isPlaying && audioSource.time > 0;
        }

        public static bool IsStopped(this AudioSource audioSource)
        {
            return !audioSource.isPlaying && audioSource.time == 0;
        }
    }
}