```csharp
using UnityEngine;
using VoxelFPS.Core;

namespace VoxelFPS.Player
{
    /// <summary>
    /// AAA-style FPS Controller customized for a Voxel environment.
    /// Handles Rigidbody/Character physics, mouse look, and basic block interaction.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public Transform CameraTransform;
        private CharacterController controller;

        [Header("Movement")]
        private float speed = 8.5f;
        private float gravity = -19.81f;
        private float jumpHeight = 2f;
        private Vector3 velocity;
        private bool isGrounded;

        [Header("Look")]
        public float mouseSensitivity = 100f;
        private float xRotation = 0f;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            
            if(GameManager.Instance != null)
            {
                speed = GameManager.Instance.ActiveConfig.BaseMovementSpeed;
                gravity = GameManager.Instance.ActiveConfig.Gravity;
            }
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
                return;

            HandleLook();
            HandleMovement();
            HandleInteraction();
        }

        private void HandleLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            CameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        private void HandleMovement()
        {
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        private void HandleInteraction()
        {
            // Left click to mine voxels
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(CameraTransform.position, CameraTransform.forward, out RaycastHit hit, 5f))
                {
                    World.VoxelChunk chunk = hit.collider.GetComponent<World.VoxelChunk>();
                    if (chunk != null)
                    {
                        // Calculate local hit point slightly inside the block to guarantee correct block ID modification
                        Vector3 hitPoint = hit.point - hit.normal * 0.1f;
                        Vector3 localPos = chunk.transform.InverseTransformPoint(hitPoint);
                        chunk.ModifyVoxel(localPos, 0); // 0 = Air (Mine block)
                    }
                }
            }
            
            // Right click to place voxels
            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(CameraTransform.position, CameraTransform.forward, out RaycastHit hit, 5f))
                {
                    World.VoxelChunk chunk = hit.collider.GetComponent<World.VoxelChunk>();
                    if (chunk != null)
                    {
                        Vector3 hitPoint = hit.point + hit.normal * 0.1f; // Slightly outside
                        Vector3 localPos = chunk.transform.InverseTransformPoint(hitPoint);
                        chunk.ModifyVoxel(localPos, 1); // 1 = Solid (Place block)
                    }
                }
            }
        }
    }
}

```
