using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManager;

    [Header("Balance UI")]
    [SerializeField] private TMP_Text balanceText;

    private void Start()
    {
        UpdateBalanceUI();
    }

    public void UpdateBalanceUI()
    {
        balanceText.text = "Balance: $" + gameManager.playerBalanceManager.playerBalance.balance;
    }
}