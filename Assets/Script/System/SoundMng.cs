using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 사운드를 관리하는 매니저 클래스
/// </summary>
public class SoundMng : Singleton<SoundMng> {
    protected SoundMng() { }

    public AudioSource backgroundAudio;
    public AudioSource effectAudio;
    Dictionary<string, AudioClip> backgrounds;
    Dictionary<string, AudioClip> effects;
    float volume;
    private void Awake() {
        //Resources 폴더를 기준으로 경로 설정
        FileMng.ins.LoadFile(ref effects, "Sound/Effect/");
        FileMng.ins.LoadFile(ref backgrounds, "Sound/Background/");

    }
    private void Start()
    {
        SetVolume();
        Scene scene = SceneManager.GetActiveScene();
        PlayBackground(scene.name);
    }
    private void Update() {
        SetVolume();
    }
    void SetVolume()
    {
        volume = DataMng.ins.settingValue.Volume;
        SetEffectVolume(volume);
        SetBackgroundVolume(volume);
    }

    public void BackGroundMute(bool muted) { backgroundAudio.mute = muted; }

    public void EffectMute(bool muted) { effectAudio.mute = muted; }

    public bool IsPlayBackGround() { return backgroundAudio.isPlaying; }

    public void PlayEffect(string name) { effectAudio.PlayOneShot(effects[name]); }

    //0~1
    public void SetEffectVolume(float scale) { effectAudio.volume = scale; }

    public void PlayBackground(string name) {
        backgroundAudio.Stop();
        backgroundAudio.loop = true;
        backgroundAudio.clip = backgrounds[name];
        backgroundAudio.Play();
    }
    public void PlayBackgroundNoLoop(string name) {
        backgroundAudio.Stop();
        backgroundAudio.loop = false;
        backgroundAudio.clip = backgrounds[name];
        backgroundAudio.Play();
    }
    public void StopBackground() { backgroundAudio.Stop(); }

    public void SetBackgroundVolume(float scale) { backgroundAudio.volume = scale; }

    public void backGroundPause() { backgroundAudio.Pause(); }

    public void EffectPause() { effectAudio.Pause(); }

    public void backGroundUnPause() { backgroundAudio.UnPause(); }

    public void EffectUnPause() { effectAudio.UnPause(); }
}