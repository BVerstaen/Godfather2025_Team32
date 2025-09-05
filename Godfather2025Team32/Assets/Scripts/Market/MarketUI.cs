using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketUI : MonoBehaviour
{
    public Team team;
    public string itemID;
    public Image Image;
    public TMP_Text PriceText;
    public Button BuyButton;

    private void Start()
    {
        MarketItem marketItem = MarketManager.Instance.findMarketItemFromID(itemID);
        if (marketItem != null)
        {
            Image.sprite = marketItem.spriteToUnlock;
            PriceText.text = marketItem.price.ToString();

            if (MarketManager.Instance.IsUnlocked(itemID, team))
                BuyButton.interactable = false;
        }
    }

    public void ClickButton()
    {
        MarketItem marketItem = MarketManager.Instance.findMarketItemFromID(itemID);
        TryUnlockUI(marketItem, team);
    }

    public void TryUnlockUI(MarketItem item, Team team)
    {
        if (MarketManager.Instance.TryUnlock(item.itemId, item.price, team))
        {
            Image.sprite = item.spriteToUnlock;
            BuyButton.interactable = false;
        }
    }
}
