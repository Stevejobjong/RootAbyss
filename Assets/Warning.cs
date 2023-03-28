using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warning : MonoBehaviour {
    public GameObject WarningSprite;
    int layerMask;
    public bool IsWarn;
    void Start() {
        layerMask = (1 << LayerMask.NameToLayer("Map"));
    }

    void Update() {
        if (WarningSprite.activeSelf) {
            WarningSprite.transform.Rotate(new Vector3(0, 90f, 0)*Time.deltaTime,Space.World);
        }
    }
    public void CreateWarning() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 30.0f, layerMask)) {
            Debug.DrawRay(transform.position, Vector3.down * 30f, Color.red);
            print(hit.point);
            if (!IsWarn) {
                WarningSprite.SetActive(true);
                WarningSprite.transform.position = hit.point + new Vector3(0f, 0.1f, 0f);
                IsWarn = true;
            }
        }
    }
    public void DestoryWarning() {
        WarningSprite.SetActive(false);
        IsWarn = false;
    }
}
