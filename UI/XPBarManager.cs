using System;
using _Project.Scripts.EventBus;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UIElements;

public class XPBarManager : MonoBehaviour {
    const string WrapperCls = "xp-bar";
    const string WrapperActiveCls = "xp-bar--active";
    const string ProgressCls = "xp-bar__progress";

    [SerializeField] [Self] UIDocument uiDoc;

    EventBinding<OnPlayerExperienceGained> experiencedGained;

    VisualElement rootEl;
    VisualElement progressEl;

    int currentLevel = 1;
    int transitionDurationMS = 100; // Must match uss
    int fullDelayMS = 200; // How long should it show 100% before clear
    float progress;
    UniTaskCompletionSource progressBarUpdateTask;

    void OnEnable() {
        this.rootEl = this.uiDoc.rootVisualElement;
        this.progressEl = this.rootEl.Q(className: ProgressCls);

        // Subscribe to events
        this.experiencedGained = new EventBinding<OnPlayerExperienceGained>(this.HandleExperienceGained);
        EventBus<OnPlayerExperienceGained>.Register(this.experiencedGained);
    }


    void OnDisable() {
        EventBus<OnPlayerExperienceGained>.Deregister(this.experiencedGained);
    }


    async UniTask UpdateProgressBar(float xp, float xpRequirement) {
        if (xp > xpRequirement) xp -= xpRequirement;
        if (xpRequirement > 0) this.progress = Mathf.Clamp(xp / xpRequirement * 100, 0, 100);

        this.progressEl.style.width = new Length(this.progress, LengthUnit.Percent);

        await UniTask.Delay(this.transitionDurationMS);
    }


// Event handlers
    void HandleExperienceGained(OnPlayerExperienceGained obj) {
        if (this.progressBarUpdateTask != null && !this.progressBarUpdateTask.Task.GetAwaiter().IsCompleted)
            return;

        this.progressBarUpdateTask = new UniTaskCompletionSource();
        this.UpdateProgressBar(obj.Data.CurrentXP, obj.Data.XPRequirement)
            .ContinueWith(() => this.progressBarUpdateTask.TrySetResult());
    }
}