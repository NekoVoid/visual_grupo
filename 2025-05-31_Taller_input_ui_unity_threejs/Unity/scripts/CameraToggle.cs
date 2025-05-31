using UnityEngine;

public class CameraToggle : MonoBehaviour
{
    public Transform playerTransform;
    public Camera mainCamera;
    public Transform canvasViewPoint;
    public PlayerController playerController;

    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private bool isInCanvasMode = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isInCanvasMode)
        {
            GoToCanvasView();
        }
    }

    public void GoToCanvasView()
    {
        originalParent = mainCamera.transform.parent;
        originalLocalPosition = mainCamera.transform.localPosition;
        originalLocalRotation = mainCamera.transform.localRotation;

        mainCamera.transform.parent = null;
        mainCamera.transform.position = canvasViewPoint.position;
        mainCamera.transform.rotation = canvasViewPoint.rotation;

        playerController.controlEnabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isInCanvasMode = true;
    }

    public void ReturnToGameplay()
    {
        // Volver cámara al jugador
        mainCamera.transform.parent = originalParent;
        mainCamera.transform.localPosition = originalLocalPosition;
        mainCamera.transform.localRotation = originalLocalRotation;

        // Mover jugador al origen
        playerTransform.position = new Vector3(0f, 1f, 0f);

        // Reactivar control
        playerController.controlEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isInCanvasMode = false;
    }

}
