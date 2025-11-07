using UnityEngine;

public class RectColumnGen : MonoBehaviour
{
    public Material material;
    public Vector3 size = new Vector3(1f, 2f, 1f); // width, height, depth
    public Vector3 position;

    private void OnPostRender()
    {
        if (material == null || PerspectiveCamera.Instance == null)
            return;

        DrawRectangularColumn();
    }

    private void DrawRectangularColumn()
    {
        GL.PushMatrix();
        material.SetPass(0);
        GL.Begin(GL.LINES);

        // Half-size for easier vertex creation
        Vector3 hs = size * 0.5f;

        // 8 vertices of a box
        Vector3[] v = new Vector3[8];
        v[0] = new Vector3(-hs.x, -hs.y, -hs.z) + position; // Bottom
        v[1] = new Vector3(hs.x, -hs.y, -hs.z) + position;
        v[2] = new Vector3(hs.x, -hs.y, hs.z) + position;
        v[3] = new Vector3(-hs.x, -hs.y, hs.z) + position;

        v[4] = new Vector3(-hs.x, hs.y, -hs.z) + position;  // Top
        v[5] = new Vector3(hs.x, hs.y, -hs.z) + position;
        v[6] = new Vector3(hs.x, hs.y, hs.z) + position;
        v[7] = new Vector3(-hs.x, hs.y, hs.z) + position;

        // Project 3D points into 2D
        Vector2[] p = new Vector2[8];
        for (int i = 0; i < 8; i++)
            p[i] = ProjectPoint(v[i]);

        // Bottom rectangle
        DrawLine(p[0], p[1]);
        DrawLine(p[1], p[2]);
        DrawLine(p[2], p[3]);
        DrawLine(p[3], p[0]);

        // Top rectangle
        DrawLine(p[4], p[5]);
        DrawLine(p[5], p[6]);
        DrawLine(p[6], p[7]);
        DrawLine(p[7], p[4]);

        // Vertical edges
        DrawLine(p[0], p[4]);
        DrawLine(p[1], p[5]);
        DrawLine(p[2], p[6]);
        DrawLine(p[3], p[7]);

        GL.End();
        GL.PopMatrix();
    }

    private Vector2 ProjectPoint(Vector3 point)
    {
        float perspective = PerspectiveCamera.Instance.GetPerspective(point.z);
        return new Vector2(point.x * perspective, point.y * perspective);
    }

    private void DrawLine(Vector2 a, Vector2 b)
    {
        GL.Vertex3(a.x, a.y, 0);
        GL.Vertex3(b.x, b.y, 0);
    }
}


