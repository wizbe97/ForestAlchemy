using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBalanceManager", menuName = "Game/Managers/PlayerBalanceManager")]
public class PlayerBalanceManagerSO : ScriptableObject
{
    public PlayerBalanceSO playerBalance;
    public GameEventSO onBalanceChangedEvent;

    public bool CanAfford(int amount)
    {
        return playerBalance.balance >= amount;
    }

    public void DeductBalance(int amount)
    {
        if (CanAfford(amount))
        {
            playerBalance.balance -= amount;
            onBalanceChangedEvent?.Raise();

        }
    }

    public void ClearBalance()
    {
        playerBalance.balance = playerBalance.startingBalace;
        onBalanceChangedEvent?.Raise();
    }
}
