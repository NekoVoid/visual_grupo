using UnityEngine;

public class BotonFisico : MonoBehaviour
{
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = Color.white;
    }

    void OnTriggerEnter(Collider other)
    {
        rend.material.color = Color.green;
    }

    void OnTriggerExit(Collider other)
    {
        rend.material.color = Color.white;
    }
}
