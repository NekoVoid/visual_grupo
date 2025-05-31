using UnityEngine;

public class DetectorColision : MonoBehaviour
{
    public ParticleSystem efectoColision;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colisi�n con: " + collision.gameObject.name);
        GetComponent<Renderer>().material.color = Color.yellow;

        if (efectoColision != null)
        {
            // Posicionar la part�cula en el punto de contacto
            efectoColision.transform.position = collision.contacts[0].point;
            efectoColision.Play();
        }
    }
}
