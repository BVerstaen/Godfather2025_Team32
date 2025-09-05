using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnerBetsUI : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI betAmountText;
    public Image playerIcon;
    
    public Sprite playerIconSprite;

    public void Setup(string playerName, int payout)
    {
        if (playerNameText) playerNameText.text = playerName;
        if (betAmountText) betAmountText.text = $"+{payout}";

        if (playerIcon && playerIconSprite )
            playerIcon.sprite = playerIconSprite;
        
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 s = rt.sizeDelta;
        s.y = 65f;
        rt.sizeDelta = s;
    }
}
