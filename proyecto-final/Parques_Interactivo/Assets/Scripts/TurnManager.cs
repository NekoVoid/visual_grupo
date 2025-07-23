using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public DiceRoller diceRoller;

    private int currentPlayerIndex = 0;
    private bool waitingForMove = false;

    public List<PlayerData> players = new(); // Se llenará automáticamente

    void Start()
    {
        GeneratePlayers(); // Llenar jugadores
        StartTurn();
    }

    void GeneratePlayers()
    {
        players.Clear();

        players.Add(new PlayerData
        {
            color = PlayerColor.RED,
            customPath = InitiateBoard2.Instance.redPath
        });

        players.Add(new PlayerData
        {
            color = PlayerColor.GREEN,
            customPath = InitiateBoard2.Instance.greenPath
        });

        players.Add(new PlayerData
        {
            color = PlayerColor.BLUE,
            customPath = InitiateBoard2.Instance.bluePath
        });

        players.Add(new PlayerData
        {
            color = PlayerColor.YELLOW,
            customPath = InitiateBoard2.Instance.yellowPath
        });
    }

    void StartTurn()
    {
        waitingForMove = false;
        PlayerData currentPlayer = players[currentPlayerIndex];
        Debug.Log("Turno del jugador: " + currentPlayer.color);

        diceRoller.RollDice();
    }

    public void OnDiceRolled(int d1, int d2)
    {
        PlayerData currentPlayer = players[currentPlayerIndex];
        currentPlayer.lastDiceResults = new int[] { d1, d2 };

        Debug.Log($"Jugador {currentPlayer.color} lanzó {d1} y {d2}");
        waitingForMove = true;

        // Aquí activar lógica para seleccionar y mover ficha
    }

    public void EndTurn()
    {
        waitingForMove = false;
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        StartTurn();
    }
}
