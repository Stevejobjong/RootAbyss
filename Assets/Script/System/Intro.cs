using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class Intro : MonoBehaviour {
    public Image img;
    public GameObject Btn;
    public GameObject Title;
    public PlayableDirector Pad;
    private void Start() {
        Screen.SetResolution(1920, 1080, false);
        Cursor.lockState = CursorLockMode.None;
    }
    public void BtnStart() {
        Btn.SetActive(false);
        Title.SetActive(false);
        StartCoroutine(LoadScene());
    }
    public void BtnExit() {
        Application.Quit();
    }
    IEnumerator LoadScene() {
        Pad.Play();
        yield return new WaitForSeconds(2.0f);
        Color c = img.color;
        while (c.a < 1) {
            c.a += Time.deltaTime * 2;
            img.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("BossBattleScene");

    }
}
