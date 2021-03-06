using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float runSpeed = 3;
    [SerializeField] public ParticleSystem plusOneVFX;

    private Rigidbody rb;
    public Animator animator;

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
        if (!start)
        {
            animator.SetTrigger("Dance");
        }
    }

    private void OnEnable()
    {
        InputManager.Instance.OnMovementInput += OnMovementInput;
        GroundCheck.OnCurrentPlatformChange += GroundCheck_OnCurrentPlatformChange;
        LevelEndScreen.Instance.OnTapToContinue += Instance_OnTapToContinue;
    }

    private void Instance_OnTapToContinue()
    {
        animator.SetTrigger("StopDance");
    }

    private void OnDisable()
    {
        InputManager.Instance.OnMovementInput -= OnMovementInput;
        GroundCheck.OnCurrentPlatformChange -= GroundCheck_OnCurrentPlatformChange;
        LevelEndScreen.Instance.OnTapToContinue -= Instance_OnTapToContinue;
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

        if(moveAmount > 0)
        {
            newPos.x += Mathf.Lerp(0, currentPlatformCollider.bounds.size.x, moveAmount / (Screen.width / 2));
        }
        else
        {
            newPos.x += Mathf.Lerp(0, -currentPlatformCollider.bounds.size.x, -moveAmount / (Screen.width / 2));
        }

        return currentPlatformCollider.ClosestPoint(newPos);
    }
}
