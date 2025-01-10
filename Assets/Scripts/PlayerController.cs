using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f; // Tốc độ di chuyển
    public float moveStep = 0.1f; // Khoảng cách di chuyển mỗi lần nhấn

    private bool isMoving;
    private Vector2 input;
    private Vector2 previousInput; // Lưu hướng di chuyển trước đó
    private Animator animator;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Lấy đầu vào từ bàn phím
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // Nếu có đầu vào mới, lưu hướng hiện tại
        if (input != Vector2.zero)
        {
            previousInput = input;
        }
        // Nếu không có đầu vào mới, dừng lại
        else if (input == Vector2.zero && !isMoving)
        {
            previousInput = Vector2.zero;
        }

        // Nếu không đang di chuyển và có đầu vào hợp lệ, bắt đầu di chuyển
        if (!isMoving && previousInput != Vector2.zero)
        {
            animator.SetFloat("moveX", input.x);
            animator.SetFloat("moveY", input.y);

            var targetPos = transform.position;
            targetPos.x += previousInput.x * moveStep;
            targetPos.y += previousInput.y * moveStep;

            if (IsWalkable(targetPos))
            {
                StartCoroutine(Move(targetPos));
            }
        }

        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        // Di chuyển từ vị trí hiện tại đến vị trí mục tiêu
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Đảm bảo nhân vật ở đúng vị trí mục tiêu
        transform.position = targetPos;
        isMoving = false;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }
        return true;
    }

    private void Interact() 
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapCircle(interactPos, 0.2f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }
}
