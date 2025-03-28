using _Project.Input;
using KBCore.Refs;
using UnityEngine;

namespace _Project.Scripts.Player {
    public class PlayerMovement : MonoBehaviour {
        static readonly int MoveX = Animator.StringToHash("MoveX");
        static readonly int MoveZ = Animator.StringToHash("MoveZ");
        [SerializeField] [Self] global::Player player;
        [SerializeField] [Self] Rigidbody rb;
        [SerializeField] [Self] Animator animator;
        InputManager inputManager;

        Vector2 movementDirection;
        Camera mainCamera;

        void Start() {
            this.mainCamera = Camera.main;
            this.inputManager = InputManager.Instance;
            this.inputManager.OnPlayer_Move += this.HandleMove;
        }

        void FixedUpdate() {
            this.UpdateMovement();
            this.UpdateAnimations();
        }

        void UpdateMovement() {
            // Transform input direction into local space relative to the player's rotation
            Vector3 movementXZ = new(this.movementDirection.x, 0f, this.movementDirection.y);
            Vector3 localMovement = this.transform.TransformDirection(movementXZ);

            this.rb.AddForce(localMovement * this.player.Stats.MoveSpeed, ForceMode.Acceleration);
        }

        void UpdateAnimations() {
            if (this.movementDirection.magnitude <= 0.01f) {
                this.animator.SetFloat(MoveX, 0f);
                this.animator.SetFloat(MoveZ, 0f);
                return;
            }

            Vector3 aimingDirection = this.GetAimingDirection();

            // Transform movement direction into local space relative to the aiming direction
            Vector3 newMoveDir =
                new Vector3(this.movementDirection.x, 0f, this.movementDirection.y).normalized;
            Vector3 localMovementDirection =
                Quaternion.Inverse(Quaternion.LookRotation(aimingDirection)) * newMoveDir;

            // Calculate the angle between the local movement direction and forward direction
            float angle = Vector3.SignedAngle(Vector3.forward, localMovementDirection, Vector3.up);

            // Convert angle to blend tree parameters
            float moveX = Mathf.Sin(angle * Mathf.Deg2Rad);
            float moveZ = Mathf.Cos(angle * Mathf.Deg2Rad);

            // Update animator parameters
            this.animator.SetFloat(MoveX, moveX);
            this.animator.SetFloat(MoveZ, moveZ);
        }

        void HandleMove(Vector2 moveDir) {
            this.movementDirection = moveDir;
        }

        Vector3 GetAimingDirection() {
            Ray ray = this.mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return this.transform.forward;
            Vector3 targetDirection = hit.point - this.transform.position;
            targetDirection.y = 0;
            return targetDirection.normalized;
        }
    }
}