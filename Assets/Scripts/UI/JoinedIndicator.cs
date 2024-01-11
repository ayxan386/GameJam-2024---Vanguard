using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinedIndicator : MonoBehaviour
{
    [SerializeField] private Image colorIndicator;
    [SerializeField] private Image playerSkinImage;
    [SerializeField] private TextMeshProUGUI playerIdText;

    public void UpdateDisplay(Color color, string playerId, Sprite playerSkin)
    {
        colorIndicator.color = color;
        playerIdText.text = playerId;
        playerSkinImage.sprite = playerSkin;
    }
}