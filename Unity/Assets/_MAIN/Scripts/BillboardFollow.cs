using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// TODO: handle gimball lock
public class BillboardFollow : MonoBehaviour
{
    public bool Follow = true;
    public Vector3 Offset = Vector3.zero;
    public Vector3 RotationOffset = Vector3.zero;
    public Vector3 PositionMultiplier = Vector3.zero;
    public float SmoothTime = 0.3f;
    public BillboardParams RotationParams;
    public const float ARRIVAL_PRECISION = 0.001f;
    public float Precision = -1;

    private Vector3 velocity = Vector3.zero;
    public Transform target;
    private Transform _target; // placeholder for target while damping to a position
    public Transform secondaryTarget;
    public UnityEvent OnReachedTarget = new UnityEvent();
    private List<System.Action> OnReachedTargetActions = new List<System.Action>();
    private Coroutine DampToCoroutine;

    public Vector3 TargetPosition
    {
        get
        {
            if (TargetPositionCalculator != null)
            {
                return TargetPositionCalculator();
            }
            else if (target != null)
            {
                return target.position + target.forward * PositionMultiplier.z + target.up * PositionMultiplier.y + target.right * PositionMultiplier.x + target.TransformVector(Offset);
            }
            else
            {
                return Vector3.zero;
            }
        }
    }

    public System.Func<Vector3> TargetPositionCalculator;

    private void OnEnable()
    {
        if (target && !target.gameObject.activeInHierarchy)
        {
            if (secondaryTarget && secondaryTarget.gameObject.activeInHierarchy)
            {
                target = secondaryTarget;
            }
        }


        if (target == null && Camera.main != null)
        {
            target = Camera.main.transform;
        }
        else
        {
            StartCoroutine(SetTargetAfterDelay(0.1f));
        }

        if (GetComponent<Rigidbody>())
        {
            this.velocity = GetComponent<Rigidbody>().velocity;
        }
    }

