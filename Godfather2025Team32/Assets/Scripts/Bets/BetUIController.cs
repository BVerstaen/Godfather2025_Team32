using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BetUIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelAddDialog;      // Panel_AddDialog
    public GameObject inputPlayerName;     // InputField_PlayerName
    public GameObject inputAmount;         // InputField_Amount
    public TMP_Dropdown teamDropdown;
    public Button buttonConfirmAdd;        // Button_ConfirmAdd
    public Button buttonCancelAdd;         // Button_Cancel

    [Header("List")]
    public Transform contentListTeam1;          // Content (inside ScrollView)
    public Transform contentListTeam2;
    public GameObject betEntryPrefab;      // BetEntryPrefab

    public Button buttonAddPlayer;         // Button_AddPlayer
    public TextMeshProUGUI textTotalPot;   // optional: Text that shows total pot

    private TMP_InputField PlayerName;
    private TMP_InputField Amount;
    

    private void Start()
    {
        if (buttonAddPlayer == null || buttonConfirmAdd == null || buttonCancelAdd == null)
        {
            Debug.LogError("[BetUIController] Un ou plusieurs boutons non assignés.");
            enabled = false;
            return;
        }

        buttonAddPlayer.onClick.AddListener(OnAddPlayerClicked);
        buttonConfirmAdd.onClick.AddListener(OnConfirmAddClicked);
        buttonCancelAdd.onClick.AddListener(OnCancelAddClicked);

        if (inputPlayerName != null) PlayerName = inputPlayerName.GetComponentInChildren<TMP_InputField>();
        if (inputAmount != null) Amount = inputAmount.GetComponentInChildren<TMP_InputField>();

        if (teamDropdown == null)
        {
            Debug.LogWarning("[BetUIController] teamDropdown non assigné. Créé-en un dans le panelAddDialog et assigne-le.");
        }
        else
        {
            // Optionnel : remplir automatiquement avec les valeurs de l'enum Team
            teamDropdown.options.Clear();
            foreach (var name in Enum.GetNames(typeof(Team)))
            {
                teamDropdown.options.Add(new TMP_Dropdown.OptionData(name));
            }
            teamDropdown.RefreshShownValue();
        }

        if (panelAddDialog != null) panelAddDialog.SetActive(false);
        RefreshList();
    }

    private void OnAddPlayerClicked()
    {
        if (panelAddDialog != null) 
            panelAddDialog.SetActive(true);

        if (Amount != null) 
            Amount.text = "";
        
        if (PlayerName != null)
        {
            PlayerName.text = "";
            PlayerName.Select();
        }

        if (teamDropdown != null) 
            teamDropdown.value = 0;
    }

    private void OnCancelAddClicked()
    {
        if (panelAddDialog != null)
            panelAddDialog.SetActive(false);
    }

    private void OnConfirmAddClicked()
    {
        string name = PlayerName != null ? PlayerName.text.Trim() : null;
        string amountText = Amount != null ? Amount.text : null;

        if (name == null)
        {
            Debug.LogError("[BetUIController] Champ PlayerName introuvable.");
            return;
        }

        if (string.IsNullOrEmpty(amountText))
        {
            Debug.LogWarning("Montant vide !");
            return;
        }

        int amount;
        if (!int.TryParse(amountText, out amount))
        {
            Debug.LogWarning("Montant invalide !");
            return;
        }

        // read selected team from dropdown
        Team selectedTeam = Team.None;
        if (teamDropdown != null)
        {
            int idx = teamDropdown.value;
            // clamp and parse
            var names = Enum.GetNames(typeof(Team));
            if (idx >= 0 && idx < names.Length)
            {
                try { selectedTeam = (Team)Enum.Parse(typeof(Team), names[idx]); }
                catch { selectedTeam = Team.None; }
            }
        }

        if (BetManager.Instance == null)
        {
            Debug.LogError("[BetUIController] BetManager.Instance est null.");
            return;
        }

        bool ok = BetManager.Instance.AddBet(name, amount, selectedTeam);
        if (!ok)
        {
            Debug.LogWarning("Impossible d'ajouter le pari (nom vide, montant invalide ou joueur déjà ajouté).");
            return;
        }

        if (panelAddDialog != null) 
            panelAddDialog.SetActive(false);
        
        RefreshList();

        if (buttonAddPlayer != null)
            buttonAddPlayer.Select();
    }

    public void RefreshList()
    {
        if (contentListTeam1 == null || contentListTeam2 == null)
        {
            Debug.LogWarning("[BetUIController] contentList non assigné.");
            return;
        }

        foreach (Transform child in contentListTeam1) 
            Destroy(child.gameObject);
        
        foreach (Transform child in contentListTeam2)
            Destroy(child.gameObject);

        if (BetManager.Instance == null) 
            return;
        
        foreach (var bet in BetManager.Instance.Bets)
        {
            Transform content = bet.team == Team.Team1 ? contentListTeam1 : contentListTeam2;
            var go = Instantiate(betEntryPrefab, content);
            
            var entry = go.GetComponent<BetEntryUI>();
            if (entry != null)
            {
                entry.Setup(bet.playerName, bet.amount, bet.team, OnRemoveEntry);
                var rt = go.GetComponent<RectTransform>();
                SetEntryHeight(rt, 90f);
            }
            else
            {
                // fallback : set first text found
                var textsTMP = go.GetComponentsInChildren<TMP_Text>();
                if (textsTMP != null && textsTMP.Length > 0)
                {
                    textsTMP[0].text = $"{bet.playerName} — {bet.amount}€ — {bet.team}";
                }
                else
                {
                    var texts = go.GetComponentsInChildren<UnityEngine.UI.Text>();
                    if (texts != null && texts.Length > 0)
                    {
                        texts[0].text = $"{bet.playerName} — {bet.amount}€ — {bet.team}";
                    }
                }

                var removeBtn = go.GetComponentInChildren<Button>();
                if (removeBtn) removeBtn.onClick.AddListener(() => OnRemoveEntry(bet.playerName));
            }
        }

        if (textTotalPot != null) 
            textTotalPot.text = "Pot total : " + BetManager.Instance.GetTotalPot().ToString();
    }

    private void OnRemoveEntry(string playerName)
    {
        if (BetManager.Instance == null) 
            return;
        
        BetManager.Instance.RemoveBet(playerName);
        RefreshList();
    }
    
    private void SetEntryHeight(RectTransform entryRT, float height)
    {
        if (entryRT == null) 
            return;
        
        Vector2 s = entryRT.sizeDelta;
        s.y = height;
        entryRT.sizeDelta = s;
    }
}