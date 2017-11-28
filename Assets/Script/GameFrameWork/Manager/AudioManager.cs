using System.Collections.Generic;
using UnityEngine;

namespace GFW
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private AudioSource audio_bg_source;
        private string current_music_name;

        private AudioSource audio_btn_source;
        private AudioSource audio_run_source;

        private List<AudioSource> audioSourceList = new List<AudioSource>();

        void Awake()
        {
            this.audio_bg_source = gameObject.AddComponent<AudioSource>();
            this.audio_btn_source = gameObject.AddComponent<AudioSource>();
            this.audio_run_source = gameObject.AddComponent<AudioSource>();
        }

        public void StopPlayBg()
        {
            if (this.audio_bg_source)
            {
                this.audio_bg_source.Stop();
            }
        }

        private AudioSource GetAudioSource()
        {
            AudioSource resultSource = null;

            for (int i = 0; i < audioSourceList.Count; i++)
            {
                AudioSource source = audioSourceList[i];
                if (!source.isPlaying)
                {
                    resultSource = source;
                    resultSource.clip = null;
                }
            }

            if (resultSource == null)
            {
                resultSource = gameObject.AddComponent<AudioSource>();
                resultSource.mute = audio_btn_source.mute;
                audioSourceList.Add(resultSource);
            }

            return resultSource;
        }

        public void GameSetting__Sound(bool isPlayGameSound)
        {
            audio_btn_source.mute = !isPlayGameSound;
            audio_run_source.mute = !isPlayGameSound;
            foreach (var source in audioSourceList)
            {
                source.mute = !isPlayGameSound;
            }
        }

        private void PlaySound(AudioSource source, AudioClip clip, bool loop)
        {
            if (source == null || clip == null)
            {
                return;
            }

            if (source.clip != null)
            {
                source.Stop();
            }
            source.clip = clip;
            source.loop = loop;
            source.pitch = Mathf.Max(1, Time.timeScale / 1.5f);
            source.Play();
        }

    }

}
