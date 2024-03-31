using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformTools
{
    public static float EaseIn(this Transform transform, float t, float pow = 2)
    {
        return Mathf.Pow(t, pow);
    }

    public static float EaseOut(this Transform transform, float t, float pow = 2)
    {
        return 1 - Mathf.Pow(1 - t, pow);
    }

    public static float EaseInOut(this Transform transform, float t, float pow = 2)
    {
        return t < 0.5f ? transform.EaseIn(t * 2, pow) / 2 : transform.EaseOut(t * 2 - 1, pow) / 2 + 0.5f;
    }

    public static float EaseInElastic(this Transform transform, float t)
    {
        float c4 = (2 * Mathf.PI) / 3;

        return t == 0
          ? 0
          : t == 1
          ? 1
          : -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * c4);
    }

    public static float EaseOutElastic(this Transform transform, float t)
    {
        float c4 = (2 * Mathf.PI) / 3;

        return t == 0
          ? 0
          : t == 1
          ? 1
          : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
    }

    public static float EaseInOutElastic(this Transform transform, float t)
    {
        float c5 = (2 * Mathf.PI) / 4.5f;

        return t == 0
          ? 0
          : t == 1
          ? 1
          : t < 0.5
          ? -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2
          : (Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2 + 1;
    }

    public static float BounceIn(this Transform transform, float t)
    {
        return 1 - transform.BounceOut(1 - t);
    }

    public static float BounceOut(this Transform transform, float t)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (t < 1 / d1)
        {
            return n1 * t * t;
        }
        else if (t < 2 / d1)
        {
            return n1 * (t -= 1.5f / d1) * t + 0.75f;
        }
        else if (t < 2.5 / d1)
        {
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        }
        else
        {
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }
    }

    public static float BounceInOut(this Transform transform, float t)
    {
        return t < 0.5f ? transform.BounceIn(t * 2) / 2 : transform.BounceOut(t * 2 - 1) / 2 + 0.5f;
    }


    // ==========	SAMPLE ANIMATIONS	==========
    // Some animations that are often reused

    /// <summary>
    /// Scales the transform in over time,
    /// starts at (0, 0, 0) and ends at (1, 1, 1)
    /// </summary>
    /// <param name="transform">the transform to scale in</param>
    /// <param name="duration">how many seconds the scale should take</param>
    /// <param name="pow">the ease in/out behavior power</param>
    /// <returns></returns>
    public static IEnumerator ScaleIn(this Transform transform, float duration, float pow = 2)
    {
        float startTime = Time.time;
        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.one;
        transform.gameObject.SetActive(true);

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            t = transform.EaseInOut(t, pow);
            transform.localScale = Vector3.Lerp(start, end, t);
            yield return new WaitForFixedUpdate();
        }

        transform.localScale = end;
        yield return new WaitForFixedUpdate();
    }

    /// <summary>
    /// Scales the transform out over time,
    /// starts at (1, 1, 1) and ends at (0, 0, 0)
    /// </summary>
    /// <param name="transform">the transform to scale out</param>
    /// <param name="duration">how many seconds the scale should take</param>
    /// <param name="pow">the ease in/out behavior power</param>
    /// <param name="disable">should the object be disabled when the scale is complete?</param>
    /// <returns></returns>
    public static IEnumerator ScaleOut(this Transform transform, float duration, float pow = 2, bool disable = false)
    {
        float startTime = Time.time;
        Vector3 start = transform.localScale;
        Vector3 end = Vector3.zero;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            t = transform.EaseInOut(t, pow);
            transform.localScale = Vector3.Lerp(start, end, t);
            yield return new WaitForFixedUpdate();
        }

        transform.localScale = end;
        if (disable)
            transform.gameObject.SetActive(false);
        yield return new WaitForFixedUpdate();
    }






    // ==========	TRANSFORMS	==========
    // Moving vectors between local, world, and intermidiate spaces

    /// <summary>
    /// Converts a point from a local transform's space to the transform's parent space
    /// parent_T_point = parent_T_transform * transform_T_point
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="point">transform_T_point</param>
    /// <returns></returns>
    /// <remarks>
    /// TODO: Test this
    /// </remarks>
    public static Vector3 TransformPointOnce(this Transform transform, Vector3 point)
    {
        // apply transform to point
        Vector3 ret;
        // scale
        ret = Vector3.Scale(point, transform.localScale);
        // rotation
        ret = transform.localRotation * ret;
        // position
        ret += transform.localPosition;
        return ret;
    }

    /// <summary>
    /// Converts a point from a parent transform's space to the transform's local space
    /// transform_T_point = transform_T_parent * parent_T_point
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="point">parent_T_point</param>
    /// <returns></returns>
    /// <remarks>
    /// TODO: Test this
    /// </remarks>
    public static Vector3 InverseTransformPointOnce(this Transform transform, Vector3 point)
    {
        // apply inverse transform to point
        Vector3 ret;
        // position
        ret = point - transform.localPosition;
        // rotation
        ret = Quaternion.Inverse(transform.localRotation) * ret;
        // scale
        ret = new Vector3(ret.x / transform.localScale.x, ret.y / transform.localScale.y, ret.z / transform.localScale.z);
        return ret;
    }

    /// <summary>
    /// Transforms a point from a local transform's space to the world space
    /// Unity's Transform.TransformPoint() does not work for nested transforms
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Vector3 TransformPointToWorld(this Transform transform, Vector3 point)
    {
        if (transform.parent == null)
            return transform.TransformPoint(point);
        else
            return transform.parent.TransformPointToWorld(transform.TransformPoint(point));
    }


    public static Vector3 InverseTransformPointToLocal(this Transform transform, Vector3 point)
    {
        if (transform.parent == null)
            return transform.InverseTransformPoint(point);
        else
        {
            Vector3 parentPoint = transform.parent.InverseTransformPointToLocal(point);
            return transform.InverseTransformPoint(parentPoint);
        }
    }

    public static Vector3 TransformDirectionToWorld(this Transform transform, Vector3 direction)
    {
        if (transform.parent == null)
            return transform.TransformDirection(direction);
        else
            return transform.parent.TransformDirectionToWorld(transform.TransformDirection(direction));
    }

    public static Vector3 InverseTransformDirectionToLocal(this Transform transform, Vector3 direction)
    {
        if (transform.parent == null)
            return transform.InverseTransformDirection(direction);
        else
        {
            Vector3 parentDirection = transform.parent.InverseTransformDirectionToLocal(direction);
            return transform.InverseTransformDirection(parentDirection);
        }
    }

    public static Vector3 TransformVectorToWorld(this Transform transform, Vector3 vector)
    {
        if (transform.parent == null)
            return transform.TransformVector(vector);
        else
            return transform.parent.TransformVectorToWorld(transform.TransformVector(vector));
    }

    public static Vector3 InverseTransformVectorToLocal(this Transform transform, Vector3 vector)
    {
        if (transform.parent == null)
            return transform.InverseTransformVector(vector);
        else
        {
            Vector3 parentVector = transform.parent.InverseTransformVectorToLocal(vector);
            return transform.InverseTransformVector(parentVector);
        }
    }
}
