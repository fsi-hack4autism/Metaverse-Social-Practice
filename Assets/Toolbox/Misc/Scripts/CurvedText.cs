using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class CurvedText : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _text;
    public string Text
    {
        get
        {
            return _text.text;
        }
        set
        {
            _text.text = value;
            CurveText();
        }
    }
    public Color Color
    {
        get
        {
            return _text.color;
        }
        set
        {
            _text.color = value;
        }
    }
    public float Radius = 1f;
    public Vector3 Center = Vector3.zero;

    private void Start()
    {
        CurveText();
    }

    private void Update()
    {
        CurveText();
    }

    /// <summary>
    /// Wrap text around a circle without deformation
    /// </summary>
    private void CurveText()
    {
        Vector3 center = transform.TransformPoint(Center);
        Vector3 axis = transform.TransformDirection(Vector3.up);

        // start rendering text at offset so text is centered
        _text.ForceMeshUpdate();
        float writeAngle = -LengthToAngle(_text.preferredWidth, Radius) / 2;
        writeAngle -= Mathf.PI / 2; // offset by 90 degrees so centered on z axis
        var textInfo = _text.textInfo;

        // Iterate through each character of the text and position it on a circle
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            // get character vertices and character center
            Vector3[] verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            // gap between characters
            float charWidth = Mathf.Abs(charInfo.bottomLeft.x - charInfo.bottomRight.x);
            bool hasGap = true; //verts. charInfo.vertexIndex + 4;
            float gapWidth = hasGap ? Mathf.Abs(verts[charInfo.vertexIndex + 4].x - charInfo.bottomRight.x) : 0f;

            // wrap character
            Vector2 start = AngleToPos(writeAngle, Radius);
            Vector2 end = Vector2.zero;
            WrapLength(charWidth, writeAngle, ref end, Radius);
            // update write angle
            writeAngle += LengthToAngle(charWidth, Radius) + LengthToAngle(gapWidth, Radius);

            // update character vertices
            verts[charInfo.vertexIndex + 0] = new Vector3(start.x, charInfo.bottomLeft.y, start.y);
            verts[charInfo.vertexIndex + 1] = new Vector3(start.x, charInfo.topLeft.y, start.y);
            verts[charInfo.vertexIndex + 2] = new Vector3(end.x, charInfo.topRight.y, end.y);
            verts[charInfo.vertexIndex + 3] = new Vector3(end.x, charInfo.bottomRight.y, end.y);
        }

        // Update Geometry
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices; // apply to actual mesh
            _text.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    /// <summary>
    /// Converts a linear length to a curved length along a circle's circumference into an angle
    /// </summary>
    /// <param name="radius">the circle radius</param>
    /// <param name="linearLength">the length the wrap onto the cirle</param>
    /// <returns>the angle in degrees that the linear length occupies</returns>
    private float LengthToAngle(float linearLength, float radius)
    {
        float circumference = 2f * Mathf.PI * radius;
        float angle = linearLength / circumference * 2 * Mathf.PI;
        return angle;
    }

    /// <summary>
    /// Wraps a linear length onto a circle's circumference.
    /// Starts at start and wraps around the circle until the end is reached.
    /// </summary>
    /// <param name="length"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="radius"></param>
    private void WrapLength(float length, Vector2 start, ref Vector2 end, float radius)
    {
        float angle = LengthToAngle(length, radius);
        float startAngle = PosToAngle(start);
        float endAngle = startAngle + angle;
        end = AngleToPos(endAngle, radius);
    }

    private void WrapLength(float length, float startAngle, ref float endAngle, float radius)
    {
        float angle = LengthToAngle(length, radius);
        endAngle = startAngle + angle;
    }

    private void WrapLength(float length, float startAngle, ref Vector2 endAngle, float radius)
    {
        float end = -1f;
        WrapLength(length, startAngle, ref end, radius);
        endAngle = AngleToPos(end, radius);
    }

    private float PosToAngle(Vector2 pos)
    {
        return Vector2.SignedAngle(Vector2.right, pos) * Mathf.Deg2Rad;
    }

    private Vector2 AngleToPos(float angle, float radius)
    {
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;
        return new Vector2(x, y);
    }


}
