using UnityEngine;

public class RaycastSelector : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Objeto seleccionado: " + hit.collider.name);
                Renderer rend = hit.transform.GetComponent<Renderer>();
                if (rend != null)
                    rend.material.color = Color.red;
            }
        }
    }
}
