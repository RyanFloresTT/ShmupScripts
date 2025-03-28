/*
    This is a UIDocument that lives across all scene
    so we can smooth transitions on scene change.
*/

using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

// -----------------------------------------------------------------------
// This script was written by my friend who helped with the project early on
// -----------------------------------------------------------------------


namespace _Project.Scripts.UI {
    public class OverlayUIManager : MonoBehaviour {
        // Classnames / Settings
        const string WrapperCls = "overlay";
        const string WrapperActiveCls = "overlay--active";
        const int TransitionDurationMS = 500; // Must match stylesheet

        // Components
        [SerializeField] UIDocument uiDoc;

        // UI Elements
        VisualElement _rootEl;
        VisualElement _wrapperEl;

        // Components / instance
        public static OverlayUIManager Instance { get; private set; }

        // Lifecycle
        void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this) Destroy(this.gameObject);
        }

        void OnEnable() {
            this._rootEl = this.uiDoc.rootVisualElement;
            this._wrapperEl = this._rootEl.Q(className: WrapperCls);

            this.uiDoc.enabled = false;
            this._wrapperEl.RemoveFromClassList(WrapperActiveCls);
        }

        // Methods
        public async UniTask ShowOverlay() {
            Debug.Log("ShowOverlay: Starting fade-in transition...");

            this.uiDoc.enabled = true;

            // Add the active class to trigger the fade-in transition
            this._wrapperEl.AddToClassList(WrapperActiveCls);
            Debug.Log(
                $"ShowOverlay: Added overlay--active class. Current classes: {string.Join(", ", this._wrapperEl.GetClasses())}");

            // Wait for fade-in to complete
            await UniTask.Delay(TransitionDurationMS);

            Debug.Log("ShowOverlay: Transition complete.");
        }

        public async UniTask HideOverlay() {
            Debug.Log("HideOverlay: Starting fade-out transition...");


            // Start the fade-out animation by adding a "fading-out" class
            this._wrapperEl.AddToClassList("fading-out");
            Debug.Log(
                $"HideOverlay: Added fading-out class. Current classes: {string.Join(", ", this._wrapperEl.GetClasses())}");

            // Wait for the transition duration (ensure this matches CSS)
            await UniTask.Delay(TransitionDurationMS);

            // Cleanup: remove the fading class and fully deactivate the overlay
            this._wrapperEl.RemoveFromClassList("fading-out");
            Debug.Log(
                $"HideOverlay: Removed fading-out and active classes. Current classes: {string.Join(", ", this._wrapperEl.GetClasses())}");

            // Disable the UI document to hide it completely
            this.uiDoc.enabled = false;
            Debug.Log("HideOverlay: UI document disabled.");
        }
    }
}