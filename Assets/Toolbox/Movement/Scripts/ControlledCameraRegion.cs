using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ControlledCameraRegion : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    public UnityEvent OnEnter = new UnityEvent();
    public UnityEvent OnExit = new UnityEvent();

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }

    public void Leave(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        OnExit.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnExit.Invoke();
        }
    }

    private (MeshRenderer, bool)[] _characterRenderers;
    public void DisableCharacterController()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = false;
        MeshRenderer[] renderers = GameObject.FindGameObjectWithTag("Player").GetComponentsInChildren<MeshRenderer>();
        _characterRenderers = new (MeshRenderer, bool)[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            _characterRenderers[i] = (renderers[i], renderers[i].enabled);
            renderers[i].enabled = false;
        }
    }

    public void EnableCharacterController()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = true;
        foreach (var (renderer, enabled) in _characterRenderers)
        {
            renderer.enabled = enabled;
        }
    }

    public void MoveCharacterOutside()
    {
        var controller = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        Vector3 vectorOut = _collider.ClosestPointOnBounds(controller.transform.position) - _collider.transform.position;

        bool wasEnabled = controller.enabled;
        controller.enabled = false;
        controller.transform.position = _collider.transform.position + vectorOut + vectorOut.normalized * controller.radius * 2;
        controller.enabled = wasEnabled;
    }
}
