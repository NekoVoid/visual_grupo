## 2025-07-02_Taller_animacion_ai_unity

## Unity

Este taller implementa una **IA autónoma para NPCs** en Unity, que patrullan un entorno, detectan jugadores, los persiguen y reaccionan mediante animaciones contextuales. Se emplea navegación con `NavMesh`, detección mediante `Raycast`, control de animaciones con `Animator Controller` y una lógica de **máquina de estados**.

Se utilizó el modelo **“Y Bot”** de Mixamo con esqueleto humanoide y animaciones: **Idle**, **Walk**, **Run**, y **Dance**. Las animaciones se integran a través de parámetros dinámicos como velocidad (`Speed`) y banderas (`IsDancing`) que el NPC controla automáticamente según su estado.

## Explicación de comportamientos implementados

- **Patrullaje:** navegación cíclica entre puntos definidos. El NPC se desplaza automáticamente y cambia su animación a `Walk`.
- **Persecución:** cuando el jugador entra en su campo de visión, el NPC cambia a `Run` y sigue al jugador.
- **Búsqueda:** al perder de vista al jugador, va al último lugar donde lo vio y espera 2 segundos.
- **Baile:** al alcanzar al jugador, se detiene completamente y ejecuta una animación de `Dance` durante unos segundos antes de volver a patrullar.

## Integración del Animator

El `Animator Controller` controla las transiciones usando solo dos parámetros (`Speed` y `IsDancing`).  
El valor de `Speed` es automático según el movimiento real del `NavMeshAgent`, y `IsDancing` se activa/desactiva desde código al entrar o salir del estado `Bailando`.  
Esto permite una lógica sencilla y reactiva.


## Pasos realizados

1. **Preparación del entorno**
   - Se creó una escena con terreno plano y obstáculos.
   - Se generó una malla de navegación (`NavMeshSurface`) marcando el suelo como navegable.
   - Se instanció el personaje con `NavMeshAgent`.

2. **Implementación del Animator Controller**
   - Se creó un `Animator Controller` con estados `Idle`, `Walk`, `Run`, `Dance`.
   - Parámetros usados:
     - `Speed` (float) para controlar desplazamiento.
     - `IsDancing` (bool) para activar el baile.
   - Transiciones configuradas usando condiciones sin `Has Exit Time` para respuestas rápidas.

3. **Programación del comportamiento del NPC**
   - Se creó la clase `NPCPatrulla.cs` con lógica basada en una **máquina de estados**:
     - `Patrullando`: recorre puntos fijos usando `NavMeshAgent`.
     - `Persiguiendo`: detecta al jugador con `Raycast` y lo persigue.
     - `Buscando`: si lo pierde, va al último lugar visto por 2 segundos.
     - `Bailando`: al alcanzar al jugador, se detiene a bailar y luego reinicia patrulla.
   - El comportamiento está centralizado en el método `Update()` con `switch` por estado.

4. **Sincronización con animaciones**
   - Se controla el parámetro `Speed` directamente desde la magnitud del agente (`agent.velocity.magnitude`).
   - Al bailar, se detiene el agente y se activa `SetBool("IsDancing", true)`.
   - Finalizado el baile, se desactiva `IsDancing` y vuelve a patrullar.


## Resultados

Script principal:

---

```csharp
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrulla : MonoBehaviour
{
    public Transform[] puntos;
    public float rangoVision = 10f;
    public float anguloVision = 45f;
    public float tiempoBusqueda = 2f;
    public float distanciaCaptura = 1.5f;
    public float duracionBaile = 3f;

    private enum EstadoNPC { Patrullando, Persiguiendo, Buscando, Bailando }
    private EstadoNPC estado = EstadoNPC.Patrullando;

    private int index = 0;
    private float temporizadorBusqueda = 0f;
    private float temporizadorBaile = 0f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform jugador;
    private Vector3 ultimaPosicionVisto;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (puntos.Length > 0)
            agent.SetDestination(puntos[0].position);
    }

    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);

        switch (estado)
        {
            case EstadoNPC.Patrullando:
                Patrullar();
                if (PuedeVerAlJugador())
                {
                    ultimaPosicionVisto = jugador.position;
                    estado = EstadoNPC.Persiguiendo;
                }
                break;

            case EstadoNPC.Persiguiendo:
                Perseguir();
                if (!PuedeVerAlJugador())
                {
                    estado = EstadoNPC.Buscando;
                    temporizadorBusqueda = tiempoBusqueda;
                    agent.SetDestination(ultimaPosicionVisto);
                }
                else if (DistanciaAlJugador() <= distanciaCaptura)
                {
                    estado = EstadoNPC.Bailando;
                    agent.ResetPath(); // detener movimiento
                    temporizadorBaile = duracionBaile;
                    animator.SetBool("IsDancing", true);
                }
                break;

            case EstadoNPC.Buscando:
                Buscar();
                break;

            case EstadoNPC.Bailando:
                Bailar();
                break;
        }
    }

    void Patrullar()
    {
        animator.SetBool("IsDancing", false);
        agent.speed = 0.5f;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            index = (index + 1) % puntos.Length;
            agent.SetDestination(puntos[index].position);
        }
    }

    void Perseguir()
    {
        agent.speed = 1.5f;
        if (jugador != null)
            agent.SetDestination(jugador.position);
    }

    void Buscar()
    {
        agent.speed = 0.9f;
        temporizadorBusqueda -= Time.deltaTime;

        if (temporizadorBusqueda <= 0f && agent.remainingDistance < 0.5f)
        {
            estado = EstadoNPC.Patrullando;
            agent.SetDestination(puntos[index].position);
        }

        if (PuedeVerAlJugador())
        {
            estado = EstadoNPC.Persiguiendo;
            ultimaPosicionVisto = jugador.position;
        }
    }

    void Bailar()
    {
        agent.speed = 0f;
        temporizadorBaile -= Time.deltaTime;

        if (temporizadorBaile <= 0f)
        {
            animator.SetBool("IsDancing", false);
            estado = EstadoNPC.Patrullando;
            agent.SetDestination(puntos[index].position);
        }
    }

    bool PuedeVerAlJugador()
    {
        if (jugador == null) return false;

        Vector3 direccion = jugador.position - transform.position;
        if (direccion.magnitude > rangoVision) return false;

        float angulo = Vector3.Angle(transform.forward, direccion);
        if (angulo > anguloVision) return false;

        if (Physics.Raycast(transform.position + Vector3.up, direccion.normalized, out RaycastHit hit, rangoVision))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    float DistanciaAlJugador()
    {
        if (jugador == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, jugador.position);
    }
}
```

---

Comportamiento final:

![Comportamiento final](capturas/gif1.gif)

---

## Reflexión

Realizar un NPC con comportamiento autónomo completo en Unity apoyado en `NavMesh`, `Animator` y un estado máquina permite disponer de un flujo de control robusto y natural. El resultado es superado cuando la IA responde a su entorno de forma creíble y coherente conjugando movimientos y animaciones. Esta implementación escalable permite sumar reacciones (ataques, esconderse, visuales, ...) y demuestra que la **IA no solo se mueve sino que se comporta expresivamente** en tiempo real. He aprendido a gestionar `NavMeshAgent` junto con `Animator`, así como a modularizar los estados en el NPC y cómo enlazar reacciones animadas en función de eventos de juego.