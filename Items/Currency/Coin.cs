using PrimeTween;
using UnityEngine;

public class Coin : MagnetizedItem {
    [field: SerializeField] public int Amount {  get; private set; }
    readonly float endVal = 1f;

    void Start() {
        Tween.PositionY(transform, endValue: endVal, duration: 1, ease: Ease.InOutSine, -1, CycleMode.Rewind);
        Tween.EulerAngles(transform, startValue: new Vector3(90, 0), endValue: new Vector3(90, 360), duration: 3, Ease.Linear, -1, CycleMode.Restart);
    }

    protected override void ApplyPickupEffect(Player entity) {
        entity.PlayerPurse.Add(Amount);
    }
}
