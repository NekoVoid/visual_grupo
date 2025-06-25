using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        float speed = 0f;

        // Detectar si W está presionado continuamente
        if (Input.GetKey(KeyCode.W))
        {
            speed = 0.5f; // caminar por defecto
        }

        // Si además estás presionando Shift mientras caminas
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            speed = 2f; // correr
        }

        animator.SetFloat("Speed", speed); // siempre actualizar el parámetro

        // Animación de baile (presionar espacio)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("IsDancing", true);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("IsDancing", false);
        }
    }
}