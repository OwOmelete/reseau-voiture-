using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerProgress player = other.gameObject.GetComponent<CollisionManager>().player;
            if (player.checkpointIndex == index - 1)
            {
                player.checkpointIndex = index;
            }
        }
    }
}