    private IEnumerator SetTargetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target == null && Camera.main != null)
        {
            target = Camera.main.transform;
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;
        impl_Follow();
        impl_Billboard();
    }

    private void impl_Follow()
    {
        if (Follow)
        {
            // move towards target
            transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref this.velocity, SmoothTime, Mathf.Infinity, Time.fixedDeltaTime);

            // check if we reached the target
            if (Vector3.Distance(transform.position, TargetPosition) < (Precision > 0 ? Precision : ARRIVAL_PRECISION))
            {
                OnReachedTarget.Invoke();
                System.Action[] onetimeCallbacks = OnReachedTargetActions.ToArray();
                OnReachedTargetActions.Clear();
                foreach (System.Action callback in onetimeCallbacks)
                {
                    callback();
                }
            }
        }
    }

    private float angularSpeed = 0;
    private void impl_Billboard(Transform target = null)
    {
        if (target == null) target = this.target;
        impl_Billboard(target.position, target.up);
    }
    private void impl_Billboard(Vector3 target, Vector3 up)
    {
        // look at target, handling gimball lock
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position, up);
        targetRotation *= Quaternion.Euler(RotationOffset);
        float dTheta = Quaternion.Angle(transform.rotation, targetRotation) * Mathf.Deg2Rad;
        if (dTheta < Precision)
        {
            angularSpeed = 0;
            return;
        }

        // if speed will take us over 1/smoothTime of the way to the target, reduce it
        angularSpeed = Mathf.Max(angularSpeed, 1 / SmoothTime);
        float smooth = angularSpeed * Time.fixedDeltaTime * RotationParams.smoothTime;
        if (Quaternion.Angle(transform.rotation, targetRotation) * Mathf.Deg2Rad < smooth)
        {
            angularSpeed *= 1 - RotationParams.decceleration;
        }
        // if speed will take us less than 1/smoothTime of the way to the target, increase it
        else if (Quaternion.Angle(transform.rotation, targetRotation) * Mathf.Deg2Rad > smooth)
        {
            angularSpeed *= 1 + RotationParams.acceleration;
        }

        // rotate towards target
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angularSpeed * Time.fixedDeltaTime);
    }

    public void OnReachedTargetOneTime(System.Action callback)
    {
        this.OnReachedTargetActions.Add(callback);
    }

    public void DampTo(Transform target, bool returnAfter = false, System.Action<Transform> onReached = null)
    {
        Collider c = target.GetComponent<Collider>();
        Vector3 targetPosition = target.position;
        if (c)
        {
            targetPosition = c.ClosestPoint(transform.position);
        }
        this.DampTo(targetPosition, returnAfter, onReached);
    }

    public void DampTo(Vector3 target, bool returnAfter = false, System.Action<Transform> onReached = null)
    {
        // Stop previous coroutine
        if (DampToCoroutine != null)
            StopCoroutine(DampToCoroutine);

        this._target = this.target;
        this.target = null;
        DampToCoroutine = StartCoroutine(DampTo(this.transform, target, SmoothTime, (Transform t) =>
        {
            this.target = _target;
            this.enabled = returnAfter;
            this.DampToCoroutine = null;
            if (onReached != null)
                onReached(t);
        }, (returnAfter) ? this : null));
    }

    public void InterruptDampTo(bool returnToTarget = true)
    {
        if (DampToCoroutine == null)
        {
            this.enabled = returnToTarget;
            return;
        }

        StopCoroutine(DampToCoroutine);
        this.target = _target;
        this.DampToCoroutine = null;
        this.enabled = returnToTarget;
    }

    public IEnumerator DampTo(Transform objectToMove, Vector3 target, float smoothTime, System.Action<Transform> onReached = null, BillboardFollow earlyBillboard = null)
    {
        yield return new WaitForFixedUpdate();

        // If object has colliders, get point on collider closest to target
        Vector3 offset = Vector3.zero;
        float originalDistance = Vector3.Distance(objectToMove.position, target);
        float precision = ARRIVAL_PRECISION;
        Collider c = objectToMove.GetComponentInChildren<Collider>();
        if (c)
        {
            offset = c.ClosestPoint(c.transform.InverseTransformPoint(target)) - objectToMove.position;
            precision = c.bounds.extents.magnitude;
            // var debug = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // debug.transform.SetParent(c.transform.parent, true);
            // debug.transform.localPosition = offset;
            // debug.transform.localScale = Vector3.one * 0.1f;
            // debug.name = "Debug 1";

            // offset = objectToMove.InverseTransformPoint(c.transform.parent.TransformPoint(offset));
            // var debug = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // debug.transform.SetParent(objectToMove, true);
            // debug.transform.localPosition = offset;
            // debug.transform.localScale = Vector3.one * 0.1f;
            // debug.name = "Debug 2";
            // Destroy(debug.GetComponent<Collider>());
        }

        // yield return new WaitForSeconds(2f);

        // interpolate over time
        Vector3 velocity = Vector3.zero;
        float remainingDistance = Vector3.Distance(objectToMove.TransformPoint(offset), target);
        while (remainingDistance > precision)
        {
            float t = 1 - remainingDistance / originalDistance;

            // set position
            // objectToMove.position = Vector3.SmoothDamp(
            //     objectToMove.position,
            //     target,
            //     ref velocity,
            //     smoothTime,
            //     Mathf.Infinity,
            //     Time.fixedDeltaTime
            //     );

            // TODO: set offset point position
            Vector3 offsetWorldPos = Vector3.SmoothDamp(
                objectToMove.TransformPoint(offset),
                target,
                ref velocity,
                smoothTime,
                Mathf.Infinity,
                Time.fixedDeltaTime
                );
            Vector3 delta = offsetWorldPos - objectToMove.TransformPoint(offset);
            // move object
            objectToMove.position += delta;

            if (earlyBillboard != null && t > 0.5f)
            {
                earlyBillboard.impl_Billboard(earlyBillboard._target); // target will be null while moving, so use _target
            }

            impl_Billboard(target, Vector3.up);

            yield return new WaitForFixedUpdate();
            remainingDistance = Vector3.Distance(objectToMove.TransformPoint(offset), target);
        }

        // set to final position
        objectToMove.position = target - offset;
        yield return new WaitForFixedUpdate();
        if (onReached != null)
            onReached(objectToMove);
    }

    public void SetOffsetFromWorldPos(Vector3 worldPos)
    {
        this.Offset = this.target.InverseTransformPoint(worldPos);
    }

    public Vector3 GetOffsetFromWorldPos(Vector3 worldPos)
    {
        return this.target.InverseTransformPoint(worldPos);
    }

    public Vector3 GetTargetFromOffset(Vector3 offset)
    {
        return target.position + target.forward * PositionMultiplier.z + target.up * PositionMultiplier.y + target.right * PositionMultiplier.x + target.TransformVector(offset);
    }

    public Vector3 GetTargetPositionFromWorldPos(Vector3 worldPos)
    {
        return GetTargetFromOffset(GetOffsetFromWorldPos(worldPos));
    }

    [System.Serializable]
    public struct BillboardParams
    {
        public float acceleration;
        public float decceleration;
        public float smoothTime;
    }
}
