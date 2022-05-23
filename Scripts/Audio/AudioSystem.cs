using System;
using System.Collections.Generic;
using System.Linq;
using Armageddon.Games;
using UnityEngine;

namespace Armageddon.Audio
{
    public class AudioSystem : GameContext
    {
        [SerializeField]
        private AudioSource m_soundAudioSourcePrefab;

        [SerializeField]
        private AudioSource m_musicAudioSource;

        public AudioClip TestMusicAudioClip;
        private readonly List<AudioSource> m_audioSources = new();

        private int m_currentIndex;

        protected override void Awake()
        {
            base.Awake();

            RegisterService(this);

            for (int i = 0; i < 15; i++)
            {
                AudioSource audioSource = Instantiate(m_soundAudioSourcePrefab, transform);
                m_audioSources.Add(audioSource);
            }
        }

        public void PlaySound(AudioClip audioClip)
        {
            AudioSource audioSource = m_audioSources[m_currentIndex++];

            audioSource.clip = audioClip;
            audioSource.Play();

            m_currentIndex %= m_audioSources.Count;
        }

        public void PlayMusic(AudioClip audioClip)
        {
            m_musicAudioSource.clip = audioClip;
            m_musicAudioSource.Play();
        }

        protected override void OnGamePaused(object sender, EventArgs e)
        {
            base.OnGamePaused(sender, e);

            foreach (AudioSource audioSource in m_audioSources.Where(audioSource => audioSource.isPlaying))
            {
                audioSource.Pause();
            }
        }

        protected override void OnGameResumed(object sender, EventArgs e)
        {
            base.OnGameResumed(sender, e);

            foreach (AudioSource audioSource in m_audioSources.Where(audioSource => !audioSource.isPlaying))
            {
                audioSource.UnPause();
            }
        }
    }
}
