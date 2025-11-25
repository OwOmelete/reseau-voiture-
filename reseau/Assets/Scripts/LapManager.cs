using System;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    public Checkpoint[] checkpoints;
    public int totalLaps;
    
    public static LapManager INSTANCE;

    
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerProgress player = other.gameObject.GetComponent<CollisionManager>().player;
            if (player.checkpointIndex == checkpoints.Length)
            {
                player.checkpointIndex = 0;
                player.lapCount++;
                Debug.Log("tour numéro " + player.lapCount);

                if (player.lapCount > totalLaps)
                {
                    Debug.Log("gagné");
                }
                else
                {
                    player.UpdateLapText();
                }
            }
        }
    }
}
