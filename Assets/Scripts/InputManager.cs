using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool inputBlocked = false;
    
    public bool GetKey(KeyCode key)
    {
        if (inputBlocked)
            return false;
        return Input.GetKey(key);
    }

    public bool GetKeyDown(KeyCode key)
    {
        if (inputBlocked)
            return false;
        return Input.GetKeyDown(key);
    }

    public bool GetKeyUp(KeyCode key)
    {
        if (inputBlocked)
            return false;
        return Input.GetKeyUp(key);
    }
}
