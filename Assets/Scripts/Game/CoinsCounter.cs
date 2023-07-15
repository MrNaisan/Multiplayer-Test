using UnityEngine;
using TMPro;
using System;

public class CoinsCounter : MonoBehaviour
{
    #region Singletone
    public CoinsCounter()
    {
        Active = this;
    }
    public static CoinsCounter Active { get; private set; }
    #endregion

    public TextMeshProUGUI Text;
    int coinsCount;

    public void ChangeCoinsCount(int coinsCount)
    {
        this.coinsCount = coinsCount;
        Text.text = $"{coinsCount}"; 
    }
}
