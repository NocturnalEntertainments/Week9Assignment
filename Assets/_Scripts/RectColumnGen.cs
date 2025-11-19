using UnityEngine;

public class RectColumnGen : MonoBehaviour
{
    public Material material;
    public Vector3 size = new Vector3(1f, 2f, 1f);
    public Vector3 position;

    public Vector3 rotation; // rotation in degrees

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

        Vector3 hs = size * 0.5f;

        // 8 vertices of the box (local before rotation)
        Vector3[] v = new Vector3[8];
        v[0] = new Vector3(-hs.x, -hs.y, -hs.z);
        v[1] = new Vector3(hs.x, -hs.y, -hs.z);
        v[2] = new Vector3(hs.x, -hs.y, hs.z);
        v[3] = new Vector3(-hs.x, -hs.y, hs.z);

        v[4] = new Vector3(-hs.x, hs.y, -hs.z);
        v[5] = new Vector3(hs.x, hs.y, -hs.z);
        v[6] = new Vector3(hs.x, hs.y, hs.z);
        v[7] = new Vector3(-hs.x, hs.y, hs.z);

        // Rotate + translate
        for (int i = 0; i < 8; i++)
            v[i] = Rotate3D(v[i], rotation) + position;

        // Project
        Vector2[] p = new Vector2[8];
        for (int i = 0; i < 8; i++)
            p[i] = ProjectPoint(v[i]);

        // Bottom
        DrawLine(p[0], p[1]);
        DrawLine(p[1], p[2]);
        DrawLine(p[2], p[3]);
        DrawLine(p[3], p[0]);

        // Top
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

    private void DrawLine(Vector2 a, Vector2 b)
    {
        GL.Vertex3(a.x, a.y, 0);
        GL.Vertex3(b.x, b.y, 0);
    }
}
