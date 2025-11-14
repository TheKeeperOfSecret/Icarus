using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private Movement movement;

    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<Movement>();
    }

    void Update()
    {
        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        animator.SetBool("PlayerFly", movement.isJumping && 
            GameManager.Instance.currentState != GameManager.GameState.GameOver);
    }
}
