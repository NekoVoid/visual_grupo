using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAnimationControl : MonoBehaviour
{
    public Animator animator;
    //public TMP_Dropdown animDropdown;
    public Transform character;  // Referencia al GameObject del personaje

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isPaused = false;

    void Start()
    {
        //animDropdown.onValueChanged.AddListener(ChangeAnimation);

        // Guardar la posición y rotación inicial
        if (character != null)
        {
            originalPosition = character.position;
            originalRotation = character.rotation;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        animator.speed = isPaused ? 0 : 1;
    }

    public void ResetCharacter()
    {
        if (character != null)
        {
            character.position = originalPosition;
            character.rotation = originalRotation;
        }

        isPaused = false;
        animator.speed = 1;
        animator.Play("Idle");
        //animDropdown.value = 0; // Actualiza visualmente el dropdown
    }

    /*
    public void ChangeAnimation(int index)
    {
        isPaused = false;
        animator.speed = 1;

        switch (index)
        {
            case 0: animator.Play("Idle"); break;
            case 1: animator.Play("Walk"); break;
            case 2: animator.Play("Run"); break;
            case 3: animator.Play("Dance"); break;
        }
    }
    */
}


