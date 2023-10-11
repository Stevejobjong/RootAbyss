using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Altar : MonoBehaviour {
    bool isOn = false;
    [SerializeField] GameObject canvas;
    private void Update() {
        if (isOn) {
            if (Input.GetKeyDown(KeyCode.E)) {
                SystemMng.ins.StartCoroutine(SystemMng.ins.VellumAppear());
                this.gameObject.SetActive(false);
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
}
