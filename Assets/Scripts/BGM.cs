using Audio.Interfaces;
using Audio.Data;
using DependencyInjection;
using UnityEngine;
using System.Collections.Generic;

public class BGM : MonoBehaviour
{
    [SerializeField] private List<SoundData> soundsData;

    private ISoundManager soundManager;

    private void Start()
    {
        if (soundsData.Count > 0)
        {
            soundManager = InterfaceDependencyInjector.Instance.Resolve<ISoundManager>();
            foreach (var soundData in soundsData)
            {
                soundManager.CreateSound()
                   .WithSoundData(soundData)
                   .Play();
            }
        }
    }
}
