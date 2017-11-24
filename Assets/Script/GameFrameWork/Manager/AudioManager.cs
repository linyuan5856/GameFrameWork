using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    static public AudioManager Instance;

    private AudioSource audio_bg_source;
    private string current_music_name;

    private AudioSource audio_btn_source;
    private AudioSource audio_run_source;

    private List<AudioSource> audioSourceList = new List<AudioSource>();

    void Awake()
    {
        Instance = this;
        this.audio_bg_source = gameObject.AddComponent<AudioSource>();
        this.audio_btn_source = gameObject.AddComponent<AudioSource>();
        this.audio_run_source = gameObject.AddComponent<AudioSource>();
    }

    public AudioSource audioRunSource
    {
        get { return audio_run_source; }
    }
  
    public void StopPlayBG()
    {
        if (this.audio_bg_source)
        {
          this.audio_bg_source.Stop();
        }
    }

    private AudioSource getAudioSource()
    {
        AudioSource resultSource = null;

        for (int i = 0; i < audioSourceList.Count;i++)
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

    private void PlaySound(AudioSource source,AudioClip clip, bool loop)
    {
        if (source==null)
        {
            return;
        }

        if (source.clip != null)
        {
            source.Stop();
            //AudioClip tempClip = source.clip;
            //source.clip = null;
            //Resources.UnloadAsset(tempClip);
        }
        source.clip = clip;
        source.loop = loop;
        source.pitch = Mathf.Max(1, Time.timeScale / 1.5f);
        source.Play();
    }

}