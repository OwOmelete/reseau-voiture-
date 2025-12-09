using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;

public class PlayerConnectionManager : NetworkBehaviour
{
    private int expectedPlayers = 0;
    private int spawnedPlayers = 0;

    private bool serverInitialized = false;
    
    public void InitServer()
    {
        if (serverInitialized) return;
        serverInitialized = true;
        
        Debug.Log("server Init");

        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        
        if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.IsHost)
        {
            OnServerStarted();
        }
    }

    private void OnServerStarted()
    {
        PrepareRaceStart();
        Debug.Log("started server");
        
        if (!NetworkObject.IsSpawned)
        {
            NetworkObject.Spawn();
        }
        ulong hostId = NetworkManager.Singleton.LocalClientId;
        StartCoroutine(WaitForPlayerSpawn(hostId));
        
        
    }
    
    private void OnClientConnected(ulong clientId)
    {
        PrepareRaceStart();

        StartCoroutine(WaitForPlayerSpawn(clientId));
    }

    private void PrepareRaceStart()
    {
        expectedPlayers = NetworkManager.Singleton.ConnectedClientsList.Count;
        Debug.Log(expectedPlayers);
    }

    private IEnumerator WaitForPlayerSpawn(ulong clientId)
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject != null);

        CheckSpawnedPlayers();
    }

    private void CheckSpawnedPlayers()
    {
        spawnedPlayers = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
                spawnedPlayers++;
        }
        Debug.Log(expectedPlayers + "expected players");
        Debug.Log(spawnedPlayers + "spawned players");

        if (spawnedPlayers == expectedPlayers)
        {
            StopAllCoroutines();
            StartCoroutine(StartCountdownDelay(5f));
        }
        
    }

    private IEnumerator StartCountdownDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        StartCountdownServer();
        
    }
    
    public void StartCountdownServer()
    {
        if (!IsServer)
        {
            Debug.LogWarning("StartCountdownServer appelé depuis un client !");
            return;
        }

        if (CountdownManager.Instance.startTime > 0)
        {
            Debug.Log("Countdown déjà lancé, retour");
            return;
        }

        double targetTime = NetworkManager.Singleton.LocalTime.Time + CountdownManager.Instance.countdownDuration;
        Debug.Log($"[Server] Lancement du countdown. targetTime={targetTime}");

        // Envoi immédiat à tous les clients
        StartCountdownClientRpc(targetTime);
    }
    
    [Rpc(SendTo.Everyone)]
    private void StartCountdownClientRpc(double startTimeFromServer)
    {
        Debug.Log("coucou");
        CountdownManager.Instance.startTime = startTimeFromServer;
        CountdownManager.Instance.raceStarted = false;
        Debug.Log($"[ClientRpc] ClientId={NetworkManager.Singleton.LocalClientId} reçoit startTime={startTimeFromServer}");
    }
}