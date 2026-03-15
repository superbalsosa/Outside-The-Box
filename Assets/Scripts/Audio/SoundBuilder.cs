using Audio.Data;
using Audio.Interfaces;
using UnityEngine;

namespace Audio
{
    public class SoundBuilder
    {
        readonly ISoundManager soundManager;
        SoundData soundData;
        Vector3 position = Vector3.zero;
        bool randomPitch;
        bool is3D;
        float pitch = 1f;
        bool pitchSet = false;

        public SoundBuilder(ISoundManager soundManager)
        {
            this.soundManager = soundManager;
        }
        public SoundBuilder WithPitch(float pitch)
        {
            this.pitch = pitch;
            this.pitchSet = true;
            return this;
        }
        public SoundBuilder WithSoundData(SoundData soundData)
        {
            this.soundData = soundData;
            return this;
        }
        public SoundBuilder WithSoundPosition(Vector3 position)
        {
            this.position = position;
            this.is3D = true;
            return this;
        }
        public SoundBuilder WithRandomPitch()
        {
            this.randomPitch = true;
            return this;

        }
        public ISoundEmitter Play()
        {
            var soundEmitted = PlayBase();
            soundEmitted.Play();

            return soundEmitted;
        }

        public void Stop(ISoundEmitter soundEmiter)
        {
            soundEmiter.Stop();
        }
        public void PlayWithDelay(SoundData soundData)
        {
            PlayBase().PlayWithDelay(soundData.secondsDelay);
        }
        private ISoundEmitter PlayBase()
        {
            var soundEmitter = soundManager.GetEmitter();
            soundEmitter.Initialize(soundData);
            soundEmitter.transform.position = position;
            soundEmitter.transform.parent = soundManager.GetPoolParent();
            soundEmitter.With3D(is3D);
            float finalPitch = pitchSet ? pitch : soundData.pitch;
            soundEmitter.WithPitch(finalPitch);

            if (randomPitch) soundEmitter.WithRandomPitch();

            return soundEmitter;
        }
        
        //If it's 3d, should add WithSoundPosition and automatically makes it 3d
        //Uncomment if need this without a position
        /*public SoundBuilder With3D()
        {
            this.is3D = true;
            return this;
        }*/
    }
}