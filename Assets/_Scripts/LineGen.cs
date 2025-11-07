using JetBrains.Annotations;
using UnityEngine;

public class LineGen : MonoBehaviour
{
    public Material material;
    public float cubeSize;
    public Vector2 cubePos;
    public float zPos;

    private void OnPostRender()
    {
        DrawLine();
    }

    public void DrawLine()
    {
        if (material == null)
        {
            Debug.LogError("Missing Material Here");
            return;
        }
        GL.PushMatrix();

        GL.Begin(GL.LINES);
        material.SetPass(0);

        var frontSquare = GetCube();
        var frontZ = PerspectiveCamera.Instance.GetPerspective(zPos + cubeSize * .5f);
        var backSquare = GetCube();
        var backZ = PerspectiveCamera.Instance.GetPerspective(zPos - cubeSize * .5f);

        var computedFront = RenderSquare(frontSquare, frontZ);
        var computedBack = RenderSquare(backSquare, backZ);

        for (int i = 0; i < 4; i++)
        {
            GL.Vertex(computedFront[i]);
            GL.Vertex(computedBack[i]);
        }

        GL.End();
        GL.PopMatrix();
    }

    public Vector2[] GetCube()
    {
        var faceArray = new Vector2[]
        {
            new Vector2 (1, 1f),
            new Vector2 (-1f, 1f),
            new Vector2 (-1f, -1f),
            new Vector2 (1f, -1f),
        };

        for(var i = 0; i < faceArray.Length; i++)
        {
            faceArray[i] = new Vector2(cubePos.x + faceArray[i].x, cubePos.y + faceArray[i].y) * cubeSize;
        }

        return faceArray;
        
    }

    private Vector2[] RenderSquare(Vector2[] squareElements, float perspective)
    {
        var computedSquare = new Vector2[squareElements.Length];
        for(var i = 0; i < squareElements.Length; i++)
        {
            computedSquare[i] = squareElements[i] * perspective;
            GL.Vertex(squareElements[i] * perspective);
            GL.Vertex(squareElements[(i + 1) % squareElements.Length] * perspective);
        }
        return computedSquare;
    }

}