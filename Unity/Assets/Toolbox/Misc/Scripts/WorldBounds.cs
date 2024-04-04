using UnityEngine;

public struct WorldBounds
{
    public Bounds bounds;
    public Transform transform;

    public Vector3 min { get { return transform.TransformPoint(bounds.min); } }
    public Vector3 max { get { return transform.TransformPoint(bounds.max); } }
    public Vector3 center { get { return transform.TransformPoint(bounds.center); } }
    public Vector3 localCenter { get { return bounds.center; } }
    public Vector3 extents
    {
        get
        {
            Vector3 localExtents = bounds.extents;
            localExtents.Scale(transform.lossyScale);
            return localExtents;
        }
    }
    public Vector3 size
    {
        get
        {
            Vector3 localSize = bounds.size;
            localSize.Scale(transform.lossyScale);
            return localSize;
        }
    }
    /// <summary>
    /// Similar to extents, but treats the transform as the center of the bounds.
    /// Returns the maximum distance from the transform to any corner of the bounds.
    /// In world space
    /// </summary>
    /// <value></value>
    public Vector3 radius
    {
        get
        {
            Vector3[] corners = this.localCorners; // relative to transform
            Vector3 radius = corners[0];
            foreach (Vector3 corner in corners)
            {
                if (corner.magnitude > radius.magnitude)
                    radius = corner;
            }

            return transform.TransformVector(radius);
        }
    }
    public Vector3 frontBottomLeft
    {
        get
        {
            Vector3 up = transform.up;
            Vector3 right = transform.right;
            Vector3 forward = transform.forward;
            Vector3 localExtents = bounds.extents;
            localExtents.Scale(transform.lossyScale);
            return transform.TransformPoint(bounds.center) - up * localExtents.y - right * localExtents.x - forward * localExtents.z;
        }
    }
    public Vector3 frontBottomRight
    {
        get
        {
            Vector3 up = transform.up;
            Vector3 right = transform.right;
            Vector3 forward = transform.forward;
            Vector3 localExtents = bounds.extents;
            localExtents.Scale(transform.lossyScale);
            return transform.TransformPoint(bounds.center) - up * localExtents.y + right * localExtents.x - forward * localExtents.z;
        }
    }
    public Vector3 frontTopLeft
    {
        get
        {
            Vector3 up = transform.up;
            Vector3 right = transform.right;
            Vector3 forward = transform.forward;
            Vector3 localExtents = bounds.extents;
            localExtents.Scale(transform.lossyScale);
            return transform.TransformPoint(bounds.center) + up * localExtents.y - right * localExtents.x - forward * localExtents.z;
        }
    }
    public Vector3 frontTopRight
    {
        get
        {
            Vector3 up = transform.up;
            Vector3 right = transform.right;
            Vector3 forward = transform.forward;
            Vector3 localExtents = bounds.extents;
            localExtents.Scale(transform.lossyScale);
            return transform.TransformPoint(bounds.center) + up * localExtents.y + right * localExtents.x - forward * localExtents.z;
        }
    }
    public Vector3 backBottomLeft
    {
        get
        {
            Vector3 up = transform.up;
            Vector3 right = transform.right;
            Vector3 forward = transform.forward;
            Vector3 localExtents = bounds.extents;
            localExtents.Scale(transform.lossyScale);
            return transform.TransformPoint(bounds.center) - up * localExtents.y - right * localExtents.x + forward * localExtents.z;
        }
    }
    public Vector3 backBottomRight
    {
        get
        {
            Vector3 up = transform.up;
            Vector3 right = transform.right;
            Vector3 forward = transform.forward;
            Vector3 localExtents = bounds.extents;
            localExtents.Scale(transform.lossyScale);
            return transform.TransformPoint(bounds.center) - up * localExtents.y + right * localExtents.x + forward * localExtents.z;
        }
    }
    public Vector3 backTopLeft
    {
        get
        {
            Vector3 up = transform.up;
            Vector3 right = transform.right;
            Vector3 forward = transform.forward;
            Vector3 localExtents = bounds.extents;
            localExtents.Scale(transform.lossyScale);
            return transform.TransformPoint(bounds.center) + up * localExtents.y - right * localExtents.x + forward * localExtents.z;
        }
    }
    public Vector3 backTopRight
    {
        get
        {
            Vector3 up = transform.up;
            Vector3 right = transform.right;
            Vector3 forward = transform.forward;
            Vector3 localExtents = bounds.extents;
            localExtents.Scale(transform.lossyScale);
            return transform.TransformPoint(bounds.center) + up * localExtents.y + right * localExtents.x + forward * localExtents.z;
        }
    }
    public Vector3[] corners
    {
        get
        {
            Vector3[] corners = new Vector3[8];
            corners[0] = frontBottomLeft;
            corners[1] = frontBottomRight;
            corners[2] = frontTopLeft;
            corners[3] = frontTopRight;
            corners[4] = backBottomLeft;
            corners[5] = backBottomRight;
            corners[6] = backTopLeft;
            corners[7] = backTopRight;
            return corners;
        }
    }
    public Vector3[] localCorners
    {
        get
        {
            Vector3[] corners = this.corners;
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = transform.InverseTransformPoint(corners[i]);
            }
            return corners;
        }
    }

}