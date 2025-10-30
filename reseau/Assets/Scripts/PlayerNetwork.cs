using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<int> randomNumber = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<PlayerData> playerData = new(new PlayerData
    {
        life = 100,
        stunt = false,
    },NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private KeyCode leftKey = KeyCode.Q;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode downKey = KeyCode.S;
    [SerializeField] private KeyCode upKey = KeyCode.Z;
    
    private Vector3 direction;

    public override void OnNetworkSpawn()
    {
        //randomNumber.OnValueChanged += (int previousValue, int newValue) => { Debug.Log(OwnerClientId + " Random Number " + randomNumber.Value); };
        playerData.OnValueChanged += (PlayerData previousValue, PlayerData newValue) => { Debug.Log(OwnerClientId +
            " life " + newValue.life + "stunt" + newValue.stunt + " message " + newValue.message); };
    }

    private void Update()
    {
        
        if (!IsOwner)
        {
            return;
        }
        
        direction = Vector3.zero;

        if (Input.GetKey(leftKey))
        {
            direction.x = -1f;
        }
        if (Input.GetKey(rightKey))
        {
            direction.x = 1f;
        }
        if (Input.GetKey(downKey))
        {
            direction.z = -1f;
        }
        if (Input.GetKey(upKey))
        {
            direction.z = 1f;
        }

        direction = direction.normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //randomNumber.Value = Random.Range(0, 100);

            /*playerData.Value = new PlayerData()
            {
                life = Random.Range(0, 100),
                stunt = playerData.Value.stunt,
                message = "hihi"
            };*/
            
            TestRpc();
        }
    }

    [Rpc(SendTo.Server)]
    void TestRpc()
    {
        Debug.Log("TestRpc" + OwnerClientId);
    }
}

public struct PlayerData : INetworkSerializable
{
    public int life;
    public bool stunt;
    public FixedString128Bytes message;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref life);
        serializer.SerializeValue(ref stunt);
        serializer.SerializeValue(ref message);
    }
}