using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class InitiateBoard : MonoBehaviour
{
  public BoxHandling box;
  public int rampBoxCount = 2;
  public int trackBoxCount = 2;
  public PlayerColor[] sectionColor = {
    PlayerColor.RED,
    PlayerColor.YELLOW,
    PlayerColor.BLUE,
    PlayerColor.GREEN
  };
  public Tuple<int, BoxType> test;

  // public UnityEvent GetBox;
  List<List<BoxHandling>> boxes;
  SplineContainer splines;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    splines = GetComponent<SplineContainer>();
  }

  // Update is called once per frame
  void Update()
  {

  }

  // [ContextMenu("place boxes")]
  public void GenerateBoxes()
  {
    DeleteBoxes();
    boxes = new List<List<BoxHandling>>();
    for (int s = 0; s < splines.Splines.Count; s++)
    {
      Spline spline = splines.Splines[s];
      if (spline.Count == 2)
      {
        boxes.Add(PlaceBoxesOnSpline(spline, rampBoxCount, s, sectionColor[s / 2]));
      }
      else
      {
        boxes.Add(PlaceBoxesOnSpline(spline, trackBoxCount, s, sectionColor[s / 2]));
      }
    }

    for (int i = 0; i < boxes.Count; i++)
    {
      //circular access to boxes list
      var nextBox = boxes[(i + 1) % boxes.Count][0];

      if (boxes[i].Count == rampBoxCount)
      {
        boxes[i][0].nextBoxes.Add(nextBox);
      }
      else
      {
        boxes[i][boxes[i].Count - 1].nextBoxes.Add(nextBox);
      }
    }
  }

  private List<BoxHandling> PlaceBoxesOnSpline(Spline spline, int boxCount, int splineIndex, PlayerColor splineColor)
  {
    List<BoxHandling> localBoxes = new List<BoxHandling>();

    //epsilon used for edge case solving
    const float epsilonSubStep = 100f;
    float epsilon = 1f / (epsilonSubStep * (boxCount - 1));

    for (int i = 0; i < boxCount; i++)
    {
      float3 pos, tangent, up;
      var t = (float)i / (float)(boxCount - 1);

      // the orientation instantiated objects is inconsistent at the edges of a spline
      // for this we use the orientation at an epsilon before the edges
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

      BoxHandling boxHandling = Instantiate<BoxHandling>(
          box,
          pos,
          Quaternion.LookRotation(tangent, up),
          gameObject.transform
      );

      boxHandling.name = $"Box{i}_{splineColor}";
      boxHandling.color = splineColor;

      if (i > 0)
      {
        localBoxes[i - 1].nextBoxes.Add(boxHandling);
      }

      localBoxes.Add(boxHandling);
    }
    return localBoxes;
  }

  private void DeleteBoxes()
  {
    while (transform.childCount > 0)
    {
      DestroyImmediate(transform.GetChild(0).gameObject);
    }
  }
}
