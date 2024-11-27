using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBalance", menuName = "Game/PlayerBalance")]
public class PlayerBalanceSO : ScriptableObject
{
    public int balance = 1000;

    public int startingBalace = 10000;
}
