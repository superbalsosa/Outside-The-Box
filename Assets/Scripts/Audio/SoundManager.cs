using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using DependencyInjection;
using Audio.Interfaces;
using Audio.Data;

namespace Audio
{
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        public static SoundManager Instance { get; private set; }

        [SerializeField] SoundEmitter soundEmitterPrefab;
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;

        IObjectPool<SoundEmitter> soundEmitterPool;
        readonly List<SoundEmitter> activeSoundEmitters = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                InterfaceDependencyInjector.Instance.Register<ISoundManager>(() => this);
                SceneManager.activeSceneChanged += OnActiveSceneChanged;
                InitializePool();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            for (int i = activeSoundEmitters.Count - 1; i >= 0; i--)
            {
                ReturnToPool(activeSoundEmitters[i]);
            }
        }
        public SoundBuilder CreateSound() => new SoundBuilder(this);
        public ISoundEmitter GetEmitter() => soundEmitterPool.Get();
        public SoundEmitter Get()
        {
            return soundEmitterPool.Get();
        }
        public void ReturnToPool(ISoundEmitter soundEmitter)
        {
            if (soundEmitter is SoundEmitter concreteEmitter)
            {
                soundEmitterPool.Release(concreteEmitter);
            }
        }
        private void InitializePool()
        {
            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
        }
        private SoundEmitter CreateSoundEmitter()
        {
            var soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);

            return soundEmitter;
        }
        private void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            activeSoundEmitters.Add(soundEmitter);
        }
        private void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(soundEmitter);
        }
        private void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            Destroy(soundEmitter.gameObject);
        }
        public void AfterFirstInteraction(string _name)
        {
            throw new System.NotImplementedException();
        }
        public void PlayQuickSound(SoundData soundData)
        {
            CreateSound()
                .WithSoundData(soundData)
                .Play();
        }
        public Transform GetPoolParent() => transform;
    } 
}