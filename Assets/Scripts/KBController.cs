using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KBController : MonoBehaviour
{
    public bool autoControl = false;
    public bool rightAuto = false;
    public bool leftAuto = false;
    public bool upAuto = false;

    public InputManager inputManager;

    void Start()
    {
        inputManager = GetComponent<InputManager>();
    }

    public int RightHold()
    {
        if (inputManager.GetKey(KeyCode.D) || (autoControl && rightAuto))
            return 1;
        if (inputManager.GetKeyUp(KeyCode.D))
            return -1;
        return 0;
    }

    public int LeftHold()
    {
        if (inputManager.GetKey(KeyCode.A) || (autoControl && leftAuto))
            return 1;
        if (inputManager.GetKeyUp(KeyCode.A))
            return -1;
        return 0;
    }

    public int UpDown()
    {
        if (inputManager.GetKeyDown(KeyCode.W) || (autoControl && upAuto))
            return 1;
        return 0;
    }
}
