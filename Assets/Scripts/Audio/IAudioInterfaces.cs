using Audio.Data;
using UnityEngine;

namespace Audio.Interfaces
{
    public interface ISoundEmitter
    {
        void Initialize(SoundData data);
        void Play();
        void Stop();
        void With3D(bool is3D);
        void PlayWithDelay(float delay);
        void WithRandomPitch(float min = -0.05f, float max = 0.05f);
        void WithPitch(float pitch);
        Transform transform { get; }
    }
    public interface ISoundManager
    {
        SoundBuilder CreateSound();
        ISoundEmitter GetEmitter();
        void ReturnToPool(ISoundEmitter emitter);
        Transform GetPoolParent();
    }
}