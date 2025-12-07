using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    public float slowDuration = 1f;

    [SerializeField] private KartController kartController;

    private bool isSlowed = false;
    private float slowTimer = 0f;

    void Update()
    {
        if (isSlowed)
        {
            slowTimer -= Time.deltaTime;

            if (slowTimer <= 0f)
                isSlowed = false;
        }
    }

    public void OnPlayerCollision(PlayerInteraction other)
    {
        if (!IsOwner) return;
        if (!kartController.isAttacking) return;
        
        PlayerHitServerRpc(other.OwnerClientId);
    }
    


    [ServerRpc(RequireOwnership = false)]
    private void PlayerHitServerRpc(ulong otherPlayerId)
    {
        Debug.Log(otherPlayerId);
        var rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { otherPlayerId }
            }
        };
        
        ApplySlowClientRpc(rpcParams);
    }
    

    [ClientRpc]
    private void ApplySlowClientRpc(ClientRpcParams rpcParams = default)
    {
        Debug.Log("je vais manger ton père victor baz");
        kartController.slow();
        slowTimer = slowDuration;
        Debug.Log($"Player {OwnerClientId} est ralenti !");
    }
}