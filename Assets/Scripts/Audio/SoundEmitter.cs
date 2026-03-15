using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Utilities;
using DependencyInjection;
using Audio.Interfaces;
using Audio.Data;

namespace Audio
{
    public class SoundEmitter : MonoBehaviour, ISoundEmitter
    {
        private AudioSource audioSource;
        private Coroutine playingCoroutine;

        private void Awake()
        {
            audioSource = gameObject.GetOrAdd<AudioSource>();

            InterfaceDependencyInjector.Instance.Register<ISoundEmitter>(() => this);
        }
        public void Initialize(SoundData data)
        {
            audioSource.clip = data.clip;
            audioSource.outputAudioMixerGroup = data.mixerGroup;
            audioSource.loop = data.isALoop;
            audioSource.playOnAwake = data.isAPlayOnAwake;
            audioSource.volume = data.volume;
            audioSource.pitch = data.pitch;
        }
        public void Play()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
            }

            audioSource.Play();
            playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }
        public void PlayWithDelay(float secondsDelay)
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
            }

            audioSource.Play();
            playingCoroutine = StartCoroutine(WaitForSoundToEnd(secondsDelay));
        }
        public void Stop()
        {
            if (this == null)
            {
                return;
            }

            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            if (audioSource != null)
            {
                audioSource.Stop();
            }
            
            SoundManager.Instance.ReturnToPool(this);
        }

        IEnumerator WaitForSoundToEnd(float secondsDelay = 0f)
        {
            if(secondsDelay > 0f) yield return new WaitForSeconds(secondsDelay);
            yield return new WaitWhile(() => audioSource.isPlaying);
            SoundManager.Instance.ReturnToPool(this);
        }
        public void With3D(bool is3D)
        {
            audioSource.spatialBlend = is3D ? 1 : 0;
        }
        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            audioSource.pitch += Random.Range(min, max);
        }
        public void WithPitch(float pitch)
        {
            audioSource.pitch = pitch;
        }
    }
}