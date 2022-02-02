using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float runSpeed = 3;
    private Rigidbody rb;
    private Animator animator;

    private static PlayerController instance;
    public static PlayerController Instance { get => instance; private set => instance = value; }

    private Collider currentPlatformCollider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        rb = GetComponentInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

    }

    private void GroundCheck_OnCurrentPlatformChange(Collider newPlatformCollider)
    {
        currentPlatformCollider = newPlatformCollider;
        transform.position = ValidateMovementInput(0);
    }

    public void StartRunning(bool start)
    {
        InputManager.Instance.EnableInput(start);
        SetMoveSpeed(start ? runSpeed : 0);
    }

    private void OnEnable()
    {
        InputManager.Instance.OnMovementInput += OnMovementInput;
        GroundCheck.OnCurrentPlatformChange += GroundCheck_OnCurrentPlatformChange;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnMovementInput -= OnMovementInput;
        GroundCheck.OnCurrentPlatformChange -= GroundCheck_OnCurrentPlatformChange;
    }

    private void SetMoveSpeed(float speed)
    {
        rb.velocity = new Vector3(0, 0, speed);
        animator.SetFloat("Velocity", speed);
    }

    private void OnMovementInput(float moveAmount)
    {
        Vector3 newPos = ValidateMovementInput(moveAmount);
        transform.position = newPos;
    }

    private Vector3 ValidateMovementInput(float moveAmount)
    {
        Vector3 newPos = transform.position;
        newPos.x += moveAmount;

        return currentPlatformCollider.ClosestPoint(newPos);
    }
}
