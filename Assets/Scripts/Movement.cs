using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header ("Movement Settings")]
    public float moveSpeed = 1f;
    public float smoothTime = 0.3f;
    public float jumpSpeed = 5;
    public float gravity = 1f;
    public float jumpDuration = 0.3f;

    [Header ("States")]
    public bool move;
    public bool isJumping = false;
    public Vector3 currentVelocity = Vector3.zero;

    private Vector3 targetPos;
    private Vector3 dir = Vector3.zero;
    private KBController controller;
    private SpriteRenderer sprite;
    private float jumpTimer = 0f;

    void Start()
    {
        controller = GetComponent<KBController>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        GetDirection();

        if (move)
        {
            targetPos = transform.position + dir;
        }
        MoveSmoothly();
    }

    private void GetDirection()
    {
        dir = Vector3.zero;
        move = false;

        if (controller.UpDown() == 1 && !isJumping)
        {
            isJumping = true;
            jumpTimer = 0f;
        }

        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            float jumpProgress = jumpTimer / jumpDuration;

            if (jumpProgress < 1f)
            {
                dir.y = jumpSpeed * (1f - jumpProgress);
                move = true;
            }
            else
            {
                isJumping = false;
            }
        }
        else
        {
            dir.y = -gravity;
            move = true;
        }

        if (controller.LeftHold() == 1)
        {
            move = true;
            dir.x = -moveSpeed;
            sprite.flipX = true;
        }

        if (controller.RightHold() == 1)
        {
            move = true;
            dir.x = moveSpeed;
            sprite.flipX = false;
        }
    }

    private void MoveSmoothly()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref currentVelocity,
            smoothTime
        );
        move = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<DamageDealer>(out DamageDealer damageDealer))
        {
            damageDealer.DealDamage(this.gameObject);
        }
        
        if (collision.gameObject.TryGetComponent<FinalCutsceneController>(out FinalCutsceneController finalCutsceneController))
        {
            finalCutsceneController.PlayCutscene();
        }
    }
}
