using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
    public Transform objectFollow;
    public float followSpeed = 10f;
    public float sensitivity = 100f;
    public float clampAngle = 40f;

    public float rotX;
    public float rotY;

    int layerMask;
    public Transform realCamera;
    public Vector3 dirNormalized;
    public Vector3 finalDir;
    public float maxDistance;
    public float minDistance;
    public float finalDistance;
    public float smoothness = 10f;
    void Start() {
        layerMask = (1 << LayerMask.NameToLayer("Map"));
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;
        sensitivity = DataMng.ins.settingValue.MouseSens;
    }

    void Update() {
        if (SystemMng.ins.state == STATE.PAUSE)
        {
            sensitivity = DataMng.ins.settingValue.MouseSens;
            return;
        }
        if (Player.ins.enemyLocked)
            return;
        rotX += -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        rotY %= 360;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        transform.rotation = Quaternion.Euler(rotX, rotY, 0);
    }
    private void LateUpdate() {
        transform.position = Vector3.MoveTowards(transform.position, objectFollow.position, followSpeed * Time.deltaTime);

        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        RaycastHit hit;

        if (Physics.Linecast(transform.position, finalDir, out hit, layerMask)) {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        } else
            finalDistance = maxDistance;
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
    }

}
