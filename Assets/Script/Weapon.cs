using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    BoxCollider attackArea;
    public TrailRenderer trailRenderer;
    void Start() {
        attackArea = GetComponent<BoxCollider>();
    }


    public void Attack() {
        StartCoroutine(Swing());
    }
    IEnumerator Swing() {
        attackArea.enabled = true;
        trailRenderer.enabled = true;
        yield return new WaitForSeconds(0.4f);
        attackArea.enabled = false;
        yield return new WaitForSeconds(0.6f);
        trailRenderer.enabled = false;
    }
}
