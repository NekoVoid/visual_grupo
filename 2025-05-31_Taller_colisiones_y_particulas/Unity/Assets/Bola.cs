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
