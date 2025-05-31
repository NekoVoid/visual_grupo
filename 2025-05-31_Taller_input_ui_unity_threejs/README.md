# 2025-05-31_Taller_input_ui_unity

## Unity

Este proyecto implementa un sistema de control para un personaje en primera persona junto con una interfaz gráfica de usuario (UI) interactiva usando Canvas en Unity. La escena principal permite mover al personaje con teclado y mouse, y alternar entre el modo de juego y una pantalla de UI que incluye coordenadas, barra de vida y un botón para reiniciar la posición del jugador.

Al estar en el modo de jugador no se puede utilizar la ui pues la camara esta dirigida a un solo punto que esta determinado por el mouse por lo que hay que cambiar a la otra vista.

## Comportamiento esperado

El jugador puede moverse libremente usando las teclas `W`, `A`, `S`, `D` y girar la cámara con el movimiento del mouse. Al hacer clic con el botón izquierdo del mouse, se imprime un mensaje en consola. Cuando se presiona la tecla `R`, la cámara deja de seguir al jugador y se muestra el Canvas con UI. En esta pantalla, el usuario puede presionar un botón para volver al modo de juego y reposicionar al jugador en `(0, 1, 0)`. Durante el juego, también se actualiza dinámicamente un texto con las coordenadas del jugador y una barra de vida animada con `PingPong`.

---

## Pasos realizados

1. **Control del jugador en primera persona**  
   Se creó un GameObject tipo cápsula como jugador, y se añadió una cámara como hijo. Se programó el script `PlayerController.cs` para permitir movimiento con teclado (`Input.GetAxis`) y rotación con mouse (`Input.GetAxis("Mouse X/Y")`), bloqueando el cursor.

2. **Detección de clics del mouse**  
   Se usó `Input.GetMouseButtonDown(0)` en el script del jugador para detectar clics e imprimir mensajes en consola.

3. **Implementación del Canvas y la UI**  
   Se añadió un Canvas con render mode en `Screen Space - Overlay`, incluyendo un texto (`TextMeshProUGUI`), un botón (`Button`) y un `Slider` para la barra de vida. Se añadió también un `EventSystem` automáticamente por Unity.

4. **Script de UIManager para mostrar coordenadas y vida**  
   Se creó el script `UIManager.cs`, que muestra la posición del jugador en el texto cada frame, y actualiza la barra de vida con un valor oscilante de `Mathf.PingPong`.

5. **Cambio de cámara entre gameplay y UI**  
   Se creó el script `CameraToggle.cs`, que guarda la posición y rotación original de la cámara. Al presionar `R`, se separa la cámara del jugador y se posiciona en una vista del Canvas, desbloqueando el cursor. Al presionar el botón de volver, la cámara regresa al jugador y el personaje se teletransporta a `(0, 1, 0)`.

6. **Asignación correcta en el OnClick del botón**  
   En el botón del Canvas, se configuró un evento `OnClick` en modo `Runtime Only`, donde se asignó el GameObject con `CameraToggle` y se seleccionó el método `ReturnToGameplay()`.

7. **Solución de problemas comunes**  
   Se solucionaron errores como el Canvas detrás de la cámara, el `Raycast` obstruido por el jugador y el conflicto con el `EventSystem` por medio de la otra vista que se activa con `R`.

---

## Resultados

Scripts

Control de cambio de camara/vista (CamaraToogle.cs)

---

```csharp
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
```
---

Objetos del canvas (UIManager.cs)

---

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    public Transform player;
    public TextMeshProUGUI coordinatesText;
    public Slider healthBar;

    void Update()
    {
        // Mostrar posición
        Vector3 pos = player.position;
        coordinatesText.text = $"X: {pos.x:F1} Y: {pos.y:F1} Z: {pos.z:F1}";

        // Cambiar barra de vida (simulada)
        healthBar.value = Mathf.PingPong(Time.time, 1f);
    }
}
```
---

Control del jugador y su camara (PlayerController.cs)

---

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    public Transform playerBody;
    public Camera cam;

    public bool controlEnabled = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!controlEnabled) return;

        // Movimiento (WSAD)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        transform.Translate(move * speed * Time.deltaTime, Space.World);

        // Rotación (Mouse)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        // Click Izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("¡Click detectado!");
        }
    }
}
```

---

Comportamiento final:

![Comportamiento de la escena Jugador y UI](gif/gif.gif)

---


