using System;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    public static CameraManager INSTANCE;
    [SerializeField] private Camera cam;
    [SerializeField] private int boostFov = 75; 

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

    public void boost(float duration)
    {
        cam.DOKill();
        cam.DOFieldOfView(boostFov, 0.2f).OnComplete(() =>
        {
            cam.DOFieldOfView(60, duration).SetEase(Ease.InOutQuint);
        });

    }
}
