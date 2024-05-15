using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;

public enum STATE { PLAY, PAUSE, CLEAR }
public class SystemMng : Singleton<SystemMng> {
    public STATE state = STATE.PLAY;
    public GameObject mainCamera;
    public GameObject TimeLineCam;
    public GameObject BossHPUI;
    public PlayableDirector PadDie;
    public PlayableDirector PadVellum;
    public PlayableDirector PadClear;
    public VellumPattern v;
    public VellumBT vellumAi;
    public TailBT tailAi;
    public TailPattern t;
    [SerializeField] GameObject youdied;
    [SerializeField] GameObject tfp;
    [SerializeField] GameObject fire;
    [SerializeField] GameObject SettingUI;
    public Enemy[] mobs;
    public Portal portal;
    public int mobCnt = 0;
    public Tip tip;
    void Start() {
        Screen.SetResolution(1920, 1080, false);
        state = STATE.PLAY;
        mobCnt = mobs.Length;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update() {
        if (portal!=null && mobCnt <= 0) {
            portal.gameObject.SetActive(true);
        }
    }
    public void NextStage() {
        Player.ins.characterController.enabled = false;
        Player.ins.transform.position = new Vector3(0, 0.4f, 36f);
        Player.ins.characterController.enabled = true;
    }
    public void FadeOut()
    {
        StartCoroutine(UIMng.ins.FadeOutCoroutine(NextStage));
    }
    public void FadeIn()
    {
        StartCoroutine(UIMng.ins.FadeInCoroutine());
    }

    public IEnumerator DieCoroutine() {
        SoundMng.ins.PlayBackgroundNoLoop("Death");
        tip.TipOff();
        TimeLineCam.SetActive(true);
        mainCamera.SetActive(false);
        state = STATE.PAUSE;
        PadDie.Play();
        yield return new WaitForSeconds(3.0f);
        UIMng.ins.FadeOutCoroutine();
        yield return new WaitForSeconds(1.0f);
        youdied.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("StartScene");
    }
    public IEnumerator VellumAppear() {
        SoundMng.ins.PlayBackground("BossBGM");
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
        PadVellum.Play();
        state = STATE.PAUSE;
        yield return new WaitForSeconds(2.0f);
        fire.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        v.Detect();
        v.StartCoroutine(v.Out());
        yield return new WaitForSeconds(1.0f);
        BossHPUI.SetActive(true);
        state = STATE.PLAY;
        yield return new WaitForSeconds(2.0f);
        vellumAi.gameObject.SetActive(true);
        tailAi.gameObject.SetActive(true);
    }
    public IEnumerator ClearCoroutine() {
        SoundMng.ins.PlayBackgroundNoLoop("Victory");
        state = STATE.CLEAR;
        BossHPUI.SetActive(false);
        TimeLineCam.SetActive(true);
        mainCamera.SetActive(false);
        PadClear.Play();

        yield return new WaitForSeconds(3.0f);
        StartCoroutine(UIMng.ins.FadeOutCoroutine());
        yield return new WaitForSeconds(1.0f);
        tfp.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene("StartScene");
    }
    
}
