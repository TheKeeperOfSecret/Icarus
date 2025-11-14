using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KBController : MonoBehaviour
{
    public bool autoControl = false;
    public bool rightAuto = false;
    public bool leftAuto = false;
    public bool upAuto = false;

    public int RightHold()
    {
        if (InputManager.GetKey(KeyCode.D) || (autoControl && rightAuto))
            return 1;
        if (InputManager.GetKeyUp(KeyCode.D))
            return -1;
        return 0;
    }

    public int LeftHold()
    {
        if (InputManager.GetKey(KeyCode.A) || (autoControl && leftAuto))
            return 1;
        if (InputManager.GetKeyUp(KeyCode.A))
            return -1;
        return 0;
    }

    public int UpDown()
    {
        if (InputManager.GetKeyDown(KeyCode.W) || (autoControl && upAuto))
            return 1;
        return 0;
    }
}
