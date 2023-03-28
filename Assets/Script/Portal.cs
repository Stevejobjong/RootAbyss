using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Portal : MonoBehaviour {
    bool isOn = false;
    [SerializeField] GameObject canvas;
    private void Update() {
        if (isOn) {
            if(Input.GetKeyDown(KeyCode.E)) {
                StartCoroutine(NextStage());
                isOn = false;
            }
        }
    }
    private void OnTriggerEnter(Collider other) {

        if (other.tag == "Player") {
            canvas.SetActive(true);
            isOn = true;
        }
    }
    private void OnTriggerExit(Collider other) {

        if (other.tag == "Player") {
            canvas.SetActive(false);
            isOn = false;
        }
    }
    IEnumerator NextStage() {
        SystemMng.ins.FadeOut();
        yield return new WaitForSeconds(2.0f);
        SystemMng.ins.FadeIn();
        gameObject.SetActive(false);

    }
}
