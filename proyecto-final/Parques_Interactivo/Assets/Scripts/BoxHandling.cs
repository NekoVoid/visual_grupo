using System.Collections.Generic;
using UnityEngine;

public class BoxHandling : MonoBehaviour
{
    public BoxType type = BoxType.REGULAR;

    private List<PieceHandling> pieces = new List<PieceHandling>();

    public Renderer rend;

    private void Awake()
    {
        if (rend == null)
            rend = GetComponent<Renderer>();
    }

    public void SetColor(Color color)
    {
        if (rend != null)
        {
            rend.material.color = color;
        }
    }

    public int AddPiece(PieceHandling piece)
    {
        // TODO: Implementar lógica de agregar pieza
        return 0;
    }

    public void RemovePiece(int id)
    {
        // TODO: Implementar lógica de remover pieza
    }

    public bool GetAndInformKill(PieceHandling piece)
    {
        // TODO: Implementar lógica de matar pieza
        return false;
    }

    void ReorganizePieces()
    {
        // TODO: Implementar lógica de reorganización
    }
}
