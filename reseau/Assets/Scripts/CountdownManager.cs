using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace DefaultNamespace
{
    public class CountdownManager : NetworkBehaviour
    {
        public TMP_Text countdownText;
        public float countdownDuration = 3f;

        public double startTime = -1;
        public bool raceStarted = false;

        public static CountdownManager Instance;
        
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Update()
        {
            // Debug Update pour chaque client
            if (startTime >= 0 )
            {
                double timeLeft = startTime - NetworkManager.Singleton.NetworkTimeSystem.LocalTime;

                if (timeLeft > 0)
                {
                    countdownText.text = Mathf.Ceil((float)timeLeft).ToString();
                }
                else if (!raceStarted)
                {
                    raceStarted = true;
                    countdownText.text = "GO!";
                    StartRace();
                    StartCoroutine(countdownFinished());
                }
            }
        }

        IEnumerator countdownFinished()
        {
            yield return new WaitForSeconds(1.5f);
            countdownText.text = "";
        }

        // Appeler depuis le serveur seulement

        private void StartRace()
        {
            KartController.player.startRace();
            Debug.Log($"[Race] ClientId={NetworkManager.Singleton.LocalClientId} : Course lancée !");
        }
    }
}
