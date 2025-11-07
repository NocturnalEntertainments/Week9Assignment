using UnityEngine;

public class SphereGen : MonoBehaviour
{
    public Material material;    // The material used for drawing the sphere lines
    public float radius = 1f;    // Radius of the sphere
    public int segments = 16;    // Number of segments (higher = smoother sphere)
    public Vector3 position;     // World position of the sphere center

    private void OnPostRender()
    {
        if (material == null || PerspectiveCamera.Instance == null)
            return;

        // Draw the wireframe sphere
        DrawSphere();
    }

    private void DrawSphere()
    {
        GL.PushMatrix();           // Save current OpenGL matrix state
        material.SetPass(0);       // Apply the material for rendering
        GL.Begin(GL.LINES);        // Begin drawing connected lines

        for (int i = 1; i < segments; i++)
        {
            float theta = Mathf.PI * i / segments; // Angle from 0 to π (top to bottom)
            float y = Mathf.Cos(theta) * radius;   // Vertical height of the circle
            float r = Mathf.Sin(theta) * radius;   // Radius of this horizontal circle

            // Draw one horizontal ring of the sphere
            DrawCircle(position + new Vector3(0, y, 0), r, segments);
        }

        for (int i = 0; i < segments; i++)
        {
            float phi = (i / (float)segments) * Mathf.PI * 2f; // Angle around Y-axis
            DrawVerticalCircle(phi);
        }

        GL.End();         // Finish drawing lines
        GL.PopMatrix();   // Restore previous matrix state
    }

    // Draw a horizontal circle (latitude) around the sphere
    private void DrawCircle(Vector3 center, float r, int seg)
    {
        Vector2 prev = Vector2.zero;
        bool first = true;

        for (int i = 0; i <= seg; i++)
        {
            // Compute 3D position of the current point on the circle
            float angle = (i / (float)seg) * Mathf.PI * 2f;
            Vector3 p3D = new Vector3(
                Mathf.Cos(angle) * r,
                center.y - position.y,
                Mathf.Sin(angle) * r
            ) + position;

            // Convert the 3D point into 2D screen space
            Vector2 p2D = ProjectPoint(p3D);

            // Connect the previous point to the current one
            if (!first)
                DrawLine(prev, p2D);

            prev = p2D;
            first = false;
        }
    }

    // Draw a vertical circle (longitude) going from top to bottom
    private void DrawVerticalCircle(float phi)
    {
        Vector2 prev = Vector2.zero;
        bool first = true;

        for (int i = 0; i <= segments; i++)
        {
            // Compute spherical coordinates for each point
            float theta = (i / (float)segments) * Mathf.PI; // From top (0) to bottom (π)
            float x = Mathf.Sin(theta) * Mathf.Cos(phi) * radius;
            float y = Mathf.Cos(theta) * radius;
            float z = Mathf.Sin(theta) * Mathf.Sin(phi) * radius;

            // Convert to world position
            Vector3 p3D = new Vector3(x, y, z) + position;

            // Project 3D point into 2D
            Vector2 p2D = ProjectPoint(p3D);

            // Draw line between consecutive projected points
            if (!first)
                DrawLine(prev, p2D);

            prev = p2D;
            first = false;
        }
    }

    // Converts a 3D world point into a 2D point using a custom perspective projection
    private Vector2 ProjectPoint(Vector3 point)
    {
        // Get perspective factor based on Z distance from camera
        float p = PerspectiveCamera.Instance.GetPerspective(point.z);

        // Apply perspective scaling to X and Y
        return new Vector2(point.x * p, point.y * p);
    }

    // Draws a line between two 2D points using GL
    private void DrawLine(Vector2 p1, Vector2 p2)
    {
        GL.Vertex3(p1.x, p1.y, 0);
        GL.Vertex3(p2.x, p2.y, 0);
    }
}
