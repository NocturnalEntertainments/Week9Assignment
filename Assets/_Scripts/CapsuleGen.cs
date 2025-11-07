using UnityEngine;

public class CapsuleGen : MonoBehaviour
{
    public Material material;
    public float radius = 1f;
    public float height = 3f;
    public int segments = 16; // More than 5 for smoothness
    public Vector3 position;

    private void OnPostRender()
    {
        if (material == null || PerspectiveCamera.Instance == null)
            return;

        DrawCapsule();
    }

    private void DrawCapsule()
    {
        GL.PushMatrix();
        material.SetPass(0);
        GL.Begin(GL.LINES);

        float halfCylHeight = (height - 2 * radius) / 2f;
        if (halfCylHeight < 0) halfCylHeight = 0;

        // Bottom & Top centers
        Vector3 bottomCenter = position + new Vector3(0, -halfCylHeight, 0);
        Vector3 topCenter = position + new Vector3(0, halfCylHeight, 0);

        // Draw main cylinder body
        for (int i = 0; i < segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            Vector3 bottom = bottomCenter + new Vector3(x, 0, z);
            Vector3 top = topCenter + new Vector3(x, 0, z);

            Vector2 b2D = ProjectPoint(bottom);
            Vector2 t2D = ProjectPoint(top);

            DrawLine(b2D, t2D);
        }

        // Draw top & bottom circle rims
        DrawCircle(bottomCenter, radius);
        DrawCircle(topCenter, radius);

        // Draw hemispheres (top and bottom)
        DrawHemisphere(topCenter, radius, true);
        DrawHemisphere(bottomCenter, radius, false);

        GL.End();
        GL.PopMatrix();
    }

    private void DrawCircle(Vector3 center, float r)
    {
        Vector2 prev = Vector2.zero;
        bool first = true;
        for (int i = 0; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 p3D = new Vector3(Mathf.Cos(angle) * r, 0, Mathf.Sin(angle) * r) + center;
            Vector2 p2D = ProjectPoint(p3D);

            if (!first)
                DrawLine(prev, p2D);
            prev = p2D;
            first = false;
        }
    }

    private void DrawHemisphere(Vector3 center, float r, bool isTop)
    {
        int rings = segments / 2;
        for (int i = 0; i <= rings; i++)
        {
            float theta = (i / (float)rings) * Mathf.PI / 2f;
            float y = Mathf.Sin(theta) * r;
            float localRadius = Mathf.Cos(theta) * r;

            Vector3 circleCenter = center + new Vector3(0, isTop ? y : -y, 0);
            DrawCircle(circleCenter, localRadius);
        }
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
