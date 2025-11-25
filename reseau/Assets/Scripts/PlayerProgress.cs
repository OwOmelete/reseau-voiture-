using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerProgress : NetworkBehaviour
{
    public int lapCount;
    public int checkpointIndex;
    public float distanceToNext = 0f;
    [SerializeField] private TMP_Text lapText;

    public NetworkVariable<int> Rank = new NetworkVariable<int>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            RacePositionManager.Instance.RegisterPlayer(this);
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
            RacePositionManager.Instance.UnregisterPlayer(this);
    }


    public void UpdateLapText()
    {
        lapText.text = lapCount + "/" + LapManager.INSTANCE.totalLaps;
    }
    
    
    private void Start()
    {
        lapCount = 1;
        checkpointIndex = 0;
        UpdateLapText();
    }

    private void Update()
    {
        if (!IsServer) return;
        Transform next;
        if (checkpointIndex == 0)
        {
            next = LapManager.INSTANCE.transform;
        }
        else
        {
            next = LapManager.INSTANCE.checkpoints[checkpointIndex - 1].transform;
        }
        distanceToNext = Vector3.Distance(transform.position, next.position);
    }

    public float GetProgress()
    {
        return lapCount * 1000000 + checkpointIndex * 10000 - distanceToNext;
    }
}
