using _Project.Scripts.Player;
using UnityEngine;
using UnityEngine.UIElements;

// -----------------------------------------------------------------------
// This script was written by my friend who helped with the project early on
// -----------------------------------------------------------------------


public class EnemyHealthBar : MonoBehaviour {
    // UI Classes
    const string barCls = "enemy-health-bar";
    const string barActiveCls = "enemy-health-bar--active";
    const string healthCls = "enemy-health-bar__health";

    // Components
    [SerializeField] Character character;
    [SerializeField] UIDocument uiDoc;
    Camera mainCamera;

    // Settings
    [SerializeField] float xOffet = -60;
    [SerializeField] float yOffet = -60;

    // UI Elements
    VisualElement rootEl;
    VisualElement barEl;
    VisualElement healthEl;

    void Awake() {
        if (this.mainCamera == null) this.mainCamera = Camera.main;
    }

    void Start() {
        this.SetPosition();
    }

    void OnEnable() {
        this.rootEl = this.uiDoc.rootVisualElement;
        this.barEl = this.rootEl.Q(className: barCls);
        this.healthEl = this.rootEl.Q(className: healthCls);

        // Always display bar
        this.DisplayHealth();
    }

    void FixedUpdate() {
        // 50fps is fine for this, performance yo!
        this.SetPosition();
        this.SetWidth();
    }

    void SetPosition() {
        if (this.mainCamera == null) {
            Debug.LogError("Main camera or enemy transform is not set.");
            return;
        }

        float offsetY = this.transform.localScale.y / 2;

        // Create a position vector
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(this.rootEl.panel,
            this.transform.position + new Vector3(0, offsetY, 0), this.mainCamera
        );

        // Assuming the rootEl is the container of the health bar
        this.rootEl.style.left = newPosition.x + this.xOffet;
        this.rootEl.style.top = newPosition.y + this.yOffet;
    }

    void SetWidth() {
        float percentage = this.character.Health.CurrentHealth / this.character.Health.MaxHealth * 100;
        this.healthEl.style.width = new Length(percentage, LengthUnit.Percent);
    }

    public void DisplayHealth() {
        this.barEl.AddToClassList(barActiveCls);
    }

    public void HideHealth() {
        this.barEl.RemoveFromClassList(barActiveCls);
    }
}