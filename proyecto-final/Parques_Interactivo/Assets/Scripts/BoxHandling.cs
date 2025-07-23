using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxHandling : MonoBehaviour
{
  public List<BoxHandling> nextBoxes;
  public BoxType type = BoxType.REGULAR;
  public PlayerColor color = PlayerColor.BLUE;
  public float test = 0.9f;

  List<PieceHandling> pieces = new List<PieceHandling>();

  public BoxHandling GetNextBox(PieceHandling piece)
  {
    if (nextBoxes.Count > 1)
    {
      return (piece.color == color) ? nextBoxes[0] : nextBoxes[1];
    }
    return nextBoxes[0] ?? this;  
  }

  // Moves a piece into this box
  public Transform AddPiece(PieceHandling piece)
  {
    pieces.Add(piece);
    ReorganizePieces();
    return pieces.Last().transform;
  }
    

  // Removes a piece from this box
  public void RemovePiece(PieceHandling piece)
  {
    if (pieces.Remove(piece)) ReorganizePieces();
  }
  public bool GetAndInformKill(PieceHandling piece)
  {
        return type == BoxType.REGULAR && pieces.Count == 1 && pieces[0].color != piece.color;
    }

  void ReorganizePieces()
  {
    int count = pieces.Count;

    for (int i = 0; i < count; i++)
    {
      var piece = pieces[i];

      var pieceRenderer = piece.GetComponent<Renderer>();

      Vector3 pos = new Vector3(
        (float)i/(float)count - (1f - 1f/count)/2f,
        1f/2f + pieceRenderer.bounds.extents.y,
        0
      );
      Quaternion rot = transform.rotation * quaternion.identity;


      pos = transform.TransformPoint(pos);

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
