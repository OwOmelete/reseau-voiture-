using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;
        public Transform[] spawnPoints;

        private void Start()
        {
            Instance = this;
        }
    }
}