using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warning : MonoBehaviour {
    public GameObject WarningSprite;
    SpriteRenderer sr;
    int layerMask;
    public bool IsWarn;
    void Start() {
        sr= WarningSprite.GetComponent<SpriteRenderer>();

        layerMask = (1 << LayerMask.NameToLayer("Map"));
    }

    void Update() {
        if (WarningSprite.activeSelf) {
            WarningSprite.transform.Rotate(new Vector3(0, 1080f, 0)*Time.deltaTime,Space.World);
        }
    }
    public void CreateWarning() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 30.0f, layerMask)) {
            Debug.DrawRay(transform.position, Vector3.down * 30f, Color.red);
            print(hit.point);
            if (!IsWarn) {
                //WarningSprite.SetActive(true);
                StartCoroutine(ShowWarning());
                WarningSprite.transform.position = hit.point + new Vector3(0f, 0.1f, 0f);
                IsWarn = true;
            }
        }
    }
    public void DestoryWarning() {
        //WarningSprite.SetActive(false);
        Color c = sr.color;
        c.a = 0;
        sr.color = c;
        IsWarn = false;
    }
    IEnumerator ShowWarning() {
        Color c = sr.color;
        while (sr.color.a < 1) {
            c.a += Time.deltaTime * 2;
            sr.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }

    }
    IEnumerator HideWarning() {
        Color c = sr.color;
        while (sr.color.a > 0) {
            c.a -= Time.deltaTime * 2;
            sr.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }

    }
}
