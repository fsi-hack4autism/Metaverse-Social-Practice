using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerHandler : MonoBehaviour
{
    [Tooltip("If you set tags, the trigger will only fire if the other object has one of these tags.")]
    public string[] Tags;
    [Tooltip("After being triggered, waits this long before being able to trigger again.")]
    public float Cooldown = 0f;
    private float _cooldownTimer = 0f;
    public UnityEvent OnTriggerStart;
    public UnityEvent OnTriggerEnd;

    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnEnable()
    {
        _cooldownTimer = Time.time - Cooldown;
    }

    private void OnTriggerEnter(Collider other)
    {
        // handle cooldown
        if (Time.time - _cooldownTimer < Cooldown) return;

        if (Tags.Length > 0)
        {
            foreach (string tag in Tags)
            {
                if (other.CompareTag(tag))
                {
                    OnTriggerStart.Invoke();
                    _cooldownTimer = Time.time;
                    return;
                }
            }
        }
        else
        {
            OnTriggerStart.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Tags.Length > 0)
        {
            foreach (string tag in Tags)
            {
                if (other.CompareTag(tag))
                {
                    OnTriggerEnd.Invoke();
                    return;
                }
            }
        }
        else
        {
            OnTriggerEnd.Invoke();
        }
    }
}
