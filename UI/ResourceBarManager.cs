using _Project.Scripts.EventBus;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UIElements;

// -----------------------------------------------------------------------
// This script was written by my friend who helped with the project early on
// -----------------------------------------------------------------------


public class ResourceBarManager : MonoBehaviour {
    const string WrapperCls = "resource-bar";
    const string WrapperActiveCls = "resource-bar--active";
    const string ProgressCls = "resource-bar__progress";

    [SerializeField] [Self] UIDocument uiDoc;

    EventBinding<OnPlayerResourceChanged> _resourceUsedBinding;

    VisualElement rootEl;
    VisualElement progressEl;

    int transitionDurationMS = 100; // Must match uss

    void OnEnable() {
        this.rootEl = this.uiDoc.rootVisualElement;
        this.progressEl = this.rootEl.Q(className: ProgressCls);

        this.progressEl.style.height = new Length(100, LengthUnit.Percent);
        // Subscribe to events
        this._resourceUsedBinding = new EventBinding<OnPlayerResourceChanged>(this.Handle_ResourceChanged);
        EventBus<OnPlayerResourceChanged>.Register(this._resourceUsedBinding);
    }


    void OnDisable() {
        EventBus<OnPlayerResourceChanged>.Deregister(this._resourceUsedBinding);
    }


    async UniTask UpdateProgressBar(float currentResource, float maxResource) {
        float progress = currentResource / maxResource * 100;

        this.progressEl.style.height = new Length(progress, LengthUnit.Percent);
        await UniTask.Delay(this.transitionDurationMS);
    }


    // Event handlers
    void Handle_ResourceChanged(OnPlayerResourceChanged obj) {
        _ = this.UpdateProgressBar(obj.Resource.CurrentAmount, obj.Resource.MaxResource);
    }
}