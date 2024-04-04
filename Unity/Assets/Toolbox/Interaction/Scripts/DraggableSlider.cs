using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DraggableSlider : Draggable
{

    [Header("Slider Settings")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Draggable _handle;
    [Tooltip("The local point on the line that the handle will be constrained to. The track will be drawn between this point and the min position.")]
    [SerializeField] private Vector3 _maxPosition = new Vector3(-1, 0, 0);
    [Tooltip("The local point on the line that the handle will be constrained to. The track will be drawn between this point and the max position.")]
    [SerializeField] private Vector3 _minPosition = new Vector3(0, 0, 0);
    [Tooltip("Adds padding to the track. This is useful if you want to make the slider shorter than the track.")]
    [SerializeField] private Padding _trackPadding;

    public LineRenderer LineRenderer { get => _lineRenderer; }
    public Draggable Handle { get => _handle; }
    public Vector3 MaxPosition
    {
        get => _maxPosition;
        set
        {
            _maxPosition = value;
            if (LineRenderer) LineRenderer.SetPosition(1, value);
            ConstraintAxis = (value - MinPosition).normalized;
            Handle.ConstraintAxis = (value - MinPosition).normalized;
        }
    }
    public Vector3 MinPosition
    {
        get => _minPosition;
        set
        {
            _minPosition = value;
            if (LineRenderer) LineRenderer.SetPosition(0, value);
            ConstraintAxis = (MaxPosition - value).normalized;
            Handle.ConstraintAxis = (MaxPosition - value).normalized;
        }
    }
    public Padding TrackPadding
    {
        get => _trackPadding;
        set
        {
            _trackPadding = value;
            SetValue(t); // reposition handle
        }
    }
    public float t { get; private set; }


    public UnityEvent<float> OnSliderUpdate = new UnityEvent<float>();




    public Transform Follow;
    public float Value;
    public bool Button;
    private void Update()
    {
        if (Follow != null)
        {
            SetHandlePosition(Follow.position);
        }

        if (Button)
        {
            Button = false;
            SetValue(Value);
        }
    }



    private void OnEnable()
    {
        // setup line renderer
        if (LineRenderer)
        {
            LineRenderer.useWorldSpace = false;
            LineRenderer.positionCount = 2;
        }
        MaxPosition = MaxPosition;
        MinPosition = MinPosition;

        // setup handle
        Handle.OnDragUpdate.AddListener(AfterDragUpdate);
        var rends = Handle.GetComponentsInChildren<MeshRenderer>();
        foreach (var rend in rends)
        {
            rend.material.SetInt("_Disabled", Interactable ? 0 : 1);
        }
    }

    private void OnDisable()
    {
        Handle.OnDragUpdate.RemoveListener(AfterDragUpdate);
    }

    private void AfterDragUpdate()
    {
        Vector3 paddedMin = MinPosition + ConstraintAxis * TrackPadding.Min;
        Vector3 paddedMax = MaxPosition - ConstraintAxis * TrackPadding.Max;
        float t = Vector3.Dot(Handle.transform.localPosition - paddedMin, ConstraintAxis) / (paddedMax - paddedMin).magnitude;
        this.t = Mathf.Clamp(t, 0, 1);
        Handle.transform.localPosition = Vector3.Lerp(paddedMin, paddedMax, this.t);
        OnSliderUpdate.Invoke(t);
    }

    public void SetValue(float t)
    {
        this.t = Mathf.Clamp(t, 0, 1);
        Vector3 paddedMin = MinPosition + ConstraintAxis * TrackPadding.Min;
        Vector3 paddedMax = MaxPosition - ConstraintAxis * TrackPadding.Max;
        Handle.transform.localPosition = Vector3.Lerp(paddedMin, paddedMax, t);
    }

    public void SetHandlePosition(Vector3 worldPos)
    {
        SetValue(PositionToPercent(worldPos));
    }

    public float PositionToPercent(Vector3 worldPos)
    {
        // Get reference points in local space on line
        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        localPos = Vector3.Project(localPos, ConstraintAxis.normalized);

        // Get percentage between min and max
        Vector3 paddedMin = MinPosition + ConstraintAxis * TrackPadding.Min;
        Vector3 paddedMax = MaxPosition - ConstraintAxis * TrackPadding.Max;
        float t = Vector2.Dot(localPos - paddedMin, ConstraintAxis) / (paddedMax - paddedMin).magnitude;
        return Mathf.Clamp(t, 0, 1);
    }

    [System.Serializable]
    public struct Padding
    {
        public float Min;
        public float Max;
    }
}
