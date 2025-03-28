using _Project.Scripts.EventBus;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class CameraZoomController : MonoBehaviour {
    [SerializeField] float maxZoom = 40f;
    [SerializeField] float minZoom = 120f;
    [SerializeField] float zoomSpeed = 5f; // Speed of zoom interpolation

    CinemachineCamera cam;
    EventBinding<OnPlayerZoomEvent> _zoomBinding;
    float targetZoom;
    bool isZooming = false;

    // Lifecycle
    void OnEnable() {
        this._zoomBinding = new EventBinding<OnPlayerZoomEvent>(this.HandleZoom);
        EventBus<OnPlayerZoomEvent>.Register(this._zoomBinding);
    }

    void OnDisable() {
        EventBus<OnPlayerZoomEvent>.Deregister(this._zoomBinding);
    }

    void Start() {
        this.cam = this.GetComponent<CinemachineCamera>();
        this.targetZoom = this.cam.Lens.FieldOfView;
    }

    void HandleZoom(OnPlayerZoomEvent evt) {
        float zoomAmount = evt.Scroll.y / 10;
        this.targetZoom -= zoomAmount;
        this.targetZoom = Mathf.Clamp(this.targetZoom, this.maxZoom, this.minZoom);

        if (!this.isZooming) this.SmoothZoomAsync().Forget();
    }

    async UniTask SmoothZoomAsync() {
        this.isZooming = true;
        float currentZoom = this.cam.Lens.FieldOfView;

        while (Mathf.Abs(currentZoom - this.targetZoom) > 0.01f) {
            currentZoom = Mathf.Lerp(currentZoom, this.targetZoom, this.zoomSpeed * Time.deltaTime);
            this.cam.Lens.FieldOfView = currentZoom;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        this.cam.Lens.FieldOfView = this.targetZoom; // Ensure final value is set
        this.isZooming = false;
    }
}