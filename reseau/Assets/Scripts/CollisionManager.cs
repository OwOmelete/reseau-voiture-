using System;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public KartController kartController;
    public PlayerProgress player;
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("yeehaw");
            if (kartController.role == KartController.Role.hider) Respawn();
        }
    }

    void Respawn()
    {
        kartController.sphere.transform.position = GameManager.INSTANCE.respawnPoint.position;
    }
}
