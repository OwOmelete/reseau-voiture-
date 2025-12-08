using System;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public KartController kartController;
    public PlayerProgress player;
    public PlayerInteraction playerInteraction;


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (kartController.isAttacking)
            {
                if (other.transform.parent.TryGetComponent<PlayerInteraction>(out var otherPlayer))
                {
                    playerInteraction.OnPlayerCollision(otherPlayer);
                }
            }
            //if (kartController.role == KartController.Role.hider) Respawn();
        }
    }

    /*private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {

            Debug.Log("yeehaw");
            if (kartController.isAttacking)
            {
                if (other.transform.parent.TryGetComponent<PlayerInteraction>(out var otherPlayer))
                {
                    playerInteraction.OnPlayerCollision(otherPlayer);
                }
            }
            //if (kartController.role == KartController.Role.hider) Respawn();
        }
    }
    */

    void Respawn()
    {
        kartController.sphere.transform.position = GameManager.INSTANCE.respawnPoint.position;
    }
}
