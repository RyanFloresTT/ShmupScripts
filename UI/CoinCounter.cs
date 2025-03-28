using _Project.Scripts.EventBus;
using KBCore.Refs;
using Saving;
using UnityEngine;
using UnityEngine.UIElements;

public class CoinCounter : MonoBehaviour {
    [SerializeField] [Self] UIDocument coinCounterUI;
    [SerializeField] SaveManager saveManager;

    EventBinding<OnPlayerGainedBasicCurrency> basicCurrencyBinding;
    EventBinding<OnPlayerGainedPermCurrency> permCurrencyBinding;

    VisualElement rootEl;
    Label basicText;
    Label permText;

    void OnEnable() {
        Purse purse = this.saveManager.SaveData.PlayerData.PermanentPurse;

        this.rootEl = this.coinCounterUI.rootVisualElement;
        this.basicText = this.rootEl.Q<Label>("Coins");
        this.permText = this.rootEl.Q<Label>("PermCoins");

        this.basicText.text = $"0";
        this.permText.text = $"{purse.Amount}";

        this.basicCurrencyBinding = new EventBinding<OnPlayerGainedBasicCurrency>(this.Handle_BasicCurrencyChange);
        this.permCurrencyBinding = new EventBinding<OnPlayerGainedPermCurrency>(this.Handle_PermrrencyChange);

        EventBus<OnPlayerGainedBasicCurrency>.Register(this.basicCurrencyBinding);
        EventBus<OnPlayerGainedPermCurrency>.Register(this.permCurrencyBinding);
    }

    void Handle_PermrrencyChange(OnPlayerGainedPermCurrency currency) {
        this.permText.text = $"{currency.Purse.Amount}";
    }

    void Handle_BasicCurrencyChange(OnPlayerGainedBasicCurrency currency) {
        this.basicText.text = $"{currency.Purse.Amount}";
    }

    void OnDisable() {
        EventBus<OnPlayerGainedBasicCurrency>.Register(this.basicCurrencyBinding);
        EventBus<OnPlayerGainedPermCurrency>.Register(this.permCurrencyBinding);
    }
}