using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Draggable : ClickHandler
{
    public bool Interactable = true;
    public Vector3 ConstraintAxis;
    public UnityEvent OnDragUpdate = new UnityEvent();
    private Coroutine _dragCoroutine;
    private bool _dragging = false;
    private (Ray, float) _startRay;
    private Vector3 _startPos;

    private void OnEnable()
    {
        this.OnPressStart.AddListener(StartDrag);
        this.OnPressEnd.AddListener(StopDrag);
    }

    private void OnDisable()
    {
        StopDrag();
        this.OnPressStart.RemoveListener(StartDrag);
        this.OnPressEnd.RemoveListener(StopDrag);
    }

    private void StartDrag()
    {
        // check if we can drag / clean up old drag
        if (!Interactable || _dragging) return;
        if (_dragCoroutine != null)
            StopCoroutine(_dragCoroutine);

        // start drag
        _startRay = (this.mouseRay, this.mouseHit.distance);
        _startPos = this.transform.position;
        _dragging = true;
        _dragCoroutine = StartCoroutine(DoDrag());
    }

    private void StopDrag()
    {
        _dragging = false;
    }

    private IEnumerator DoDrag()
    {
        Vector3 delta = Vector3.zero;
        while (Interactable && _dragging)
        {
            // get delta in 3D based on camera plane
            Vector3 mousePos3D = this.mouseRay.origin + this.mouseRay.direction * _startRay.Item2;
            delta = mousePos3D - (_startRay.Item1.origin + _startRay.Item1.direction * _startRay.Item2);

            // constrain to local axis if needed
            if (ConstraintAxis.magnitude > 0)
            {
                delta = Vector3.Project(delta, transform.parent.TransformDirection(ConstraintAxis.normalized));
            }

            // apply delta
            this.transform.position = _startPos + delta;
            OnDragUpdate.Invoke();
            yield return null;
        }

        // clean up
        _dragCoroutine = null;
    }

    /// <summary>
    /// Projects a world position onto the constraint axis
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns>the world position of the point projected onto the axis</returns>
    public Vector3 ProjectOntoConstraint(Vector3 worldPos)
    {
        if (ConstraintAxis.magnitude == 0) return Vector3.zero;
        return transform.TransformPoint(ProjectOntoLocalConstraint(worldPos));
    }

    /// <summary>
    /// Projects a world position onto the constraint axis
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns>the local position of the point projected onto the axis</returns>
    public Vector3 ProjectOntoLocalConstraint(Vector3 worldPos)
    {
        if (ConstraintAxis.magnitude == 0) return Vector3.zero;

        // Get reference points in local space on line
        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        Vector3 proj = Vector3.Project(localPos, ConstraintAxis.normalized);
        return proj;
    }
}
