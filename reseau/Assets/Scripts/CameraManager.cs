using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager INSTANCE;

    private void Awake()
    {
        if (INSTANCE)
        {
            Destroy(gameObject);
        }
        else
        {
            INSTANCE = this;
        }
    }
}
