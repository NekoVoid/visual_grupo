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
