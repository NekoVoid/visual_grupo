using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class InitiateBoard : MonoBehaviour
{
    public GameObject box;
    public int boxCount = 2;
    public UnityEvent GetBox;
    SplineContainer splines;
    GameObject[][] boxes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        splines = GetComponent<SplineContainer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("place boxes")]
    public void GenerateBoxes()
    {
        DeleteBoxes();
        boxes = new GameObject[splines.Splines.Count][];
        for (int s = 0; s < splines.Splines.Count; s++)
        {
            Spline spline = splines.Splines[s];
            if (spline.Count == 2)
            {
                boxes[s] = PlaceBoxesOnSpline(spline, boxCount);
            }
            else
            {
                boxes[s] = PlaceBoxesOnSpline(spline, boxCount * 2);
            }
        }
    }

    private GameObject[] PlaceBoxesOnSpline(Spline spline, int boxCount)
    {
        GameObject[] boxes = new GameObject[boxCount];

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

            GameObject g = Instantiate(
                box,
                pos,
                Quaternion.LookRotation(tangent, up),
                gameObject.transform
            );

            boxes[i] = g;
        }
        return boxes;
    }

    private void DeleteBoxes()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
