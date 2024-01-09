using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;

    [Header("Joining indicators")]
    [SerializeField] private GameObject joiningScreen;

    [SerializeField] private JoinedIndicator joinedIndicatorPrefab;

    [SerializeField] private Transform indicatorParent;
    [SerializeField] private Color[] playerColors;
    [SerializeField] private Transform spawnLocation;


    public bool CanMove { get; private set; }

    public static MainGameManager Instance { get; private set; }

    public void PlayerJoined(PlayerController player)
    {
        var playerIndex = player.PlayerIndex;
        Instantiate(joinedIndicatorPrefab, indicatorParent).UpdateDisplay(
            playerColors[playerIndex], "P" + playerIndex);

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