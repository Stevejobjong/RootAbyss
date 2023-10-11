using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;

public class SystemMng : Singleton<SystemMng> {
    public enum STATE { PLAY, PAUSE }
    public STATE state = STATE.PLAY;
    public GameObject mainCamera;
    public GameObject TimeLineCam;
    public GameObject BossHPUI;
    public PlayableDirector PadDie;
    public PlayableDirector PadVellum;
    public PlayableDirector PadClear;
    public VellumBT v;
    public VellumAI vellumAi;
    public TailAI tailAi;
    public TailBT t;
    bool isSetting = false;
    bool isClear = false;
    [SerializeField] Image img;
    [SerializeField] GameObject youdied;
    [SerializeField] GameObject tfp;
    [SerializeField] GameObject fire;
    [SerializeField] GameObject SettingUI;
    [SerializeField] Slider MouseSensitivity;
    [SerializeField] Slider Volume;
    public Enemy[] mobs;
    public Portal portal;
    public int mobCnt = 0;
    public float mouseSens;
    public float volume;
    public string path;
    public Tip tip;
    Data settingValue;
    void Start() {
        Screen.SetResolution(1920, 1080, false);
        state = STATE.PLAY;
        mobCnt = mobs.Length;
        Cursor.lockState = CursorLockMode.Locked;
        path = Application.persistentDataPath + "/SettingData.json";
        if (File.Exists(path))
            LoadData();
        else
            InitData();
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!isClear) {

                if (isSetting) {
                    HideSettingUI();
                } else {
                    ShowSettingUI();
                }
            }
        }
        if (mobCnt <= 0) {
            portal.gameObject.SetActive(true);
        }
        mouseSens = MouseSensitivity.value;
        volume = Volume.value;
    }
    public void ShowSettingUI() {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        state = STATE.PAUSE;
        SettingUI.SetActive(true);
        isSetting = true;
    }
    public void HideSettingUI() {
        SaveData();
        SettingUI.SetActive(false);
        Time.timeScale = 1f;
        state = STATE.PLAY;
        Cursor.lockState = CursorLockMode.Locked;
        isSetting = false;
    }
    public void ToMain() {
        Time.timeScale = 1f;
        state = STATE.PLAY;
        SceneManager.LoadScene("StartScene");
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

    public void InitData() {
        settingValue = new Data(200f,0.3f);
        mouseSens = 200f;
        volume = 0.3f;
        var result = JsonConvert.SerializeObject(settingValue);
        print(result);
        File.WriteAllText(path, result);
    }
    public void LoadData() {
        string JsonFile;
        if (File.Exists(path)) {
            JsonFile = File.ReadAllText(path);
            print(JsonFile);
            settingValue = JsonConvert.DeserializeObject<Data>(JsonFile);
            MouseSensitivity.value = settingValue.MouseSens;
            Volume.value = settingValue.Volume;
        }
    }

    public void SaveData() {
        settingValue.MouseSens = mouseSens;
        settingValue.Volume = volume;
        var result = JsonConvert.SerializeObject(settingValue);
        print(result);
        File.WriteAllText(path, result);
    }
    IEnumerator FadeOutCoroutine() {
        img.gameObject.SetActive(true);
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
            img.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        img.gameObject.SetActive(false);
    }
    public IEnumerator DieCoroutine() {
        SoundMng.ins.PlayBackgroundNoLoop("Death");
        tip.TipOff();
        TimeLineCam.SetActive(true);
        mainCamera.SetActive(false);
        img.gameObject.SetActive(true);
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
        /*
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
        v.StartCoroutine(v.SelectPattern());
        t.StartCoroutine(t.pattern1());
        */
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
        state = STATE.PAUSE;
        isClear = true;
        BossHPUI.SetActive(false);
        TimeLineCam.SetActive(true);
        mainCamera.SetActive(false);
        PadClear.Play();

        img.gameObject.SetActive(true);
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
    [System.Serializable]
    public class Data {
        public float MouseSens;
        public float Volume;
        public Data(float m, float v) {
            this.MouseSens = m;
            this.Volume = v;
        }
    }
}
