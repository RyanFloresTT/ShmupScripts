using _Project.Input;
using UnityEngine;

namespace _Project.Scripts.Player {
    public class PlayerLook : MonoBehaviour {
        Camera mainCamera;
        Vector2 lookDirection;
        InputManager inputManager;

        void Awake() {
            this.mainCamera = Camera.main;
        }

        void Update() {
            this.UpdateLook();
        }

        void Start() {
            this.inputManager = InputManager.Instance;
            this.inputManager.OnPlayer_Look += this.HandleLook;
        }

        void OnDisable() {
            if (this.inputManager != null) this.inputManager.OnPlayer_Look -= this.HandleLook;
        }

        void UpdateLook() {
            if (this.lookDirection == Vector2.zero) return;

            Ray ray = this.mainCamera.ScreenPointToRay(new Vector3(this.lookDirection.x, this.lookDirection.y, 0));

            Plane groundPlane = new(Vector3.up, this.transform.position);

            if (!groundPlane.Raycast(ray, out float enter)) return;
            Vector3 worldPosition = ray.GetPoint(enter);

            Vector3 direction = (worldPosition - this.transform.position).normalized;
            direction.y = 0f;

            this.transform.forward = direction;
        }

        void HandleLook(Vector2 inputDirection) {
            this.lookDirection = inputDirection;
        }
    }
}