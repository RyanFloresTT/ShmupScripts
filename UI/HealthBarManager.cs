using _Project.Scripts.EventBus;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UIElements;

// -----------------------------------------------------------------------
// This script was written by my friend who helped with the project early on
// -----------------------------------------------------------------------

public class HealthBarManager : MonoBehaviour {
    const string WrapperCls = "health-bar";
    const string WrapperActiveCls = "health-bar--active";
    const string ProgressCls = "health-bar__progress";

    [SerializeField] [Self] UIDocument uiDoc;

    EventBinding<OnPlayerHealthChanged> _healthChanged;

    VisualElement rootEl;
    VisualElement progressEl;

    int transitionDurationMS = 100; // Must match uss

    void OnEnable() {
        this.rootEl = this.uiDoc.rootVisualElement;
        this.progressEl = this.rootEl.Q(className: ProgressCls);
        this.progressEl.style.height = new Length(100, LengthUnit.Percent);
        // Subscribe to events
        this._healthChanged = new EventBinding<OnPlayerHealthChanged>(this.Handle_HealthChanged);
        EventBus<OnPlayerHealthChanged>.Register(this._healthChanged);
    }


    void OnDisable() {
        EventBus<OnPlayerHealthChanged>.Deregister(this._healthChanged);
    }


    async UniTask UpdateProgressBar(float health, float maxHealth) {
        float progress = health / maxHealth * 100;

        this.progressEl.style.height = new Length(progress, LengthUnit.Percent);
        await UniTask.Delay(this.transitionDurationMS);
    }


// Event handlers
    void Handle_HealthChanged(OnPlayerHealthChanged obj) {
        _ = this.UpdateProgressBar(obj.Health.CurrentHealth, obj.Health.MaxHealth);
    }
}