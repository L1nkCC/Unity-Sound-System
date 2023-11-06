using UnityEngine;

namespace CC.SoundSystem
{
    public static class AudioSourceExtensions
    {
        public static void Play<T>(this AudioSource audioSource, T soundNode, ulong delay = 0) where T : struct
        {
            Node node = GetNodeFromEnum(soundNode);
            audioSource.volume = node.GetVolume();
            audioSource.Play();
        }
        public static void PlayOneShot<T>(this AudioSource audioSource, AudioClip clip, T soundNode) where T : struct
        {
            Node node = GetNodeFromEnum(soundNode);
            audioSource.PlayOneShot(clip, node.GetVolume());
        }
        public static float GetVolume<T>(T soundNode) where T : struct
        {
            return GetNodeFromEnum(soundNode).GetVolume();
        }
        private static Node GetNodeFromEnum<T>(T soundNode) where T : struct
        {
            if (!typeof(T).IsEnum || typeof(T).GetEnumUnderlyingType() != typeof(int))
            {
                throw new System.ArgumentException("Play<T> must be passed a Enum created throught the CC.SoundSystem Methods");
            }

            if (!typeof(T).IsEnum) throw new System.ArgumentException("Must pass a Enum type to get the appropriate domain");
            string domainName = typeof(T).Name;
            Debug.Log(domainName + " typeof " + typeof(T).Name);
            if (domainName == null) throw new System.ArgumentException("Enum passed was not a domain");

            Node node = Domain.GetNode(domainName, System.Enum.GetName(typeof(T), soundNode));
            Debug.Log(typeof(T) + "    " + soundNode.ToString());
            if (node == null) throw new System.ArgumentException("No node exists meeting conditions");

            return node;
        }
    }
}
