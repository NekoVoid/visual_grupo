using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public PlayerColor color;
    public List<PieceHandling> pieces;
    public List<BoxHandling> customPath; // Ruta específica de casillas

    [HideInInspector]
    public int[] lastDiceResults;
}
