using System;
using Game.Managers;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Utilities
{
    public class CameraLocator : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        
        private void Awake()
        {
            GameManager.Instance.LocateCamera(_cinemachineCamera);
        }
    }
}
