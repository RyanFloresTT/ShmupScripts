using PrimeTween;
using System.Collections;
using UnityEngine;

public abstract class MagnetizedItem : Pickup {
    Coroutine moveCoroutine;

    public void StartMovingToPlayer(Transform player) {
        if (moveCoroutine != null) {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveToPlayer(player));
    }

    public void StopMoving() {
        if (moveCoroutine != null) {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    IEnumerator MoveToPlayer(Transform player) {
        while (true) {
            Tween.Position(transform, player.position, 0.1f, Ease.Linear);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
