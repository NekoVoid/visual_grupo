using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PieceHandling : MonoBehaviour
{
  public PlayerColor color = PlayerColor.BLUE;
  public int testBoxAdd = 0;
  BoxHandling currentBox;
  

  public void Killed()
  {
    //TODO: immplement
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {

  }

  [ContextMenu("test add to board")]
  public void TestAddBoard()
  {
    if (currentBox != null)
    {
      currentBox.RemovePiece(this);
    }

    var board = GameObject.Find("BoxBoard");
    if (board == null) return;
    var box = board.transform.GetChild(testBoxAdd).GetComponent<BoxHandling>();

    Debug.Log(box);

    if (box == null) return;
    box.AddPiece(this);
    currentBox = box;

  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      Debug.Log("what");
      if (currentBox != null)
      {
        var next = currentBox.GetNextBox(this);
        currentBox.RemovePiece(this);

        currentBox = next;
        currentBox.AddPiece(this);
      }
    }
  }
}
