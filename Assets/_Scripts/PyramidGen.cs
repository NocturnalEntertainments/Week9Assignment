using UnityEngine;

public class PyramidGen : MonoBehaviour
{
    public Material material;
    public float pyramidSize = 1f;
    public Vector3 pyramidPos;

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

        // Pyramid vertices (in world 3D space)
        Vector3 A = new Vector3(-1, 0, -1) * pyramidSize + pyramidPos;
        Vector3 B = new Vector3(1, 0, -1) * pyramidSize + pyramidPos;
        Vector3 C = new Vector3(1, 0, 1) * pyramidSize + pyramidPos;
        Vector3 D = new Vector3(-1, 0, 1) * pyramidSize + pyramidPos;
        Vector3 E = new Vector3(0, 1.5f, 0) * pyramidSize + pyramidPos;

        // Project each 3D vertex into 2D
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
