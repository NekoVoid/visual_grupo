# 2025-07-02_Taller_motion_design_interactivo_eventos

## Unity

Este taller implementa un sistema interactivo de animación en Unity donde las acciones del jugador (teclado o mouse) disparan cambios de animación en un personaje humanoide. Se utilizó el modelo **“Y Bot”** descargado desde Mixamo en formato `.FBX`, junto con varias animaciones: **Idle**, **Dance**, **Walk** y **Jump**.

El sistema fue construido mediante un `Animator Controller` configurado con parámetros (`float`, `bool`, `trigger`) que permiten transiciones suaves entre estados. La lógica de entrada está definida en un script que escucha eventos como presionar teclas específicas o hacer clic, y activa las animaciones correspondientes.

**Motion Design** en este contexto se refiere a la integración coherente entre interacción del usuario y movimiento animado del personaje por medio de las teclas o cambios y alteraciones dentro de la escena que provocan nuevas animaciones como es el hecho de ir mas rapido desatara correr en vez de caminar.

## Descripción del modelo utilizado

- **Nombre del personaje:** Y Bot (modelo humanoide).
- **Animaciones utilizadas:**
  - `Idle`: posición en reposo.
  - `Walk`: caminata básica.
  - `Jump`: salto vertical.
  - `Dance`: gira sobre si mismo.

Todos los clips fueron descargados por separado desde Mixamo, exportados en formato `.FBX for Unity`, y asignados al mismo rig humanoide del modelo principal.

## Pasos realizados

1. **Importación del modelo y clips desde Mixamo**
   - Se descargó el modelo `Y Bot` con esqueleto y la animación `Idle`.
   - Posteriormente se descargaron los clips de `Walk`, `Jump` y `Dance` sin esqueleto para mantener consistencia.
   - Todos los `.FBX` fueron arrastrados a la carpeta `Assets` de Unity.
   - A cada clip se le asignó rig `Humanoid` y se desactivó `Loop Time` en animaciones que no deben repetirse (como `Jump` o `Dance`).

2. **Configuración del Animator Controller**
   - Se creó el `Animator Controller` del personaje y se añadieron los estados (`Idle`, `Walk`, `Jump`, `Dance`).
   - Se definieron parámetros:
     - `Speed` (float) para caminar/correr.
     - `IsDancing` (bool) para animaciones de bucle como bailar.
     - `Jump` (trigger) para activar el salto sin condición de entrada.
   - Se añadieron transiciones con o sin `Has Exit Time` según el tipo de animación.

3. **Configuración del personaje en la escena**
   - Se instanció el modelo `Y Bot` en la escena.
   - Se le asignó el `Animator Controller` configurado.
   - Se posicionó correctamente el personaje en el escenario 3D.

4. **Programación del sistema de eventos**
   - Se creó el script `PlayerAnimationController.cs` que escucha entradas del jugador:
     - `W` → activa `Walk`.
     - `W + Shift` → aumenta `Speed` para correr.
     - `Espacio` → activa una animación de tipo `Dance`.
     - `J` → activa la animación de `Jump` (mediante `SetTrigger`).
   - El script usa `Animator.SetFloat`, `SetBool` y `SetTrigger` para cambiar entre animaciones.

## Resultados

Script principal:

---

```csharp
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
```

---

Comportamiento final:

![Comportamiento final](gif/gif.gif)

---

## Reflexión

La utilización de motion design interactivo enriqueció la experiencia del usuario al hacer que el personaje respondiera de forma inmediata y visual a las acciones del usuario. Combinar **inputs físicos (teclado)** con **animación esquelética** no solo añade estética a una escena, sino que devuelve la inmediatez de la interactividad entre el usuario y el entorno, haciendo la experiencia más natural. Aprendí a implementar `Animator.SetTrigger`, a controlar la lógica de transición con parámetros y a combinar varios clips de Mixamo sobre un único personaje de manera eficiente.