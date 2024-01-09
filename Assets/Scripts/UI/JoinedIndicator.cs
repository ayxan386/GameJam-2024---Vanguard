using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinedIndicator : MonoBehaviour
{
    [SerializeField] private Image colorIndicator;
    [SerializeField] private TextMeshProUGUI playerIdText;

    public void UpdateDisplay(Color color, string playerId)
    {
        colorIndicator.color = color;
        playerIdText.text = playerId;
    }
}