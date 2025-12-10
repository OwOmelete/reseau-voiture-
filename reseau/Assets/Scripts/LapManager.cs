using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LapManager : NetworkBehaviour
{
    public Checkpoint[] checkpoints;
    public int totalLaps;
    
    public static LapManager INSTANCE;

    public List<PlayerProgress> finalResults = new List<PlayerProgress>();
    

    
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

    public void PlayerFinished(PlayerProgress player)
    {
        if (!IsServer) return;

        if (!finalResults.Contains(player))
        {
            finalResults.Add(player);
            player.FinalPosition.Value = finalResults.Count;
            
            if (finalResults.Count == RacePositionManager.Instance.players.Count)
            {
                ulong[] playerIds = finalResults.ConvertAll(p => p.OwnerClientId).ToArray();
                ShowFinalResultsClientRpc(playerIds);
            }
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
                player.UpdateLapText();

                if (player.lapCount > totalLaps)
                {
                    LapManager.INSTANCE.PlayerFinished(player);
                }
            }
        }
    }

    [ClientRpc]
    void ShowFinalResultsClientRpc(ulong[] playerIds)
    {
        KartController.player.updateResultText(playerIds);
    }
}
