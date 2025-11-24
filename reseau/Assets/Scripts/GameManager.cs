using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform respawnPoint;
    public static GameManager INSTANCE;

    private void Awake()
    {
        if (INSTANCE)
        {
            Destroy(gameObject);
        }
        else
        {
            INSTANCE = this;
        }
    }
}
