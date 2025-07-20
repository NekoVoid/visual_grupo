using System;
using System.Collections.Generic;
using UnityEngine;

public class BoxHandling : MonoBehaviour
{
  public List<GameObject> nextBoxes;
  public BoxType type = BoxType.REGULAR;
  public PlayerColor color = PlayerColor.BLUE;

  private List<PieceHandling> pieces = new List<PieceHandling>();

  public GameObject GetNextBox(PieceHandling piece)
  {
    if (nextBoxes.Count > 1 && piece.color == color)
    {
      return nextBoxes[1];
    }
    return nextBoxes[0] ?? gameObject;
  }

  // Moves a piece into this box
  public int AddPiece(PieceHandling piece)
  {
    //TODO: implement
    return 0;
  }

// Removes a piece from this box
  public void RemovePiece(int id)
  {
    //TODO: immplement
  }

  public bool GetAndInformKill(PieceHandling piece)
  {
    //TODO: implement
    return false;
  }

  void ReorganizePieces()
  {
    //TODO: implement
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
