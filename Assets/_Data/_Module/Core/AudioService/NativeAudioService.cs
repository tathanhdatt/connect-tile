using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.AudioService
{
    public class NativeAudioService : MonoBehaviour, IAudioService
    {
        private readonly Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

        private void Awake()
        {
            var sources = GetComponentsInChildren<AudioSource>();
            foreach (var source in sources)
            {
                this.audioSources.Add(source.name, source);
            }
        }

        public void PlaySfx(string sfxName)
        {
            var sfxKey = $"Sfx_{sfxName}";
            if (this.audioSources.TryGetValue(sfxKey, out var sfx))
            {
                sfx.Play();
            }
        }

        public void StopSfx(string sfxName)
        {
            var sfxKey = $"Sfx_{sfxName}";
            if (this.audioSources.TryGetValue(sfxKey, out var sfx))
            {
                sfx.Stop();
            }
        }

        public void PlayMusic(string musicName)
        {
            var musicKey = $"Music_{musicName}";
            if (this.audioSources.TryGetValue(musicKey, out var music))
            {
                music.Play();
            }
        }

        public void StopMusic(string musicName)
        {
            var musicKey = $"Music_{musicName}";
            if (this.audioSources.TryGetValue(musicKey, out var music))
            {
                music.Stop();
            }
        }

        public void SetVolume(float val)
        {
            foreach (var source in this.audioSources.Values)
            {
                source.volume = val;
            }
        }

        public void SetVolumeSfx(float val)
        {
            
        }

        public void SetVolumeMusic(float val)
        {
            
        }
    }
}