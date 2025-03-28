using UnityEngine;

public class XpOrb : MagnetizedItem {
    [SerializeField] int xpToGain;
    public int XpToGain { get; set; }

    void Awake() {
        XpToGain = xpToGain;
    }

    protected override void ApplyPickupEffect(Player player) {
        player.XP.GainExperince(XpToGain);
    }
}