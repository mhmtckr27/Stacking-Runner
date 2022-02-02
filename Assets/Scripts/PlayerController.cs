using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;


    private static PlayerController instance;
    public static PlayerController Instance { get => instance; private set => instance = value; }

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

    public void StartRunning()
    {
        InputManager.Instance.EnableInput(true);
        SetMoveSpeed(3);
    }

    private void OnEnable()
    {
        InputManager.Instance.OnMovementInput += OnMovementInput;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnMovementInput -= OnMovementInput;
    }

    private void SetMoveSpeed(float speed)
    {
        rb.velocity = new Vector3(0, 0, speed);
        animator.SetFloat("Velocity", speed);
    }

    private void OnMovementInput(float moveAmount)
    {
        transform.Translate(moveAmount, 0, 0);
    }

}
