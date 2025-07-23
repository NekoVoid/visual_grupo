using UnityEngine;

public class PieceHandling : MonoBehaviour
{
    [Header("Piece Settings")]
    public PlayerColor color = PlayerColor.BLUE;
    public Piece pieceLogic;
    public Transform startPosition; // Position where the piece returns when killed

    private void Awake()
    {
        if (pieceLogic == null)
        {
            pieceLogic = GetComponent<Piece>();
        }

        if (pieceLogic != null)
        {
            pieceLogic.playerColor = color;
        }
    }

    /// <summary>
    /// Assigns the path the piece should follow on the board.
    /// </summary>
    public void SetPiecePath(Transform[] path)
    {
        if (pieceLogic != null)
        {
            pieceLogic.SetPath(path);
        }
    }

    /// <summary>
    /// Moves the piece a specific number of steps.
    /// </summary>
    public void MovePiece(int steps)
    {
        if (pieceLogic != null)
        {
            pieceLogic.Move(steps);
        }
    }

    /// <summary>
    /// Resets the piece to its starting position and index when killed.
    /// </summary>
    public void Killed()
    {
        if (startPosition != null && pieceLogic != null)
        {
            transform.position = startPosition.position;
            pieceLogic.ResetToStart();
        }
    }
}
