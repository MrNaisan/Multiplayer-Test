using UnityEngine;
using System;

public class FireController : MonoBehaviour
{
    #region Singletone
    public FireController()
    {
        Active = this;
    }
    public static FireController Active { get; private set; }
    #endregion

    public Action OnFire;
    [NonSerialized]
    public int mainPlayerID;

    public void Fire()
    {
        OnFire?.Invoke();
    }
}
