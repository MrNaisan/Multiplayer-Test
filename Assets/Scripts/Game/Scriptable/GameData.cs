using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/GameData")]
public class GameData : DataHolder
{
    #region Singleton

    private static GameData _default;
    public static GameData Default => _default;

    #endregion

    [Header("Player Settings")]
    public Player Player;
    [Space]

    [Header("WeaponSettings")]
    public Weapon Weapon;
    [Space]

    [Header("Coins Settings")]
    public int MaxCoinsCount;
    public float CoinsSpawnRate;
    [Space]

    [Header("RoomSettings")]
    public int MinPlayersToStart;
    [Range(2,6)]public int MaxPlayersInRoom;

    public override void Init()
    {
        _default = this;
    }
}
