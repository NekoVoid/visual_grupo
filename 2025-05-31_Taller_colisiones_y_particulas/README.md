# 2025-05-31_Taller_colisiones_y_particulas

## Unity

En este proyecto se creó una escena interactiva en Unity con uso del sistema de físicas, detección de colisiones y efectos visuales con partículas. La escena incluye una **esfera interactiva** que el usuario puede agarrar y mover con el mouse, **cubos que detectan colisiones** mediante `OnCollisionEnter` y `OnTriggerEnter`, y **un sistema de partículas** que se activa dinámicamente al detectar eventos de impacto. Además, se incluye lógica para detectar objetos con raycasts y cambiar sus colores.

## Comportamiento esperado

Al iniciar la escena, el usuario puede hacer clic sobre la esfera para "agarrarla" y moverla con el mouse a diferentes alturas usando las teclas `W` y `S`. La esfera rebota al chocar con el plano u otros objetos gracias a su material físico. Cuando colisiona con el cubo sólido, este cambia de color y emite partículas. Si la esfera atraviesa el cubo sin collider (con `isTrigger`), el cubo cambia de color mientras esté dentro. Además, el usuario puede hacer clic sobre cualquier objeto con un `Raycast` para resaltarlo en rojo. El sistema responde visual y físicamente a todas las interacciones.

---

## Pasos realizados

1. **Creación de los GameObjects**  
   - Se colocaron en la escena una esfera (`BolaInteractiva`), un plano, un cubo con collider (`DetectorColision`) y un cubo sin collider pero con `isTrigger` (`BotonFisico`).

2. **Asignación de componentes de físicas**  
   Se agregó un `Rigidbody` a la esfera para que pudiera interactuar con la gravedad y colisionar con otros objetos. A los objetos estáticos como el plano se les añadió un `Collider` sin Rigidbody.

3. **Script para interacción con la esfera**  
   Se creó el script `Bola.cs` que permite al usuario agarrar la esfera con el mouse mediante `Raycast` y moverla a una altura variable, desactivando temporalmente la gravedad (`rb.useGravity = false`) y posicionándola con `Lerp`.

4. **Detección de colisiones con materiales visuales**  
   En el cubo `Coli`, se implementó `OnCollisionEnter()` para detectar impactos físicos y cambiar su color a amarillo.

5. **Detección de triggers sin colisión física**  
   En el cubo sin collider (`BotonFisico`), se emplearon `OnTriggerEnter()` y `OnTriggerExit()` para cambiar colores al contacto sin una colisión física tradicional.

6. **Uso de Raycasting para selección de objetos**  
   Se implementó `RaycastSelector.cs`, un script que permite seleccionar objetos con el clic del mouse y cambia su color a rojo usando `Physics.Raycast()` y `Renderer.material`.

7. **Activación de partículas en colisiones**  
   Se implementó un sistema de partículas modificado por medio del inspector de este objeto, se instancia y se activa dinámicamente al detectar una colisión en el método `OnCollisionEnter()`, haciendo uso de `Instantiate()` y `ParticleSystem.Play()`.

8. **Configuración de rebote**  
   Se mejoró el comportamiento físico de la esfera haciendo que rebote al colisionar, aumentando la propiedad `Bounciness` en un `PhysicMaterial` y asignándolo al `Collider` de la esfera.

---

## Resultados

Scripts

Esfera con colision(Bola.cs)

---

```csharp
using UnityEngine;

public class BolaInteractiva : MonoBehaviour
{
    private Rigidbody rb;
    private bool agarrada = false;
    private float altura = 1.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Aumentar altura al presionar W
        if (Input.GetKey(KeyCode.W))
        {
            altura += Time.deltaTime * 2f; // Puedes ajustar la velocidad
        }

        if (Input.GetKey(KeyCode.S))
        {
            altura -= Time.deltaTime * 2f;
            altura = Mathf.Max(0.5f, altura); // evita que se vaya al suelo
        }


        // Clic del mouse hacia abajo
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    agarrada = true;
                    rb.useGravity = false;
                    rb.linearVelocity = Vector3.zero;
                }
            }
        }

        // Mientras mantengo clic y tengo la bola agarrada
        if (agarrada && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 destino = hit.point;
                destino.y = altura; // mantiene a la bola flotando
                transform.position = Vector3.Lerp(transform.position, destino, 10f * Time.deltaTime);
            }
        }

        // Al soltar clic
        if (Input.GetMouseButtonUp(0) && agarrada)
        {
            agarrada = false;
            rb.useGravity = true;
        }
    }
}
```
---

Cubo con OnTrigger (BotonFisico.cs)

---

```csharp
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
```
---

Cubo con colision y particulas(Coli.cs)

---

```csharp
using UnityEngine;

public class DetectorColision : MonoBehaviour
{
    public ParticleSystem efectoColision;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colisión con: " + collision.gameObject.name);
        GetComponent<Renderer>().material.color = Color.yellow;

        if (efectoColision != null)
        {
            // Posicionar la partícula en el punto de contacto
            efectoColision.transform.position = collision.contacts[0].point;
            efectoColision.Play();
        }
    }
}
```
---

Cambio de color al seleccionar(RaycastSelector.cs)

---

```csharp
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
```

---

Comportamiento final:

![Colision y particulas](gif/gif.gif)

---

## Comentarios - Reflexión

Además de mostrar partículas o cambiar colores al colisionar, se pueden vincular otras acciones como activar luces para retroalimentación visual, reproducir sonidos mediante `AudioSource.Play()` para una experiencia auditiva más rica, cambiar de escena con `SceneManager.LoadScene()` al tocar un objeto clave, aplicar fuerzas adicionales usando `Rigidbody.AddForce()` para reacciones físicas más dinámicas, destruir o desactivar objetos con `Destroy()` o `SetActive(false)` para mecánicas de juego, o incluso mostrar elementos de la interfaz como paneles o mensajes informativos.


