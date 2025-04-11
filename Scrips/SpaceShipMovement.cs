using UnityEngine;
using Fusion;

public class SpaceFighterController : NetworkBehaviour
{
    public CharacterController controller;
    public float moveSpeed = 5f;
    public float rotationSpeed = 2.0f; // Tốc độ xoay nhân vật
    private Vector3 velocity;
    private bool isCursorLocked = false; // Biến theo dõi trạng thái khóa con trỏ
    private float currentRotationX = 0f; // Góc quay hiện tại theo trục X
    private float maxRotationX = 80f; // Giới hạn góc quay tối đa theo trục X

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        float mouseX = Input.GetAxis("Mouse X"); // Lấy input từ chuột X
        float mouseY = Input.GetAxis("Mouse Y"); // Lấy input từ chuột Y

        // Tính toán hướng di chuyển
        Vector3 moveVertical = transform.forward * verticalInput;
        Vector3 moveHorizontal = transform.right * horizontalInput;
        Vector3 move = (moveVertical + moveHorizontal).normalized;

        // Giảm tốc độ khi di chuyển ngang
        float currentMoveSpeed = moveSpeed;
        if (horizontalInput != 0)
        {
            currentMoveSpeed = moveSpeed * 0.5f;
        }

        // Di chuyển nhân vật
        controller.Move((move * currentMoveSpeed + velocity) * Time.fixedDeltaTime);

        // Xoay nhân vật dựa trên input chuột
        if (isCursorLocked)
        {
            transform.Rotate(Vector3.up * mouseX * rotationSpeed);

            // Xoay theo trục X và giới hạn góc quay
            currentRotationX -= mouseY * rotationSpeed; // Đảo ngược hướng xoay
            currentRotationX = Mathf.Clamp(currentRotationX, -maxRotationX, maxRotationX);
            transform.rotation = Quaternion.Euler(currentRotationX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }

        // Xử lý khóa/mở khóa con trỏ
        if (Input.GetKeyDown(KeyCode.Y))
        {
            isCursorLocked = !isCursorLocked; // Đảo ngược trạng thái
            if (isCursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}