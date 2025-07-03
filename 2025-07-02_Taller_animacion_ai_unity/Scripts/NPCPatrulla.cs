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
