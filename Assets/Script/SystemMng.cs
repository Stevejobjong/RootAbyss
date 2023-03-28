using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SystemMng : Singleton<SystemMng> {
    public enum STATE { PLAY, PAUSE }
    public STATE state = STATE.PLAY;
    public GameObject mainCamera;
    public GameObject TimeLineCam;
    public GameObject BossHPUI;
    public PlayableDirector PadDie;
    public PlayableDirector PadVellum;
    public PlayableDirector PadClear;
    public Vellum v;
    [SerializeField] Image img;
    [SerializeField] GameObject youdied;
    [SerializeField] GameObject tfp;
    [SerializeField] GameObject fire;
    public Enemy[] mobs;
    public Portal portal;
    public int mobCnt = 0;
    void Start() {
        state = STATE.PLAY;
        mobCnt = mobs.Length;
    }
    void Update() {
        if (mobCnt <= 0) {
            portal.gameObject.SetActive(true);
        }
    }
    public void NextStage() {
        Player.ins.characterController.enabled = false;
        Player.ins.transform.position = new Vector3(0, 0.4f, 36f);
        Player.ins.characterController.enabled = true;
    }
    public void FadeOut() {
        StartCoroutine(FadeOutCoroutine());
    }
    public void FadeIn() {
        StartCoroutine(FadeInCoroutine());
    }
    IEnumerator FadeOutCoroutine() {
        Color c = img.color;
        while (c.a < 1) {
            c.a += Time.deltaTime * 2;
            img.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        NextStage();
    }
    IEnumerator FadeInCoroutine() {
        Color c = img.color;
        while (c.a > 0) {
            c.a -= Time.deltaTime * 2;
            //print("fadein");
            img.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    public IEnumerator DieCoroutine() {
        TimeLineCam.SetActive(true);
        mainCamera.SetActive(false);
        state = STATE.PAUSE;
        PadDie.Play();
        yield return new WaitForSeconds(3.0f);
        Color c = img.color;
        while (c.a < 1) {
            c.a += Time.deltaTime * 2;
            img.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(1.0f);
        youdied.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("StartScene");
    }
    public IEnumerator VellumAppear() {
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
    }
    public IEnumerator ClearCoroutine() {
        BossHPUI.SetActive(false);
        TimeLineCam.SetActive(true);
        mainCamera.SetActive(false);
        state = STATE.PAUSE;
        PadClear.Play();
        
        yield return new WaitForSeconds(3.0f);
        Color c = img.color;
        while (c.a < 1) {
            c.a += Time.deltaTime * 2;
            img.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(1.0f);
        tfp.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene("StartScene");
    }
}
