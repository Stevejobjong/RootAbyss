using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Tip : MonoBehaviour {
    [SerializeField] TMP_Text tipTxt;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && SystemMng.ins.state == SystemMng.STATE.PLAY) {
            StopAllCoroutines();
            StartCoroutine(ShowTip());
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            StopAllCoroutines();
            StartCoroutine(HideTip());
        }
    }
    IEnumerator ShowTip() {
        Color c = tipTxt.color;
        while (c.a < 1) {
            c.a += Time.deltaTime * 2;
            tipTxt.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    IEnumerator HideTip() {
        Color c = tipTxt.color;
        while (c.a > 0) {
            c.a -= Time.deltaTime * 2;
            tipTxt.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    public void TipOff() {
        Color c = tipTxt.color;
        c.a = 0;
        tipTxt.color = c;
    }
}
