using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float deathVelocity = 2f;

    public void DealDamage(GameObject target)
    {
        if (GameManager.Instance.currentState == GameManager.GameState.GameOver) return;

        Movement movement = target.GetComponent<Movement>();
        if (movement != null)
        {
            movement.moveSpeed /= deathVelocity;
            movement.gravity *= deathVelocity;
            movement.jumpSpeed *= deathVelocity;
            movement.jumpDuration *= deathVelocity / 1.5f;
            movement.isJumping = true;
        }

        KBController controller = target.GetComponent<KBController>();
        if (controller != null)
        {
            controller.inputManager.inputBlocked = true;
            controller.autoControl = true;

            if (target.transform.position.x >= 0)
                controller.rightAuto = true;
            else
                controller.leftAuto = true;
        }

        GameManager.Instance.TriggerGameOver();
    }
}
