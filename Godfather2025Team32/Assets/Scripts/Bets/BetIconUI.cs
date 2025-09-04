using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BetIconUI : MonoBehaviour
{
    public Image avatarImage;
    public Image borderImage;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI nameText;

    public void SetBorderColor(Color c)
    {
        if (borderImage != null) 
            borderImage.color = c;
    }

    public void SetAvatar(Sprite s)
    {
        if (avatarImage != null) 
            avatarImage.sprite = s;
    }

    public void SetAmount(int amount)
    {
        if (amountText != null) 
            amountText.text = amount.ToString();
    }

    public void SetName(string n)
    {
        if (nameText != null) 
            nameText.text = n;
    }
}
