using UnityEngine;

public class CylinderGen : MonoBehaviour
{
    public Material material;
    public float height = 2f;
    public float radius = 1f;
    public int segments = 16;
    public Vector3 position;

    public Vector3 rotation; // X Y Z rotation (in degrees)

    private void OnPostRender()
    {
        if (material == null || PerspectiveCamera.Instance == null)
            return;

        DrawCylinder();
    }

    private void DrawCylinder()
    {
        GL.PushMatrix();
        material.SetPass(0);
        GL.Begin(GL.LINES);

        float halfHeight = height / 2f;

        Vector3[] bottom = new Vector3[segments];
        Vector3[] top = new Vector3[segments];

        // Generate vertices
        for (int i = 0; i < segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            bottom[i] = new Vector3(x, -halfHeight, z);
            top[i]    = new Vector3(x,  halfHeight, z);

            // Rotate
            bottom[i] = Rotate3D(bottom[i], rotation) + position;
            top[i]    = Rotate3D(top[i], rotation) + position;
        }

        // Project
        Vector2[] bottom2D = new Vector2[segments];
        Vector2[] top2D = new Vector2[segments];

        for (int i = 0; i < segments; i++)
        {
            bottom2D[i] = ProjectPoint(bottom[i]);
            top2D[i]    = ProjectPoint(top[i]);
        }

        // Base circles
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            DrawLine(bottom2D[i], bottom2D[next]);
            DrawLine(top2D[i], top2D[next]);
        }

        // Vertical edges
        for (int i = 0; i < segments; i++)
            DrawLine(bottom2D[i], top2D[i]);

        GL.End();
        GL.PopMatrix();
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
