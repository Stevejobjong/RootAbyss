using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMng : Singleton<UIMng>
{
    [SerializeField] Image img;
    [SerializeField] GameObject SettingUI;
    public Slider MouseSensitivity;
    public Slider Volume;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SystemMng.ins.state!=STATE.CLEAR)
            {

                if (SettingUI.activeSelf)
                {
                    HideSettingUI();
                }
                else
                {
                    ShowSettingUI();
                }
            }
        }
        if(SettingUI.activeSelf)
        {
            DataMng.ins.settingValue.Volume = Volume.value;
            DataMng.ins.settingValue.MouseSens = MouseSensitivity.value;
        }
    }
    public void ShowSettingUI()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        SystemMng.ins.state = STATE.PAUSE;
        SettingUI.SetActive(true);
        MouseSensitivity.value = DataMng.ins.settingValue.MouseSens;
        Volume.value = DataMng.ins.settingValue.Volume;
    }
    public void HideSettingUI()
    {
        DataMng.ins.SaveSettingData();
        SettingUI.SetActive(false);
        Time.timeScale = 1f;
        SystemMng.ins.state = STATE.PLAY;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ToMain()
    {
        Time.timeScale = 1f;
        SystemMng.ins.state = STATE.PLAY;
        SceneManager.LoadScene("StartScene");
    }
    public IEnumerator FadeOutCoroutine(Action NextStage = null)
    {
        img.gameObject.SetActive(true);
        Color c = img.color;
        while (c.a < 1)
        {
            c.a += Time.deltaTime * 2;
            img.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        NextStage?.Invoke();
    }
    public IEnumerator FadeInCoroutine()
    {
        Color c = img.color;
        while (c.a > 0)
        {
            c.a -= Time.deltaTime * 2;
            img.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        img.gameObject.SetActive(false);
    }
}
