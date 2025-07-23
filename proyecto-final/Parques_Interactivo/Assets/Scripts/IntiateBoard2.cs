using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class InitiateBoard2 : MonoBehaviour
{
    public static InitiateBoard2 Instance;

    void Awake()
    {
        Instance = this;
    }

    public BoxHandling box;
    public TurnManager turnManager;
    public int boxCount = 8;
    private SplineContainer splines;

    // Todas las casillas generadas por orden
    public List<BoxHandling> allBoxes = new List<BoxHandling>();

    // Caminos lógicos por jugador
    public List<BoxHandling> redPath = new List<BoxHandling>();
    public List<BoxHandling> bluePath = new List<BoxHandling>();
    public List<BoxHandling> yellowPath = new List<BoxHandling>();
    public List<BoxHandling> greenPath = new List<BoxHandling>();

    // partes de color}

    public List<BoxHandling> RedParts = new List<BoxHandling>();
    public List<BoxHandling> BlueParts = new List<BoxHandling>();
    public List<BoxHandling> YellowParts = new List<BoxHandling>();
    public List<BoxHandling> GreenParts = new List<BoxHandling>();

    [ContextMenu("Place Boxes and Create Paths")]
    public void GenerateBoxesAndPaths()
    {
        DeleteBoxes();
        allBoxes.Clear();
        redPath.Clear(); bluePath.Clear(); yellowPath.Clear(); greenPath.Clear();
        RedParts.Clear(); BlueParts.Clear(); GreenParts.Clear(); YellowParts.Clear();

        splines = GetComponent<SplineContainer>();
        int boxIndex = 0;

        for (int s = 0; s < splines.Splines.Count; s++)
        {
            Spline spline = splines.Splines[s];
            int currentCount = (spline.Count == 2) ? boxCount : boxCount * 2;

            //epsilon used for edge case solving
            const float epsilonSubStep = 100f;
            float epsilon = 1f / (epsilonSubStep * (currentCount - 1));

            for (int i = 0; i < currentCount; i++)
            {
                float t = (float)i / (float)(currentCount - 1);
                float3 pos, tangent, up;

                if (t == 0 || t == 1)
                {
                    pos = spline.EvaluatePosition(t);
                    tangent = spline.EvaluateTangent(t > 0 ? t - epsilon : t + epsilon);
                    up = spline.EvaluateUpVector(t > 0 ? t - epsilon : t + epsilon);
                }
                else
                {
                    spline.Evaluate(t, out pos, out tangent, out up);
                }

                GameObject g = Instantiate(
                    box.gameObject,
                    pos,
                    Quaternion.LookRotation(tangent, up),
                    gameObject.transform
                );

                g.name = $"Box{boxIndex}_Spline{s}";

                BoxHandling boxHandling = g.GetComponent<BoxHandling>();
                allBoxes.Add(boxHandling);

                boxIndex++;
            }
        }

        GenerateLogicalPaths();

    }

    List<BoxHandling> NormalizePath(List<BoxHandling> path, int startIndex)
    {
        var normalized = path
            .OrderBy(box => {
                int index = allBoxes.IndexOf(box);
                return index >= startIndex ? index : index + 1000; // para que los menores al inicio queden al final
            })
            .ToList();

        return normalized;
    }

    void AssignPathsToPlayers()
    {
        foreach (var player in turnManager.players)
        {
            switch (player.color)
            {
                case PlayerColor.RED:
                    player.customPath = redPath;
                    break;
                case PlayerColor.GREEN:
                    player.customPath = greenPath;
                    break;
                case PlayerColor.BLUE:
                    player.customPath = bluePath;
                    break;
                case PlayerColor.YELLOW:
                    player.customPath = yellowPath;
                    break;
            }

            Debug.Log($"Camino asignado a jugador {player.color} con {player.customPath.Count} casillas");
        }
    }

    private void GenerateLogicalPaths()
    {
        // RED - Start 12
        for (int i = 0; i <= 7; i++) redPath.Add(allBoxes[i]);
        for (int i = 12; i <= 24; i++) redPath.Add(allBoxes[i]);
        for (int i = 32; i <= 48; i++) redPath.Add(allBoxes[i]);
        for (int i = 56; i <= 72; i++) redPath.Add(allBoxes[i]);
        for (int i = 80; i <= 95; i++) redPath.Add(allBoxes[i]);

        // GREEN - Start 36
        for (int i = 8; i <= 31; i++) greenPath.Add(allBoxes[i]);
        for (int i = 36; i <= 48; i++) greenPath.Add(allBoxes[i]);
        for (int i = 56; i <= 72; i++) greenPath.Add(allBoxes[i]);
        for (int i = 80; i <= 95; i++) greenPath.Add(allBoxes[i]);
        greenPath.Add(allBoxes[0]);

        // BLUE - Start 50
        bluePath.Add(allBoxes[0]);
        for (int i = 8; i <= 24; i++) bluePath.Add(allBoxes[i]);
        for (int i = 32; i <= 55; i++) bluePath.Add(allBoxes[i]);
        for (int i = 60; i <= 72; i++) bluePath.Add(allBoxes[i]);
        for (int i = 80; i <= 95; i++) bluePath.Add(allBoxes[i]);


        // YELLOW - Start 84
        yellowPath.Add(allBoxes[0]);
        for (int i = 56; i <= 79; i++) yellowPath.Add(allBoxes[i]);
        for (int i = 84; i <= 95; i++) yellowPath.Add(allBoxes[i]);
        for (int i = 8; i <= 24; i++) yellowPath.Add(allBoxes[i]);
        for (int i = 32; i <= 48; i++) yellowPath.Add(allBoxes[i]);

        // Normalizar cada camino
        redPath = NormalizePath(redPath, 12);
        greenPath = NormalizePath(greenPath, 36);
        bluePath = NormalizePath(bluePath, 50);
        yellowPath = NormalizePath(yellowPath, 84);

        // Mostrar caminos
        //Debug.Log("RED PATH: " + string.Join(", ", redPath.Select(b => allBoxes.IndexOf(b))));
        //Debug.Log("GREEN PATH: " + string.Join(", ", greenPath.Select(b => allBoxes.IndexOf(b))));
        //Debug.Log("BLUE PATH: " + string.Join(", ", bluePath.Select(b => allBoxes.IndexOf(b))));
        //Debug.Log("YELLOW PATH: " + string.Join(", ", yellowPath.Select(b => allBoxes.IndexOf(b))));

        // RED
        for (int i = 0; i <= 7; i++) RedParts.Add(allBoxes[i]);
        RedParts.Add(allBoxes[12]);
        RedParts.Add(allBoxes[19]);
        // GREEN
        for (int i = 24; i <= 31; i++) GreenParts.Add(allBoxes[i]);
        GreenParts.Add(allBoxes[36]);
        GreenParts.Add(allBoxes[43]);
        // BLUE
        for (int i = 48; i <= 55; i++) BlueParts.Add(allBoxes[i]);
        BlueParts.Add(allBoxes[60]);
        BlueParts.Add(allBoxes[67]);
        // YELLOW
        for (int i = 72; i <= 79; i++) YellowParts.Add(allBoxes[i]);
        YellowParts.Add(allBoxes[84]);
        YellowParts.Add(allBoxes[91]);

        ColorPath(RedParts, Color.red);
        ColorPath(GreenParts, Color.green);
        ColorPath(BlueParts, Color.blue);
        ColorPath(YellowParts, Color.yellow);
    }

    private void DeleteBoxes()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }


    // Para pruebas

    private void ColorPath(List<BoxHandling> path, Color color)
    {
        foreach (BoxHandling box in path)
        {
            Renderer rend = box.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = color;
            }
        }
    }

    [ContextMenu("Colorear caminos por jugador")]
    public void ColorAllPaths()
    {
        //ColorPath(redPath, Color.red);
        //ColorPath(greenPath, Color.green);
        //ColorPath(bluePath, Color.blue);
        //ColorPath(yellowPath, Color.yellow);
    }

}
