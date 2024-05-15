using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossPattern : MonoBehaviour {
    public bool patternFirst = false;
    protected float min = -10.0f;
    protected float max = -0.1f;
    protected bool isDead = false;
    public Transform t;
    public Warning warning;
    protected virtual void Update() {
        if (SystemMng.ins.state == STATE.PAUSE || SystemMng.ins.state == STATE.CLEAR || isDead)
            return;
        Detect();
    }
    public void Detect() {
        Vector3 d = Vector3.Scale(Player.ins.transform.position, new Vector3(1, 0, 1));
        Vector3 s = Vector3.Scale(t.position, new Vector3(1, 0, 1));
        t.rotation = Quaternion.Slerp(t.rotation, Quaternion.LookRotation(d - s), Time.deltaTime * 10);

    }
    protected IEnumerator Burrow() {
        while (gameObject.transform.position.y > min) {
            transform.Translate(0, -Time.deltaTime * 20, 0);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    public virtual IEnumerator Out() {
        yield return null;
    }
    protected virtual IEnumerator Move() {
        yield return null;
    }
    public virtual IEnumerator Pattern1() {
        yield return null;
    }
}
