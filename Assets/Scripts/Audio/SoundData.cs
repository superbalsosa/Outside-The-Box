using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio.Data
{
    [Serializable]
    public class SoundData
    {
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        public bool isALoop;
        public bool isAPlayOnAwake;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public float secondsDelay = 0;
    } 
}