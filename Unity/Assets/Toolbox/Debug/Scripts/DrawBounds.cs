using UnityEngine;

public class DrawBounds : MonoBehaviour
{
    public Color color = Color.white;
    public Bounds? bounds;
    public WorldBounds? bounds_w;
    public bool drawRadius = true;

    public DrawBounds SetBounds(WorldBounds bounds)
    {
        this.bounds_w = bounds;
        return this;
    }

    public DrawBounds SetBounds(Bounds bounds)
    {
        this.bounds = bounds;
        return this;
    }

    public DrawBounds SetBounds(Bounds bounds, Color c)
    {
        this.bounds = bounds;
        this.color = c;
        return this;
    }

    void OnDrawGizmos()
    {
        if (bounds == null && bounds_w == null) return;

        Gizmos.color = color;
        if (bounds != null)
        {
            Gizmos.DrawWireCube(bounds.Value.center, bounds.Value.size);
        }
        else
        {
            Gizmos.DrawLine(bounds_w.Value.frontBottomLeft, bounds_w.Value.frontBottomRight);
            Gizmos.DrawLine(bounds_w.Value.frontBottomRight, bounds_w.Value.frontTopRight);
            Gizmos.DrawLine(bounds_w.Value.frontTopRight, bounds_w.Value.frontTopLeft);
            Gizmos.DrawLine(bounds_w.Value.frontTopLeft, bounds_w.Value.frontBottomLeft);

            Gizmos.DrawLine(bounds_w.Value.backBottomLeft, bounds_w.Value.backBottomRight);
            Gizmos.DrawLine(bounds_w.Value.backBottomRight, bounds_w.Value.backTopRight);
            Gizmos.DrawLine(bounds_w.Value.backTopRight, bounds_w.Value.backTopLeft);
            Gizmos.DrawLine(bounds_w.Value.backTopLeft, bounds_w.Value.backBottomLeft);

            Gizmos.DrawLine(bounds_w.Value.frontBottomLeft, bounds_w.Value.backBottomLeft);
            Gizmos.DrawLine(bounds_w.Value.frontBottomRight, bounds_w.Value.backBottomRight);
            Gizmos.DrawLine(bounds_w.Value.frontTopRight, bounds_w.Value.backTopRight);
            Gizmos.DrawLine(bounds_w.Value.frontTopLeft, bounds_w.Value.backTopLeft);

            if (drawRadius)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(bounds_w.Value.transform.position, bounds_w.Value.radius + bounds_w.Value.transform.position);
            }
        }
    }
}