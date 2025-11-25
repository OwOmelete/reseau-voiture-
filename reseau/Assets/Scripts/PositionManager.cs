using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;

public class RacePositionManager : NetworkBehaviour
{
    public static RacePositionManager Instance;
    private List<PlayerProgress> players = new List<PlayerProgress>();

    void Awake()
    {
        Instance = this;
    }

    public void RegisterPlayer(PlayerProgress p)
    {
        players.Add(p);
    }

    public void UnregisterPlayer(PlayerProgress p)
    {
        players.Remove(p);
    }

    void Update()
    {
        if (!IsServer || players.Count == 0)
            return;
        // Tri direct sur la liste
        var sorted = players
            .OrderByDescending(p => p.GetProgress())
            .ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].Rank.Value = i + 1;
        }
    }
}