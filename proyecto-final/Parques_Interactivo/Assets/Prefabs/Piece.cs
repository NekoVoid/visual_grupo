using System.Collections;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [Header("Piece Setup")]
    public PlayerColor playerColor;

    [HideInInspector]
    public Transform[] path;

    private int currentIndex = 0;
    private bool isMoving = false;

    [Header("Materials")]
    public Material redMaterial;
    public Material blueMaterial;
    public Material greenMaterial;
    public Material yellowMaterial;

    private void Start()
    {
        ApplyColor();
    }

    public void SetPath(Transform[] pathPoints)
    {
        path = pathPoints;
        currentIndex = 0;
        transform.position = path[currentIndex].position;
    }

    public void Move(int steps)
    {
        if (!isMoving && path != null && currentIndex + steps < path.Length)
        {
            StartCoroutine(MoveAlongPath(steps));
        }
    }

    private IEnumerator MoveAlongPath(int steps)
    {
        isMoving = true;

        while (steps > 0 && currentIndex + 1 < path.Length)
        {
            currentIndex++;
            Vector3 target = path[currentIndex].position;

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, 5f * Time.deltaTime);
                yield return null;
            }

            steps--;
            yield return new WaitForSeconds(0.05f);
        }

        isMoving = false;
    }

    public void ResetToStart()
    {
        currentIndex = 0;
        if (path != null && path.Length > 0)
        {
            transform.position = path[0].position;
        }
    }

    private void ApplyColor()
    {
        Material selectedMaterial = null;

        switch (playerColor)
        {
            case PlayerColor.RED:
                selectedMaterial = redMaterial;
                break;
            case PlayerColor.BLUE:
                selectedMaterial = blueMaterial;
                break;
            case PlayerColor.GREEN:
                selectedMaterial = greenMaterial;
                break;
            case PlayerColor.YELLOW:
                selectedMaterial = yellowMaterial;
                break;
        }

        if (selectedMaterial != null)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material = selectedMaterial;
            }
        }
    }
}
