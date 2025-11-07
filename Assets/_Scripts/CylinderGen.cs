using UnityEngine;

public class CylinderGen : MonoBehaviour
{
    public Material material;
    public float height = 2f;
    public float radius = 1f;
    public int segments = 16; // More than 5 as requested
    public Vector3 position;

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

        // Generate top and bottom circle points
        Vector3[] bottom = new Vector3[segments];
        Vector3[] top = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            bottom[i] = new Vector3(x, -halfHeight, z) + position;
            top[i] = new Vector3(x, halfHeight, z) + position;
        }

        // Project to 2D using custom PerspectiveCamera
        Vector2[] bottom2D = new Vector2[segments];
        Vector2[] top2D = new Vector2[segments];
        for (int i = 0; i < segments; i++)
        {
            bottom2D[i] = ProjectPoint(bottom[i]);
            top2D[i] = ProjectPoint(top[i]);
        }

        // Draw base circles
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            DrawLine(bottom2D[i], bottom2D[next]);
            DrawLine(top2D[i], top2D[next]);
        }

        // Draw vertical edges
        for (int i = 0; i < segments; i++)
        {
            DrawLine(bottom2D[i], top2D[i]);
        }

        GL.End();
        GL.PopMatrix();
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