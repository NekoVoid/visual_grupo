using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxHandling : MonoBehaviour
{
  public List<BoxHandling> nextBoxes;
  public BoxType type = BoxType.REGULAR;
  public PlayerColor color = PlayerColor.BLUE;

  List<PieceHandling> pieces = new List<PieceHandling>();

  public BoxHandling GetNextBox(PieceHandling piece)
  {
    if (nextBoxes.Count < 1)
    {
      return (piece.color == color)? nextBoxes[0]: nextBoxes[1];
    }
    return nextBoxes[0] ?? this;  
  }

  // Moves a piece into this box
  public void AddPiece(PieceHandling piece)
  {
    pieces.Add(piece);
    ReorganizePieces();
  }

  // Removes a piece from this box
  public void RemovePiece(PieceHandling piece)
  {
    if (pieces.Remove(piece)) ReorganizePieces();
  }
  public bool GetAndInformKill(PieceHandling piece)
  {
    //TODO: implement
    return false;
  }

  void ReorganizePieces()
  {
    int count = pieces.Count;
    foreach (var piece in pieces)
    {
      Vector3 pos;
      Quaternion rot;
      transform.GetPositionAndRotation(out pos, out rot);

      var pieceRenderer = piece.GetComponent<Renderer>();

      pos.y += pieceRenderer.bounds.extents.y + transform.lossyScale.y/2;

      piece.transform.SetPositionAndRotation(pos, rot);
    }
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}
