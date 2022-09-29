using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager {
    AudioSource[] _audioSources = new AudioSource[(int)EnumList.SoundType.MaxTypeCount];


    public void Init() {
        GameObject root = GameObject.Find("@Sound");
        if(root == null) {
            root = new GameObject { name = "@Sound" };
            

            string[] names = System.Enum.GetNames(typeof(EnumList.SoundType));
            for(int i = 0; i < names.Length - 1; ++i) {
                GameObject go = new GameObject { name = names[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();

                go.transform.parent = root.transform;
            }

            _audioSources[(int)EnumList.SoundType.BGM].loop = true;
        }
    }

    public void Play(string path, EnumList.SoundType soundType = EnumList.SoundType.Effect, float volume = 1.0f, float pitch = 1.0f) {
        if(soundType == EnumList.SoundType.BGM) {
            AudioClip audioClip = Resources.Load<AudioClip>($"Sounds/{path}");

            if(audioClip == null) {
                Debug.Log("SoundPath is Wrong!");
                return;
            }

            AudioSource audioSource = _audioSources[(int)EnumList.SoundType.BGM];
            if (audioSource.isPlaying) {
                audioSource.Stop();
            }

            if(volume > 1.0f || volume < 0) {
                volume = 0.7f;
            }

            audioSource.pitch = pitch;
            audioSource.volume = volume;
            audioSource.clip = audioClip;
            audioSource.Play();

        }
        else if(soundType == EnumList.SoundType.Effect) {
            AudioClip audioClip = Resources.Load<AudioClip>($"Sounds/{path}");

            if (audioClip == null) {
                Debug.Log("SoundPath is Wrong!");
                return;
            }

            AudioSource audioSource = _audioSources[(int)EnumList.SoundType.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }

    }

    public void StopAll() {
        string[] names = System.Enum.GetNames(typeof(EnumList.SoundType));
        for (int i = 0; i < names.Length - 1; ++i) {
            _audioSources[i].Stop();
        }
    }
}
