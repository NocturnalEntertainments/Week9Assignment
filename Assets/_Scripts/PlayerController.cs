using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Material cubeMaterial;
    public float cubeSize = 1f;
    public Vector3 position; // now full 3D
    public float jumpForce = 6f;
    public float gravity = -9.8f;

    public PlatformController platform; // assign your platform

    private Vector3 velocity;
    private bool isGrounded = false;

    private void Update()
    {
        HandleJump();
        ApplyGravity();
        CheckCollision();
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpForce;
            isGrounded = false;
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;

        position += velocity * Time.deltaTime;
    }

    void CheckCollision()
    {
        CubeDimensions cubeDim = new CubeDimensions
        {
            minX = position.x - cubeSize / 2f,
            maxX = position.x + cubeSize / 2f,
            minY = position.y - cubeSize / 2f,
            maxY = position.y + cubeSize / 2f,
            minZ = position.z - cubeSize / 2f,
            maxZ = position.z + cubeSize / 2f
        };

        CubeDimensions platDim = platform.GetDimensions();

        if (cubeDim.Collide(platDim))
        {
            // Snap cube to top of platform using platform.scale.y instead of platform.size
            position.y = platform.position.y + platform.scale.y / 2f + cubeSize / 2f;
            velocity = Vector3.zero;
            isGrounded = true;
            cubeMaterial.color = Color.green;
        }
        else
        {
            cubeMaterial.color = Color.red;
        }
    }


    private void OnPostRender()
    {
        if (cubeMaterial == null || PerspectiveCamera.Instance == null) return;

        cubeMaterial.SetPass(0);
        GL.PushMatrix();
        GL.Begin(GL.LINES);

        Vector3 s = Vector3.one * (cubeSize / 2f);

        // 8 vertices
        Vector3[] verts = new Vector3[8];
        verts[0] = position + new Vector3(-s.x, -s.y, -s.z);
        verts[1] = position + new Vector3(s.x, -s.y, -s.z);
        verts[2] = position + new Vector3(s.x, -s.y, s.z);
        verts[3] = position + new Vector3(-s.x, -s.y, s.z);
        verts[4] = position + new Vector3(-s.x, s.y, -s.z);
        verts[5] = position + new Vector3(s.x, s.y, -s.z);
        verts[6] = position + new Vector3(s.x, s.y, s.z);
        verts[7] = position + new Vector3(-s.x, s.y, s.z);

        // Helper to apply perspective
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


    public struct CubeDimensions
    {
        public float minX, maxX, minY, maxY, minZ, maxZ;

        public bool Collide(CubeDimensions other)
        {
            return (minX <= other.maxX && maxX >= other.minX) &&
                   (minY <= other.maxY && maxY >= other.minY) &&
                   (minZ <= other.maxZ && maxZ >= other.minZ);
        }
    }
}
