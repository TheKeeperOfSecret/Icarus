using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static bool inputBlocked = false;
    
    public static bool GetKey(KeyCode key)
    {
        if (inputBlocked)
            return false;
        return Input.GetKey(key);
    }

    public static bool GetKeyDown(KeyCode key)
    {
        if (inputBlocked)
            return false;
        return Input.GetKeyDown(key);
    }

    public static bool GetKeyUp(KeyCode key)
    {
        if (inputBlocked)
            return false;
        return Input.GetKeyUp(key);
    }
}
