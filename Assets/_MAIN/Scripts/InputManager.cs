using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [Header("Desktop Controls")]
    public UnityEvent OnSpaceDown = new UnityEvent();
    public UnityEvent OnSpaceUp = new UnityEvent();

    [Header("Quest Controls")]
    public UnityEvent OnADown = new UnityEvent();
    public UnityEvent OnAUp = new UnityEvent();

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space)) OnSpaceDown.Invoke();
        if (Input.GetKeyUp(KeyCode.Space)) OnSpaceUp.Invoke();

        //if (Unity.XR.Oculus.Input.OculusRemote.)
    }
}
