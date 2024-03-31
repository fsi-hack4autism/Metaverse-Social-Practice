using UnityEngine;

public class DrawVector : MonoBehaviour
{
    public Color color = Color.white;
    public Vector3? from;
    public Vector3? to;

    public DrawVector SetVector(Vector3 vec, Transform origin)
    {
        from = origin.position;
        to = origin.TransformVector(vec);
        return this;
    }

    public DrawVector SetVector(Vector3 vec)
    {
        from = Vector3.zero;
        to = vec;
        return this;
    }

    public DrawVector SetVector(Vector3 from, Vector3 to)
    {
        this.from = from;
        this.to = to;
        return this;
    }

    void OnDrawGizmos()
    {
        if (from == null || to == null) return;

        Gizmos.color = color;
        Gizmos.DrawLine(from.Value, to.Value);
    }
}
