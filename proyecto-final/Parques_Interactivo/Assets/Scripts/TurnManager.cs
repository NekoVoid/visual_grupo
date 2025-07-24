using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public DiceRoller diceRoller;
    public GameObject piecePrefab;
    public WebcamSetup handInfo;

    private int currentPlayerIndex = 0;
    private int[] diceValues = new int[2];
    private bool[] diceUsed = new bool[2];
    private int selectedDiceIndex = -1;
    private int selectedPieceIndex = -1;

    private PlayerData currentPlayer;
    public List<PlayerData> players = new();

    private enum TurnStage
    {
        WaitingForRoll,
        SelectingDice,
        SelectingPiece,
        TurnEnded
    }

    private TurnStage stage = TurnStage.WaitingForRoll;

    void Start()
    {
        GeneratePlayers();
        StartTurn();
    }

    void GeneratePlayers()
    {
        players.Clear();

        foreach (PlayerColor color in Enum.GetValues(typeof(PlayerColor)))
        {
            var player = new PlayerData
            {
                color = color,
                pieces = new List<PieceHandling>()
            };

            var boxObject = GameObject.Find("Start_" + color);
            if (boxObject == null)
            {
                Debug.LogError($"No se encontr� la casilla Start_{color}");
                continue;
            }

            var box = boxObject.GetComponent<BoxHandling>();
            if (box == null)
            {
                Debug.LogError($"Start_{color} no tiene componente BoxHandling");
                continue;
            }

            for (int i = 0; i < 4; i++)
            {
                GameObject pieceObj = Instantiate(piecePrefab, box.transform.position, Quaternion.identity);
                pieceObj.name = $"Piece_{color}_{i + 1}";
                var piece = pieceObj.GetComponent<PieceHandling>();
                piece.color = color;
                piece.currentBox = box;

                box.AddPiece(piece);
                player.pieces.Add(piece);
            }

            players.Add(player);
        }
    }

    void StartTurn()
    {
        currentPlayer = players[currentPlayerIndex];
        diceUsed[0] = false;
        diceUsed[1] = false;
        selectedDiceIndex = -1;
        selectedPieceIndex = -1;
        stage = TurnStage.WaitingForRoll;

        Debug.Log("Turno del jugador: " + currentPlayer.color);
    }

    void Update()
    {
        switch (stage)
        {
            case TurnStage.WaitingForRoll:
                if (handInfo.action == "LANZAR")
                {
                    diceRoller.RollDice(); // Esto debe terminar llamando a OnDiceRolled
                }
                break;

            case TurnStage.SelectingDice:
                if (handInfo.action == "ESCOGER_1") selectedDiceIndex = 0;
                if (handInfo.action == "ESCOGER_2") selectedDiceIndex = 1;

                if (selectedDiceIndex != -1 && !diceUsed[selectedDiceIndex] && handInfo.action == "ACEPTAR")
                {
                    Debug.Log($"Dado seleccionado: {diceValues[selectedDiceIndex]}");
                    stage = TurnStage.SelectingPiece;
                }
                break;

            case TurnStage.SelectingPiece:
                if (handInfo.action == "ESCOGER_1") selectedPieceIndex = 0;
                if (handInfo.action == "ESCOGER_2") selectedPieceIndex = 1;
                if (handInfo.action == "ESCOGER_3") selectedPieceIndex = 2;
                if (handInfo.action == "ESCOGER_4") selectedPieceIndex = 3;

                if (selectedPieceIndex != -1 && handInfo.action == "ACEPTAR")
                {
                    var piece = currentPlayer.pieces[selectedPieceIndex];
                    int steps = diceValues[selectedDiceIndex];
                    Debug.Log($"Moviendo ficha {selectedPieceIndex + 1} con {steps} pasos");

                    for (int i = 0; i < steps; i++)
                    {
                        piece.MoveOneStep();
                    }

                    diceUsed[selectedDiceIndex] = true;

                    if (!diceUsed[0] || !diceUsed[1])
                    {
                        stage = TurnStage.SelectingDice;
                        selectedDiceIndex = -1;
                        selectedPieceIndex = -1;
                    }
                    else
                    {
                        stage = TurnStage.TurnEnded;
                    }
                }
                break;

            case TurnStage.TurnEnded:
                EndTurn();
                break;
        }
    }

    public void OnDiceRolled(int d1, int d2)
    {
        diceValues[0] = d1;
        diceValues[1] = d2;
        Debug.Log($"Dados lanzados: {d1} y {d2}");
        stage = TurnStage.SelectingDice;
    }

    public void EndTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        StartTurn();
    }
}
