using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : Boss {

    public override IEnumerator pattern1() {
        StartCoroutine(Move());
        yield return new WaitForSeconds(2.0f);
        warning.CreateWarning();
        yield return new WaitForSeconds(1.5f);
        warning.DestoryWarning();
        StartCoroutine(Out());
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Burrow());
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(pattern1());
    }
    public override IEnumerator Out() {
        while (gameObject.transform.position.y < max) {
            transform.Translate(0, Time.deltaTime * 20, 0);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    protected override IEnumerator Move() {
        Vector3 BossPos = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
        Vector3 PlayerPos = Vector3.Scale(Player.ins.transform.position, new Vector3(1, 0, 1));

        while (Vector3.Distance(BossPos, PlayerPos) > 1f) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(PlayerPos.x, transform.position.y, PlayerPos.z), 20 * Time.deltaTime);
            BossPos = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
            PlayerPos = Vector3.Scale(Player.ins.transform.position, new Vector3(1, 0, 1));
            yield return new WaitForSeconds(Time.deltaTime);
        }

    }
}
