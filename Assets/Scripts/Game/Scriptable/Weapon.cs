using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Weapon")]
public class Weapon : ScriptableObject
{
    public int Damage;
    public float FireRate;
    public float ProjectileSpeed;
}
