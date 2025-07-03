using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        float speed = 0f;

        // Caminar
        if (Input.GetKey(KeyCode.W))
        {
            speed = 0.5f; // caminar
        }

        // Correr
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            speed = 2f; // correr
        }

        animator.SetFloat("Speed", speed); // actualizar siempre

        // Baile (IsDancing)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("IsDancing", true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("IsDancing", false);
        }

        // Salto con tecla J
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger("Jump");
        }
    }
}
