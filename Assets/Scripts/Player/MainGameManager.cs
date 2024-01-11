using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{
    [Header("Joining indicators")]
    [SerializeField] private Button startGameButton;

    [SerializeField] private GameObject joiningScreen;

    [SerializeField] private JoinedIndicator joinedIndicatorPrefab;

    [SerializeField] private Transform indicatorParent;
    [SerializeField] private Color[] playerColors;
    [SerializeField] private Sprite[] playerSkins;
    [SerializeField] private Transform spawnLocation;


    public bool CanMove { get; private set; }

    [field: SerializeField]
    public LayerMask[] PlayerLayers { get; private set; }

    public List<PlayerController> Players { get; private set; }

    public static MainGameManager Instance { get; private set; }

    public void PlayerJoined(PlayerController player)
    {
        if (Players == null)
        {
            Players = new List<PlayerController>();
        }

        startGameButton.interactable = true;
        startGameButton.Select();

        Players.Add(player);

        var playerIndex = player.PlayerIndex;
        player.PlayerColor = playerColors[playerIndex];

        Instantiate(joinedIndicatorPrefab, indicatorParent).UpdateDisplay(
            playerColors[playerIndex], $"P{playerIndex + 1}" , playerSkins[playerIndex]);

        player.MoveTo(spawnLocation);
    }

    public void StartGame()
    {
        joiningScreen.SetActive(false);
        CanMove = true;
    }

    private void Awake()
    {
        Instance = this;
    }
}