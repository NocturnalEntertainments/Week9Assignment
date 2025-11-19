using UnityEngine;

public class SphereGen : MonoBehaviour
{
    public Material material;
    public float radius = 1f;
    public int segments = 16;
    public Vector3 position;

    public Vector3 rotation; // rotation in degrees

    private void OnPostRender()
    {
        if (material == null || PerspectiveCamera.Instance == null)
            return;

        DrawSphere();
    }

    private void DrawSphere()
    {
        GL.PushMatrix();
        material.SetPass(0);
        GL.Begin(GL.LINES);

        // Horizontal rings (latitude)
        for (int i = 1; i < segments; i++)
        {
            float theta = Mathf.PI * i / segments;
            float y = Mathf.Cos(theta) * radius;
            float r = Mathf.Sin(theta) * radius;

            DrawCircle(position + new Vector3(0, y, 0), r, segments);
        }

        // Vertical rings (longitude)
        for (int i = 0; i < segments; i++)
        {
            float phi = (i / (float)segments) * Mathf.PI * 2f;
            DrawVerticalCircle(phi);
        }

        GL.End();
        GL.PopMatrix();
    }

    private void DrawCircle(Vector3 center, float r, int seg)
    {
        Vector2 prev = Vector2.zero;
        bool first = true;

        for (int i = 0; i <= seg; i++)
        {
            float angle = (i / (float)seg) * Mathf.PI * 2f;

            Vector3 p3D = new Vector3(
                Mathf.Cos(angle) * r,
                center.y - position.y,
                Mathf.Sin(angle) * r
            );

            // Rotate
            p3D = Rotate3D(p3D, rotation) + position;

            Vector2 p2D = ProjectPoint(p3D);

            if (!first)
                DrawLine(prev, p2D);

            prev = p2D;
            first = false;
        }
    }

    private void DrawVerticalCircle(float phi)
    {
        Vector2 prev = Vector2.zero;
        bool first = true;

        for (int i = 0; i <= segments; i++)
        {
            float theta = (i / (float)segments) * Mathf.PI;

            float x = Mathf.Sin(theta) * Mathf.Cos(phi) * radius;
            float y = Mathf.Cos(theta) * radius;
            float z = Mathf.Sin(theta) * Mathf.Sin(phi) * radius;

            Vector3 p3D = new Vector3(x, y, z);

            // Rotate
            p3D = Rotate3D(p3D, rotation) + position;

            Vector2 p2D = ProjectPoint(p3D);

            if (!first)
                DrawLine(prev, p2D);

            prev = p2D;
            first = false;
        }
    }

    private Vector3 Rotate3D(Vector3 p, Vector3 rot)
    {
        float rx = rot.x * Mathf.Deg2Rad;
        float ry = rot.y * Mathf.Deg2Rad;
        float rz = rot.z * Mathf.Deg2Rad;

        // X
        p = new Vector3(
            p.x,
            p.y * Mathf.Cos(rx) - p.z * Mathf.Sin(rx),
            p.y * Mathf.Sin(rx) + p.z * Mathf.Cos(rx)
        );

        // Y
        p = new Vector3(
            p.x * Mathf.Cos(ry) + p.z * Mathf.Sin(ry),
            p.y,
            -p.x * Mathf.Sin(ry) + p.z * Mathf.Cos(ry)
        );

        // Z
        p = new Vector3(
            p.x * Mathf.Cos(rz) - p.y * Mathf.Sin(rz),
            p.x * Mathf.Sin(rz) + p.y * Mathf.Cos(rz),
            p.z
        );

        return p;
    }

    private Vector2 ProjectPoint(Vector3 point)
    {
        float p = PerspectiveCamera.Instance.GetPerspective(point.z);
        return new Vector2(point.x * p, point.y * p);
    }

    private void DrawLine(Vector2 p1, Vector2 p2)
    {
        GL.Vertex3(p1.x, p1.y, 0);
        GL.Vertex3(p2.x, p2.y, 0);
    }
}
