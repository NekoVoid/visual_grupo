using UnityEngine;

public class GeometryGenerator : MonoBehaviour
{
    public int gridSize = 5;
    public int spiralSteps = 50;
    public float spiralRadius = 5f;
    public float spiralHeight = 0.5f;

    void Start()
    {
        GenerateGrid();
        GenerateSpiral();
        GenerateFractal(transform.position, 2);
        GenerateCustomMesh();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x, 0, z);
            }
        }
    }

    void GenerateCustomMesh()
    {
        GameObject customCube = new GameObject("CustomCube");
        MeshFilter mf = customCube.AddComponent<MeshFilter>();
        MeshRenderer mr = customCube.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mf.mesh = mesh;

        Vector3[] vertices = new Vector3[]
        {
            // Cara frontal
            new Vector3(0, 0, 0), // 0
            new Vector3(1, 0, 0), // 1
            new Vector3(1, 1, 0), // 2
            new Vector3(0, 1, 0), // 3

            // Cara trasera
            new Vector3(0, 0, 1), // 4
            new Vector3(1, 0, 1), // 5
            new Vector3(1, 1, 1), // 6
            new Vector3(0, 1, 1)  // 7
        };

        int[] triangles = new int[]
        {
            // Frontal
            0, 2, 1,
            0, 3, 2,

            // Derecha
            1, 2, 6,
            1, 6, 5,

            // Trasera
            5, 6, 7,
            5, 7, 4,

            // Izquierda
            4, 7, 3,
            4, 3, 0,

            // Abajo
            0, 1, 5,
            0, 5, 4,

            // Arriba
            3, 6, 2,
            3, 7, 6
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Color personalizado (rojo)
        mr.material = new Material(Shader.Find("Standard"));
        mr.material.color = Color.red;
    }


    void GenerateSpiral()
    {
        for (int i = 0; i < spiralSteps; i++)
        {
            float angle = i * 0.3f;
            float x = Mathf.Cos(angle) * spiralRadius;
            float z = Mathf.Sin(angle) * spiralRadius;
            float y = i * spiralHeight;

            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.transform.position = new Vector3(x, y, z);
        }
    }

    void GenerateFractal(Vector3 position, int depth)
    {
        if (depth == 0) return;

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        float scale = 1f / depth;
        sphere.transform.localScale = Vector3.one * scale;

        Vector3[] directions = {
            Vector3.up, Vector3.down,
            Vector3.left, Vector3.right,
            Vector3.forward, Vector3.back
        };

        foreach (Vector3 dir in directions)
        {
            GenerateFractal(position + dir * 1.5f * scale, depth - 1);
        }
    }
}
