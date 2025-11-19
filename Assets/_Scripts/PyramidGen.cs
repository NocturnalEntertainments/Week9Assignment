using UnityEngine;

public class PyramidGen : MonoBehaviour
{
    public Material material;
    public float pyramidSize = 1f;
    public Vector3 pyramidPos;

    public Vector3 rotation; // rotation.x, rotation.y, rotation.z in degrees

    private void OnPostRender()
    {
        if (material == null || PerspectiveCamera.Instance == null)
            return;

        DrawPyramid3D();
    }

    private void DrawPyramid3D()
    {
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        material.SetPass(0);

        // Original local pyramid vertices
        Vector3 A = new Vector3(-1, 0, -1) * pyramidSize;
        Vector3 B = new Vector3(1, 0, -1) * pyramidSize;
        Vector3 C = new Vector3(1, 0, 1) * pyramidSize;
        Vector3 D = new Vector3(-1, 0, 1) * pyramidSize;
        Vector3 E = new Vector3(0, 1.5f, 0) * pyramidSize;

        // Rotate everything
        A = Rotate3D(A, rotation) + pyramidPos;
        B = Rotate3D(B, rotation) + pyramidPos;
        C = Rotate3D(C, rotation) + pyramidPos;
        D = Rotate3D(D, rotation) + pyramidPos;
        E = Rotate3D(E, rotation) + pyramidPos;

        // Project to 2D
        Vector2 a2D = ProjectPoint(A);
        Vector2 b2D = ProjectPoint(B);
        Vector2 c2D = ProjectPoint(C);
        Vector2 d2D = ProjectPoint(D);
        Vector2 e2D = ProjectPoint(E);

        // Base
        DrawLine(a2D, b2D);
        DrawLine(b2D, c2D);
        DrawLine(c2D, d2D);
        DrawLine(d2D, a2D);

        // Sides
        DrawLine(e2D, a2D);
        DrawLine(e2D, b2D);
        DrawLine(e2D, c2D);
        DrawLine(e2D, d2D);

        GL.End();
        GL.PopMatrix();
    }

    private Vector3 Rotate3D(Vector3 p, Vector3 rotDeg)
    {
        float rx = rotDeg.x * Mathf.Deg2Rad;
        float ry = rotDeg.y * Mathf.Deg2Rad;
        float rz = rotDeg.z * Mathf.Deg2Rad;

        // Rotate around X
        p = new Vector3(
            p.x,
            p.y * Mathf.Cos(rx) - p.z * Mathf.Sin(rx),
            p.y * Mathf.Sin(rx) + p.z * Mathf.Cos(rx)
        );

        // Rotate around Y
        p = new Vector3(
            p.x * Mathf.Cos(ry) + p.z * Mathf.Sin(ry),
            p.y,
            -p.x * Mathf.Sin(ry) + p.z * Mathf.Cos(ry)
        );

        // Rotate around Z
        p = new Vector3(
            p.x * Mathf.Cos(rz) - p.y * Mathf.Sin(rz),
            p.x * Mathf.Sin(rz) + p.y * Mathf.Cos(rz),
            p.z
        );

        return p;
    }

    private Vector2 ProjectPoint(Vector3 point)
    {
        float perspective = PerspectiveCamera.Instance.GetPerspective(point.z);
        return new Vector2(point.x * perspective, point.y * perspective);
    }

    private void DrawLine(Vector2 p1, Vector2 p2)
    {
        GL.Vertex3(p1.x, p1.y, 0);
        GL.Vertex3(p2.x, p2.y, 0);
    }
}
