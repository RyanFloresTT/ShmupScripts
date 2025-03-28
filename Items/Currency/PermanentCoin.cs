public class PermanentCoin : Coin {
    protected override void ApplyPickupEffect(Player entity) {
        entity.PlayerPermPurse.Add(Amount);
    }
}