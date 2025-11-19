using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Material material;
    public Vector3 position;    // center position
    public Vector3 scale = new Vector3(1f, 1f, 1f); // independent X, Y, Z scales

    private void OnPostRender()
    {
        if (material == null) return;

        material.SetPass(0);
        GL.PushMatrix();
        GL.Begin(GL.LINES);

        Vector3 s = scale / 2f; // half-scale for vertices

        // 8 corners of the cube
        Vector3[] verts = new Vector3[8];
        verts[0] = position + new Vector3(-s.x, -s.y, -s.z);
        verts[1] = position + new Vector3(s.x, -s.y, -s.z);
        verts[2] = position + new Vector3(s.x, -s.y, s.z);
        verts[3] = position + new Vector3(-s.x, -s.y, s.z);
        verts[4] = position + new Vector3(-s.x, s.y, -s.z);
        verts[5] = position + new Vector3(s.x, s.y, -s.z);
        verts[6] = position + new Vector3(s.x, s.y, s.z);
        verts[7] = position + new Vector3(-s.x, s.y, s.z);

        Vector3 ApplyPerspective(Vector3 v)
        {
            float perspective = PerspectiveCamera.Instance.GetPerspective(v.z);
            return new Vector3(v.x * perspective, v.y * perspective, v.z);
        }

        for (int i = 0; i < verts.Length; i++)
            verts[i] = ApplyPerspective(verts[i]);

        // Bottom face
        GL.Vertex(verts[0]); GL.Vertex(verts[1]);
        GL.Vertex(verts[1]); GL.Vertex(verts[2]);
        GL.Vertex(verts[2]); GL.Vertex(verts[3]);
        GL.Vertex(verts[3]); GL.Vertex(verts[0]);

        // Top face
        GL.Vertex(verts[4]); GL.Vertex(verts[5]);
        GL.Vertex(verts[5]); GL.Vertex(verts[6]);
        GL.Vertex(verts[6]); GL.Vertex(verts[7]);
        GL.Vertex(verts[7]); GL.Vertex(verts[4]);

        // Vertical edges
        GL.Vertex(verts[0]); GL.Vertex(verts[4]);
        GL.Vertex(verts[1]); GL.Vertex(verts[5]);
        GL.Vertex(verts[2]); GL.Vertex(verts[6]);
        GL.Vertex(verts[3]); GL.Vertex(verts[7]);

        GL.End();
        GL.PopMatrix();
    }

    public PlayerController.CubeDimensions GetDimensions()
    {
        return new PlayerController.CubeDimensions
        {
            minX = position.x - scale.x / 2f,
            maxX = position.x + scale.x / 2f,
            minY = position.y - scale.y / 2f,
            maxY = position.y + scale.y / 2f,
            minZ = position.z - scale.z / 2f,
            maxZ = position.z + scale.z / 2f
        };
    }
}
